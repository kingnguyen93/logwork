using Acr.UserDialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LogWork.Constants;
using LogWork.Helpers;
using LogWork.IServices;
using LogWork.Models;
using LogWork.Models.Response;
using LogWork.Models.SetSync;
using LogWork.Views.Popups;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;

namespace LogWork.Services
{
    public class SyncInvoiceService : BaseService, ISyncInvoiceService
    {
        private readonly string[] ExcludedUpdateProperties =
        {
            "Id",
            "UserId",
            "AccountId",
            "IsToSync"
        };

        private DateTime timeStart;
        private int totalRowInserted, totalRowUpdated;

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
                return await SyncFromServer(method);
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

            Debug.WriteLine("Sync Invoice From Server Started!");

            if (Settings.LastSyncInvoice != 0)
            {
                await SyncToServer(method);
            }

            var response = await restClient.GetStringAsync(ApiURI.URL_BASE(CurrentAccount) + ApiURI.URL_GET_INVOICE(CurrentUserName, Settings.CurrentPassword, Settings.LastSyncInvoice));
            if (!string.IsNullOrWhiteSpace(response) && !response.Equals("[]"))
            {
                ProcessData(response);
            }
            else
            {
                SetLastSyncInvoice();
            }

            Debug.WriteLine("Sync Invoice Time: " + DateTime.Now.Subtract(timeStart).TotalSeconds);
            Debug.WriteLine("Total Invoice Inserted: " + totalRowInserted);
            Debug.WriteLine("Total Invoice Updated: " + totalRowUpdated);
            Debug.WriteLine("Sync Invoice From Server Ended!");

            return await Task.FromResult(true);
        }

        private void ProcessData(string response)
        {
            var result = SyncData.FromJson(response);

            if (result != null)
            {
                if (DoSync(result))
                {
                    SetLastSyncInvoice();
                }

                return;
            }
        }

