using LogWork.Helpers;
using LogWork.IServices;
using LogWork.Models.Response;
using System;
using TinyMVVM;
using TinyMVVM.IoC;

using Xamarin.Forms;

namespace LogWork.ViewModels
{
    public class BaseViewModel : TinyViewModel
    {
        protected readonly ISyncDataService syncService;

        protected readonly LoginResponse CurrentUser = Settings.CurrentUser;

        public BaseViewModel()
        {
            syncService = TinyIOC.Container.Resolve<ISyncDataService>();
        }

        public override void OnPushed(NavigationParameters parameters)
        {
            base.OnPushed(parameters);

            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await LocationHelper.CheckLocationPermission(false) && LocationHelper.IsGeolocationAvailable(false) && AppSettings.MobileLocationIsActive)
                {
                    await LocationHelper.StartListening(TimeSpan.FromSeconds(30), 20);
                }
            });
        }

        protected void RegisterMessagingCenter<TSender>(object subscriber, string message, Action<TSender> callback, TSender source = null) where TSender : class
        {
            MessagingCenter.Unsubscribe<TSender>(subscriber, message);
            MessagingCenter.Subscribe<TSender>(subscriber, message, callback, source);
        }

        protected void RegisterMessagingCenter<TSender, TArgs>(object subscriber, string message, Action<TSender, TArgs> callback, TSender source = null) where TSender : class
        {
            MessagingCenter.Unsubscribe<TSender>(subscriber, message);
            MessagingCenter.Subscribe<TSender, TArgs>(subscriber, message, callback, source);
        }
    }
}