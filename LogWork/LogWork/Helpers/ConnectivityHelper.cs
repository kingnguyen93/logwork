using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;

namespace LogWork.Helpers
{
    public class ConnectivityHelper
    {
        public static bool IsNetworkAvailable(bool showAlert = true)
        {
            if (!CrossConnectivity.IsSupported)
                return true;

            if (!CrossConnectivity.Current.IsConnected)
            {
                if (showAlert)
                {
                    //You are offline, notify the user
                    Application.Current.MainPage?.DisplayAlert("Network", TranslateExtension.GetValue("alert_no_internt_message"), "OK");
                }

                return false;
            }

            return true;
        }
    }
}