        private void SetLastSyncInvoice()
        {
            Settings.LastSyncInvoice = (long)DateTime.Now.ToUnixTimestamp();

            if (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("APP_LAST_SYNCHRO_INVOICE")) is Setting lastSyncInvoice)
            {
                lastSyncInvoice.Value = Settings.LastSyncInvoice.ToString();
                App.LocalDb.Update(lastSyncInvoice);
            }
        }

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
                                case "i":
                                    InsertOrUpdateInvoice(item.V);
                                    break;

                                case "ip":
                                    InsertOrUpdateInvoiceProduct(item.V);
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                }

                App.LocalDb.Commit();

                return true;
            }
            catch (Exception ex)
            {
                App.LocalDb.Rollback();

                totalRowInserted = 0;

                Debug.WriteLine("Sync Error: ");
                Debug.WriteLine(ex.GetBaseException().Message);
                Debug.WriteLine(ex);

                return false;
            }
        }

        private void InsertOrUpdateInvoice(Dictionary<string, string> data)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<Models.Response.InvoiceResponse>(JsonConvert.SerializeObject(data), App.DefaultDeserializeSettings);
                if (result == null || result.ServerId == 0)
                    return;
                if (App.LocalDb.Table<Invoice>().FirstOrDefault(iv => iv.ServerId == result.ServerId) is Invoice invoice)
                {
                    //PropertyExtension.UpdateProperties(result, invoice, ExcludedUpdateProperties);
                    invoice.UpdateFromResponse(result);

                    invoice.UserId = CurrentUser.Id;

                    App.LocalDb.Update(invoice);
                    totalRowUpdated++;
                }
                else
                {
                    App.LocalDb.InsertOrReplace(new Invoice(result)
                    {
                        UserId = CurrentUser.Id,
                        AccountId = CurrentUser.FkAccountId,
                    });
                    totalRowInserted++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Insert Invoice failed: " + ex);
            }
        }

        private void InsertOrUpdateInvoiceProduct(Dictionary<string, string> data)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<InvoiceProductResponse>(JsonConvert.SerializeObject(data), App.DefaultDeserializeSettings);
                if (result == null || result.ServerId == 0)
                    return;
                
                if (App.LocalDb.Table<InvoiceProduct>().FirstOrDefault(ip => ip.ServerId == result.ServerId) is InvoiceProduct invoiceProduct)
                {
                    //PropertyExtension.UpdateProperties(result, invoiceProduct, ExcludedUpdateProperties);
                    invoiceProduct.UpdateFromResponse(result);

                    invoiceProduct.UserId = CurrentUser.Id;

                    App.LocalDb.Update(invoiceProduct);
                    totalRowUpdated++;
                }
                else
                {
                    App.LocalDb.InsertOrReplace(new InvoiceProduct(result)
                    {
                        UserId = CurrentUser.Id,
                        AccountId = CurrentUser.FkAccountId,
                    });
                    totalRowInserted++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Insert InvoiceProduct failed: " + ex);
            }
        }

        public async Task<bool> SyncToServer(int method)
        {
            var invoices = App.LocalDb.Table<Invoice>().Where(inv => inv.UserId == CurrentUser.Id && inv.IsToSync).ToArray();
            var invoices_lines = App.LocalDb.Table<InvoiceProduct>().Where(ip => ip.UserId == CurrentUser.Id && ip.IsToSync).ToArray();

            JObject @params = new JObject();

            if (invoices.LongLength > 0)
                @params.Add(new JProperty("invoices", JArray.FromObject(invoices).RemoveEmptyChildren(invoices.FirstOrDefault()?.PropertyIgnore)));

            if (invoices_lines.LongLength > 0)
                @params.Add(new JProperty("invoices_lines", JArray.FromObject(invoices_lines).RemoveEmptyChildren(invoices_lines.FirstOrDefault()?.PropertyIgnore)));

            if (@params.Count > 0)
            {
                @params.Add(new JProperty("api_version", ApiURI.API_MOBILE_TO_SERVER_VERSION));
                @params.Add(new JProperty("appVersion", ApiURI.APP_VERSION));
            }
            else
            {
                return await Task.FromResult(true);
            }

            var response = await restClient.PostAsync(ApiURI.URL_BASE(CurrentAccount) + ApiURI.URL_SET_INVOICE(CurrentUserName, Settings.CurrentPassword), @params);
            var responseContent = await response.Content.ReadAsStringAsync();
            Debug.WriteLine("RESPONSE: " + responseContent);
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<SetSyncInvoiceResponse>(responseContent, App.DefaultDeserializeSettings);
                if (result != null && result.Success.Equals("OK"))
                {
                    foreach (var inv in invoices)
                    {
                        if (result.Invoices.FirstOrDefault(c => ((c.ServerId > 0 && c.ServerId == inv.ServerId) || Guid.TryParse(c.AppId, out Guid id) && inv.Id.Equals(id))) is Models.SetSync.InvoiceResponse invr && invr.ServerId > 0)
                        {
                            var invoice = inv.DeepCopy();
                            App.LocalDb.Delete(inv);
                            invoice.ServerId = invr.ServerId;
                            invoice.InvoiceNumber = invr.InvoiceNumber;
                            //invoice.SynchronizationDate = DateTime.Now;
                            invoice.IsToSync = false;
                            App.LocalDb.Insert(invoice);
                        }
                        else
                        {
                            inv.IsToSync = false;
                            //inv.SynchronizationDate = DateTime.Now;
                            App.LocalDb.Update(inv);
                        }
                    }

                    foreach (var invl in invoices_lines)
                    {
                        if (result.InvoicesLines.FirstOrDefault(a => ((a.ServerId > 0 && a.ServerId == invl.ServerId) || Guid.TryParse(a.AppId, out Guid id) && invl.Id.Equals(id))) is Models.SetSync.InvoiceLineResponse invlr && invlr.ServerId > 0)
                        {
                            var invoiceProduct = invl.DeepCopy();
                            App.LocalDb.Delete(invl);
                            invoiceProduct.ServerId = invlr.ServerId;
                            //invoiceProduct.SynchronizationDate = DateTime.Now;
                            invoiceProduct.IsToSync = false;
                            App.LocalDb.Insert(invoiceProduct);
                        }
                        else
                        {
                            invl.IsToSync = false;
                            //invl.SynchronizationDate = DateTime.Now;
                            App.LocalDb.Update(invl);
                        }
                    }

                    Debug.WriteLine("SET_SYNC_INVOICE Successed!");
                }
            }
            else
            {
                Debug.WriteLine("SET_SYNC_INVOICE Failed: ");
            }

            return await Task.FromResult(true);
        }
    }
}