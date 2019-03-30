using LogWork.Constants;
using LogWork.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using TinyMVVM;

namespace LogWork.ViewModels.Shared
{
    public class UserLookUpViewModel : TinyViewModel
    {
        private readonly int CurrentUserId = Settings.CurrentUserId;

        private CancellationTokenSource cts;
        
        private List<User> listUser;
        public List<User> ListUser { get => listUser; set => SetProperty(ref listUser, value); }

        private string searchKey;
        public string SearchKey { get => searchKey; set => SetProperty(ref searchKey, value, onChanged: OnsearchKeyChanged); }

        public ICommand UserSelectedCommand { get; private set; }
        public ICommand CancelSearchCommand { get; private set; }

        public UserLookUpViewModel()
        {
            UserSelectedCommand = new AwaitCommand<User>(ClientSelected);
            CancelSearchCommand = new AwaitCommand(CancelSearch);
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
                    return App.LocalDb.Table<User>().ToList().FindAll(u => u.UserId == CurrentUserId && u.ServerId != CurrentUserId && u.IsActif == 1);
                }
                else
                {
                    return App.LocalDb.Table<User>().ToList().FindAll(u => u.UserId == CurrentUserId && u.ServerId != CurrentUserId && ((!string.IsNullOrWhiteSpace(u.FullName) && u.FullName.UnSignContains(SearchKey)) || (!string.IsNullOrWhiteSpace(u.Phone) && u.Phone.UnSignContains(SearchKey))) && u.IsActif == 1);
                }
            }, token).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    ListUser = task.Result;
                }
                else if (task.IsFaulted && !token.IsCancellationRequested)
                {
                    //CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }
            }));
        }

        private async void ClientSelected(User value, TaskCompletionSource<bool> tcs)
        {
            MessagingCenter.Send(this, MessageKey.USER_SELECTED, value);

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