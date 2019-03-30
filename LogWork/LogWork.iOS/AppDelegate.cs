using Foundation;
using UIKit;
using Xamarin.Forms;

namespace LogWork.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Forms.Init();

            // Init Plugin
            ZXing.Net.Mobile.Forms.iOS.Platform.Init();

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();

            // Status Bar Style
            UIView statusBar = UIApplication.SharedApplication.ValueForKey(new NSString("statusBarWindow")).ValueForKey(new NSString("statusBar")) as UIView;
            statusBar.BackgroundColor = UIColor.FromRGB((float)Color.DodgerBlue.R, (float)Color.DodgerBlue.G, (float)Color.DodgerBlue.B);
            app.StatusBarStyle = UIStatusBarStyle.LightContent;

            UINavigationBar.Appearance.BarTintColor = UIColor.FromRGB((float)Color.DodgerBlue.R, (float)Color.DodgerBlue.G, (float)Color.DodgerBlue.B);
            UINavigationBar.Appearance.TintColor = UIColor.White;
            UIBarButtonItem.Appearance.TintColor = UIColor.White;

            Plugin.LocalNotification.Platform.iOS.LocalNotificationService.Init();

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                //Change UIApplicationState to suit different situations
                if (UIApplication.SharedApplication.ApplicationState != UIApplicationState.Active)
                {
                    Plugin.LocalNotification.Platform.iOS.LocalNotificationService.NotifyNotificationTapped(notification);
                }
                else
                {
                    base.ReceivedLocalNotification(application, notification);
                }
            }
            else
            {
                base.ReceivedLocalNotification(application, notification);
            }
        }
    }
}