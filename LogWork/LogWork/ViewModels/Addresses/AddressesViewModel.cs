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

namespace LogWork.ViewModels.Addresses
{
    public class AddressesViewModel : TinyViewModel
    {
        private readonly ISyncDataService syncService;
        private readonly IAddressService addressService;

        private readonly int CurrentUserId = Settings.CurrentUserId;

        private CancellationTokenSource cts;

        private ObservableCollection<Address> listAddress = new ObservableCollection<Address>();
        public ObservableCollection<Address> ListAddress { get => listAddress; set => SetProperty(ref listAddress, value); }

        public ICommand GetSyncCommand { get; set; }
        public ICommand LoadMoreAddressCommand { get; set; }
        public ICommand SearchAddressCommand { get; set; }
        public ICommand AddAddressCommand { get; set; }
        public ICommand EditAddressCommand { get; set; }

        public AddressesViewModel(ISyncDataService syncService, IAddressService addressService)
        {
            this.syncService = syncService;
            this.addressService = addressService;

            GetSyncCommand = new Command(GetSync);
            LoadMoreAddressCommand = new Command(LoadMoreAddress);
            SearchAddressCommand = new Command<string>(SearchAddress);
            AddAddressCommand = new AwaitCommand(AddAddress);
            EditAddressCommand = new AwaitCommand<Address>(EditAddress);
        }

        public override void Init(NavigationParameters parameters)
        {
            base.Init(parameters);

            LoadMoreAddress(null);
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();
            
            MessagingCenter.Subscribe<NewAddressViewModel>(this, MessageKey.ADDRESS_CHANGED, OnAddressChanged);
            MessagingCenter.Subscribe<AddressDetailViewModel>(this, MessageKey.ADDRESS_CHANGED, OnAddressChanged);
        }

        public override void OnPopped()
        {
            base.OnPopped();

            MessagingCenter.Unsubscribe<NewAddressViewModel>(this, MessageKey.ADDRESS_CHANGED);
            MessagingCenter.Unsubscribe<AddressDetailViewModel>(this, MessageKey.ADDRESS_CHANGED);
        }

        private void OnAddressChanged(object sender)
        {
            ListAddress.Clear();
            LoadMoreAddress(null);
        }

        private void GetSync(object sender)
        {
            syncService.SyncFromServer(method: 2, onSuccess: () => OnAddressChanged(null), showOverlay: true);
        }

        private void LoadMoreAddress(object sender)
        {
            Task.Run(async () =>
            {
                return await addressService.GetAddresses(CurrentUserId, offset: ListAddress.Count, limit: 20);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    ListAddress.AddRange(task.Result);
                }
            }));
        }

        private void SearchAddress(string newValue)
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
                    return await addressService.GetAddresses(CurrentUserId, limit: 20);
                }
                else
                {
                    return await addressService.SearchAddress(CurrentUserId, newValue, 100);
                }
            }, token).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    ListAddress = task.Result.ToObservableCollection();
                }
                else if (task.IsFaulted && !token.IsCancellationRequested)
                {
                    //CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }
            }));
        }

        private async void AddAddress(object sender, TaskCompletionSource<bool> tcs)
        {
            if (IsBusy)
                return;

            await CoreMethods.PushViewModel<NewAddressViewModel>(modal: true);

            tcs.SetResult(true);
        }

        private async void EditAddress(Address value, TaskCompletionSource<bool> tcs)
        {
            if (IsBusy)
            {
                tcs.SetResult(true);
                return;
            }

            var parameters = new NavigationParameters()
            {
                { ContentKey.SELECTED_ADDRESS,  value}
            };

            await CoreMethods.PushViewModel<AddressDetailViewModel>(parameters);

            tcs.SetResult(true);
        }
    }
}