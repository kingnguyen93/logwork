using LogWork.Constants;
using LogWork.Models;
using System.Threading.Tasks;
using System.Windows.Input;
using TinyMVVM;

using Xamarin.Forms;
using Xamarin.Forms.Extensions;

namespace LogWork.ViewModels.Shared
{
    public class MediaDetailViewModel : TinyViewModel
    {
        private MediaLink mediaLink;
        public MediaLink MediaLink { get => mediaLink; set => SetProperty(ref mediaLink, value); }

        public ICommand CancelCommand { get; private set; }
        public ICommand DeleteMediaCommand { get; private set; }
        public ICommand SaveMediaCommand { get; private set; }
        public ICommand ShareMediaCommand { get; private set; }

        public MediaDetailViewModel()
        {
            CancelCommand = new AwaitCommand(Cancel);
            DeleteMediaCommand = new AwaitCommand(DeleteMedia);
            SaveMediaCommand = new AwaitCommand(SaveMedia);
            ShareMediaCommand = new AwaitCommand(ShareMedia);
        }

        public override void Init(NavigationParameters parameters)
        {
            base.Init(parameters);

            MediaLink = parameters?.GetValue<MediaLink>(ContentKey.SELECTED_MEDIA)?.DeepCopy();

            Title = MediaLink.Media.FileName;
        }

        private async void DeleteMedia(object sender, TaskCompletionSource<bool> tcs)
        {
            if (await CoreMethods.DisplayAlert(TranslateExtension.GetValue("alert_title_media"), TranslateExtension.GetValue("alert_message_delete_media_confirm"), TranslateExtension.GetValue("alert_message_yes"), TranslateExtension.GetValue("alert_message_no")))
            {
                MediaLink.IsDelete = true;

                MessagingCenter.Send(this, MessageKey.MEDIA_SAVED, MediaLink);

                await CoreMethods.PopViewModel(modal: IsModal);
            };

            tcs.SetResult(true);
        }

        private async void SaveMedia(object sender, TaskCompletionSource<bool> tcs)
        {
            MessagingCenter.Send(this, MessageKey.MEDIA_SAVED, MediaLink);

            await CoreMethods.PopViewModel(modal: IsModal);

            tcs.SetResult(true);
        }

        private void ShareMedia(object sender, TaskCompletionSource<bool> tcs)
        {
            tcs.SetResult(true);
        }

        private async void Cancel(object sender, TaskCompletionSource<bool> tcs)
        {
            await CoreMethods.PopViewModel(modal: IsModal);

            tcs.TrySetResult(true);
        }
    }
}