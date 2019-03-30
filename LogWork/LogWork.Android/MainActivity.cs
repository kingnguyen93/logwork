using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace LogWork.Droid
{
    [Activity(Label = "Log Work", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);

            Forms.SetFlags("FastRenderers_Experimental");
            Forms.Init(this, savedInstanceState);

            // UserDialogs
            Acr.UserDialogs.UserDialogs.Init(this);

            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            ZXing.Mobile.MobileBarcodeScanner.Initialize(Application);

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);

            Plugin.LocalNotification.Platform.Droid.LocalNotificationService.NotifyNotificationTapped(Intent);
            Plugin.LocalNotification.Platform.Droid.LocalNotificationService.NotificationIconId = Resource.Drawable.icon;

            LoadApplication(new App());

            Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        }

        protected override void OnNewIntent(Intent intent)
        {
            Plugin.LocalNotification.Platform.Droid.LocalNotificationService.NotifyNotificationTapped(intent);

            base.OnNewIntent(intent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}