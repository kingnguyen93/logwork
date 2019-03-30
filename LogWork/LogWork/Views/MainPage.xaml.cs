using LogWork.Helpers;
using LogWork.Models;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using TinyMVVM;
using Xamarin.Forms.Xaml;
using Task = System.Threading.Tasks.Task;

namespace LogWork
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage
    {
        private int lastPage;

        public MainPage()
        {
            InitializeComponent();

            Detail = new NavigationContainer(ViewModelResolver.ResolveViewModel<ViewModels.Interventions.InterventionsViewModel>())
            {
                BarTextColor = Color.FromHex("#FFFFFF")
            };
            lastPage = (int)MenuItemType.Interventions;
        }

        public async Task NavigateFromMenu(int id)
        {
            if (lastPage == id)
            {
                IsPresented = false;
                return;
            }

            try
            {
                Page newPage = null;

                switch (id)
                {
                    case (int)MenuItemType.Interventions:
                        newPage = ViewModelResolver.ResolveViewModel<ViewModels.Interventions.InterventionsViewModel>();
                        break;

                    case (int)MenuItemType.InterventionsNotAssigned:
                        newPage = ViewModelResolver.ResolveViewModel<ViewModels.InterventionsNotAssigned.InterventionsNotAssignedViewModel>();
                        break;

                    case (int)MenuItemType.Quote:
                        newPage = ViewModelResolver.ResolveViewModel<ViewModels.Quotes.QuotesViewModel>();
                        break;

                    case (int)MenuItemType.Invoice:
                        newPage = ViewModelResolver.ResolveViewModel<ViewModels.Invoices.InvoicesViewModel>();
                        break;

                    case (int)MenuItemType.Messages:
                        newPage = ViewModelResolver.ResolveViewModel<ViewModels.Messages.MessagesViewModel>();
                        break;

                    case (int)MenuItemType.Customers:
                        newPage = ViewModelResolver.ResolveViewModel<ViewModels.Customers.CustomersViewModel>();
                        break;

                    case (int)MenuItemType.Addresses:
                        newPage = ViewModelResolver.ResolveViewModel<ViewModels.Addresses.AddressesViewModel>();
                        break;

                    case (int)MenuItemType.Tracking:
                        newPage = ViewModelResolver.ResolveViewModel<ViewModels.Tracking.TrackingViewModel>();
                        break;

                    case (int)MenuItemType.Settings:
                        newPage = ViewModelResolver.ResolveViewModel<ViewModels.SystemSettings.SettingsViewModel>();
                        break;

                    case (int)MenuItemType.About:
                        newPage = ViewModelResolver.ResolveViewModel<ViewModels.About.AboutViewModel>();
                        break;

                    case (int)MenuItemType.LogOut:
                        await LogOut();
                        break;
                }

                if (newPage != null)
                {
                    if (Detail is NavigationPage)
                    {
                        ((NavigationPage)Detail).NotifyAllChildrenPopped();
                    }
                    if (Detail is INavigationService)
                    {
                        ((INavigationService)Detail).NotifyChildrenPageWasPopped();
                    }

                    Detail = new NavigationContainer(newPage)
                    {
                        BarBackgroundColor = Color.FromHex("#2196F3"),
                        BarTextColor = Color.White
                    };
                    lastPage = id;

                    if (Device.RuntimePlatform == Device.Android)
                        await Task.Delay(100);
                    
                    IsPresented = false;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(TranslateExtension.GetValue("alert_title_error"), ex.GetBaseException().Message, TranslateExtension.GetValue("alert_message_ok"));
            }
        }

        private async Task LogOut()
        {
            if (await DisplayAlert(TranslateExtension.GetValue("alert_title_log_out"), TranslateExtension.GetValue("alert_message_log_out_confirm"), TranslateExtension.GetValue("alert_message_yes"), TranslateExtension.GetValue("alert_message_no")))
            {
                Settings.RemoveCurrentUserName();
                Settings.RemoveCurrentPassword();
                
                Settings.RemoveCurrentUser();

                Settings.RemoveLoggedIn();

                Application.Current.MainPage = new NavigationContainer(ViewModelResolver.ResolveViewModel<ViewModels.Login.LoginViewModel>())
                {
                    BarBackgroundColor = Color.DodgerBlue,
                    BarTextColor = Color.White
                };
            }
        }
    }
}