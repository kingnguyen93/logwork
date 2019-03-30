using LogWork.Constants;
using LogWork.Models;
using LogWork.ViewModels.Shared;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using TinyMVVM;

namespace LogWork.ViewModels.Quotes
{
    public class AddProductViewModel : BaseViewModel
    {
        private InvoiceProduct product;
        public InvoiceProduct Product { get => product; set => SetProperty(ref product, value); }

        public ICommand CancelSearchCommand { get; private set; }
        public ICommand SearchProductCommand { get; private set; }
        public ICommand IncreaseProductQuantityCommand { get; private set; }
        public ICommand DecreaseProductQuantityCommand { get; private set; }
        public ICommand AddProductCommand { get; private set; }

        public AddProductViewModel()
        {
            CancelSearchCommand = new AwaitCommand(CancelSearch);
            SearchProductCommand = new AwaitCommand(SearchProduct);
            IncreaseProductQuantityCommand = new AwaitCommand(IncreaseProductQuantity);
            DecreaseProductQuantityCommand = new AwaitCommand(DecreaseProductQuantity);
            AddProductCommand = new AwaitCommand(AddProduct);
        }

        public override void Init(NavigationParameters parameters)
        {
            base.Init(parameters);

            Product = new InvoiceProduct()
            {
                Id = Guid.NewGuid(),
                UserId = CurrentUser.Id,
                AccountId = CurrentUser.FkAccountId,
                IsActif = 1,
                IsToSync = true
            };

            Product.PropertyChanged += Product_PropertyChanged;

            RegisterMessagingCenter<ProductLookUpViewModel, Product>(this, MessageKey.PRODUCT_SELECTED, OnProductSelected);
        }

        private async void CancelSearch(object sender, TaskCompletionSource<bool> tcs)
        {
            await CoreMethods.PopViewModel(modal: IsModal);

            tcs.TrySetResult(true);
        }

        private async void SearchProduct(object sender, TaskCompletionSource<bool> tcs)
        {
            await CoreMethods.PushViewModel<ProductLookUpViewModel>(modal: true);

            tcs.TrySetResult(true);
        }

        private void OnProductSelected(object sender, Product selectedProduct)
        {
            Product.FkProductAppId = selectedProduct.Id;
            Product.FkProductServerId = selectedProduct.ServerId;
            Product.Label = selectedProduct.Nom;
            Product.Quantity = selectedProduct.Quantity;
            Product.PriceUnit = selectedProduct.Price;
            Product.PriceUnitWithTax = Product.PriceUnit + (Product.PriceUnit * (Product.RateOfTax / 100));
            Product.RateOfTax = selectedProduct.Tax;
            Product.TotalPrice = Product.PriceUnit * Product.Quantity;
            Product.AmountOfTax = Product.TotalPrice * (Product.RateOfTax / 100);
            Product.TotalPriceWithTax = Product.PriceUnitWithTax * Product.Quantity;

            if (selectedProduct.Quantity == 0)
                Product.Quantity = 1;
        }

        private void IncreaseProductQuantity(object sender, TaskCompletionSource<bool> tcs)
        {
            Product.Quantity += 1;

            tcs.TrySetResult(true);
        }

        private void DecreaseProductQuantity(object sender, TaskCompletionSource<bool> tcs)
        {
            if (Product.Quantity >= 1)
                Product.Quantity -= 1;

            tcs.TrySetResult(true);
        }

        private void Product_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Product.PropertyChanged -= Product_PropertyChanged;

            if (e.PropertyName.Equals(nameof(Product.Quantity)))
            {
                Product.TotalPrice = Product.PriceUnit * Product.Quantity;
                Product.AmountOfTax = Product.TotalPrice * (Product.RateOfTax / 100);
                Product.TotalPriceWithTax = Product.PriceUnitWithTax * Product.Quantity;
            }
            else if (e.PropertyName.Equals(nameof(Product.PriceUnit)))
            {
                Product.PriceUnitWithTax = Product.PriceUnit + (Product.PriceUnit * (Product.RateOfTax / 100));
                Product.TotalPrice = Product.PriceUnit * Product.Quantity;
                Product.AmountOfTax = Product.TotalPrice * (Product.RateOfTax / 100);
                Product.TotalPriceWithTax = Product.PriceUnitWithTax * Product.Quantity;
            }
            else if (e.PropertyName.Equals(nameof(Product.RateOfTax)))
            {
                Product.PriceUnitWithTax = Product.PriceUnit + (Product.PriceUnit * (Product.RateOfTax / 100));
                Product.AmountOfTax = Product.TotalPrice * (Product.RateOfTax / 100);
                Product.TotalPriceWithTax = Product.PriceUnitWithTax * Product.Quantity;
            }

            Product.PropertyChanged += Product_PropertyChanged;
        }

        private async void AddProduct(object sender, TaskCompletionSource<bool> tcs)
        {
            MessagingCenter.Send(this, MessageKey.PRODUCT_SELECTED, Product);

            await CoreMethods.PopViewModel(modal: IsModal);

            tcs.TrySetResult(true);
        }
    }
}