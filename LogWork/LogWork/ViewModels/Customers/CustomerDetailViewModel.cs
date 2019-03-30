using Acr.UserDialogs;
using LogWork.Constants;
using LogWork.IServices;
using LogWork.Models;
using LogWork.ViewModels.Addresses;
using LogWork.ViewModels.Interventions;
using Plugin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TinyMVVM;

using Xamarin.Forms;
using Xamarin.Forms.Extensions;

namespace LogWork.ViewModels.Customers
{
    public class CustomerDetailViewModel : TinyViewModel
    {
        private readonly IClientService clientService;

        private readonly int CurrentUserId = Settings.CurrentUserId;

        private Client client;
        public Client Client { get => client; set => SetProperty(ref client, value); }

        private List<Intervention> listIntervention;
        public List<Intervention> ListIntervention { get => listIntervention; set => SetProperty(ref listIntervention, value); }

        public ICommand SendMailCommand { get; private set; }
        public ICommand ViewAddressCommand { get; set; }
        public ICommand ViewInterventionCommand { get; set; }
        public ICommand AddInterventionCommand { get; set; }

        public CustomerDetailViewModel(IClientService clientService)
        {
            this.clientService = clientService;

            SendMailCommand = new AwaitCommand(SendMail);
            ViewAddressCommand = new AwaitCommand<Address>(ViewAddress);
            ViewInterventionCommand = new AwaitCommand<Intervention>(ViewIntervention);
            AddInterventionCommand = new AwaitCommand(AddIntervention);
        }

        public override void Init(NavigationParameters parameters)
        {
            base.Init(parameters);

            Client = parameters?.GetValue<Client>(ContentKey.SELECTED_CUSTOMER)?.DeepCopy();

            Title = Client.Title;

            InitClient();
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();

            MessagingCenter.Subscribe<AddressDetailViewModel>(this, MessageKey.ADDRESS_CHANGED, OnAddressChanged);
            MessagingCenter.Subscribe<InterventionDetailViewModel>(this, MessageKey.INTERVENTION_CHANGED, OnInterventionChanged);
            MessagingCenter.Subscribe<NewInterventionViewModel>(this, MessageKey.INTERVENTION_CHANGED, OnInterventionChanged);
        }

        public override void OnPopped()
        {
            base.OnPopped();

            MessagingCenter.Unsubscribe<InterventionDetailViewModel>(this, MessageKey.INTERVENTION_CHANGED);
            MessagingCenter.Unsubscribe<NewInterventionViewModel>(this, MessageKey.INTERVENTION_CHANGED);
            MessagingCenter.Unsubscribe<AddressDetailViewModel>(this, MessageKey.ADDRESS_CHANGED);
        }

        private void OnAddressChanged(object sender)
        {
            InitClient();
        }

        private void OnInterventionChanged(object sender)
        {
            GetInterventions();
        }

        private async void InitClient()
        {
            Client.Addresses = await clientService.GetAddresses(Client);
            GetInterventions();
        }

        private void GetInterventions()
        {
            ListIntervention = App.LocalDb.Table<Intervention>().ToList().FindAll(i => i.UserId == Client.UserId && ((i.FkClientServerId > 0 && i.FkClientServerId == Client.ServerId) && (!i.FkClientAppId.IsGuidEmpty() && !Client.Id.IsGuidEmpty() && i.FkClientAppId.Equals(Client.Id))) && i.IsActif == 1);
            foreach (var item in ListIntervention)
            {
                item.Client = Client;
                item.Address = App.LocalDb.Table<Address>().ToList().FirstOrDefault(a => a.UserId == CurrentUserId && ((a.ServerId > 0 && a.ServerId == item.FkAddressServerId) || (!a.Id.IsGuidEmpty() && !item.FkAddressAppId.IsGuidEmpty() && a.Id.Equals(item.FkAddressAppId))) && a.IsActif == 1);
            }
        }

        private void SendMail(object sender, TaskCompletionSource<bool> tcs)
        {
            try
            {
                if (!Client.Email.EmailValidate())
                {
                    tcs.SetResult(true);
                    return;
                }

                var emailMessenger = CrossMessaging.Current.EmailMessenger;
                if (emailMessenger.CanSendEmail)
                {
                    // Send simple e-mail to single receiver without attachments, bcc, cc etc.
                    emailMessenger.SendEmail(
                        Client.Email,
                        "LogWork Test Send Mail To Client",
                        Client.ToString());

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

        private async void ViewAddress(Address value, TaskCompletionSource<bool> tcs)
        {
            if (IsBusy)
            {
                tcs.SetResult(true);
                return;
            }

            var parameters = new NavigationParameters()
            {
                { ContentKey.SELECTED_ADDRESS, value}
            };

            await CoreMethods.PushViewModel<AddressDetailViewModel>(parameters);

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

        private async void AddIntervention(object sender, TaskCompletionSource<bool> tcs)
        {
            if (IsBusy)
                return;

            var parameters = new NavigationParameters()
            {
                { ContentKey.SELECTED_CUSTOMER, Client}
            };

            await CoreMethods.PushViewModel<NewInterventionViewModel>(parameters);

            tcs.SetResult(true);
        }
    }
}