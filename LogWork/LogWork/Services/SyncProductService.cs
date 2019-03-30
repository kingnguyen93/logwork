using Acr.UserDialogs;
using Newtonsoft.Json;
using LogWork.Constants;
using LogWork.Helpers;
using LogWork.IServices;
using LogWork.Models;
using LogWork.Views.Popups;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;

namespace LogWork.Services
{
    public class SyncProductService : BaseService, ISyncProductService
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

            Debug.WriteLine("Sync Product From Server Started!");

            var response = await restClient.GetStringAsync(ApiURI.URL_BASE(CurrentAccount) + ApiURI.URL_GET_PRODUCTS(CurrentUserName, Settings.CurrentPassword, Settings.LastSyncInvoice));
            if (!string.IsNullOrWhiteSpace(response) && !response.Equals("[]") && !(response.Contains("SUCCESS\":\"No result") || response.Equals("{\"SUCCESS\":\"No result\"}")))
            {
                ProcessData(response);
            }
            else
            {
                SetLastSyncProduct();
            }

            Debug.WriteLine("Sync Product Time: " + DateTime.Now.Subtract(timeStart).TotalSeconds);
            Debug.WriteLine("Total Product Inserted: " + totalRowInserted);
            Debug.WriteLine("Total Product Updated: " + totalRowUpdated);

            Debug.WriteLine("Sync Product From Server Ended!");

            return await Task.FromResult(true);
        }

        private void ProcessData(string response)
        {
            var result = JsonConvert.DeserializeObject<List<ProductSync>>(response, App.DefaultDeserializeSettings);

            if (result != null)
            {
                if (DoSync(result))
                {
                    SetLastSyncProduct();
                }

                return;
            }
        }

        private void SetLastSyncProduct()
        {
            Settings.LastSyncProduct = (long)DateTime.Now.ToUnixTimestamp();

            if (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("APP_LAST_SYNCHRO_PRODUCT")) is Setting lastSyncProduct)
            {
                lastSyncProduct.Value = Settings.LastSyncProduct.ToString();
                App.LocalDb.Update(lastSyncProduct);
            }
        }

        private bool DoSync(List<ProductSync> syncDatas)
        {
            try
            {
                App.LocalDb.BeginTransaction();

                foreach (var data in syncDatas)
                {
                    InsertProduct(data);
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

        private void InsertProduct(ProductSync result)
        {
            if (result == null || result.ServerId == 0)
                return;

            if (App.LocalDb.Table<Product>().FirstOrDefault(ip => ip.ServerId == result.ServerId) is Product product)
            {
                //PropertyExtension.UpdateProperties(result, product, ExcludedUpdateProperties);
                product.UpdateFromSync(result);

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
    }
}