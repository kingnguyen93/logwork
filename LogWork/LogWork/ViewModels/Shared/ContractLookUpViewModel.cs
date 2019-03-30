using LogWork.Constants;
using LogWork.IServices;
using LogWork.Models;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TinyMVVM;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;

namespace LogWork.ViewModels.Shared
{
    public class ContractLookUpViewModel : TinyViewModel
    {
        private readonly IContractService contractService;

        private readonly int CurrentUserId = Settings.CurrentUserId;

        private CancellationTokenSource cts;

        private ObservableCollection<Contract> listContract;
        public ObservableCollection<Contract> ListContract { get => listContract; set => SetProperty(ref listContract, value); }

        private string searchKey;
        public string SearchKey { get => searchKey; set => SetProperty(ref searchKey, value, onChanged: OnsearchKeyChanged); }

        public ICommand CancelSearchCommand { get; set; }

        public ICommand ContractSelectedCommand { get; set; }

        private Client client;
        public Client Client { get => client; set => SetProperty(ref client, value); }

        public ContractLookUpViewModel(IContractService contractService)
        {
            this.contractService = contractService;

            ContractSelectedCommand = new AwaitCommand<Contract>(ContractSelected);
            CancelSearchCommand = new AwaitCommand(CancelSearch);
        }

        public override void Init(NavigationParameters parameters)
        {
            Client = parameters?.GetValue<Client>(ContentKey.SELECTED_CUSTOMER)?.DeepCopy();

            OnsearchKeyChanged();
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();

            //MessagingCenter.Subscribe<NewCustomerViewModel>(this, MessageKey.CUSTOMER_CHANGED, OnCustomerChanged);
        }

        public override void OnPopped()
        {
            base.OnPopped();

            //MessagingCenter.Unsubscribe<NewCustomerViewModel>(this, MessageKey.CUSTOMER_CHANGED);
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
                Task.Delay(250, token).Wait();
                if (Client != null)
                    return await contractService.SearchContractByClient(Client, CurrentUserId, SearchKey);
                else
                    return await contractService.SearchContract(CurrentUserId, SearchKey);
            }, token).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    ListContract = task.Result.ToObservableCollection();
                }
                else if (task.IsFaulted && !token.IsCancellationRequested)
                {
                    //CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }
            }));
        }

        private async void ContractSelected(Contract value, TaskCompletionSource<bool> tcs)
        {
            MessagingCenter.Send(this, MessageKey.CONTRACT_SELECTED, value);

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