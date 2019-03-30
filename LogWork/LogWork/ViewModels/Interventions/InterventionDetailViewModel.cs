using Acr.UserDialogs;
using LogWork.Constants;
using LogWork.Helpers;
using LogWork.IServices;
using LogWork.Models;
using LogWork.Models.Response;
using LogWork.ViewModels.Shared;
using Plugin.ExternalMaps;
using Plugin.Messaging;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TinyMVVM;

using Xamarin.Forms;
using Xamarin.Forms.Extensions;

namespace LogWork.ViewModels.Interventions
{
    public class InterventionDetailViewModel : TinyViewModel
    {
        private readonly IInterventionService interventionService;

        private readonly LoginResponse CurrentUser = Settings.CurrentUser;

        private Intervention intervention;
        public Intervention Intervention { get => intervention; set => SetProperty(ref intervention, value); }

        private bool isDone;
        public bool IsDone { get => isDone; set => SetProperty(ref isDone, value); }

        private bool isAssigned;
        public bool IsAssigned { get => isAssigned; set => SetProperty(ref isAssigned, value); }

        private bool detailVisible = true;
        public bool DetailVisible { get => detailVisible; set => SetProperty(ref detailVisible, value); }

        private Color detailTabColor = Color.FromHex("#47CEC0");
        public Color DetailTabColor { get => detailTabColor; set => SetProperty(ref detailTabColor, value); }

        private bool mediaVisible;
        public bool MediaVisible { get => mediaVisible; set => SetProperty(ref mediaVisible, value); }

        private Color mediaTabColor;
        public Color MediaTabColor { get => mediaTabColor; set => SetProperty(ref mediaTabColor, value); }

        public ICommand DeleteInterventionCommand { get; private set; }
        public ICommand EditInterventionCommand { get; private set; }
        public ICommand AssignToMeCommand { get; private set; }
        public ICommand OpenInterventionDetailCommand { get; private set; }
        public ICommand SendMailToCustomerCommand { get; private set; }
        public ICommand ViewDetailCommand { get; private set; }
        public ICommand ViewMediasCommand { get; private set; }
        public ICommand OpenMapCommand { get; private set; }
        public ICommand TakePhotoCommand { get; private set; }
        public ICommand PickPhotoCommand { get; private set; }
        public ICommand SignatureCommand { get; private set; }
        public ICommand ViewMediaCommand { get; private set; }
        public ICommand DeleteMediaCommand { get; private set; }
        public ICommand ViewHistoryCommand { get; private set; }

        public InterventionDetailViewModel(IInterventionService interventionService)
        {
            this.interventionService = interventionService;

            DeleteInterventionCommand = new AwaitCommand(DeleteIntervention);
            EditInterventionCommand = new AwaitCommand(EditIntervention);
            AssignToMeCommand = new AwaitCommand(AssignToMe);
            OpenInterventionDetailCommand = new AwaitCommand(OpenInterventionDetail);
            SendMailToCustomerCommand = new AwaitCommand(SendMailToCustomer);
            ViewDetailCommand = new AwaitCommand(ViewDetail);
            ViewMediasCommand = new AwaitCommand(ViewMedias);
            OpenMapCommand = new AwaitCommand(OpenMap);
            TakePhotoCommand = new AwaitCommand(TakePhoto);
            PickPhotoCommand = new AwaitCommand(PickPhoto);
            SignatureCommand = new AwaitCommand(Signature);
            ViewMediaCommand = new AwaitCommand<MediaLink>(ViewMedia);
            DeleteMediaCommand = new AwaitCommand<MediaLink>(DeleteMedia);
            ViewHistoryCommand = new AwaitCommand(ViewHistory);
        }

        public override void Init(NavigationParameters parameters)
        {
            base.Init(parameters);

            Intervention = parameters.GetValue<Intervention>(ContentKey.SELECTED_INTERVENTION)?.DeepCopy();

            InitIntervention();

            if (!IsAssigned && Intervention.LastViewDate == null)
            {
                Intervention.LastViewDate = DateTime.Now;
                App.LocalDb.Update(Intervention);
                MessagingCenter.Send(this, MessageKey.INTERVENTION_CHANGED);
                MessagingCenter.Send(this, MessageKey.INTERVENTION_CHANGED, Intervention.Id);
            }

            PropertyChanged += ViewModel_PropertyChanged;
        }

        public override void OnPushed(NavigationParameters parameters)
        {
            base.OnPushed(parameters);

            MessagingCenter.Subscribe<MediaDetailViewModel, MediaLink>(this, MessageKey.MEDIA_SAVED, OnMediaSaved);
            MessagingCenter.Subscribe<NewInterventionViewModel>(this, MessageKey.INTERVENTION_CHANGED, OnInterventionChanged);
        }

