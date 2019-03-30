using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using LogWork.IServices;
using LogWork.Models;
using LogWork.Services;
using Plugin.LocalNotification;
using SQLite;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using TinyMVVM;
using TinyMVVM.IoC;
using Xamarin.Forms;
using Xamarin.Forms.Converters;
using Xamarin.Forms.Extensions;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace LogWork
{
    public partial class App : Application
    {
        public static bool IsDeveloperMode { get; set; } = false; // For testing or debugging

        public static SQLiteConnection LocalDb { get; set; }

        public static JsonSerializerSettings DefaultDeserializeSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Converters = new List<JsonConverter> { new IgnoreDataTypeConverter() }
        };

        public App()
        {
            InitializeComponent();

            Init();

            AppSettings.ReloadSetting();

            AssetsExtension.InitAssetsExtension("AppResources.Assets", typeof(App).GetTypeInfo().Assembly);
            ImageResourceExtension.InitImageResourceExtension("AppResources.Assets", typeof(App).GetTypeInfo().Assembly);
            TranslateExtension.InitTranslateExtension("AppResources.Localization.Resources", CultureInfo.CurrentCulture, typeof(App).GetTypeInfo().Assembly);

            RegisterDependency();

            if (Settings.LoggedIn)
            {
                MainPage = ViewModelResolver.ResolveViewModel<ViewModels.SyncViewModel>();
            }
            else
            {
                MainPage = new NavigationContainer(ViewModelResolver.ResolveViewModel<ViewModels.Login.LoginViewModel>())
                {
                    BarBackgroundColor = Color.FromHex("#2196F3"),
                    BarTextColor = Color.White
                };
            }
        }

        private void RegisterDependency()
        {
            TinyIOC.Container.Register<IRestClient, RestClient>().AsSingleton();

            TinyIOC.Container.Register<ILoginService, LoginService>().AsMultiInstance();
            TinyIOC.Container.Register<ISyncDataService, SyncDataService>().AsMultiInstance();

            TinyIOC.Container.Register<IInterventionService, InterventionService>().AsMultiInstance();
            TinyIOC.Container.Register<IClientService, ClientService>().AsMultiInstance();
            TinyIOC.Container.Register<IAddressService, AddressService>().AsMultiInstance();
            TinyIOC.Container.Register<IContractService, ContractService>().AsMultiInstance();

            TinyIOC.Container.Register<ISyncProductService, SyncProductService>().AsMultiInstance();

            //invoice
            TinyIOC.Container.Register<IInvoiceService, InvoiceService>().AsMultiInstance();
            TinyIOC.Container.Register<ISyncInvoiceService, SyncInvoiceService>().AsMultiInstance();
        }

        private void Init()
        {
            LocalDb = DependencyService.Get<ILocalDbService>().GetDbConnection();

            LocalDb.CreateTable<Filiale>();
            LocalDb.CreateTable<Intervention>();
            LocalDb.CreateTable<Client>();
            LocalDb.CreateTable<Address>();
            LocalDb.CreateTable<Chemin>();
            LocalDb.CreateTable<UniteLink>();
            LocalDb.CreateTable<Unite>();
            LocalDb.CreateTable<UniteItem>();
            LocalDb.CreateTable<LinkInterventionTask>();
            LocalDb.CreateTable<Tasks>();
            LocalDb.CreateTable<MediaLink>();
            LocalDb.CreateTable<Media>();
            LocalDb.CreateTable<LinkInterventionProduct>();
            LocalDb.CreateTable<Product>();
            LocalDb.CreateTable<CategoryTracking>();
            LocalDb.CreateTable<Location>();
            LocalDb.CreateTable<Tracking>();
            LocalDb.CreateTable<User>();
            LocalDb.CreateTable<Message>();
            LocalDb.CreateTable<Setting>();
            LocalDb.CreateTable<SettingItem>();

            //Invoice and Product in invoice
            LocalDb.CreateTable<Invoice>();
            LocalDb.CreateTable<InvoiceProduct>();

            //contract
            LocalDb.CreateTable<Contract>();

            // Local Notification tap event listener
            MessagingCenter.Instance.Subscribe<LocalNotificationTappedEvent>(this, typeof(LocalNotificationTappedEvent).FullName, OnLocalNotificationTapped);
        }

        private void OnLocalNotificationTapped(LocalNotificationTappedEvent e)
        {
            //Debug.WriteLine("Local Notification Tapped: " + e.Data);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}