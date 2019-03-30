using LogWork.Constants;
using LogWork.Models;
using LogWork.Models.Response;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TinyMVVM;

using Xamarin.Forms;
using Xamarin.Forms.Extensions;

namespace LogWork.ViewModels.Messages
{
    public class MessageDetailViewModel : TinyViewModel
    {
        private readonly LoginResponse CurrentUser = Settings.CurrentUser;

        private MessageByUser selectedMessage;

        private Message newMessage = new Message();
        public Message NewMessage { get => newMessage; set => SetProperty(ref newMessage, value); }

        private ObservableCollection<Message> messages;
        public ObservableCollection<Message> Messages { get => messages; set => SetProperty(ref messages, value); }

        public ICommand SendMessageCommand { get; private set; }

        public MessageDetailViewModel()
        {
            SendMessageCommand = new AwaitCommand(SendMessage);
        }

        public override void Init(NavigationParameters parameters)
        {
            base.Init(parameters);

            selectedMessage = parameters.GetValue<MessageByUser>(ContentKey.SELECTED_MESSAGE);

            Title = selectedMessage?.Title;

            Messages = App.LocalDb.Table<Message>().ToList()
                .FindAll(mes => mes.UserId == CurrentUser.Id
                    && mes.Title == selectedMessage.Title
                    && mes.IsDelete == 0 && mes.IsActif == 1)
                .OrderBy(mes => mes.AddDate).ThenBy(mes => mes.EditDate).ToObservableCollection();

            foreach (var mes in Messages)
            {
                mes.UserFrom = App.LocalDb.Table<User>()
                    .FirstOrDefault(u => u.UserId == CurrentUser.Id
                        && ((u.ServerId > 0 && u.ServerId == mes.FkUserServerIdFrom) || (!u.Id.Equals(Guid.Empty) && !mes.FkUserAppliIdFrom.Equals(Guid.Empty) && u.Id.Equals(mes.FkUserAppliIdFrom)))
                        && u.IsActif == 1);

                mes.UserTo = App.LocalDb.Table<User>()
                    .FirstOrDefault(u => u.UserId == CurrentUser.Id
                        && ((u.ServerId > 0 && u.ServerId == mes.FkUserServerIdTo) || (!u.Id.Equals(Guid.Empty) && !mes.FkUserAppliIdTo.Equals(Guid.Empty) && u.Id.Equals(mes.FkUserAppliIdTo)))
                        && u.IsActif == 1);
            }
        }

        private void SendMessage(object sender, TaskCompletionSource<bool> tcs)
        {
            NewMessage.Id = Guid.NewGuid();
            NewMessage.UserId = CurrentUser.Id;

            NewMessage.Title = selectedMessage.Title;
            NewMessage.FkUserAppliIdFrom = CurrentUser.Uuid;
            NewMessage.FkUserServerIdFrom = CurrentUser.Id;
            NewMessage.FkUserAppliIdTo = CurrentUser.Uuid == selectedMessage.LastMessage.FkUserAppliIdFrom ? selectedMessage.LastMessage.FkUserAppliIdTo : selectedMessage.LastMessage.FkUserAppliIdFrom;
            NewMessage.FkUserServerIdTo = CurrentUser.Id == selectedMessage.LastMessage.FkUserServerIdFrom ? selectedMessage.LastMessage.FkUserServerIdTo : selectedMessage.LastMessage.FkUserServerIdFrom;

            NewMessage.FkMessagerieServerId = selectedMessage.LastMessage.ServerId;

            NewMessage.UserFrom = App.LocalDb.Table<User>()
                .FirstOrDefault(u => u.UserId == CurrentUser.Id
                    && ((u.ServerId > 0 && u.ServerId == NewMessage.FkUserServerIdFrom) || (!u.Id.Equals(Guid.Empty) && !NewMessage.FkUserAppliIdFrom.Equals(Guid.Empty) && u.Id.Equals(NewMessage.FkUserAppliIdFrom)))
                    && u.IsActif == 1);

            NewMessage.UserTo = App.LocalDb.Table<User>()
                .FirstOrDefault(u => u.UserId == CurrentUser.Id
                    && ((u.ServerId > 0 && u.ServerId == NewMessage.FkUserServerIdTo) || (!u.Id.Equals(Guid.Empty) && !NewMessage.FkUserAppliIdTo.Equals(Guid.Empty) && u.Id.Equals(NewMessage.FkUserAppliIdTo)))
                    && u.IsActif == 1);

            NewMessage.IsActif = 1;
            NewMessage.AddDate = DateTime.Now;

            NewMessage.IsToSync = true;

            App.LocalDb.Insert(NewMessage);

            MessagingCenter.Send(this, MessageKey.MESSAGE_CHANGED);

            Messages.Add(NewMessage);

            NewMessage = new Message();

            tcs.TrySetResult(true);
        }
    }
}