        public override void OnPopped()
        {
            MessagingCenter.Unsubscribe<MediaDetailViewModel, MediaLink>(this, MessageKey.MEDIA_SAVED);
            MessagingCenter.Unsubscribe<NewInterventionViewModel>(this, MessageKey.INTERVENTION_CHANGED);
        }

        private async void OnInterventionChanged(NewInterventionViewModel sender)
        {
            Intervention = await interventionService.GetIntervention(Intervention.Id);

            InitIntervention();
        }

        private void InitIntervention()
        {
            if (Intervention != null)
            {
                if (Intervention.Code != 0)
                    Title = "Nº " + Intervention.Code;
                else
                    Title = " ";

                Intervention.PlanningTasks = new ObservableCollection<LinkInterventionTask>();
                Intervention.PlanningTasks.AddRange(Intervention.LinkInterventionTasks.FindAll(lit => lit.PlanningMinute > 0));
                Intervention.DoneTasks = new ObservableCollection<LinkInterventionTask>();
                Intervention.DoneTasks.AddRange(Intervention.LinkInterventionTasks.FindAll(lit => lit.DoneMinute > 0));

                IsAssigned = Intervention.FkUserServerlId != 0;

                Intervention.PropertyChanged += Intervention_PropertyChanged;
            }
        }

        private async void Intervention_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Intervention.PropertyChanged -= Intervention_PropertyChanged;

            if (e.PropertyName == nameof(Intervention.SendMail))
            {
                Intervention.IsToSync = true;

                await interventionService.UpdateIntervention(Intervention);

                MessagingCenter.Send(this, MessageKey.INTERVENTION_CHANGED);
            }

            Intervention.PropertyChanged += Intervention_PropertyChanged;
        }

        private async void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged -= ViewModel_PropertyChanged;

            if (e.PropertyName == nameof(IsDone))
            {
                try
                {
                    if (IsDone)
                    {
                        Intervention.IsDone = 1;

                        if (await LocationHelper.CheckLocationPermission(false) && LocationHelper.IsGeolocationAvailable(false) && LocationHelper.IsGeolocationEnabled(false))
                        {
                            var position = await LocationHelper.GetCurrentPosition(showOverlay: false);

                            if (position != null && (position.Latitude != 0 && position.Longitude != 0))
                            {
                                Intervention.DoneLatitude = position.Latitude;
                                Intervention.DoneLongitude = position.Longitude;
                                Intervention.DoneAltitude = position.Altitude;
                            }
                        }

                        await interventionService.CalculateDoneTime(Intervention);

                        MessagingCenter.Send(this, MessageKey.INTERVENTION_CHANGED);
                    }
                    else
                    {
                        if (await CoreMethods.DisplayAlert(TranslateExtension.GetValue("alert_title_intervention"), TranslateExtension.GetValue("alert_message_intervention_undone"), TranslateExtension.GetValue("yes"), TranslateExtension.GetValue("no")))
                        {
                            Intervention.IsDone = AppSettings.MobileShowToggleProgress ? 2 : 0;

                            await interventionService.CalculateDoneTime(Intervention);

                            MessagingCenter.Send(this, MessageKey.INTERVENTION_CHANGED);
                        }
                        else
                        {
                            Intervention.IsDone = 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), ex.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }
            }

            PropertyChanged += ViewModel_PropertyChanged;
        }

        private async void DeleteIntervention(object sender, TaskCompletionSource<bool> tcs)
        {
            if (await CoreMethods.DisplayAlert(TranslateExtension.GetValue("alert_title_intervention"), TranslateExtension.GetValue("alert_message_delete_intervention_confirm"), TranslateExtension.GetValue("alert_message_yes"), TranslateExtension.GetValue("alert_message_no")))
            {
                Intervention.IsActif = 0;
                Intervention.IsToSync = true;

                await interventionService.UpdateIntervention(Intervention);

                MessagingCenter.Send(this, MessageKey.INTERVENTION_CHANGED);

                await CoreMethods.PopViewModel();
            };

            tcs.TrySetResult(true);
        }

