using Acr.UserDialogs;
using LogWork.Constants;
using LogWork.IServices;
using LogWork.Models;
using LogWork.ViewModels.Customers;
using LogWork.ViewModels.Interventions;
using Plugin.ExternalMaps;
using Plugin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TinyMVVM;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;

namespace LogWork.ViewModels.Addresses
{
    public class AddressDetailViewModel : TinyViewModel
    {
        private readonly IAddressService addressService;

        private readonly int CurrentUserId = Settings.CurrentUserId;

        private Address address;
        public Address Address { get => address; set => SetProperty(ref address, value); }

        private List<Client> listClient;
        public List<Client> ListClient { get => listClient; set => SetProperty(ref listClient, value); }

        private List<Intervention> listIntervention;
        public List<Intervention> ListIntervention { get => listIntervention; set => SetProperty(ref listIntervention, value); }

        public ICommand OpenMapCommand { get; set; }
        public ICommand ViewClientCommand { get; set; }
        public ICommand SendMailCommand { get; private set; }
        public ICommand ViewInterventionCommand { get; set; }
        public ICommand AddInterventionCommand { get; set; }

        public AddressDetailViewModel(IAddressService addressService)
        {
            this.addressService = addressService;

            OpenMapCommand = new AwaitCommand(OpenMap);
            ViewClientCommand = new AwaitCommand<Client>(ViewClient);
            SendMailCommand = new AwaitCommand<Client>(SendMail);
            ViewInterventionCommand = new AwaitCommand<Intervention>(ViewIntervention);
            AddInterventionCommand = new AwaitCommand<Client>(AddIntervention);
        }

        public override void Init(NavigationParameters parameters)
        {
            base.Init(parameters);

            Address = parameters?.GetValue<Address>(ContentKey.SELECTED_ADDRESS)?.DeepCopy();

            Title = TranslateExtension.GetValue("address") + " #" + Address.Code;

            InitAddress();
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();

            MessagingCenter.Subscribe<InterventionDetailViewModel>(this, MessageKey.INTERVENTION_CHANGED, OnInterventionChanged);
            MessagingCenter.Subscribe<NewInterventionViewModel>(this, MessageKey.INTERVENTION_CHANGED, OnInterventionChanged);
        }

        public override void OnPopped()
        {
            base.OnPopped();

            MessagingCenter.Unsubscribe<InterventionDetailViewModel>(this, MessageKey.INTERVENTION_CHANGED);
            MessagingCenter.Unsubscribe<NewInterventionViewModel>(this, MessageKey.INTERVENTION_CHANGED);
        }

        private void OnInterventionChanged(object sender)
        {
            GetInterventions();
        }

        private void InitAddress()
        {
            ListClient = App.LocalDb.Table<Client>().ToList().FindAll(c => c.UserId == Address.UserId && ((c.ServerId > 0 && c.ServerId == Address.FkClientServerId) || (!c.Id.IsGuidEmpty() && !Address.FkClientAppliId.IsGuidEmpty() && c.Id.Equals(Address.FkClientAppliId))) && c.IsActif == 1);

            GetInterventions();
        }

        private void GetInterventions()
        {
            ListIntervention = App.LocalDb.Table<Intervention>().ToList().FindAll(i => i.UserId == Address.UserId && ((i.FkAddressServerId > 0 && i.FkAddressServerId == Address.ServerId) || (!i.FkAddressAppId.IsGuidEmpty() && !Address.Id.IsGuidEmpty() && i.FkAddressAppId.Equals(Address.Id))) && i.IsActif == 1);
            foreach (var item in ListIntervention)
            {
                item.Client = App.LocalDb.Table<Client>().ToList().FirstOrDefault(c => c.UserId == CurrentUserId && ((c.ServerId > 0 && c.ServerId == item.FkClientServerId) || (!c.Id.IsGuidEmpty() && c.Id.Equals(item.FkClientAppId))) && c.IsActif == 1);
                item.Address = App.LocalDb.Table<Address>().ToList().FirstOrDefault(a => a.UserId == CurrentUserId && ((a.ServerId > 0 && a.ServerId == item.FkAddressServerId) || (!a.Id.IsGuidEmpty() && a.Id.Equals(item.FkAddressAppId))) && a.IsActif == 1);
            }
        }

        private async void OpenMap(object sender, TaskCompletionSource<bool> tcs)
        {
            try
            {
                await CrossExternalMaps.Current.NavigateTo("", Address.Latitude, Address.Longitude);
            }
            catch (Exception ex)
            {
                await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), ex.GetBaseException().Message, TranslateExtension.GetValue("ok"));
            }

            tcs.SetResult(true);
        }

        private async void ViewClient(Client value, TaskCompletionSource<bool> tcs)
        {
            if (IsBusy)
            {
                tcs.SetResult(true);
                return;
            }

            var parameters = new NavigationParameters()
            {
                { ContentKey.SELECTED_CUSTOMER, value}
            };

            await CoreMethods.PushViewModel<CustomerDetailViewModel>(parameters);

            tcs.SetResult(true);
        }

        private void SendMail(Client selectedClient, TaskCompletionSource<bool> tcs)
        {
            try
            {
                if (!selectedClient.Email.EmailValidate())
                {
                    tcs.SetResult(true);
                    return;
                }

                var emailMessenger = CrossMessaging.Current.EmailMessenger;
                if (emailMessenger.CanSendEmail)
                {
                    // Send simple e-mail to single receiver without attachments, bcc, cc etc.
                    emailMessenger.SendEmail(
                        selectedClient.Email,
                        "LogWork Test Send Mail To Client",
                        selectedClient.ToString());

                    //UserDialogs.Instance.Toast(new ToastConfig(TranslateExtension.GetValue("alert_message_send_mail_successed")));
                }
                else
                {
                    UserDialogs.Instance.Toast(new ToastConfig(TranslateExtension.GetValue("alert_message_cant_send_email")));
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Toast(new ToastConfig(ex.Message));
            }

            tcs.SetResult(true);
        }

        private async void ViewIntervention(Intervention value, TaskCompletionSource<bool> tcs)
        {
            if (IsBusy)
            {
                tcs.SetResult(true);
                return;
            }

            var parameters = new NavigationParameters()
            {
                { ContentKey.SELECTED_INTERVENTION, value}
            };

            await CoreMethods.PushViewModel<InterventionDetailViewModel>(parameters);

            tcs.SetResult(true);
        }

        private async void AddIntervention(Client value, TaskCompletionSource<bool> tcs)
        {
            if (IsBusy)
                return;

            var parameters = new NavigationParameters()
            {
                { ContentKey.SELECTED_CUSTOMER, value}
            };

            await CoreMethods.PushViewModel<NewInterventionViewModel>(parameters);

            tcs.SetResult(true);
        }
    }
}