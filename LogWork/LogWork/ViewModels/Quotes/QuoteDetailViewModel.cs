using LogWork.Common;
using LogWork.Constants;
using LogWork.IServices;
using LogWork.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using TinyMVVM;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;

namespace LogWork.ViewModels.Quotes
{
    public class QuoteDetailViewModel : BaseViewModel
    {
        private readonly IInvoiceService invoiceService;

        private Guid QuoteId;

        private string invoiceID;
        public string InvoiceID { get => invoiceID; set => SetProperty(ref invoiceID, value); }

        private Invoice quote;
        public Invoice Quote { get => quote; set => SetProperty(ref quote, value); }

        public ICommand EditQuoteCommand { get; private set; }
        public ICommand DeleteQuoteCommand { get; private set; }

        public QuoteDetailViewModel(IInvoiceService invoiceService)
        {
            this.invoiceService = invoiceService;

            EditQuoteCommand = new AwaitCommand(EditQuote);
            DeleteQuoteCommand = new AwaitCommand(DeleteQuote);
        }

        public async override void Init(NavigationParameters parameters)
        {
            base.Init(parameters);

            QuoteId = parameters.GetValue<Invoice>(ContentKey.SELECTED_QUOTE)?.Id ?? Guid.Empty;

            Quote = await invoiceService.GetInvoiceDetail(QuoteId);

            Title = Quote.IsInvoice == 0 ? TranslateExtension.GetValue("page_title_quote_detail") : TranslateExtension.GetValue("page_title_invoice_detail");

            InvoiceID = string.Format("{0}: {1}", "ID", Quote?.InvoiceNumber ?? (Quote?.ServerId > 0 ? Quote?.ServerId.ToString() : "Empty"));

            RegisterMessagingCenter<NewQuoteViewModel>(this, MessageKey.QUOTE_CHANGED, OnQuoteChanged);
        }

        private async void OnQuoteChanged(object sender)
        {
            Quote = await invoiceService.GetInvoiceDetail(QuoteId);
        }

        private async void EditQuote(object sender, TaskCompletionSource<bool> tcs)
        {
            try
            {
                var parameters = new NavigationParameters()
                {
                    { ContentKey.SELECTED_QUOTE,  Quote},
                    { ContentKey.QUOTE_MODE, EditMode.Modify}
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

        private async void DeleteQuote(object sender, TaskCompletionSource<bool> tcs)
        {
            if (await CoreMethods.DisplayAlert(TranslateExtension.GetValue("alert_title_quote"), TranslateExtension.GetValue("alert_message_delete_quote_confirm"), TranslateExtension.GetValue("alert_message_yes"), TranslateExtension.GetValue("alert_message_no")))
            {
                Quote.IsActif = 0;
                Quote.IsToSync = true;

                App.LocalDb.Update(Quote);

                MessagingCenter.Send(this, MessageKey.QUOTE_CHANGED);

                await CoreMethods.PopViewModel();
            };

            tcs.TrySetResult(true);
        }
    }
}