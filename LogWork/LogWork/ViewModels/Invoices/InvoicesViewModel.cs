using LogWork.Common;
using LogWork.Constants;
using LogWork.IServices;
using LogWork.Models;
using LogWork.ViewModels.Quotes;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using TinyMVVM;

namespace LogWork.ViewModels.Invoices
{
    public class InvoicesViewModel : BaseViewModel
    {
        private readonly IInvoiceService invoiceService;

        private CancellationTokenSource cts;

        private bool isInvoiceChanged = true;

        private ObservableCollection<Invoice> listInvoice = new ObservableCollection<Invoice>();
        public ObservableCollection<Invoice> ListInvoice { get => listInvoice; set => SetProperty(ref listInvoice, value); }

        public ICommand GetSyncCommand { get; set; }
        public ICommand LoadMoreInvoiceCommand { get; set; }
        public ICommand SearchInvoiceCommand { get; set; }
        public ICommand AddInvoiceCommand { get; set; }
        public ICommand ViewInvoiceCommand { get; set; }

        public InvoicesViewModel(IInvoiceService invoiceService)
        {
            this.invoiceService = invoiceService;

            GetSyncCommand = new Command(GetSync);
            LoadMoreInvoiceCommand = new Command(LoadMoreClient);
            SearchInvoiceCommand = new Command<string>(SearchInvoice);
            AddInvoiceCommand = new AwaitCommand(AddInvoice);
            ViewInvoiceCommand = new AwaitCommand<Invoice>(ViewInvoice);
        }

        public override void Init(NavigationParameters parameters)
        {
            base.Init(parameters);

            RegisterMessagingCenter<NewQuoteViewModel>(this, MessageKey.INVOICE_CHANGED, OnInvoiceChanged);
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            if (isInvoiceChanged)
            {
                ListInvoice.Clear();
                LoadMoreClient(null);

                isInvoiceChanged = false;
            }
        }

        private void OnInvoiceChanged(object sender)
        {
            isInvoiceChanged = true;
        }

        private void GetSync(object sender)
        {
            syncService.SyncFromServer(method: 2, onSuccess: () => LoadMoreClient(null), showOverlay: true);
        }

        private void LoadMoreClient(object sender)
        {
            Task.Run(async () =>
            {
                return await invoiceService.GetInvoices(offset: ListInvoice.Count, limit: 20);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    ListInvoice.AddRange(task.Result);
                }
            }));
        }

        private void SearchInvoice(string newValue)
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
                    return await invoiceService.GetInvoices(limit: 20);
                }
                else
                {
                    return await invoiceService.SearchInvoices(newValue, 100);
                }
            }, token).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    ListInvoice = task.Result.ToObservableCollection();
                }
            }));
        }

        private async void AddInvoice(object sender, TaskCompletionSource<bool> tcs)
        {
            try
            {
                var parameters = new NavigationParameters()
                {
                    { ContentKey.INVOICE_TYPE, InvoiceType.Invoice},
                    { ContentKey.QUOTE_MODE, EditMode.New}
                };

                await CoreMethods.PushViewModel<NewQuoteViewModel>(parameters);
            }
            catch (Exception ex)
            {
                await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), ex.Message, TranslateExtension.GetValue("ok"));
            }
            finally
            {
                tcs.TrySetResult(true);
            }
        }

        private async void ViewInvoice(Invoice invoice, TaskCompletionSource<bool> tcs)
        {
            try
            {
                var parameters = new NavigationParameters()
                {
                    { ContentKey.SELECTED_QUOTE, invoice},
                    { ContentKey.QUOTE_MODE, EditMode.Modify}
                };

                await CoreMethods.PushViewModel<QuoteDetailViewModel>(parameters);
            }
            catch (Exception ex)
            {
                await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), ex.Message, TranslateExtension.GetValue("ok"));
            }
            finally
            {
                tcs.TrySetResult(true);
            }
        }
    }
}