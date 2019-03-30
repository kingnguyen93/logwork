using LogWork.Constants;
using LogWork.Models;
using LogWork.Models.Response;
using LogWork.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using TinyMVVM;

namespace LogWork.ViewModels.Messages
{
    public class NewMessageViewModel : TinyViewModel
    {
        private readonly LoginResponse CurrentUser = Settings.CurrentUser;

        private Message newMessage = new Message();
        public Message NewMessage { get => newMessage; set => SetProperty(ref newMessage, value); }

        private User userReceive;
        public User UserReceive { get => userReceive; set => SetProperty(ref userReceive, value); }

        private string userText;
        public string UserText { get => userText; set => SetProperty(ref userText, value, onChanged: OnUserTextChanged); }

        private List<User> listUserResult;
        public List<User> ListUserResult { get => listUserResult; set => SetProperty(ref listUserResult, value); }

        private bool listUserResultVisible;
        public bool ListUserResultVisible { get => listUserResultVisible; set => SetProperty(ref listUserResultVisible, value); }

        public ICommand CancelCommand { get; private set; }
        public ICommand AddUserCommand { get; private set; }
        public ICommand SendMessageCommand { get; private set; }

        public NewMessageViewModel()
        {
            CancelCommand = new AwaitCommand(Cancel);
            AddUserCommand = new AwaitCommand(AddUser);
            SendMessageCommand = new AwaitCommand(SendMessage);
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();

            MessagingCenter.Subscribe<UserLookUpViewModel, User>(this, MessageKey.USER_SELECTED, OnUserSelected);
        }

        public override void OnPopped()
        {
            base.OnPopped();

            MessagingCenter.Unsubscribe<UserLookUpViewModel, User>(this, MessageKey.USER_SELECTED);
        }

        private void OnUserSelected(UserLookUpViewModel sender, User value)
        {
            UserReceive = value;
        }

        private void OnUserTextChanged()
        {
            if (IsDisposing)
                return;

            if (string.IsNullOrWhiteSpace(UserText))
            {
                ListUserResultVisible = false;
            }
            else
            {
                ListUserResult = App.LocalDb.Table<User>().ToList().FindAll(u => u.UserId == CurrentUser.Id && ((string.IsNullOrWhiteSpace(u.FullName) && u.FullName.UnSignContains(UserText)) || (string.IsNullOrWhiteSpace(u.Phone) && u.Phone.UnSignContains(UserText))) && u.IsActif == 1);
                ListUserResultVisible = true;
            }
        }

        private async void Cancel(object sender, TaskCompletionSource<bool> tcs)
        {
            await CoreMethods.PopViewModel(modal: IsModal);

            tcs.SetResult(true);
        }

        private async void AddUser(object sender, TaskCompletionSource<bool> tcs)
        {
            await CoreMethods.PushViewModel<UserLookUpViewModel>(modal: IsModal);

            tcs.SetResult(true);
        }

        private async void SendMessage(object sender, TaskCompletionSource<bool> tcs)
        {
            if (UserReceive == null)
            {
                await CoreMethods.DisplayAlert("", "Please an user to send", TranslateExtension.GetValue("ok"));
                return;
            }

            if (string.IsNullOrWhiteSpace(NewMessage.Title))
            {
                await CoreMethods.DisplayAlert("", "Title is empty", TranslateExtension.GetValue("ok"));
                return;
            }

            if (string.IsNullOrWhiteSpace(NewMessage.Content))
            {
                await CoreMethods.DisplayAlert("", "Content is empty", TranslateExtension.GetValue("ok"));
                return;
            }

            NewMessage.Id = Guid.NewGuid();
            NewMessage.UserId = CurrentUser.Id;

            NewMessage.FkUserAppliIdFrom = CurrentUser.Uuid;
            NewMessage.FkUserServerIdFrom = CurrentUser.Id;
            NewMessage.FkUserAppliIdTo = UserReceive.Id;
            NewMessage.FkUserServerIdTo = UserReceive.ServerId;

            NewMessage.IsActif = 1;
            NewMessage.AddDate = DateTime.Now;

            NewMessage.IsToSync = true;

            App.LocalDb.Insert(NewMessage);

            MessagingCenter.Send(this, MessageKey.MESSAGE_CHANGED);

            await CoreMethods.PopViewModel(modal: IsModal);

            tcs.TrySetResult(true);
        }
    }
}