        private async void EditIntervention(object sender, TaskCompletionSource<bool> tcs)
        {
            try
            {
                var parameters = new NavigationParameters()
                {
                    { ContentKey.INTERVENTION_MODE,  1},
                    { ContentKey.SELECTED_INTERVENTION,  Intervention}
                };

                await CoreMethods.PushViewModel<NewInterventionViewModel>(parameters);
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

        private void AssignToMe(object sender, TaskCompletionSource<bool> tcs)
        {
            if (!ConnectivityHelper.IsNetworkAvailable())
            {
                tcs.TrySetResult(true);
                return;
            }

            UserDialogs.Instance.Loading(TranslateExtension.GetValue("alert_message_assigning")).Show();

            Task.Run(async () =>
            {
                return await interventionService.AssignToMe(Intervention.ServerId);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(async () =>
            {
                UserDialogs.Instance.Loading().Hide();

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result)
                    {
                        Intervention.FkUserAppId = CurrentUser.Uuid;
                        Intervention.FkUserServerlId = CurrentUser.Id;

                        await interventionService.UpdateIntervention(Intervention);

                        MessagingCenter.Send(this, MessageKey.INTERVENTION_ASSIGNED);

                        await CoreMethods.DisplayAlert("", TranslateExtension.GetValue("alert_message_intervention_assigned"), TranslateExtension.GetValue("ok"));

                        await CoreMethods.PopViewModel();
                    }
                    else
                    {
                        await CoreMethods.DisplayAlert("", TranslateExtension.GetValue("alert_message_intervention_already_assigned"), TranslateExtension.GetValue("ok"));
                    }
                }
                else
                {
                    if (task.IsFaulted && task.Exception?.GetBaseException().Message is string message)
                        Debug.Write("ASSING_INTERVENTION_ERROR: " + message);

                    await CoreMethods.DisplayAlert("", TranslateExtension.GetValue("alert_message_intervention_assign_error"), TranslateExtension.GetValue("ok"));
                }
            }));

            tcs.TrySetResult(true);
        }

        private void OpenInterventionDetail(object sender, TaskCompletionSource<bool> tcs)
        {
            if (Intervention.ServerId > 0 && !string.IsNullOrWhiteSpace(Intervention.Nonce))
            {
                Device.OpenUri(new Uri(ApiURI.URL_PDF(Settings.CurrentAccount, Intervention.ServerId, Intervention.Nonce)));
            }

            tcs.TrySetResult(true);
        }

        private void ViewDetail(object sender, TaskCompletionSource<bool> tcs)
        {
            DetailTabColor = Color.FromHex("#47CEC0");
            DetailVisible = true;

            MediaTabColor = Color.Transparent;
            MediaVisible = false;

            tcs.TrySetResult(true);
        }

        private void ViewMedias(object sender, TaskCompletionSource<bool> tcs)
        {
            MediaTabColor = Color.FromHex("#47CEC0");
            MediaVisible = true;

            DetailTabColor = Color.Transparent;
            DetailVisible = false;

            tcs.TrySetResult(true);
        }

        private void SendMailToCustomer(object sender, TaskCompletionSource<bool> tcs)
        {
            try
            {
                var emailMessenger = CrossMessaging.Current.EmailMessenger;
                if (emailMessenger.CanSendEmail)
                {
                    // Send simple e-mail to single receiver without attachments, bcc, cc etc.
                    emailMessenger.SendEmail(
                        Intervention.Client.Email,
                        "LogWork Test",
                        Intervention.ToString());

                    UserDialogs.Instance.Toast(new ToastConfig(TranslateExtension.GetValue("alert_message_send_mail_successed")));
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

            tcs.TrySetResult(true);
        }

        private async void OpenMap(object sender, TaskCompletionSource<bool> tcs)
        {
            if (CrossExternalMaps.IsSupported)
            {
                await CrossExternalMaps.Current.NavigateTo("", Intervention.Address.Latitude, Intervention.Address.Longitude);
            }
            else
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    Device.OpenUri(new Uri(string.Format("http://maps.apple.com/?daddr=" + Intervention.Address.Latitude + "," + Intervention.Address.Longitude + "")));
                }
                else if (Device.RuntimePlatform == Device.Android)
                {
                    Device.OpenUri(new Uri(string.Format("http://maps.google.com/?daddr=" + Intervention.Address.Latitude + "," + Intervention.Address.Longitude + "")));
                }
            }

            tcs.TrySetResult(true);
        }

        private async void TakePhoto(object sender, TaskCompletionSource<bool> tcs)
        {
            var photo = await PhotoHelper.TakePhotoStreamAsync();

            if (photo != null)
                SetMedia(photo, false);

            tcs.TrySetResult(true);
        }

        private async void PickPhoto(object sender, TaskCompletionSource<bool> tcs)
        {
            var photo = await PhotoHelper.PickPhotoStreamAsync();

            if (photo != null)
                SetMedia(photo, false);

            tcs.TrySetResult(true);
        }

        private async void Signature(object sender, TaskCompletionSource<bool> tcs)
        {
            var signaturePad = new Views.Popups.SignaturePad();
            signaturePad.ContentSaved += SignaturePad_ContentSaved;

            await CoreMethods.PushPage(signaturePad, modal: true);

            tcs.TrySetResult(true);
        }

        private void SignaturePad_ContentSaved(object sender, Stream content)
        {
            if (content != null)
                SetMedia(content, true);
        }

        private async void SetMedia(Stream content, bool isSignature)
        {
            string fileName;

            if (isSignature)
                fileName = "SIG_" + DateTime.Now.ToString("ddMMyyyyHHmmss");
            else
                fileName = "IMG_" + DateTime.Now.ToString("ddMMyyyyHHmmss");

            var media = new Media()
            {
                Id = Guid.NewGuid(),
                UserId = CurrentUser.Id,
                AccountId = CurrentUser.Id,
                FileName = fileName,
                Year = DateTime.Today.Year.ToString(),
                Month = DateTime.Today.Month.ToString(),
                FileData = await content.ToBase64(),
                IsActif = 1,
                AddDate = DateTime.Now,
                EditDate = DateTime.Now,
                IsToSync = true
            };

            var mediaLink = new MediaLink()
            {
                Id = Guid.NewGuid(),
                UserId = CurrentUser.Id,
                FkColumnAppliId = Intervention.Id,
                FkColumnServerId = Intervention.ServerId,
                FkMediaAppliId = media.Id,
                FkMediaServerId = media.ServerId,
                LinkTable = "intervention",
                IsActif = 1,
                AddDate = DateTime.Now,
                EditDate = DateTime.Now,
                Media = media,
                IsToSync = true
            };

            Intervention.MediaLinks.Add(mediaLink);

            App.LocalDb.InsertOrReplace(media);
            App.LocalDb.InsertOrReplace(mediaLink);

            Intervention.IsToSync = true;

            Intervention.OnPropertyChanged("MediaLinks");
        }

        private async void ViewMedia(MediaLink mediaLink, TaskCompletionSource<bool> tcs)
        {
            try
            {
                var parameters = new NavigationParameters()
                {
                    { ContentKey.SELECTED_MEDIA, mediaLink}
                };

                await CoreMethods.PushViewModel<MediaDetailViewModel>(parameters, modal: true);
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

        private void OnMediaSaved(object sender, MediaLink mediaLink)
        {
            try
            {
                if (mediaLink.IsDelete)
                {
                    if (mediaLink.ServerId > 0)
                    {
                        if (mediaLink.Media != null && mediaLink.Media.ServerId > 0)
                        {
                            mediaLink.Media.SetProperty(nameof(mediaLink.Media.IsActif), 0);
                            mediaLink.Media.IsToSync = true;

                            App.LocalDb.Update(mediaLink.Media);
                        }
                        else
                        {
                            App.LocalDb.Delete(mediaLink.Media);
                        }

                        mediaLink.SetProperty(nameof(mediaLink.IsActif), 0);
                        mediaLink.IsToSync = true;

                        App.LocalDb.Update(mediaLink);
                    }
                    else
                    {
                        App.LocalDb.Delete(mediaLink.Media);
                        App.LocalDb.Delete(mediaLink);
                    }

                    Intervention.MediaLinks.Remove(mediaLink);
                }
                else
                {
                    mediaLink.Media.EditDate = DateTime.Now;
                    mediaLink.EditDate = DateTime.Now;

                    mediaLink.Media.IsToSync = true;
                    mediaLink.IsToSync = true;

                    App.LocalDb.Update(mediaLink.Media);
                    App.LocalDb.Update(mediaLink);
                }

                if (Intervention.MediaLinks.FirstOrDefault(m => m.Id == mediaLink.Id) is MediaLink mediaLnk)
                {
                    Intervention.MediaLinks[Intervention.MediaLinks.IndexOf(mediaLnk)] = mediaLink;
                }
            }
            catch (Exception ex)
            {
                CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), ex.Message, TranslateExtension.GetValue("ok"));
            }
        }

        private void DeleteMedia(MediaLink mediaLink, TaskCompletionSource<bool> tcs)
        {
            if (mediaLink.ServerId > 0)
            {
                if (mediaLink.Media != null && mediaLink.Media.ServerId > 0)
                {
                    mediaLink.Media.IsActif = 0;
                    mediaLink.Media.IsToSync = true;

                    App.LocalDb.Update(mediaLink.Media);
                }
                else
                {
                    App.LocalDb.Delete(mediaLink.Media);
                }

                mediaLink.IsActif = 0;
                mediaLink.IsToSync = true;

                App.LocalDb.Update(mediaLink);
            }
            else
            {
                if (mediaLink.Media != null)
                    App.LocalDb.Delete(mediaLink.Media);
                App.LocalDb.Delete(mediaLink);
            }

            Intervention.MediaLinks.Remove(mediaLink);

            Intervention.OnPropertyChanged("MediaLinks");

            tcs.TrySetResult(true);
        }

        private async void ViewHistory(object sender, TaskCompletionSource<bool> tcs)
        {
            try
            {
                var parameters = new NavigationParameters()
                {
                    { ContentKey.SELECTED_INTERVENTION, Intervention}
                };

                await CoreMethods.PushViewModel<InterventionHistoryViewModel>(parameters);
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