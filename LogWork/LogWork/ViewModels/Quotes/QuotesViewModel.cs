using LogWork.Common;
using LogWork.Constants;
using LogWork.IServices;
using LogWork.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using TinyMVVM;

namespace LogWork.ViewModels.Quotes
{
    public class QuotesViewModel : BaseViewModel
    {
        private readonly IInvoiceService invoiceService;

        private bool isQuoteChanged = true;

        private List<Invoice> listInvoice = new List<Invoice>();
        public List<Invoice> ListInvoice { get => listInvoice; set => SetProperty(ref listInvoice, value); }

        public ICommand GetSyncCommand { get; set; }
        public ICommand GetQuotesCommand { get; set; }
        public ICommand AddQuoteCommand { get; set; }
        public ICommand ViewInvoiceCommand { get; set; }

        public QuotesViewModel(IInvoiceService invoiceService)
        {
            this.invoiceService = invoiceService;

            GetSyncCommand = new Command(GetSync);
            GetQuotesCommand = new Command(GetListQuote);
            AddQuoteCommand = new AwaitCommand(AddQuote);
            ViewInvoiceCommand = new AwaitCommand<Invoice>(ViewInvoice);
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            if (isQuoteChanged)
            {
                GetListQuote();
                isQuoteChanged = false;
            }
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();

            RegisterMessagingCenter<NewQuoteViewModel>(this, MessageKey.QUOTE_CHANGED, OnQuoteChanged);
            RegisterMessagingCenter<QuoteDetailViewModel>(this, MessageKey.QUOTE_CHANGED, OnQuoteChanged);
        }

        private void OnQuoteChanged(object sender)
        {
            isQuoteChanged = true;
        }

        private void GetSync(object sender)
        {
            syncService.SyncFromServer(1, onSuccess: GetListQuote, showOverlay: true);
        }

        private void GetListQuote()
        {
            Task.Run(async () =>
            {
                return await invoiceService.GetQuotes();
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsBusy = false;

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    ListInvoice = task.Result;;
                }
                else if (task.IsFaulted)
                {
                    CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }
            }));
        }

        private async void AddQuote(object sender, TaskCompletionSource<bool> tcs)
        {
            try
            {
                var parameters = new NavigationParameters()
                {
                    { ContentKey.INVOICE_TYPE, InvoiceType.Quote},
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