using LogWork.Constants;
using LogWork.IServices;
using LogWork.Models;
using LogWork.ViewModels.Addresses;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TinyMVVM;

using Xamarin.Forms;
using Xamarin.Forms.Extensions;

namespace LogWork.ViewModels.Shared
{
    public class AddressLookUpViewModel : TinyViewModel
    {
        private readonly IAddressService addressService;

        private readonly int CurrentUserId = Settings.CurrentUserId;

        private CancellationTokenSource cts;

        private Client client;
        public Client Client { get => client; set => SetProperty(ref client, value); }

        private List<Address> listAddress;
        public List<Address> ListAddress { get => listAddress; set => SetProperty(ref listAddress, value); }

        private string searchKey;
        public string SearchKey { get => searchKey; set => SetProperty(ref searchKey, value, onChanged: OnsearchKeyChanged); }

        public ICommand AddAddressCommand { get; private set; }
        public ICommand AddressSelectedCommand { get; private set; }
        public ICommand CancelSearchCommand { get; private set; }

        public AddressLookUpViewModel(IAddressService addressService)
        {
            this.addressService = addressService;

            AddAddressCommand = new AwaitCommand(AddAddress);
            AddressSelectedCommand = new AwaitCommand<Address>(AddressSelected);
            CancelSearchCommand = new AwaitCommand(CancelSearch);
        }

        public override void Init(NavigationParameters parameters)
        {
            base.Init(parameters);

            Client = parameters?.GetValue<Client>(ContentKey.SELECTED_CUSTOMER)?.DeepCopy();

            OnsearchKeyChanged();
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();

            MessagingCenter.Subscribe<NewAddressViewModel>(this, MessageKey.ADDRESS_CHANGED, OnAddressChanged);
        }

        public override void OnPopped()
        {
            base.OnPopped();

            MessagingCenter.Unsubscribe<NewAddressViewModel>(this, MessageKey.ADDRESS_CHANGED);
        }

        private void OnAddressChanged(object sender)
        {
            SearchKey = "";
            OnsearchKeyChanged();
        }

        private void OnsearchKeyChanged()
        {
            if (IsDisposing)
                return;

            if (cts != null)
                cts.Cancel();

            cts = new CancellationTokenSource();
            var token = cts.Token;

            Task.Run(async () =>
            {
                await Task.Delay(250, token);

                if (string.IsNullOrWhiteSpace(SearchKey))
                {
                    if (Client != null)
                        return await addressService.GetAddressesByClient(Client);
                    else
                        return await addressService.GetAddresses(CurrentUserId, limit: 100);
                }
                else
                {
                    if (Client != null)
                        return (await addressService.GetAddressesByClient(Client)).FindAll(a => (a.Code > 0 && a.Code.ToString().Contains(searchKey)) || (!string.IsNullOrWhiteSpace(a.FullAddress) && a.FullAddress.UnSignContains(searchKey)));
                    else
                        return await addressService.SearchAddress(CurrentUserId, SearchKey);
                }
            }, token).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    ListAddress = task.Result;
                }
                else if (task.IsFaulted && !token.IsCancellationRequested)
                {
                    //CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }
            }));
        }

        private async void AddAddress(object sender, TaskCompletionSource<bool> tcs)
        {
            var @params = new NavigationParameters
            {
                { ContentKey.SELECTED_CUSTOMER, Client }
            };

            await CoreMethods.PushViewModel<NewAddressViewModel>(@params, modal: true);

            tcs.TrySetResult(true);
        }

        private async void AddressSelected(Address value, TaskCompletionSource<bool> tcs)
        {
            MessagingCenter.Send(this, MessageKey.ADDRESS_SELECTED, value);
            await CoreMethods.PopViewModel(modal: true);

            tcs.TrySetResult(true);
        }

        private async void CancelSearch(object sender, TaskCompletionSource<bool> tcs)
        {
            await CoreMethods.PopViewModel(modal: true);

            tcs.TrySetResult(true);
        }
    }
}