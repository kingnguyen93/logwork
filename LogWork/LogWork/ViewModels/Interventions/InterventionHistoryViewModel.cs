using Acr.UserDialogs;
using LogWork.Constants;
using LogWork.Helpers;
using LogWork.IServices;
using LogWork.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using TinyMVVM;

using Xamarin.Forms;
using Xamarin.Forms.Extensions;

namespace LogWork.ViewModels.Interventions
{
    public class InterventionHistoryViewModel : TinyViewModel
    {
        private readonly IInterventionService interventionService;

        private int interventionId;

        private List<InterventionHistory> interventionHistory = new List<InterventionHistory>();
        public List<InterventionHistory> InterventionHistory { get => interventionHistory; set => SetProperty(ref interventionHistory, value); }

        public ICommand GetHistoryCommand { get; private set; }

        public InterventionHistoryViewModel(IInterventionService interventionService)
        {
            this.interventionService = interventionService;

            GetHistoryCommand = new Command(GetHistory);
        }

        public override void Init(NavigationParameters parameters)
        {
            base.Init(parameters);

            interventionId = parameters?.GetValue<Intervention>(ContentKey.SELECTED_INTERVENTION)?.ServerId ?? 0;

            GetHistory(null);
        }

        private void GetHistory(object sender)
        {
            if (!ConnectivityHelper.IsNetworkAvailable())
                return;

            UserDialogs.Instance.Loading(TranslateExtension.GetValue("alert_message_get_history")).Show();

            Task.Run(async () =>
            {
                return await interventionService.GetHistory(interventionId);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(async () =>
            {
                UserDialogs.Instance.Loading().Hide();

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    InterventionHistory.Clear();
                    if (task.Result != null && task.Result.Count > 0)
                    {
                        var interventionHistory = new List<InterventionHistory>();

                        foreach (var inter in task.Result)
                        {
                            interventionHistory.Add(new InterventionHistory
                            {
                                IsDone = inter.IsDone == 1 ? TranslateExtension.GetValue("yes") : TranslateExtension.GetValue("no"),
                                Date = inter.IsDone == 1 ? inter.DoneDateStart?.ToString("yyyy-MM-dd") : inter.PlanningDateStart?.ToString("yyyy-MM-dd"),
                                Comment = inter.IsDone == 1 ? inter.DoneComment : inter.PlanningComment
                            });
                        }
                        InterventionHistory = interventionHistory;
                    }
                }
                else if (task.IsFaulted && task.Exception?.GetBaseException().Message is string message)
                {
                    await CoreMethods.DisplayAlert("", message, TranslateExtension.GetValue("ok"));
                }
            }));
        }
    }
}