using LogWork.Common;
using LogWork.Constants;
using LogWork.Models;
using LogWork.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TinyMVVM;

using Xamarin.Forms;
using Xamarin.Forms.Extensions;

namespace LogWork.ViewModels.Quotes
{
    public class NewQuoteViewModel : BaseViewModel
    {
        private List<InvoiceProduct> DeletedProducts = new List<InvoiceProduct>();

        private EditMode editMode;
        public EditMode EditMode { get => editMode; set => SetProperty(ref editMode, value); }

        private Invoice quote;
        public Invoice Quote { get => quote; set => SetProperty(ref quote, value); }

        private DateTime iDate = DateTime.Now;
        public DateTime IDate { get => iDate; set => SetProperty(ref iDate, value); }

        private DateTime iDueDate = DateTime.Now;
        public DateTime IDueDate { get => iDueDate; set => SetProperty(ref iDueDate, value); }

        public ICommand OnClientFocusedCommand { get; private set; }
        public ICommand OnAddresseFocusedCommand { get; private set; }
        public ICommand IDateUnFocusedCommand { get; private set; }
        public ICommand IDueDateUnFocusedCommand { get; private set; }
        public ICommand AddProductCommand { get; private set; }
        public ICommand DeleteProductCommand { get; private set; }
        public ICommand SaveQuoteCommand { get; private set; }

        public NewQuoteViewModel()
        {
            OnClientFocusedCommand = new AwaitCommand(OnClientFocused);
            OnAddresseFocusedCommand = new AwaitCommand(OnAddresseFocused);
            IDateUnFocusedCommand = new AwaitCommand(IDateUnFocused);
            IDueDateUnFocusedCommand = new AwaitCommand(IDueDateUnFocused);
            AddProductCommand = new AwaitCommand(AddProduct);
            DeleteProductCommand = new AwaitCommand<InvoiceProduct>(DeleteProduct);
            SaveQuoteCommand = new AwaitCommand(OnSaveQuote);
        }

        public override void Init(NavigationParameters parameters)
        {
            base.Init(parameters);

            EditMode = parameters?.GetValue<EditMode>(ContentKey.QUOTE_MODE) ?? EditMode.New;

            if (EditMode == EditMode.Modify)
            {
                Quote = parameters?.GetValue<Invoice>(ContentKey.SELECTED_QUOTE)?.DeepCopy();

                Title = string.Format("{0}-{1}", "Q", (Quote?.InvoiceNumber ?? (Quote?.ServerId > 0 ? Quote?.ServerId.ToString() : null)) ?? TranslateExtension.GetValue("page_title_edit_quote"));
            }
            else
            {
                var invoiceType = parameters?.GetValue<InvoiceType>(ContentKey.INVOICE_TYPE) ?? InvoiceType.Quote;

                Quote = new Invoice()
                {
                    Id = Guid.NewGuid(),
                    UserId = CurrentUser.Id,
                    AccountId = CurrentUser.FkAccountId,
                    IsInvoice = invoiceType == InvoiceType.Quote ? 0 : 1,
                    IsDraft = 1,
                    IsActif = 1,
                    LinkInvoiceProducts = new ObservableCollection<InvoiceProduct>(),
                    IsToSync = true
                };

                Title = TranslateExtension.GetValue("page_title_new_quote");
            }

            RegisterMessagingCenter<ClientLookUpViewModel, Client>(this, MessageKey.CLIENT_SELECTED, OnClientSelected);
            RegisterMessagingCenter<AddressLookUpViewModel, Address>(this, MessageKey.ADDRESS_SELECTED, OnAddressSelected);
            RegisterMessagingCenter<AddProductViewModel, InvoiceProduct>(this, MessageKey.PRODUCT_SELECTED, OnProductSelected);
        }

        private async void OnClientFocused(object sender, TaskCompletionSource<bool> tcs)
        {
            await CoreMethods.PushViewModel<ClientLookUpViewModel>(modal: true);

            tcs.TrySetResult(true);
        }

        private void OnClientSelected(object sender, Client client)
        {
            Quote.FkClientAppId = client.Id;
            Quote.FkClientServerId = client.ServerId;
            Quote.Client = client;

            if (App.LocalDb.Table<Address>().ToList().FindAll(a => ((a.FkClientServerId > 0 && a.FkClientServerId == Quote.Client.ServerId) || (!a.FkClientAppliId.Equals(Guid.Empty) && a.Id.Equals(Quote.Client.Id)))) is List<Address> addresses && addresses.Count > 0)
            {
                OnAddressSelected(null, addresses.First());
            }
            else
            {
                Quote.Address = null;
            }

            Quote.OnPropertyChanged();
        }

