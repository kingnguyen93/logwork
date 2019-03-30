using LogWork.Constants;
using LogWork.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using TinyMVVM;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;

namespace LogWork.ViewModels.Customers
{
    public class NewCustomerViewModel : TinyViewModel
    {
        private int mode; // 0: Add 1: Edit
        public int Mode { get => mode; set => SetProperty(ref mode, value); }

        public readonly int CurrentUserId = Settings.CurrentUserId;

        private Client client;
        public Client Client { get => client; set => SetProperty(ref client, value); }

        public ICommand CancelCommand { get; private set; }
        public ICommand SaveCustomerCommand { get; private set; }

        public NewCustomerViewModel()
        {
            CancelCommand = new AwaitCommand(Cancel);
            SaveCustomerCommand = new AwaitCommand(SaveCustomer);
        }

        public override void Init(NavigationParameters parameters)
        {
            base.Init(parameters);

            Mode = parameters?.GetValue<int>(ContentKey.NEW_CUSTOMER_MODE) ?? 0;

            if (Mode != 0)
            {
                Client = parameters?.GetValue<Client>(ContentKey.SELECTED_CUSTOMER)?.DeepCopy();

                Title = string.IsNullOrWhiteSpace(Client?.Title) ? TranslateExtension.GetValue("page_title_edit_customer") : Client?.Title;
            }
            else
            {
                Title = TranslateExtension.GetValue("page_title_new_customer");

                Client = new Client()
                {
                    Id = Guid.NewGuid(),
                    UserId = CurrentUserId
                };
            }
        }

        private async void Cancel(object sender, TaskCompletionSource<bool> tcs)
        {
            await CoreMethods.PopViewModel(modal: IsModal);

            tcs.SetResult(true);
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(Client.Title))
            {
                CoreMethods.DisplayAlert("", "Title is Empty", TranslateExtension.GetValue("ok"));
                return false;
            }

            return true;
        }

        private async void SaveCustomer(object sender, TaskCompletionSource<bool> tcs)
        {
            try
            {
                if (!ValidateInput())
                {
                    tcs.TrySetResult(true);
                    return;
                }

                Client.IsToSync = true;

                if (Mode != 0)
                {
                    Client.EditDate = DateTime.Now;

                    if (App.LocalDb.Update(Client) == 0)
                    {
                        tcs.TrySetResult(true);
                        return;
                    }
                }
                else
                {
                    Client.AddDate = DateTime.Now;
                    Client.EditDate = DateTime.Now;
                    Client.IsActif = 1;

                    if (App.LocalDb.Insert(Client) == 0)
                    {
                        tcs.TrySetResult(true);
                        return;
                    }
                }

                MessagingCenter.Send(this, MessageKey.CUSTOMER_CHANGED, Client);
                MessagingCenter.Send(this, MessageKey.CUSTOMER_CHANGED);

                await CoreMethods.PopViewModel(modal: IsModal);
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