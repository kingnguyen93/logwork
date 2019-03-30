using LogWork.IServices;
using System;
using Xamarin.Forms;

namespace LogWork.Helpers
{
    public class SyncHelper
    {
        private static readonly Lazy<SyncHelper> lazy = new Lazy<SyncHelper>(() => new SyncHelper());
        public static SyncHelper Instance => lazy.Value;

        protected readonly ISyncDataService syncService;

        public bool IsRunning;

        private SyncHelper()
        {
            syncService = TinyIoC.TinyIoCContainer.Current.Resolve<ISyncDataService>();
        }

        public void StartAutoSync(int interval)
        {
            if (IsRunning)
                return;

            IsRunning = true;

            Device.StartTimer(TimeSpan.FromMinutes(interval), () =>
            {
                if (!IsRunning)
                    return false;

                syncService.SyncFromServer(method: 1, onSuccess: OnSuccess, showOverlay: false);

                return IsRunning;
            });
        }

        private void OnSuccess()
        {
        }

        public void StopAutoSync()
        {
            IsRunning = false;
        }
    }
}