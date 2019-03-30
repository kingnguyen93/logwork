using Acr.UserDialogs;
using LogWork.Constants;
using LogWork.IServices;
using LogWork.Models;
using LogWork.ViewModels.Interventions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using TinyMVVM;

namespace LogWork.ViewModels.InterventionsNotAssigned
{
    public class InterventionsNotAssignedViewModel : TinyViewModel
    {
        private readonly ISyncDataService syncService;
        private readonly IInterventionService interventionService;

        private Guid interventionChanged = Guid.Empty;
        private bool interventionAssigned = false;

        private CancellationTokenSource cts;

        private DateTime selectedDate = DateTime.Now;
        public DateTime SelectedDate { get => selectedDate; set => SetProperty(ref selectedDate, value); }

        private ObservableCollection<Intervention> listIntervention = new ObservableCollection<Intervention>();
        public ObservableCollection<Intervention> ListIntervention { get => listIntervention; set => SetProperty(ref listIntervention, value); }

        public ICommand GetSyncCommand { get; set; }
        public ICommand BeforeDayCommand { get; set; }
        public ICommand NextDayCommand { get; set; }
        public ICommand GetInterventionsCommand { get; set; }
        public ICommand LoadMoreInterventionsCommand { get; set; }
        public ICommand EditInterventionCommand { get; set; }

        public InterventionsNotAssignedViewModel(ISyncDataService syncService, IInterventionService interventionService)
        {
            this.syncService = syncService;
            this.interventionService = interventionService;

            GetSyncCommand = new Command(GetSync);
            BeforeDayCommand = new Command(BeforeDay);
            NextDayCommand = new Command(NextDay);
            GetInterventionsCommand = new Command(GetInterventions);
            LoadMoreInterventionsCommand = new Command(LoadMoreInterventions);
            EditInterventionCommand = new AwaitCommand<Intervention>(EditIntervention);
        }

        public override void Init(NavigationParameters parameters)
        {
            base.Init(parameters);

            GetInterventions();
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();

            MessagingCenter.Subscribe<InterventionDetailViewModel>(this, MessageKey.INTERVENTION_ASSIGNED, OnInterventionAssigned);
            MessagingCenter.Subscribe<InterventionDetailViewModel, Guid>(this, MessageKey.INTERVENTION_CHANGED, OnInterventionChanged);
        }

        public override void OnPopped()
        {
            base.OnPopped();

            MessagingCenter.Unsubscribe<InterventionDetailViewModel>(this, MessageKey.INTERVENTION_ASSIGNED);
            MessagingCenter.Unsubscribe<InterventionDetailViewModel, Guid>(this, MessageKey.INTERVENTION_CHANGED);
        }

        private void OnInterventionAssigned(InterventionDetailViewModel sender)
        {
            interventionAssigned = true;
        }

        private void OnInterventionChanged(object sender, Guid e)
        {
            interventionChanged = e;
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            if (interventionAssigned)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    SyncFromServer(1);
                    interventionAssigned = false;
                });
            }

            if (!interventionChanged.Equals(Guid.Empty))
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    UpdateChangedIntervention(interventionChanged);
                    interventionChanged = Guid.Empty;
                });
            }
        }

        private async void UpdateChangedIntervention(Guid interventionChanged)
        {
            if (ListIntervention.FirstOrDefault(i => i.Id.Equals(interventionChanged)) is Intervention intervention)
            {
                int old = ListIntervention.IndexOf(intervention);
                if (old > 0)
                {
                    ListIntervention[old] = await interventionService.GetIntervention(interventionChanged);
                }
            }
        }

        private void GetSync(object sender)
        {
            SyncFromServer(2);
        }

        private void SyncFromServer(int method)
        {
            syncService.SyncFromServer(method: method, onSuccess: GetInterventions, showOverlay: true);
        }

        private void BeforeDay(object sender)
        {
            SelectedDate = SelectedDate.Subtract(TimeSpan.FromDays(1));

            GetInterventions();
        }

        private void NextDay(object sender)
        {
            SelectedDate = SelectedDate.AddDays(1);

            GetInterventions();
        }

        private void LoadMoreInterventions(object sender)
        {
            Task.Run(async () =>
            {
                return await interventionService.GetNotAssignedInterventions(SelectedDate, ListIntervention.Count, 20);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    ListIntervention.AddRange(task.Result);
                }
            }));
        }

        private void GetInterventions()
        {
            if (cts != null)
                cts.Cancel();

            cts = new CancellationTokenSource();
            var token = cts.Token;

            IsBusy = true;

            Task.Run(async () =>
            {
                await Task.Delay(200, token);
                return await interventionService.GetNotAssignedInterventions(SelectedDate, 0, 20);
            }, token).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsBusy = false;

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    ListIntervention.Clear();
                    ListIntervention = task.Result.ToObservableCollection();
                }
            }));
        }

        private void EditIntervention(Intervention sender, TaskCompletionSource<bool> tcs)
        {
            if (IsBusy)
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