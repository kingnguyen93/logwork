using LogWork.Constants;
using LogWork.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using TinyMVVM;

using Xamarin.Forms;
using Xamarin.Forms.Extensions;

namespace LogWork.ViewModels.Addresses
{
    public class NewAddressViewModel : TinyViewModel
    {
        private readonly int CurrentUserId = Settings.CurrentUserId;

        private int mode; // 0: Add 1: Edit
        public int Mode { get => mode; set => SetProperty(ref mode, value); }

        private Client client;
        public Client Client { get => client; set => SetProperty(ref client, value); }

        private Address address;
        public Address Address { get => address; set => SetProperty(ref address, value); }

        public ICommand CancelCommand { get; private set; }
        public ICommand SaveAddressCommand { get; private set; }

        public NewAddressViewModel()
        {
            CancelCommand = new AwaitCommand(Cancel);
            SaveAddressCommand = new AwaitCommand(SaveAddress);
        }

        public override void Init(NavigationParameters parameters)
        {
            base.Init(parameters);

            Mode = parameters?.GetValue<int>(ContentKey.NEW_CUSTOMER_MODE) ?? 0;

            if (Mode != 0)
            {
                Address = parameters?.GetValue<Address>(ContentKey.SELECTED_ADDRESS)?.DeepCopy();

                Title = TranslateExtension.GetValue("address") + " #" + Address.Code;
            }
            else
            {
                Title = TranslateExtension.GetValue("page_title_new_address");

                Address = new Address()
                {
                    Id = Guid.NewGuid(),
                    UserId = CurrentUserId
                };

                Client = parameters?.GetValue<Client>(ContentKey.SELECTED_CUSTOMER);
            }
        }

        private async void Cancel(object sender, TaskCompletionSource<bool> tcs)
        {
            await CoreMethods.PopViewModel(modal: IsModal);

            tcs.SetResult(true);
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(Address.Adresse))
            {
                CoreMethods.DisplayAlert("", "Adresse is Empty", TranslateExtension.GetValue("ok"));
                return false;
            }

            return true;
        }

        private async void SaveAddress(object sender, TaskCompletionSource<bool> tcs)
        {
            try
            {
                if (!ValidateInput())
                {
                    tcs.TrySetResult(true);
                    return;
                }

                Address.IsToSync = true;

                if (Mode != 0)
                {
                    Address.EditDate = DateTime.Now;

                    if (App.LocalDb.Update(Address) == 0)
                    {
                        tcs.TrySetResult(true);
                        return;
                    }
                }
                else
                {
                    if (Client != null)
                    {
                        Address.FkClientAppliId = Client.Id;
                        Address.FkClientServerId = Client.ServerId;
                    }

                    Address.AddDate = DateTime.Now;
                    Address.EditDate = DateTime.Now;
                    Address.IsActif = 1;

                    if (App.LocalDb.Insert(Address) == 0)
                    {
                        tcs.TrySetResult(true);
                        return;
                    }
                }

                MessagingCenter.Send(this, MessageKey.ADDRESS_CHANGED, Address);
                MessagingCenter.Send(this, MessageKey.ADDRESS_CHANGED);

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