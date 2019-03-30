using LogWork.Constants;
using LogWork.IServices;
using LogWork.Models;
using LogWork.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using TinyMVVM;

namespace LogWork.ViewModels.Messages
{
    public class MessagesViewModel : TinyViewModel
    {
        private readonly ISyncDataService syncService;
        private readonly LoginResponse CurrentUser = Settings.CurrentUser;

        private bool dataChanged = true;

        private List<Message> listMessage;
        public List<Message> ListMessage { get => listMessage; set => SetProperty(ref listMessage, value); }

        private List<MessageByUser> listMessageByUser;
        public List<MessageByUser> ListMessageByUser { get => listMessageByUser; set => SetProperty(ref listMessageByUser, value); }

        public ICommand GetSyncCommand { get; set; }
        public ICommand ViewMessageCommand { get; private set; }
        public ICommand CreateNewMessageCommand { get; private set; }

        public MessagesViewModel(ISyncDataService syncService)
        {
            this.syncService = syncService;

            GetSyncCommand = new Command(GetSync);
            ViewMessageCommand = new AwaitCommand<MessageByUser>(ViewMessage);
            CreateNewMessageCommand = new AwaitCommand(CreateNewMessage);
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            if (dataChanged)
            {
                SyncFromServer(2);

                dataChanged = false;
            }
        }

        private void GetMessages()
        {
            ListMessage = App.LocalDb.Table<Message>().ToList()
                .FindAll(mes => mes.UserId == CurrentUser.Id
                    && (((mes.FkUserServerIdFrom > 0 && mes.FkUserServerIdFrom == CurrentUser.Id) || (!mes.FkUserAppliIdFrom.Equals(Guid.Empty) && mes.FkUserAppliIdFrom.Equals(CurrentUser.Uuid)))
                    || ((mes.FkUserServerIdTo > 0 && mes.FkUserServerIdTo == CurrentUser.Id) || (!mes.FkUserAppliIdTo.Equals(Guid.Empty) && mes.FkUserAppliIdTo.Equals(CurrentUser.Uuid))))
                    && mes.IsDelete == 0 && mes.IsActif == 1)
                .OrderByDescending(mes => mes.AddDate).ThenByDescending(mes => mes.EditDate).ToList();

            if (ListMessage != null && ListMessage.Count > 0)
            {
                List<MessageByUser> result = new List<MessageByUser>();
                foreach (var mes in ListMessage.GroupBy(mes => mes.Title))
                {
                    result.Add(new MessageByUser()
                    {
                        Title = mes.Key,
                        LastMessage = mes.LastOrDefault()
                    });
                }
                ListMessageByUser = result;
            }
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();

            MessagingCenter.Subscribe<NewMessageViewModel>(this, MessageKey.MESSAGE_CHANGED, OnMessageChanged);
            MessagingCenter.Subscribe<MessageDetailViewModel>(this, MessageKey.MESSAGE_CHANGED, OnMessageChanged);
        }

        public override void OnPopped()
        {
            base.OnPopped();

            MessagingCenter.Unsubscribe<NewMessageViewModel>(this, MessageKey.MESSAGE_CHANGED);
            MessagingCenter.Unsubscribe<MessageDetailViewModel>(this, MessageKey.MESSAGE_CHANGED);
        }

        private void OnMessageChanged(object sender)
        {
            dataChanged = true;
        }

        private void GetSync(object sender)
        {
            SyncFromServer(2);
        }

        private void SyncFromServer(int method)
        {
            syncService.SyncFromServer(method: method, onSuccess: GetMessages, showOverlay: true);
        }

        private async void ViewMessage(MessageByUser value, TaskCompletionSource<bool> tcs)
        {
            var parameters = new NavigationParameters()
            {
                { ContentKey.SELECTED_MESSAGE, value}
            };

            await CoreMethods.PushViewModel<MessageDetailViewModel>(parameters);

            tcs.TrySetResult(true);
        }

        private async void CreateNewMessage(object sender, TaskCompletionSource<bool> tcs)
        {
            await CoreMethods.PushViewModel<NewMessageViewModel>(modal: true);

            tcs.TrySetResult(true);
        }
    }
}