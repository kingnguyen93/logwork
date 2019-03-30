using Acr.UserDialogs;
using LogWork.Helpers;
using LogWork.IServices;
using LogWork.Models;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms.Extensions;
using TinyMVVM;

namespace LogWork.ViewModels.Tracking
{
    public class TrackingViewModel : TinyViewModel
    {
        private readonly ISyncDataService syncService;
        private readonly int CurrentUserId = Settings.CurrentUserId;

        private List<Models.Tracking> listTracking;
        public List<Models.Tracking> ListTracking { get => listTracking; set => SetProperty(ref listTracking, value); }

        private List<CategoryTracking> listCategoryTracking;
        public List<CategoryTracking> ListCategoryTracking { get => listCategoryTracking; set => SetProperty(ref listCategoryTracking, value); }

        public ICommand GetSyncCommand { get; set; }
        public ICommand TrackingSelectedCommand { get; private set; }
        public ICommand ScanTrackingCommand { get; private set; }
        public ICommand ManualTrackingCommand { get; private set; }

        public TrackingViewModel(ISyncDataService syncService)
        {
            this.syncService = syncService;

            GetSyncCommand = new AwaitCommand(GetSync);
            TrackingSelectedCommand = new AwaitCommand<Models.Tracking>(TrackingSelected);
            ScanTrackingCommand = new AwaitCommand(ScanTracking);
            ManualTrackingCommand = new AwaitCommand(ManualTracking);
        }

        public override void Init(NavigationParameters parameters)
        {
            base.Init(parameters);

            ListCategoryTracking = App.LocalDb.Table<CategoryTracking>().ToList().FindAll(ct => ct.UserId == CurrentUserId && ct.IsActif == 1);

            GetListTracking();
        }

        private void GetListTracking()
        {
            ListTracking = App.LocalDb.Table<Models.Tracking>().ToList().FindAll(tr => tr.UserId == CurrentUserId && tr.IsActif == 1).OrderByDescending(tr => tr.Date).ToList();
        }

        private void GetSync(object sender, TaskCompletionSource<bool> tcs)
        {
            syncService.SyncFromServer(method: 2, onSuccess: GetListTracking, showOverlay: true);

            tcs.TrySetResult(true);
        }

        private void TrackingSelected(Models.Tracking value, TaskCompletionSource<bool> tcs)
        {
            tcs.TrySetResult(true);
        }

        private async void ScanTracking(object sender, TaskCompletionSource<bool> tcs)
        {
            try
            {
                if (!await CameraHelper.CheckCameraPermission())
                    return;

                var result = await BarcodeHelper.Instance.ScanQrCode();
                //var result = @"https://test.organilog.com/script/tracking/qrcode.php?type=client&id=267210&user=0&nonce=REPAS";

                if (!string.IsNullOrWhiteSpace(result))
                {
                    if (!(result.Length < 20 || result.ToLower().IndexOf("organilog.com") < 0))
                    {
                        var url = new Uri(result);
                        if (url.Host.Split('.')[0].Equals(Settings.CurrentAccount))
                        {
                            Dictionary<string, string> pars = new Dictionary<string, string>();

                            var queries = url.Query.Replace("?", "").Split('&');
                            foreach (var query in queries)
                            {
                                var pair = query.Split('=');
                                pars.Add(pair[0], pair[1]);
                            }

                            AddTracking(pars["type"], Convert.ToInt32(pars["id"]), pars["type"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SCAN_TRACKING_ERROR: " + ex.GetBaseException().Message);
                await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), ex.GetBaseException().Message, TranslateExtension.GetValue("ok"));
            }
            finally
            {
                tcs.TrySetResult(true);
            }
        }

        private async void ManualTracking(object sender, TaskCompletionSource<bool> tcs)
        {
            try
            {
                string result = await CoreMethods.DisplayActionSheet(TranslateExtension.GetValue("page_title_tracking"), TranslateExtension.GetValue("alert_message_cancel"), null, ListCategoryTracking.Select(ct => ct.Title).ToArray());

                if (!string.IsNullOrWhiteSpace(result) && !result.Equals(TranslateExtension.GetValue("alert_message_cancel")))
                {
                    if (ListCategoryTracking.Find(ct => ct.Title.Equals(result)) is CategoryTracking categoryTracking)
                    {
                        AddTracking(categoryTracking.Title, categoryTracking.ServerId, "custom");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MANUAL_TRACKING_ERROR: " + ex.GetBaseException().Message);
                await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), ex.GetBaseException().Message, TranslateExtension.GetValue("ok"));
            }
            finally
            {
                tcs.TrySetResult(true);
            }
        }

        private async void AddTracking(string title, int itemId, string type)
        {
            try
            {
                Position position = null;

                if (await LocationHelper.CheckLocationPermission(false) && LocationHelper.IsGeolocationAvailable(false) && LocationHelper.IsGeolocationEnabled(false))
                {
                    position = await LocationHelper.GetCurrentPosition(showOverlay: false);

                    if (position == null || (position.Latitude == 0 && position.Longitude == 0))
                    {
                        UserDialogs.Instance.Toast(new ToastConfig(TranslateExtension.GetValue("alert_message_get_location_error")));
                        //await CoreMethods.DisplayAlert(TranslateExtension.GetValue("page_title_tracking"), TranslateExtension.GetValue("alert_message_get_location_error"), TranslateExtension.GetValue("ok"));
                        //return;
                    }
                }

                var tracking = new Models.Tracking()
                {
                    Id = Guid.NewGuid(),
                    UserId = CurrentUserId,
                    Nonce = title,
                    ItemId = itemId,
                    Type = type,
                    Latitude = position?.Latitude ?? 0,
                    Longitude = position?.Longitude ?? 0,
                    Altitude = position?.Altitude ?? 0,
                    IsActif = 1,
                    Date = DateTime.Now,
                    IsToSync = true
                };

                App.LocalDb.InsertOrReplace(tracking);

                GetListTracking();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ADD_TRACKING_ERROR: " + ex.GetBaseException().Message);
                await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), ex.GetBaseException().Message, TranslateExtension.GetValue("ok"));
            }
        }
    }
}