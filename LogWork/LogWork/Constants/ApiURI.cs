using System;
using Xamarin.Forms;

namespace LogWork.Constants
{
    public static class ApiURI
    {
        public static string APP_VERSION_NUMBER => "3.0";

        public static string APP_VERSION = "v3.0";
        public static string API_MOBILE_TO_SERVER_VERSION = "10";
        public static string API_MOBILE_TO_SERVER_MEDIA_VERSION = "11";
        public static string API_SERVER_TO_MOBILE_VERSION = "14";
        public static string API_SERVER_TO_MOBILE_INVOICE_VERSION = "2";
        public static string API_SERVER_TO_MOBILE_PRODUCT_VERSION = "9";
        public static string API_HISTORY_VERSION = "1";

        private static readonly int deviceType = Device.RuntimePlatform == Device.Android ? 1 : 2;

        public static string URL_BASE(string account) => string.Format("http://{0}.organilog.com/script/api/", account);

        public static string URL_GET_LOGIN(string userName, string password) =>
            string.Format("get-login.php?user_name={0}&password={1}", userName, password);

        public static string URL_GET_SYNC(string userName, string password, string lastSync = "0") =>
            string.Format("get-sync.php?user_name={0}&password={1}&device_type={2}&api_version={3}&appVersion={4}&wifi=1&last_synchro={5}",
                userName, password, deviceType, API_SERVER_TO_MOBILE_VERSION, APP_VERSION, lastSync);

        public static string URL_GET_MEDIA(string account, int accountId, int userId, string mediaYear, string mediaMonth, string mediaName) =>
            string.Format("http://{0}.organilog.com/media/{1}/{2}/{3}/{4}/{5}", account, accountId, userId, mediaYear, mediaMonth, mediaName);

        public static string URL_SET_MEDIA(string account, string userName, string password, int method, string mediaInfo) =>
            string.Format("http://{0}.organilog.com/script/api/set-media.php?user_name={1}&password={2}&api_version={3}&appVersion={4}&sMethod={5}{6}",
                account, userName, password, API_MOBILE_TO_SERVER_MEDIA_VERSION, APP_VERSION, method, mediaInfo);

        public static string URL_PDF(string account, int intId, string nonce) =>
            string.Format("http://{0}.organilog.com/intervention_view.php?id={1}&nonce={2}", account, intId, nonce);

        public static string URL_SET_SYNC(string userName, string password, int method, int netWorkStatus = 1) =>
            string.Format("set-sync.php?user_name={0}&password={1}&device_type={2}&api_version={3}&appVersion={4}&sMethod={5}&wifi={6}&format=json",
                userName, password, deviceType, API_MOBILE_TO_SERVER_VERSION, APP_VERSION, method, netWorkStatus);

        public static string URL_SET_TRACKING(string userName, string password) =>
            string.Format("set-tracking.php?user_name={0}&password={1}&api_version={2}&appVersion={3}",
                userName, password, 1, APP_VERSION);

        public static string URL_SET_GEOLOC(string userName, string password) =>
            string.Format("set-geoloc.php?user_name={0}&password={1}&api_version={2}&appVersion={3}",
                userName, password, 3, APP_VERSION);
        
        public static string URL_GET_INTERVENTION_HISTORY(string userName, string password, int intId) =>
            string.Format("get-interventions-historique-sync.php?user_name={0}&password={1}&device_type={2}&api_version={3}&appVersion={4}&&int_id={5}",
                userName, password, deviceType, API_HISTORY_VERSION, APP_VERSION, intId);

        public static string URL_SET_INTERVENTION_ASSIGN(string userName, string password, int intId) =>
            string.Format("set-intervention-assignation.php?user_name={0}&password={1}&device_type={2}&api_version={3}&appVersion={4}&&intervention_id={5}",
                userName, password, deviceType, API_HISTORY_VERSION, APP_VERSION, intId);

        public static string URL_GET_PRODUCTS(string userName, string password, long lastSync = 0) =>
            string.Format("get-products.php?user_name={0}&password={1}&device_type=${2}&api_version=${3}&last_synchro={4}",
                userName, password, deviceType, API_SERVER_TO_MOBILE_VERSION, lastSync);

        public static string URL_GET_PRODUCT_BY_NAME(string userName, string password, string nom) =>
            string.Format("get-product-by-name.php?user_name={0}&password={1}&device_type=${2}&name=${3}",
                userName, password, deviceType, nom);

        public static string URL_GET_INVOICE(string userName, string password, long lastSync = 0) =>
            string.Format("get-invoices-api.php?user_name={0}&password={1}&device_type={2}&api_version={3}&appVersion={4}&wifi=1&last_synchro={5}",
                userName, password, deviceType, API_SERVER_TO_MOBILE_INVOICE_VERSION, APP_VERSION, lastSync);

        public static string URL_SET_INVOICE(string userName, string password) =>
            string.Format("set-invoices-api.php?user_name={0}&password={1}&api_version={2}&appVersion={3}",
                userName, password, API_MOBILE_TO_SERVER_VERSION, APP_VERSION);
    }
}