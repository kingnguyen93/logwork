using Acr.UserDialogs;
using LogWork.Constants;
using LogWork.Helpers;
using LogWork.IServices;
using LogWork.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using TinyMVVM;

namespace LogWork.ViewModels.Interventions
{
    public class InterventionsViewModel : BaseViewModel
    {
        private readonly IInterventionService interventionService;

        private bool interventionChanged = true;

        private CancellationTokenSource cts;

        private DateTime selectedDate = DateTime.Now;
        public DateTime SelectedDate { get => selectedDate; set => SetProperty(ref selectedDate, value, onChanged: () => GetInterventions(null)); }

        private List<Intervention> listIntervention = new List<Intervention>();
        public List<Intervention> ListIntervention { get => listIntervention; set => SetProperty(ref listIntervention, value); }

        private bool isRefreshingIntervention;
        public bool IsRefreshingIntervention { get => isRefreshingIntervention; set => SetProperty(ref isRefreshingIntervention, value); }

        public ICommand GetSyncCommand { get; private set; }
        public ICommand BeforeDayCommand { get; private set; }
        public ICommand NextDayCommand { get; private set; }
        public ICommand GetInterventionsCommand { get; private set; }
        public ICommand ChangeDoneCommand { get; private set; }
        public ICommand AddInterventionCommand { get; private set; }
        public ICommand EditInterventionCommand { get; private set; }

        public InterventionsViewModel(IInterventionService interventionService)
        {
            this.interventionService = interventionService;

            GetSyncCommand = new Command(GetSync);
            BeforeDayCommand = new Command(BeforeDay);
            NextDayCommand = new Command(NextDay);
            GetInterventionsCommand = new Command(GetInterventions);
            ChangeDoneCommand = new AwaitCommand<Intervention>(ChangeDone);
            AddInterventionCommand = new AwaitCommand(AddIntervention);
            EditInterventionCommand = new AwaitCommand<Intervention>(EditIntervention);
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
            interventionChanged = true;
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            if (interventionChanged)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    GetInterventions(null);
                    interventionChanged = false;
                });
            }
        }

        private void GetSync(object sender)
        {
            syncService.SyncFromServer(method: 2, onSuccess: () => GetInterventions(null), showOverlay: true);
        }

        private void BeforeDay(object sender)
        {
            SelectedDate = SelectedDate.Subtract(TimeSpan.FromDays(1));
        }

        private void NextDay(object sender)
        {
            SelectedDate = SelectedDate.AddDays(1);
        }

        private void GetInterventions(object sender)
        {
            if (IsDisposing)
                return;

            IsRefreshingIntervention = true;

            if (cts != null)
                cts.Cancel();

            cts = new CancellationTokenSource();
            var token = cts.Token;

            Task.Run(async () =>
            {
                await Task.Delay(250, token);
                return await interventionService.GetInterventions(SelectedDate, AppSettings.MobileShowNotDone);
            }, token).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    ListIntervention = task.Result ?? new List<Intervention>();
                }

                IsRefreshingIntervention = false;
            }));
        }

        private async void ChangeDone(Intervention intervention, TaskCompletionSource<bool> tcs)
        {
            try
            {
                if (intervention.IsDone == 0)
                {
                    if (await CoreMethods.DisplayAlert(TranslateExtension.GetValue("alert_title_intervention"), TranslateExtension.GetValue("alert_message_intervention_undone"), TranslateExtension.GetValue("yes"), TranslateExtension.GetValue("no")))
                    {
                        await interventionService.CalculateDoneTime(intervention);
                    }
                    else
                    {
                        intervention.IsDone = 1;
                    }
                }
                else
                {
                    if (await LocationHelper.CheckLocationPermission(false) && LocationHelper.IsGeolocationAvailable(false) && LocationHelper.IsGeolocationEnabled(false))
                    {
                        var position = await LocationHelper.GetCurrentPosition(showOverlay: false);

                        if (position != null && (position.Latitude != 0 && position.Longitude != 0))
                        {
                            intervention.DoneLatitude = position.Latitude;
                            intervention.DoneLongitude = position.Longitude;
                            intervention.DoneAltitude = position.Altitude;
                        }
                    }

                    await interventionService.CalculateDoneTime(intervention);
                }
            }
            catch (Exception ex)
            {
                await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), ex.GetBaseException().Message, TranslateExtension.GetValue("ok"));
            }
            finally
            {
                tcs.TrySetResult(true);
            }
        }

        private void AddIntervention(object sender, TaskCompletionSource<bool> tcs)
        {
            if (IsRefreshingIntervention)
            {
                tcs.SetResult(true);
                return;
            }

            UserDialogs.Instance.Loading(TranslateExtension.GetValue("message_loading")).Show();

            Task.Run(async () =>
            {
                var intervention = new Intervention()
                {
                    Id = Guid.NewGuid(),
                    UserId = CurrentUser.Id,
                    FkUserAppId = CurrentUser.Uuid,
                    FkUserServerlId = CurrentUser.Id,
                    LinkInterventionTasks = new List<LinkInterventionTask>(),
                    UniteLinks = new List<UniteLink>(),
                    LinkInterventionProducts = new ObservableCollection<LinkInterventionProduct>(),
                    MediaLinks = new ObservableCollection<MediaLink>(),
                    IsActif = 1
                };

                intervention = await interventionService.GetRelations(intervention);

                return intervention;
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(async () =>
            {
                UserDialogs.Instance.Loading().Hide();

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    try
                    {
                        var parameters = new NavigationParameters()
                        {
                            { ContentKey.SELECTED_INTERVENTION, task.Result}
                        };

                        await CoreMethods.PushViewModel<NewInterventionViewModel>(parameters);
                    }
                    catch (Exception ex)
                    {
                        await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), ex.Message, TranslateExtension.GetValue("ok"));
                    }
                }
                else if (task.IsFaulted)
                {
                    await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }

                tcs.TrySetResult(true);
            }));
        }

        private void EditIntervention(Intervention sender, TaskCompletionSource<bool> tcs)
        {
            if (IsRefreshingIntervention)
            {
                tcs.SetResult(true);
                return;
            }

            UserDialogs.Instance.Loading(TranslateExtension.GetValue("message_loading")).Show();

            Task.Run(async () =>
            {
                return await interventionService.GetIntervention(sender.Id);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(async () =>
            {
                UserDialogs.Instance.Loading().Hide();

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    try
                    {
                        var parameters = new NavigationParameters()
                        {
                            { ContentKey.SELECTED_INTERVENTION, task.Result}
                        };

                        await CoreMethods.PushViewModel<InterventionDetailViewModel>(parameters);
                    }
                    catch (Exception ex)
                    {
                        await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), ex.Message, TranslateExtension.GetValue("ok"));
                    }
                }
                else if (task.IsFaulted)
                {
                    await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }

                tcs.TrySetResult(true);
            }));
        }
    }
}