        private async void OnAddresseFocused(object sender, TaskCompletionSource<bool> tcs)
        {
            var @params = new NavigationParameters
            {
                { ContentKey.SELECTED_CUSTOMER, Quote.Client }
            };

            await CoreMethods.PushViewModel<AddressLookUpViewModel>(@params, modal: true);

            tcs.TrySetResult(true);
        }

        private void OnAddressSelected(object sender, Address address)
        {
            Quote.FkAddressAppId = address.Id;
            Quote.FkAddressServerId = address.ServerId;
            Quote.Address = address;

            Quote.OnPropertyChanged();
        }

        private void IDateUnFocused(object value, TaskCompletionSource<bool> tcs)
        {
            Quote.IDate = IDate;

            tcs.TrySetResult(true);
        }

        private void IDueDateUnFocused(object value, TaskCompletionSource<bool> tcs)
        {
            Quote.IDueDate = IDueDate;

            tcs.TrySetResult(true);
        }

        private async void AddProduct(object sender, TaskCompletionSource<bool> tcs)
        {
            await CoreMethods.PushViewModel<AddProductViewModel>(modal: true);

            tcs.TrySetResult(true);
        }

        private void OnProductSelected(object sender, InvoiceProduct product)
        {
            product.FKInvoiceAppId = Quote.Id;
            product.FkInvoiceServerId = Quote.ServerId;
            product.Position = Quote.LinkInvoiceProducts?.Count ?? 0;
            product.AddDate = DateTime.Now;
            product.EditDate = DateTime.Now;

            Quote.LinkInvoiceProducts.Add(product.DeepCopy());

            Quote.CachePtHt = Quote.LinkInvoiceProducts?.Sum(ip => ip.TotalPrice);
            Quote.CachePtTax = Quote.LinkInvoiceProducts?.Sum(ip => ip.AmountOfTax);
            Quote.CachePtTtcToPay = Quote.LinkInvoiceProducts?.Sum(ip => ip.TotalPriceWithTax) ?? 0 - Quote.AmountPaid ?? 0;
        }

        private void DeleteProduct(InvoiceProduct value, TaskCompletionSource<bool> tcs)
        {
            if (EditMode == EditMode.New)
            {
                Quote.LinkInvoiceProducts.Remove(value);
            }
            else
            {
                if (value.ServerId > 0)
                {
                    Public.SetProperty(value, nameof(value.IsActif), 0);
                    value.IsToSync = true;
                }
                else
                {
                    Quote.LinkInvoiceProducts.Remove(value);
                    DeletedProducts.Add(value);
                }
            }

            tcs.TrySetResult(true);
        }

        private async void OnSaveQuote(object sender, TaskCompletionSource<bool> tcs)
        {
            try
            {
                if (EditMode == EditMode.New)
                {
                    if (Quote.FkClientAppId.Equals(Guid.Empty) && Quote.FkClientServerId == 0)
                    {
                        await CoreMethods.DisplayAlert(TranslateExtension.GetValue("alert_title_quote"), TranslateExtension.GetValue("alert_message_intervention_no_client"), TranslateExtension.GetValue("ok"));

                        tcs.TrySetResult(true);
                        return;
                    }

                    if (Quote.FkAddressAppId.Equals(Guid.Empty) && Quote.FkAddressServerId == 0)
                    {
                        await CoreMethods.DisplayAlert(TranslateExtension.GetValue("alert_title_quote"), TranslateExtension.GetValue("alert_message_intervention_no_address"), TranslateExtension.GetValue("ok"));

                        tcs.TrySetResult(true);
                        return;
                    }
                }

                App.LocalDb.BeginTransaction();

                if (EditMode == EditMode.New)
                {
                    Quote.AddDate = DateTime.Now;
                    Quote.EditDate = DateTime.Now;

                    App.LocalDb.InsertOrReplace(Quote);
                }
                else
                {
                    Quote.EditDate = DateTime.Now;

                    App.LocalDb.Update(Quote);
                }

                foreach (var ip in Quote.LinkInvoiceProducts)
                {
                    App.LocalDb.InsertOrReplace(ip);
                }

                foreach (var ip in DeletedProducts)
                {
                    App.LocalDb.Delete(ip);
                }

                App.LocalDb.Commit();

                MessagingCenter.Send(this, MessageKey.INVOICE_CHANGED);
                MessagingCenter.Send(this, MessageKey.QUOTE_CHANGED);

                await CoreMethods.PopViewModel();
            }
            catch (Exception ex)
            {
                App.LocalDb.Rollback();
                await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), ex.Message, TranslateExtension.GetValue("ok"));
            }
            finally
            {
                tcs.TrySetResult(true);
            }
        }
    }
}