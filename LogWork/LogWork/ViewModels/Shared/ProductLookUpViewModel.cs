using LogWork.Constants;
using LogWork.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using TinyMVVM;

namespace LogWork.ViewModels.Shared
{
    public class ProductLookUpViewModel : TinyViewModel
    {
        private readonly int CurrentUserId = Settings.CurrentUserId;

        private CancellationTokenSource cts;
        
        private ObservableCollection<Product> listProduct;
        public ObservableCollection<Product> ListProduct { get => listProduct; set => SetProperty(ref listProduct, value); }

        private string searchKey;
        public string SearchKey { get => searchKey; set => SetProperty(ref searchKey, value, onChanged: OnsearchKeyChanged); }

        private Product selectedProduct;
        public Product SelectedProduct { get => selectedProduct; set => SetProperty(ref selectedProduct, value); }

        private decimal quantity = 1;
        public decimal Quantity { get => quantity; set => SetProperty(ref quantity, value); }

        public ICommand CancelSearchCommand { get; private set; }
        public ICommand LoadMoreProductCommand { get; set; }
        public ICommand AddProductCommand { get; private set; }

        public ProductLookUpViewModel()
        {
            CancelSearchCommand = new AwaitCommand(CancelSearch);
            LoadMoreProductCommand = new Command(LoadMoreProduct);
            AddProductCommand = new AwaitCommand(AddProduct);
        }

        public override void Init(NavigationParameters parameters)
        {
            base.Init(parameters);

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

            Task.Run(() =>
            {
                Task.Delay(250, token).Wait();

                if (string.IsNullOrWhiteSpace(SearchKey))
                {
                    return App.LocalDb.Table<Product>().ToList().FindAll(pr => pr.UserId == CurrentUserId && !string.IsNullOrWhiteSpace(pr.Nom) && pr.IsActif == 1).Take(20);
                }
                else
                {
                    return App.LocalDb.Table<Product>().ToList().FindAll(pr => pr.UserId == CurrentUserId && !string.IsNullOrWhiteSpace(pr.Nom) && ((pr.CodeId > 0 && pr.CodeId.ToString().Contains(searchKey)) || (!string.IsNullOrWhiteSpace(pr.Nom) && pr.Nom.UnSignContains(SearchKey))) && pr.IsActif == 1).Take(20);
                }
            }, token).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    ListProduct = task.Result.ToObservableCollection();
                }
                else if (task.IsFaulted && !token.IsCancellationRequested)
                {
                    //CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }
            }));
        }

        private void LoadMoreProduct(object sender)
        {
            Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(SearchKey))
                {
                    return App.LocalDb.Table<Product>().ToList().FindAll(pr => pr.UserId == CurrentUserId && !string.IsNullOrWhiteSpace(pr.Nom) && pr.IsActif == 1).Skip(ListProduct.Count).Take(20);
                }
                else
                {
                    return App.LocalDb.Table<Product>().ToList().FindAll(pr => pr.UserId == CurrentUserId && !string.IsNullOrWhiteSpace(pr.Nom) && ((pr.CodeId > 0 && pr.CodeId.ToString().Contains(searchKey)) || (string.IsNullOrWhiteSpace(pr.Nom) && pr.Nom.UnSignContains(SearchKey))) && pr.IsActif == 1).Skip(ListProduct.Count).Take(20);
                }
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    ListProduct.AddRange(task.Result);
                }
            }));
        }

        private async void AddProduct(object sender, TaskCompletionSource<bool> tcs)
        {
            if (SelectedProduct == null)
            {
                tcs.TrySetResult(true);
                return;
            }

           

            SelectedProduct.Quantity = Quantity;

            MessagingCenter.Send(this, MessageKey.PRODUCT_SELECTED, SelectedProduct);

            await CoreMethods.PopViewModel(modal: IsModal);

            tcs.TrySetResult(true);
        }

        private async void CancelSearch(object sender, TaskCompletionSource<bool> tcs)
        {
            await CoreMethods.PopViewModel(modal: IsModal);

            tcs.TrySetResult(true);
        }
    }
}