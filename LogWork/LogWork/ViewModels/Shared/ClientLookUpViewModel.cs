using LogWork.Constants;
using LogWork.IServices;
using LogWork.Models;
using LogWork.ViewModels.Customers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using TinyMVVM;

namespace LogWork.ViewModels.Shared
{
    public class ClientLookUpViewModel : TinyViewModel
    {
        private readonly IClientService clientService;

        private readonly int CurrentUserId = Settings.CurrentUserId;

        private CancellationTokenSource cts;

        private List<Client> listClient;
        public List<Client> ListClient { get => listClient; set => SetProperty(ref listClient, value); }

        private string searchKey;
        public string SearchKey { get => searchKey; set => SetProperty(ref searchKey, value, onChanged: OnsearchKeyChanged); }

        public ICommand AddClientCommand { get; private set; }
        public ICommand ClientSelectedCommand { get; private set; }
        public ICommand CancelSearchCommand { get; private set; }

        public ClientLookUpViewModel(IClientService clientService)
        {
            this.clientService = clientService;

            AddClientCommand = new AwaitCommand(AddClient);
            ClientSelectedCommand = new AwaitCommand<Client>(ClientSelected);
            CancelSearchCommand = new AwaitCommand(CancelSearch);
        }

        public override void Init(NavigationParameters parameters)
        {
            base.Init(parameters);

            OnsearchKeyChanged();
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();

            MessagingCenter.Subscribe<NewCustomerViewModel>(this, MessageKey.CUSTOMER_CHANGED, OnCustomerChanged);
        }

        public override void OnPopped()
        {
            base.OnPopped();

            MessagingCenter.Unsubscribe<NewCustomerViewModel>(this, MessageKey.CUSTOMER_CHANGED);
        }

        private void OnCustomerChanged(object sender)
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
                    return await clientService.GetClients(CurrentUserId, limit: 100);
                }
                else
                {
                    return await clientService.SearchClient(CurrentUserId, SearchKey, 100);
                }
            }, token).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    ListClient = task.Result;
                }
                else if (task.IsFaulted && !token.IsCancellationRequested)
                {
                    CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }
            }));
        }

        private async void AddClient(object sender, TaskCompletionSource<bool> tcs)
        {
            await CoreMethods.PushViewModel<NewCustomerViewModel>(modal: true);

            tcs.TrySetResult(true);
        }

        private async void ClientSelected(Client value, TaskCompletionSource<bool> tcs)
        {
            MessagingCenter.Send(this, MessageKey.CLIENT_SELECTED, value);
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