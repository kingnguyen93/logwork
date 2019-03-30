using Acr.UserDialogs;
using LogWork.Helpers;
using LogWork.IServices;
using LogWork.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using TinyMVVM;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;

namespace LogWork.ViewModels.Login
{
    public class LoginViewModel : TinyViewModel
    {
        private readonly ILoginService loginService;

        private string account;
        public string Account { get => account; set => SetProperty(ref account, value); }

        private string userName;
        public string UserName { get => userName; set => SetProperty(ref userName, value); }

        private string password;
        public string Password { get => password; set => SetProperty(ref password, value); }

        public ICommand LoginCommand { get; set; }

        public LoginViewModel(ILoginService loginService)
        {
            this.loginService = loginService;

            LoginCommand = new Command(Login);
        }

        private void Login(object sender)
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
                return;

            if (string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(password))
            {
                CoreMethods.DisplayAlert(TranslateExtension.GetValue("alert_message_error"), TranslateExtension.GetValue("error_fulfill_login"), TranslateExtension.GetValue("alert_message_ok"));
                return;
            }

            UserDialogs.Instance.Loading(TranslateExtension.GetValue("alert_message_logging")).Show();

            IsBusy = true;

            Task.Run(async () =>
            {
                return await loginService.Login(Account, UserName, Password);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                UserDialogs.Instance.Loading().Hide();

                IsBusy = false;

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if ((Settings.CurrentUserId > 0 && Settings.CurrentUserId != task.Result.Id) || Settings.CurrentAccount != Account)
                        {
                            Settings.LastSync = "0";
                            if (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("APP_LAST_SYNCHRO")) is Setting lastSync)
                            {
                                lastSync.Value = "0";
                                App.LocalDb.Update(lastSync);
                            }
                            Settings.LastSyncProduct = 0;
                            if (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("APP_LAST_SYNCHRO_PRODUCT")) is Setting lastSyncProduct)
                            {
                                lastSyncProduct.Value = "0";
                                App.LocalDb.Update(lastSyncProduct);
                            }
                            ClearAllData();
                        }

                        Settings.CurrentAccount = Account;
                        Settings.CurrentUserName = UserName;
                        Settings.CurrentPassword = Password;

                        Settings.CurrentUserId = task.Result.Id;
                        Settings.CurrentUser = task.Result;

                        Settings.LoggedIn = true;

                        Application.Current.MainPage = ViewModelResolver.ResolveViewModel<SyncViewModel>();
                    }
                    else
                    {
                        CoreMethods.DisplayAlert(TranslateExtension.GetValue("alert_title_authentication_fail"), TranslateExtension.GetValue("alert_message_wrong_user_credentials"), TranslateExtension.GetValue("alert_message_ok"));
                    }
                }
                else
                {
                    if (task.IsFaulted && task.Exception?.GetBaseException().Message is string message)
                    {
                        //CoreMethods.DisplayAlert(TranslateExtension.GetValue("alert_title_error"), message, TranslateExtension.GetValue("alert_message_ok"));
                        Debug.WriteLine("LOGIN_ERROR: " + message);
                    }
                    CoreMethods.DisplayAlert(TranslateExtension.GetValue("alert_title_authentication_fail"), TranslateExtension.GetValue("alert_message_wrong_user_credentials"), TranslateExtension.GetValue("alert_message_ok"));
                }
            }));
        }

        private void ClearAllData()
        {
            App.LocalDb.DropTable<Filiale>();
            App.LocalDb.DropTable<Intervention>();
            App.LocalDb.DropTable<Client>();
            App.LocalDb.DropTable<Address>();
            App.LocalDb.DropTable<Chemin>();
            App.LocalDb.DropTable<UniteLink>();
            App.LocalDb.DropTable<Unite>();
            App.LocalDb.DropTable<UniteItem>();
            App.LocalDb.DropTable<LinkInterventionTask>();
            App.LocalDb.DropTable<Tasks>();
            App.LocalDb.DropTable<MediaLink>();
            App.LocalDb.DropTable<Media>();
            App.LocalDb.DropTable<LinkInterventionProduct>();
            App.LocalDb.DropTable<Product>();
            App.LocalDb.DropTable<CategoryTracking>();
            App.LocalDb.DropTable<Location>();
            App.LocalDb.DropTable<Models.Tracking>();
            App.LocalDb.DropTable<User>();
            App.LocalDb.DropTable<Message>();
            App.LocalDb.DropTable<Invoice>();
            App.LocalDb.DropTable<InvoiceProduct>();
            App.LocalDb.DropTable<Setting>();
            App.LocalDb.DropTable<SettingItem>();

            App.LocalDb.CreateTable<Filiale>();
            App.LocalDb.CreateTable<Intervention>();
            App.LocalDb.CreateTable<Client>();
            App.LocalDb.CreateTable<Address>();
            App.LocalDb.CreateTable<Chemin>();
            App.LocalDb.CreateTable<UniteLink>();
            App.LocalDb.CreateTable<Unite>();
            App.LocalDb.CreateTable<UniteItem>();
            App.LocalDb.CreateTable<LinkInterventionTask>();
            App.LocalDb.CreateTable<Tasks>();
            App.LocalDb.CreateTable<MediaLink>();
            App.LocalDb.CreateTable<Media>();
            App.LocalDb.CreateTable<LinkInterventionProduct>();
            App.LocalDb.CreateTable<Product>();
            App.LocalDb.CreateTable<CategoryTracking>();
            App.LocalDb.CreateTable<Location>();
            App.LocalDb.CreateTable<Models.Tracking>();
            App.LocalDb.CreateTable<User>();
            App.LocalDb.CreateTable<Message>();
            App.LocalDb.CreateTable<Invoice>();
            App.LocalDb.CreateTable<InvoiceProduct>();
            App.LocalDb.CreateTable<Setting>();
            App.LocalDb.CreateTable<SettingItem>();
        }
    }
}