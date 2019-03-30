using LogWork.Constants;
using LogWork.IServices;
using LogWork.Models;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using TinyMVVM;

namespace LogWork.ViewModels.Customers
{
    public class CustomersViewModel : TinyViewModel
    {
        private readonly ISyncDataService syncService;
        private readonly IClientService clientService;

        private readonly int CurrentUserId = Settings.CurrentUserId;

        private CancellationTokenSource cts;

        private ObservableCollection<Client> listClient = new ObservableCollection<Client>();
        public ObservableCollection<Client> ListClient { get => listClient; set => SetProperty(ref listClient, value); }

        public ICommand GetSyncCommand { get; set; }
        public ICommand LoadMoreClientCommand { get; set; }
        public ICommand SearchCustomerCommand { get; set; }
        public ICommand AddClientCommand { get; set; }
        public ICommand EditClientCommand { get; set; }

        public CustomersViewModel(ISyncDataService syncService, IClientService clientService)
        {
            this.syncService = syncService;
            this.clientService = clientService;

            GetSyncCommand = new Command(GetSync);
            LoadMoreClientCommand = new Command(LoadMoreClient);
            SearchCustomerCommand = new Command<string>(SearchCustomer);
            AddClientCommand = new AwaitCommand(AddClient);
            EditClientCommand = new AwaitCommand<Client>(EditClient);
        }

        public override void Init(NavigationParameters parameters)
        {
            base.Init(parameters);
            
            LoadMoreClient(null);
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();

            MessagingCenter.Subscribe<NewCustomerViewModel>(this, MessageKey.CUSTOMER_CHANGED, OnCustomerChanged);
            MessagingCenter.Subscribe<CustomerDetailViewModel>(this, MessageKey.CUSTOMER_CHANGED, OnCustomerChanged);
        }

        public override void OnPopped()
        {
            base.OnPopped();

            MessagingCenter.Unsubscribe<NewCustomerViewModel>(this, MessageKey.CUSTOMER_CHANGED);
            MessagingCenter.Unsubscribe<CustomerDetailViewModel>(this, MessageKey.CUSTOMER_CHANGED);
        }

        private void OnCustomerChanged(object sender)
        {
            ListClient.Clear();
            LoadMoreClient(null);
        }

        private void GetSync(object sender)
        {
            syncService.SyncFromServer(method: 2, onSuccess: () => OnCustomerChanged(null), showOverlay: true);
        }

        private void LoadMoreClient(object sender)
        {
            Task.Run(async () =>
            {
                return await clientService.GetClients(CurrentUserId, offset: ListClient.Count, limit: 20);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    ListClient.AddRange(task.Result);
                }
            }));
        }

        private void SearchCustomer(string newValue)
        {
            if (cts != null)
                cts.Cancel();

            cts = new CancellationTokenSource();
            var token = cts.Token;

            Task.Run(async () =>
            {
                await Task.Delay(250, token);

                if (string.IsNullOrWhiteSpace(newValue))
                {
                    return await clientService.GetClients(CurrentUserId, limit: 20);
                }
                else
                {
                    return await clientService.SearchClient(CurrentUserId, newValue, 100);
                }
            }, token).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    ListClient = task.Result.ToObservableCollection();
                }
                else if (task.IsFaulted && !token.IsCancellationRequested)
                {
                    //CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }
            }));
        }

        private async void AddClient(object sender, TaskCompletionSource<bool> tcs)
        {
            if (IsBusy)
                return;

            await CoreMethods.PushViewModel<NewCustomerViewModel>(modal: true);

            tcs.SetResult(true);
        }

        private async void EditClient(Client sender, TaskCompletionSource<bool> tcs)
        {
            if (IsBusy)
            {
                tcs.SetResult(true);
                return;
            }

            var parameters = new NavigationParameters()
            {
                { ContentKey.SELECTED_CUSTOMER, sender}
            };

            await CoreMethods.PushViewModel<CustomerDetailViewModel>(parameters);

            tcs.SetResult(true);
        }
    }
}