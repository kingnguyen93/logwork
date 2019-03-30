using Acr.UserDialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LogWork.AppResources.Localization;
using LogWork.Constants;
using LogWork.Helpers;
using LogWork.IServices;
using LogWork.Models;
using LogWork.Models.Response;
using LogWork.Models.SetSync;
using LogWork.Views.Popups;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;

namespace LogWork.Services
{
    public class SyncDataService : BaseService, ISyncDataService
    {
        private readonly ISyncProductService syncProductService;
        private readonly ISyncInvoiceService syncInvoiceService;

        private readonly string[] ExcludedUpdateProperties =
        {
            "Id",
            "UserId",
            "IsToSync"
        };

        private DateTime timeStart;
        private int totalRowInserted, totalRowUpdated;
        private int totalInterventionInserted, totalMessageInserted;

        public SyncDataService(ISyncProductService syncProductService, ISyncInvoiceService syncInvoiceService)
        {
            this.syncProductService = syncProductService;
            this.syncInvoiceService = syncInvoiceService;
        }

        public Task<bool> SyncFromServer(int method, Action onSuccess, Action<string> onError = null, bool showOverlay = false)
        {
            if (!ConnectivityHelper.IsNetworkAvailable(method == 2))
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    onError?.Invoke(TranslateExtension.GetValue("alert_no_internt_message"));
                });
                return Task.FromResult(false);
            }

            if (showOverlay)
                DependencyService.Get<IPopupService>().ShowContent(new LoadingScreen1() { Message = TranslateExtension.GetValue("content_message_synchronizing") });

            Task.Run(async () =>
            {
                return (await SyncFromServer(method) && await syncProductService.SyncFromServer(method) && await syncInvoiceService.SyncFromServer(method));
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                if (showOverlay)
                    DependencyService.Get<IPopupService>().HideContent();

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result)
                    {
                        if (showOverlay)
                            UserDialogs.Instance.Toast(new ToastConfig(TranslateExtension.GetValue("alert_message_sync_completed")) { });
                        onSuccess?.Invoke();
                    }
                    else
                    {
                        if (showOverlay)
                            UserDialogs.Instance.Toast(new ToastConfig(TranslateExtension.GetValue("alert_message_sync_failed")) { });
                        onError?.Invoke(TranslateExtension.GetValue("alert_message_sync_failed"));
                    }
                }
                else if (task.IsFaulted && task.Exception?.GetBaseException().Message is string message)
                {
                    if (showOverlay)
                        UserDialogs.Instance.Toast(new ToastConfig(message) { });
                    onError?.Invoke(message);
                }
            }));

            return Task.FromResult(true);
        }

        public async Task<bool> SyncFromServer(int method)
        {
            timeStart = DateTime.Now;
            totalRowInserted = 0;
            totalRowUpdated = 0;
            totalInterventionInserted = 0;
            totalMessageInserted = 0;

            Debug.WriteLine("Sync From Server Started!");

            if (Settings.LastSync != "0")
            {
                await SyncToServer(method);
                await SyncTrackingToServer();

                if (AppSettings.MobileLocationIsActive)
                    await SyncLocationToServer();

                SyncMediaToServer(method);
            }

            var response = await restClient.GetStringAsync(ApiURI.URL_BASE(CurrentAccount) + ApiURI.URL_GET_SYNC(CurrentUserName, Settings.CurrentPassword, Settings.LastSync));
            if (!string.IsNullOrWhiteSpace(response) && !response.Equals("[]"))
            {
                ProcessData(response);
            }
            else
            {
                SetLastSync();
            }

            //SyncMediasFromServer();

            if (AppSettings.NewInterventionNotification && Settings.LastSync != "0" && totalInterventionInserted > 0)
            {
                DependencyService.Get<ILocalNotificationService>().Show(new LocalNotification
                {
                    NotificationId = 100,
                    Title = Resources.notification_title,
                    Description = totalInterventionInserted != 1 ? Resources.notification_new_interventions : Resources.notification_new_intervention,
                    ReturningData = "Total Inserted Interventions: " + totalInterventionInserted, // Returning data when tapped on notification.
                    //NotifyTime = DateTime.Now.AddSeconds(30) // Used for Scheduling local notification.
                });

                //var notificationService = DependencyService.Get<ILocalNotificationService>();
                //notificationService.Cancel(100);

                MessagingCenter.Send(this, MessageKey.INTERVENTION_CHANGED);
            }

            if (AppSettings.NewMessageNotification && Settings.LastSync != "0" && totalMessageInserted > 0)
            {
                DependencyService.Get<ILocalNotificationService>().Show(new LocalNotification
                {
                    NotificationId = 101,
                    Title = Resources.notification_title,
                    Description = (totalInterventionInserted > 1) ? Resources.notification_new_messages : "Nouvelle intervention reçue",
                    ReturningData = "Total Inserted Messages: " + totalMessageInserted, // Returning data when tapped on notification.
                    //NotifyTime = DateTime.Now.AddSeconds(30) // Used for Scheduling local notification.
                });
            }

            Debug.WriteLine("Execute Time: " + DateTime.Now.Subtract(timeStart).TotalSeconds);
            Debug.WriteLine("Total Inserted Rows: " + totalRowInserted);
            Debug.WriteLine("Total Updated Rows: " + totalRowUpdated);
            Debug.WriteLine("Total Inserted Interventions: " + totalInterventionInserted);
            Debug.WriteLine("Total Inserted Messages: " + totalMessageInserted);
            Debug.WriteLine("Sync From Server Ended!");

            return await Task.FromResult(true);
        }

        private void ProcessData(string response)
        {
            var result = SyncData.FromJson(response);

            if (result != null && result.Count > 0)
            {
                if (DoSync(result))
                {
                    SetLastSync();
                }

                return;
            }
        }

        private void SetLastSync()
        {
            Settings.LastSync = ((long)DateTime.Now.ToUnixTimestamp()).ToString();

            if (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("APP_LAST_SYNCHRO")) is Setting lastSync)
            {
                lastSync.Value = Settings.LastSync.ToString();
                App.LocalDb.Update(lastSync);
            }
        }

        //TODO move this function to common
        private long ConvertToTimestamp(DateTime value)
        {
            long epoch = (value.Ticks - 621355968000000000) / 10000000;
            return epoch;
        }

        private bool settingsChanged;

        private bool DoSync(Dictionary<string, List<SyncData>> syncDatas)
        {
            try
            {
                App.LocalDb.BeginTransaction();

                foreach (var data in syncDatas)
                {
                    foreach (var item in data.Value)
                    {
                        if (item != null && item.V != null)
                        {
                            switch (item.I)
                            {
                                case "a":
                                    InsertOrUpdateAddress(item.V);
                                    break;

                                case "c":
                                    InsertOrUpdateClient(item.V);
                                    break;

                                case "che":
                                    InsertOrUpdateChemin(item.V);
                                    break;

                                case "fi":
                                    InsertOrUpdateFiliale(item.V);
                                    break;

                                case "int":
                                    InsertOrUpdateIntervention(item.V);
                                    break;

                                case "lip":
                                    InsertOrUpdateLinkInterventionProduct(item.V);
                                    break;

                                case "lit":
                                    InsertOrUpdateLinkInterventionTask(item.V);
                                    break;

                                case "mes":
                                    InsertOrUpdateMessage(item.V);
                                    break;

                                case "med":
                                    InsertOrUpdateMedia(item.V);
                                    break;

                                case "medl":
                                    InsertOrUpdateMediaLink(item.V);
                                    break;

                                case "pro":
                                    InsertOrUpdateProduct(item.V);
                                    break;

                                case "ta":
                                    InsertOrUpdateTasks(item.V);
                                    break;

                                case "tcc":
                                    InsertOrUpdateCategoryTracking(item.V);
                                    break;

                                case "se":
                                    InsertOrUpdateSetting(item.V);
                                    break;

                                case "u":
                                    InsertOrUpdateUser(item.V);
                                    break;

                                case "un":
                                    InsertOrUpdateUnite(item.V);
                                    break;

                                case "uli":
                                    InsertOrUpdateUniteItem(item.V);
                                    break;

                                case "unl":
                                    InsertOrUpdateUniteLink(item.V);
                                    break;
                                case "con": //contract
                                    InsertOrUpdateContract(item.V);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }

                App.LocalDb.Commit();

                if (settingsChanged)
                {
                    AppSettings.ReloadSetting();
                    settingsChanged = false;
                }

                return true;
            }
            catch (Exception ex)
            {
                App.LocalDb.Rollback();

                totalRowInserted = 0;

                Debug.WriteLine("Sync Error: ");
                Debug.WriteLine(ex);

                return false;
            }
        }
        private void InsertOrUpdateContract(Dictionary<string, string> data)
        {
            var result = JsonConvert.DeserializeObject<ContractResponse>(JsonConvert.SerializeObject(data), App.DefaultDeserializeSettings);
            if (result == null || result.ServerId == 0)
                return;
            if (App.LocalDb.Table<Contract>().FirstOrDefault(adr => adr.ServerId == result.ServerId) is Contract contract)
            {
                //PropertyExtension.UpdateProperties(result, address, ExcludedUpdateProperties);
                contract.UpdateFromResponse(result);

                contract.UserId = CurrentUser.Id;

                App.LocalDb.Update(contract);
                totalRowUpdated++;
            }
            else
            {
                App.LocalDb.InsertOrReplace(new Contract(result)
                {
                    UserId = CurrentUser.Id
                });
                totalRowInserted++;
            }
        }
        private void InsertOrUpdateAddress(Dictionary<string, string> data)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<AddressResponse>(JsonConvert.SerializeObject(data), App.DefaultDeserializeSettings);
                if (result == null || result.ServerId == 0)
                    return;
                if (App.LocalDb.Table<Address>().FirstOrDefault(adr => adr.ServerId == result.ServerId) is Address address)
                {
                    //PropertyExtension.UpdateProperties(result, address, ExcludedUpdateProperties);
                    address.UpdateFromResponse(result);

                    address.UserId = CurrentUser.Id;

                    App.LocalDb.Update(address);
                    totalRowUpdated++;
                }
                else
                {
                    App.LocalDb.InsertOrReplace(new Address(result)
                    {
                        UserId = CurrentUser.Id
                    });
                    totalRowInserted++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Deserialize Address failed: " + ex);
            }

        }

        private void InsertOrUpdateClient(Dictionary<string, string> data)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<ClientResponse>(JsonConvert.SerializeObject(data), App.DefaultDeserializeSettings);
                if (result == null || result.ServerId == 0)
                    return;
                if (App.LocalDb.Table<Client>().FirstOrDefault(adr => adr.ServerId == result.ServerId) is Client client)
                {
                    //PropertyExtension.UpdateProperties(result, client, ExcludedUpdateProperties);
                    client.UpdateFromResponse(result);

                    client.UserId = CurrentUser.Id;

                    App.LocalDb.Update(client);
                    totalRowUpdated++;
                }
                else
                {
                    App.LocalDb.InsertOrReplace(new Client(result)
                    {
                        UserId = CurrentUser.Id
                    });
                    totalRowInserted++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Deserialize Client failed: " + ex);
            }
        }

        private void InsertOrUpdateChemin(Dictionary<string, string> data)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<CheminResponse>(JsonConvert.SerializeObject(data), App.DefaultDeserializeSettings);
                if (result == null || result.ServerId == 0)
                    return;
                if (App.LocalDb.Table<Chemin>().FirstOrDefault(adr => adr.ServerId == result.ServerId) is Chemin chemin)
                {
                    //PropertyExtension.UpdateProperties(result, chemin, ExcludedUpdateProperties);
                    chemin.UpdateFromResponse(result);

                    chemin.UserId = CurrentUser.Id;

                    App.LocalDb.Update(chemin);
                    totalRowUpdated++;
                }
                else
                {
                    App.LocalDb.InsertOrReplace(new Chemin(result)
                    {
                        UserId = CurrentUser.Id
                    });
                    totalRowInserted++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Deserialize Chemin failed: " + ex);
            }
        }

        private void InsertOrUpdateFiliale(Dictionary<string, string> data)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<FilialeResponse>(JsonConvert.SerializeObject(data), App.DefaultDeserializeSettings);
                if (result == null || result.ServerId == 0)
                    return;
                if (App.LocalDb.Table<Filiale>().FirstOrDefault(adr => adr.ServerId == result.ServerId) is Filiale filiale)
                {
                    //PropertyExtension.UpdateProperties(result, filiale, ExcludedUpdateProperties);
                    filiale.UpdateFromResponse(result);

                    filiale.UserId = CurrentUser.Id;

                    App.LocalDb.Update(filiale);
                    totalRowUpdated++;
                }
                else
                {
                    App.LocalDb.InsertOrReplace(new Filiale(result)
                    {
                        UserId = CurrentUser.Id
                    });
                    totalRowInserted++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Deserialize Filiale failed: " + ex);
            }
        }

        private void InsertOrUpdateIntervention(Dictionary<string, string> data)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<InterventionResponse>(JsonConvert.SerializeObject(data), App.DefaultDeserializeSettings);
                if (result == null || result.ServerId == 0)
                    return;
                if (App.LocalDb.Table<Intervention>().FirstOrDefault(adr => adr.ServerId == result.ServerId) is Intervention intervention)
                {
                    //PropertyExtension.UpdateProperties(result, intervention, ExcludedUpdateProperties);
                    intervention.UpdateFromResponse(result);

                    intervention.UserId = CurrentUser.Id;

                    App.LocalDb.Update(intervention);
                    totalRowUpdated++;
                }
                else
                {
                    App.LocalDb.InsertOrReplace(new Intervention(result)
                    {
                        UserId = CurrentUser.Id
                    });
                    totalRowInserted++;
                    totalInterventionInserted++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Deserialize Intervention failed: " + ex);
            }
        }

        private void InsertOrUpdateLinkInterventionProduct(Dictionary<string, string> data)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<LinkInterventionProductResponse>(JsonConvert.SerializeObject(data), App.DefaultDeserializeSettings);
                if (result == null || result.ServerId == 0)
                    return;
                if (App.LocalDb.Table<LinkInterventionProduct>().FirstOrDefault(adr => adr.ServerId == result.ServerId) is LinkInterventionProduct linkInterventionProduct)
                {
                    //PropertyExtension.UpdateProperties(result, linkInterventionProduct, ExcludedUpdateProperties);
                    linkInterventionProduct.UpdateFromResponse(result);

                    linkInterventionProduct.UserId = CurrentUser.Id;

                    App.LocalDb.Update(linkInterventionProduct);
                    totalRowUpdated++;
                }
                else
                {
                    App.LocalDb.InsertOrReplace(new LinkInterventionProduct(result)
                    {
                        UserId = CurrentUser.Id
                    });
                    totalRowInserted++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Deserialize LinkInterventionProduct failed: " + ex);
            }
        }

        private void InsertOrUpdateLinkInterventionTask(Dictionary<string, string> data)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<LinkInterventionTaskResponse>(JsonConvert.SerializeObject(data), App.DefaultDeserializeSettings);
                if (result == null || result.ServerId == 0)
                    return;
                if (App.LocalDb.Table<LinkInterventionTask>().FirstOrDefault(adr => adr.ServerId == result.ServerId) is LinkInterventionTask linkInterventionTask)
                {
                    //PropertyExtension.UpdateProperties(result, linkInterventionTask, ExcludedUpdateProperties);
                    linkInterventionTask.UpdateFromResponse(result);

                    linkInterventionTask.UserId = CurrentUser.Id;

                    App.LocalDb.Update(linkInterventionTask);
                    totalRowUpdated++;
                }
                else
                {
                    App.LocalDb.InsertOrReplace(new LinkInterventionTask(result)
                    {
                        UserId = CurrentUser.Id
                    });
                    totalRowInserted++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Deserialize LinkInterventionTask failed: " + ex);
            }
        }

        private void InsertOrUpdateMessage(Dictionary<string, string> data)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<MessageResponse>(JsonConvert.SerializeObject(data), App.DefaultDeserializeSettings);
                if (result == null || result.ServerId == 0)
                    return;
                if (App.LocalDb.Table<Message>().FirstOrDefault(adr => adr.ServerId == result.ServerId) is Message message)
                {
                    //PropertyExtension.UpdateProperties(result, message, ExcludedUpdateProperties);
                    message.UpdateFromResponse(result);

                    message.UserId = CurrentUser.Id;

                    App.LocalDb.Update(message);
                    totalRowUpdated++;
                }
                else
                {
                    App.LocalDb.InsertOrReplace(new Message(result)
                    {
                        UserId = CurrentUser.Id
                    });
                    totalRowInserted++;
                    totalMessageInserted++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Deserialize Message failed: " + ex);
            }
        }

        private void InsertOrUpdateMedia(Dictionary<string, string> data)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<MediaResponse>(JsonConvert.SerializeObject(data), App.DefaultDeserializeSettings);
                if (result == null || result.ServerId == 0)
                    return;
                if (App.LocalDb.Table<Media>().FirstOrDefault(adr => adr.ServerId == result.ServerId) is Media media)
                {
                    //PropertyExtension.UpdateProperties(result, media, ExcludedUpdateProperties);
                    media.UpdateFromResponse(result);

                    media.UserId = CurrentUser.Id;

                    App.LocalDb.Update(media);
                    totalRowUpdated++;

                    //SyncMediaFromServer(media);
                }
                else
                {
                    var med = new Media(result)
                    {
                        UserId = CurrentUser.Id
                    };

                    App.LocalDb.InsertOrReplace(med);
                    totalRowInserted++;

                    //SyncMediaFromServer(med);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Deserialize Media failed: " + ex);
            }
        }

        private void InsertOrUpdateMediaLink(Dictionary<string, string> data)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<MediaLinkResponse>(JsonConvert.SerializeObject(data), App.DefaultDeserializeSettings);
                if (result == null || result.ServerId == 0)
                    return;
                if (App.LocalDb.Table<MediaLink>().FirstOrDefault(adr => adr.ServerId == result.ServerId) is MediaLink mediaLink)
                {
                    //PropertyExtension.UpdateProperties(result, mediaLink, ExcludedUpdateProperties);
                    mediaLink.UpdateFromResponse(result);

                    mediaLink.UserId = CurrentUser.Id;

                    App.LocalDb.Update(mediaLink);
                    totalRowUpdated++;
                }
                else
                {
                    App.LocalDb.InsertOrReplace(new MediaLink(result)
                    {
                        UserId = CurrentUser.Id
                    });
                    totalRowInserted++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Deserialize MediaLink failed: " + ex);
            }
        }

        private void InsertOrUpdateProduct(Dictionary<string, string> data)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<ProductResponse>(JsonConvert.SerializeObject(data), App.DefaultDeserializeSettings);
                if (result == null || result.ServerId == 0)
                    return;
                if (App.LocalDb.Table<Product>().FirstOrDefault(adr => adr.ServerId == result.ServerId) is Product product)
                {
                    //PropertyExtension.UpdateProperties(result, product, ExcludedUpdateProperties);
                    product.UpdateFromResponse(result);

                    product.UserId = CurrentUser.Id;

                    App.LocalDb.Update(product);
                    totalRowUpdated++;
                }
                else
                {
                    App.LocalDb.InsertOrReplace(new Product(result)
                    {
                        UserId = CurrentUser.Id
                    });
                    totalRowInserted++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Deserialize Product failed: " + ex);
            }
        }

        private void InsertOrUpdateTasks(Dictionary<string, string> data)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<TaskResponse>(JsonConvert.SerializeObject(data), App.DefaultDeserializeSettings);
                if (result == null || result.ServerId == 0)
                    return;
                if (App.LocalDb.Table<Tasks>().FirstOrDefault(adr => adr.ServerId == result.ServerId) is Tasks task)
                {
                    //PropertyExtension.UpdateProperties(result, task, ExcludedUpdateProperties);
                    task.UpdateFromResponse(result);

                    task.UserId = CurrentUser.Id;

                    App.LocalDb.Update(task);
                    totalRowUpdated++;
                }
                else
                {
                    App.LocalDb.InsertOrReplace(new Tasks(result)
                    {
                        UserId = CurrentUser.Id
                    });
                    totalRowInserted++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Deserialize Tasks failed: " + ex);
            }
        }

        private void InsertOrUpdateCategoryTracking(Dictionary<string, string> data)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<CategoryTrackingResponse>(JsonConvert.SerializeObject(data), App.DefaultDeserializeSettings);
                if (result == null || result.ServerId == 0)
                    return;
                if (App.LocalDb.Table<CategoryTracking>().FirstOrDefault(adr => adr.ServerId == result.ServerId) is CategoryTracking categoryTracking)
                {
                    //PropertyExtension.UpdateProperties(result, categoryTracking, ExcludedUpdateProperties);
                    categoryTracking.UpdateFromResponse(result);

                    categoryTracking.UserId = CurrentUser.Id;

                    App.LocalDb.Update(categoryTracking);
                    totalRowUpdated++;
                }
                else
                {
                    App.LocalDb.InsertOrReplace(new CategoryTracking(result)
                    {
                        UserId = CurrentUser.Id
                    });
                    totalRowInserted++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Deserialize CategoryTracking failed: " + ex);
            }
        }

        private void InsertOrUpdateSetting(Dictionary<string, string> data)
        {
            try
            {
                var newSetting = JsonConvert.DeserializeObject<SettingResponse>(JsonConvert.SerializeObject(data), App.DefaultDeserializeSettings);
                if (newSetting != null && !string.IsNullOrWhiteSpace(newSetting.Name))
                {
                    if (App.LocalDb.Table<Setting>().FirstOrDefault(a => a.Name.Equals(newSetting.Name)) is Setting oldSetting)
                    {
                        if (string.IsNullOrWhiteSpace(newSetting.Value))
                            oldSetting.Value = newSetting.Value;
                        if (newSetting.IsActif.HasValue)
                            oldSetting.IsActif = newSetting.IsActif.Value;

                        App.LocalDb.Update(oldSetting);
                        totalRowUpdated++;
                    }
                    else
                    {
                        Setting setting = new Setting(newSetting)
                        {
                            UserId = CurrentUser.Id
                        };
                        App.LocalDb.Insert(setting);
                        totalRowInserted++;
                    }
                    settingsChanged = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Deserialize Setting failed: " + ex);
            }
        }

        private void InsertOrUpdateUser(Dictionary<string, string> data)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<UserResponse>(JsonConvert.SerializeObject(data), App.DefaultDeserializeSettings);
                if (result == null || result.ServerId == 0)
                    return;
                if (App.LocalDb.Table<User>().FirstOrDefault(adr => adr.ServerId == result.ServerId) is User user)
                {
                    //PropertyExtension.UpdateProperties(result, user, ExcludedUpdateProperties);
                    user.UpdateFromResponse(result);

                    user.UserId = CurrentUser.Id;

                    App.LocalDb.Update(user);
                    totalRowUpdated++;
                }
                else
                {
                    App.LocalDb.InsertOrReplace(new User(result)
                    {
                        UserId = CurrentUser.Id
                    });
                    totalRowInserted++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Deserialize User failed: " + ex);
            }
        }

        private void InsertOrUpdateUnite(Dictionary<string, string> data)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<UniteResponse>(JsonConvert.SerializeObject(data), App.DefaultDeserializeSettings);
                if (result == null || result.ServerId == 0)
                    return;
                if (App.LocalDb.Table<Unite>().FirstOrDefault(adr => adr.ServerId == result.ServerId) is Unite unite)
                {
                    //PropertyExtension.UpdateProperties(result, unite, ExcludedUpdateProperties);
                    unite.UpdateFromResponse(result);

                    unite.UserId = CurrentUser.Id;

                    App.LocalDb.Update(unite);
                    totalRowUpdated++;
                }
                else
                {
                    App.LocalDb.InsertOrReplace(new Unite(result)
                    {
                        UserId = CurrentUser.Id
                    });
                    totalRowInserted++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Deserialize Unite failed: " + ex);
            }
        }

        private void InsertOrUpdateUniteItem(Dictionary<string, string> data)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<UniteItemResponse>(JsonConvert.SerializeObject(data), App.DefaultDeserializeSettings);
                if (result == null || result.ServerId == 0)
                    return;
                if (App.LocalDb.Table<UniteItem>().FirstOrDefault(adr => adr.ServerId == result.ServerId) is UniteItem uniteItem)
                {
                    //PropertyExtension.UpdateProperties(result, uniteItem, ExcludedUpdateProperties);
                    uniteItem.UpdateFromResponse(result);

                    uniteItem.UserId = CurrentUser.Id;

                    App.LocalDb.Update(uniteItem);
                    totalRowUpdated++;
                }
                else
                {
                    App.LocalDb.InsertOrReplace(new UniteItem(result)
                    {
                        UserId = CurrentUser.Id
                    });
                    totalRowInserted++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Deserialize UniteItem failed: " + ex);
            }
        }

        private void InsertOrUpdateUniteLink(Dictionary<string, string> data)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<UniteLinkResponse>(JsonConvert.SerializeObject(data), App.DefaultDeserializeSettings);
                if (result == null || result.ServerId == 0)
                    return;
                if (App.LocalDb.Table<UniteLink>().FirstOrDefault(adr => adr.ServerId == result.ServerId) is UniteLink uniteLink)
                {
                    //PropertyExtension.UpdateProperties(result, uniteLink, ExcludedUpdateProperties);
                    uniteLink.UpdateFromResponse(result);

                    uniteLink.UserId = CurrentUser.Id;

                    App.LocalDb.Update(uniteLink);
                    totalRowUpdated++;
                }
                else
                {
                    App.LocalDb.InsertOrReplace(new UniteLink(result)
                    {
                        UserId = CurrentUser.Id
                    });
                    totalRowInserted++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Deserialize UniteLink failed: " + ex);
            }
        }

        private void SyncMediasFromServer()
        {
            var medias = App.LocalDb.Table<Media>().ToList().FindAll(med => med.UserId == CurrentUser.Id && string.IsNullOrWhiteSpace(med.FileData) && med.IsActif == 1).ToArray();

            foreach (var med in medias)
            {
                SyncMediaFromServer(med);
            }
        }

        private void SyncMediaFromServer(Media media)
        {
            Task.Run(async () =>
            {
                if (await restClient.GetStreamAsync(ApiURI.URL_GET_MEDIA(CurrentAccount, CurrentUser.FkAccountId, media.AccountId, media.Year, media.Month, media.FileName)) is Stream response)
                {
                    var content = response?.ToBase64String();
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        media.FileData = content;
                        App.LocalDb.Update(media);
                    }
                }
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    Debug.WriteLine("GET_MEDIA " + media.FileName + " Successed!");
                }
                else if (task.IsFaulted && task.Exception?.GetBaseException().Message is string message)
                {
                    Debug.WriteLine("GET_MEDIA " + media.FileName + " Error: " + message);
                }
            }));
        }

        public async Task<bool> SyncToServer(int method)
        {
            var clients = App.LocalDb.Table<Client>().Where(c => c.UserId == CurrentUser.Id && c.IsToSync).ToArray();
            var addresses = App.LocalDb.Table<Address>().Where(i => i.UserId == CurrentUser.Id && i.IsToSync).ToArray();
            var messageries = App.LocalDb.Table<Message>().Where(m => m.UserId == CurrentUser.Id && m.IsToSync).ToArray();
            var interventions = App.LocalDb.Table<Intervention>().Where(i => i.UserId == CurrentUser.Id && i.IsToSync).ToArray();
            var lits = App.LocalDb.Table<LinkInterventionTask>().Where(lit => lit.UserId == CurrentUser.Id && lit.IsToSync).ToArray();
            var lips = App.LocalDb.Table<LinkInterventionProduct>().Where(lip => lip.UserId == CurrentUser.Id && lip.IsToSync).ToArray();
            var unite_links = App.LocalDb.Table<UniteLink>().Where(unl => unl.UserId == CurrentUser.Id && unl.IsToSync).ToArray();
            var mediaLinks = App.LocalDb.Table<MediaLink>().Where(medl => medl.UserId == CurrentUser.Id && medl.IsToSync).ToArray();

            JObject @params = new JObject();

            if (clients.LongLength > 0)
                @params.Add(new JProperty("clients", JArray.FromObject(clients).RemoveEmptyChildren()));

            if (addresses.LongLength > 0)
                @params.Add(new JProperty("addresses", JArray.FromObject(addresses).RemoveEmptyChildren()));

            if (messageries.LongLength > 0)
                @params.Add(new JProperty("messageries", JArray.FromObject(messageries).RemoveEmptyChildren(string.Join(",", messageries.FirstOrDefault().ExcludedProperties))));

            if (interventions.LongLength > 0)
                @params.Add(new JProperty("interventions", JArray.FromObject(interventions).RemoveEmptyChildren()));

            if (lits.LongLength > 0)
                @params.Add(new JProperty("lit", JArray.FromObject(lits).RemoveEmptyChildren()));

            if (lips.LongLength > 0)
                @params.Add(new JProperty("lip", JArray.FromObject(lips).RemoveEmptyChildren()));

            if (unite_links.LongLength > 0)
                @params.Add(new JProperty("unite_links", JArray.FromObject(unite_links).RemoveEmptyChildren()));

            if (mediaLinks.LongLength > 0)
                @params.Add(new JProperty("mediaLinks", JArray.FromObject(mediaLinks).RemoveEmptyChildren()));

            if (@params.Count > 0)
            {
                @params.Add(new JProperty("api_version", ApiURI.API_MOBILE_TO_SERVER_VERSION));
                @params.Add(new JProperty("appVersion", ApiURI.APP_VERSION));
            }
            else
            {
                return await Task.FromResult(true);
            }

            var response = await restClient.PostAsync(ApiURI.URL_BASE(CurrentAccount) + ApiURI.URL_SET_SYNC(CurrentUserName, Settings.CurrentPassword, method), @params);
            var responseContent = await response.Content.ReadAsStringAsync();
            Debug.WriteLine("RESPONSE: " + responseContent);
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<SetSyncResponse>(responseContent, App.DefaultDeserializeSettings);
                if (result != null && result.Success.Equals("OK"))
                {
                    foreach (var cli in clients)
                    {
                        if (result.Clients.FirstOrDefault(c => ((c.ServerId > 0 && c.ServerId == cli.ServerId) || Guid.TryParse(c.AppId, out Guid id) && cli.Id.Equals(id))) is SetSyncClientResponse clir && clir.ServerId > 0)
                        {
                            var client = cli.DeepCopy();
                            App.LocalDb.Delete(cli);
                            client.ServerId = clir.ServerId;
                            client.Code = clir.CodeId;
                            client.SynchronizationDate = DateTime.Now;
                            client.IsToSync = false;
                            App.LocalDb.Insert(client);
                        }
                        else
                        {
                            cli.IsToSync = false;
                            cli.SynchronizationDate = DateTime.Now;
                            App.LocalDb.Update(cli);
                        }
                    }

                    foreach (var adr in addresses)
                    {
                        if (result.Adresses.FirstOrDefault(a => ((a.ServerId > 0 && a.ServerId == adr.ServerId) || Guid.TryParse(a.AppId, out Guid id) && adr.Id.Equals(id))) is SetSyncAdresseResponse adrr && adrr.ServerId > 0)
                        {
                            var address = adr.DeepCopy();
                            App.LocalDb.Delete(adr);
                            address.ServerId = adrr.ServerId;
                            address.Code = adrr.CodeId;
                            address.SynchronizationDate = DateTime.Now;
                            address.IsToSync = false;
                            App.LocalDb.Insert(address);
                        }
                        else
                        {
                            adr.IsToSync = false;
                            adr.SynchronizationDate = DateTime.Now;
                            App.LocalDb.Update(adr);
                        }
                    }

                    foreach (var inter in interventions)
                    {
                        if (result.Interventions.FirstOrDefault(i => ((i.ServerId > 0 && i.ServerId == inter.ServerId) || Guid.TryParse(i.AppId, out Guid id) && inter.Id.Equals(id))) is SetSyncInterventionResponse intr && intr.ServerId > 0)
                        {
                            var intervention = inter.DeepCopy();
                            App.LocalDb.Delete(inter);
                            intervention.ServerId = intr.ServerId;
                            intervention.Code = intr.CodeId;
                            intervention.Nonce = intr.Nonce;
                            intervention.SynchronizationDate = DateTime.Now;
                            intervention.IsToSync = false;
                            App.LocalDb.Insert(intervention);
                        }
                        else
                        {
                            inter.IsToSync = false;
                            inter.SynchronizationDate = DateTime.Now;
                            App.LocalDb.Update(inter);
                        }
                    }

                    foreach (var mes in messageries)
                    {
                        if (result.Messageries.FirstOrDefault(m => ((m.ServerId > 0 && m.ServerId == mes.ServerId) || Guid.TryParse(m.AppId, out Guid id) && mes.Id.Equals(id))) is SetSyncMessageResponse mesr && mesr.ServerId > 0)
                        {
                            var message = mes.DeepCopy();
                            App.LocalDb.Delete(mes);
                            message.ServerId = mesr.ServerId;
                            message.SynchronizationDate = DateTime.Now;
                            message.IsToSync = false;
                            App.LocalDb.Insert(message);
                        }
                        else
                        {
                            mes.IsToSync = false;
                            mes.SynchronizationDate = DateTime.Now;
                            App.LocalDb.Update(mes);
                        }
                    }

                    foreach (var lit in lits)
                    {
                        if (result.Lits.FirstOrDefault(l => ((l.ServerId > 0 && l.ServerId == lit.ServerId) || Guid.TryParse(l.AppId, out Guid id) && lit.Id.Equals(id))) is SetSyncLitResponse litr && litr.ServerId > 0)
                        {
                            var linkInterventionTask = lit.DeepCopy();
                            App.LocalDb.Delete(lit);
                            linkInterventionTask.ServerId = litr.ServerId;
                            linkInterventionTask.SynchronizationDate = DateTime.Now;
                            linkInterventionTask.IsToSync = false;
                            App.LocalDb.Insert(linkInterventionTask);
                        }
                        else
                        {
                            lit.IsToSync = false;
                            lit.SynchronizationDate = DateTime.Now;
                            App.LocalDb.Update(lit);
                        }
                    }

                    foreach (var lip in lips)
                    {
                        if (result.Lips.FirstOrDefault(l => ((l.ServerId > 0 && l.ServerId == lip.ServerId) || Guid.TryParse(l.AppId, out Guid id) && lip.Id.Equals(id))) is SetSyncLipResponse lipr && lipr.ServerId > 0)
                        {
                            var linkInterventionProduct = lip.DeepCopy();
                            App.LocalDb.Delete(lip);
                            linkInterventionProduct.ServerId = lipr.ServerId;
                            linkInterventionProduct.SynchronizationDate = DateTime.Now;
                            linkInterventionProduct.IsToSync = false;
                            App.LocalDb.Insert(linkInterventionProduct);
                        }
                        else
                        {
                            lip.IsToSync = false;
                            lip.SynchronizationDate = DateTime.Now;
                            App.LocalDb.Update(lip);
                        }
                    }

                    foreach (var unl in unite_links)
                    {
                        if (result.UniteLinks.FirstOrDefault(u => ((u.ServerId > 0 && u.ServerId == unl.ServerId) || Guid.TryParse(u.AppId, out Guid id) && unl.Id.Equals(id))) is SetSyncUniteLinkResponse unlr && unlr.ServerId > 0)
                        {
                            var uniteLink = unl.DeepCopy();
                            App.LocalDb.Delete(unl);
                            uniteLink.ServerId = unlr.ServerId;
                            uniteLink.SynchronizationDate = DateTime.Now;
                            uniteLink.IsToSync = false;
                            App.LocalDb.Insert(uniteLink);
                        }
                        else
                        {
                            unl.IsToSync = false;
                            unl.SynchronizationDate = DateTime.Now;
                            App.LocalDb.Update(unl);
                        }
                    }

                    foreach (var medl in mediaLinks)
                    {
                        if (result.UniteLinks.FirstOrDefault(m => ((m.ServerId > 0 && m.ServerId == medl.ServerId) || Guid.TryParse(m.AppId, out Guid id) && medl.Id.Equals(id))) is SetSyncUniteLinkResponse medlr && medlr.ServerId > 0)
                        {
                            var mediaLink = medl.DeepCopy();
                            App.LocalDb.Delete(medl);
                            mediaLink.ServerId = medlr.ServerId;
                            mediaLink.SynchronizationDate = DateTime.Now;
                            mediaLink.IsToSync = false;
                            App.LocalDb.Insert(mediaLink);
                        }
                        else
                        {
                            medl.IsToSync = false;
                            medl.SynchronizationDate = DateTime.Now;
                            App.LocalDb.Update(medl);
                        }
                    }

                    Debug.WriteLine("SET_SYNC Successed!");
                }
            }
            else
            {
                Debug.WriteLine("SET_SYNC Failed: ");
            }

            return await Task.FromResult(true);
        }

        public async Task SyncTrackingToServer()
        {
            try
            {
                var trackings = App.LocalDb.Table<Tracking>().ToList().FindAll(tr => tr.UserId == CurrentUser.Id && tr.IsActif == 1 && tr.IsToSync);

                if (trackings == null || trackings.Count == 0)
                    return;

                JObject @params = new JObject
                {
                    new JProperty("api_version", 1),
                    new JProperty("tracking", JArray.FromObject(trackings).RemoveEmptyChildren())
                };

                var result = await restClient.PostAsync<SyncResponse, JObject>(ApiURI.URL_BASE(CurrentAccount) + ApiURI.URL_SET_TRACKING(CurrentUserName, Settings.CurrentPassword), @params);

                if (result != null && result.Success.Equals("OK"))
                {
                    foreach (var tr in trackings)
                    {
                        tr.IsToSync = false;
                        App.LocalDb.Update(tr);
                    }

                    Debug.WriteLine("SET_TRACKING Successed!");
                }
                else
                {
                    Debug.WriteLine("SET_TRACKING Error!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SET_TRACKING Error: " + ex?.GetBaseException().Message);
            }
        }

        public async Task SyncLocationToServer()
        {
            try
            {
                var locations = App.LocalDb.Table<Location>().ToList().FindAll(lo => lo.UserId == CurrentUser.Id && lo.IsToSync);

                if (locations == null || locations.Count == 0)
                    return;

                JObject @params = new JObject
                {
                    new JProperty("api_version", 3),
                    new JProperty("location", JArray.FromObject(locations).RemoveEmptyChildren())
                };

                var result = await restClient.PostAsync<SyncResponse, JObject>(ApiURI.URL_BASE(CurrentAccount) + ApiURI.URL_SET_GEOLOC(CurrentUserName, Settings.CurrentPassword), @params);

                if (result != null && result.Success.Equals("OK"))
                {
                    App.LocalDb.Table<Location>().Delete();
                    Debug.WriteLine("SET_GEOLOC Successed!");
                }
                else
                {
                    Debug.WriteLine("SET_GEOLOC Error!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SET_GEOLOC Error: " + ex?.GetBaseException().Message);
            }
        }

        public void SyncMediaToServer(int method)
        {
            var medias = App.LocalDb.Table<Media>().Where(med => med.UserId == CurrentUser.Id && med.IsToSync).ToArray();

            foreach (var med in medias)
            {
                if (!(App.LocalDb.Table<MediaLink>().ToList().FirstOrDefault(medl => medl.UserId == CurrentUser.Id
                    && ((!medl.FkMediaAppliId.Equals(Guid.Empty) && !med.Id.Equals(Guid.Empty) && medl.FkMediaAppliId.Equals(med.Id))
                    || (medl.FkMediaServerId > 0 && med.ServerId > 0 && medl.FkMediaServerId == med.ServerId))) is MediaLink mediaLink))
                {
                    continue;
                }

                SyncMediaToServer(mediaLink, med, method);
            }
        }

        private void SyncMediaToServer(MediaLink mediaLink, Media media, int method)
        {
            Task.Run(async () =>
            {
                string mediaInfo = "";
                mediaInfo += "&mAppId=" + media.Id.ToString();
                mediaInfo += "&mId=" + media.ServerId;
                mediaInfo += "&mCode=" + media.Code;
                mediaInfo += "&mFileName=" + media.FileName;
                mediaInfo += "&mFileSize=" + media.FileSize;
                mediaInfo += "&mImageWidth=" + media.ImageWidth;
                mediaInfo += "&mImageHeight=" + media.ImageHeight;
                mediaInfo += "&mLegend=" + media.Legend;
                mediaInfo += "&mCreatedOn=" + (media.AddDate?.ToUnixTimeSeconds(true) ?? 0);
                mediaInfo += "&mModifDate=" + (media.EditDate?.ToUnixTimeSeconds(true) ?? 0);
                mediaInfo += "&mActif=" + media.IsActif;
                mediaInfo += "&mMediaSize=" + App.LocalDb.Table<MediaLink>().ToList().Count(medl => medl.UserId == CurrentUser.Id
                                                && ((!medl.FkColumnAppliId.Equals(Guid.Empty) && !mediaLink.FkColumnAppliId.Equals(Guid.Empty) && medl.FkColumnAppliId.Equals(mediaLink.FkColumnAppliId))
                                                || (medl.FkColumnServerId > 0 && mediaLink.FkColumnServerId > 0 && medl.FkColumnServerId == mediaLink.FkColumnServerId)
                                                && medl.IsActif == 1));
                mediaInfo += "&intAppliId=" + mediaLink.FkColumnAppliId.ToString();
                mediaInfo += "&intId=" + mediaLink.FkColumnServerId;

                if (!string.IsNullOrWhiteSpace(media.FileName) && media.FileName.StartsWith("SIG_"))
                {
                    mediaInfo += "&mIsSign=1";
                }

                if (await WebService.Instance.PostMediaAsync<SetMediaResponse>(ApiURI.URL_SET_MEDIA(CurrentAccount, CurrentUserName, Settings.CurrentPassword, method, mediaInfo), media.FileData) is SetMediaResponse response)
                {
                    if (response.Success.Equals("OK"))
                    {
                        media.ServerId = response.MediaId;
                        media.Code = response.MediaCodeId;
                        media.FileName = response.MediaFileName;
                        media.FileData = null;
                        media.IsToSync = false;

                        mediaLink.ServerId = response.MediaLinkId;

                        App.LocalDb.Update(media);
                        App.LocalDb.Update(mediaLink);

                        Debug.WriteLine("SET_MEDIA Successed: " + media.FileName);
                    }
                    else
                    {
                        Debug.WriteLine("SET_MEDIA Error: " + media.FileName);
                    }
                }
            });
        }
    }
}