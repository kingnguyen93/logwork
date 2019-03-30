using LogWork.Constants;
using LogWork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using TinyMVVM;

namespace LogWork.ViewModels.SystemSettings
{
    public class SettingsViewModel : TinyViewModel
    {
        private readonly int CurrentUserId = Settings.CurrentUserId;

        private List<Models.MenuItem> listSetting;
        public List<Models.MenuItem> ListSetting { get => listSetting; set => SetProperty(ref listSetting, value); }

        public ICommand NavigateToDetailCommand { get; set; }

        public SettingsViewModel()
        {
            NavigateToDetailCommand = new AwaitCommand<Models.MenuItem>(NavigateToDetail);
        }

        public override void Init(NavigationParameters parameters)
        {
            base.Init(parameters);

            InitSettings();
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();

            MessagingCenter.Subscribe<SettingDetailViewModel>(this, MessageKey.SETTINGS_CHANGED, OnSettingsChanged);
        }

        public override void OnPopped()
        {
            base.OnPopped();

            MessagingCenter.Unsubscribe<SettingDetailViewModel>(this, MessageKey.SETTINGS_CHANGED);
        }

        private void OnSettingsChanged(object sender)
        {
            InitSettings();
        }

        private void InitSettings()
        {
            var result = new List<Models.MenuItem>();

            foreach (var setting in App.LocalDb.Table<Setting>().ToList().FindAll(se => se.UserId == CurrentUserId && se.IsActif == 1).GroupBy(se => se.Category))
            {
                if (AppSettings.StopAccessSetting && !string.IsNullOrWhiteSpace(setting.Key)
                    && !setting.Key.Equals("CATEGORY_LANGUAGES") && !setting.Key.Equals("CATEGORY_VISUAL"))
                    continue;

                string tilte = setting.Key;

                if (string.IsNullOrWhiteSpace(tilte))
                {
                    if (!App.IsDeveloperMode)
                    {
                        continue;
                    }
                    else
                    {
                        tilte = "CATEGORY_NO_CATEGORY";
                    }
                }

                result.Add(new Models.MenuItem
                {
                    Title = tilte,
                    Detail = TranslateExtension.GetValue(tilte)
                });
            }

            ListSetting = result;
        }

        private async void NavigateToDetail(Models.MenuItem sender, TaskCompletionSource<bool> tcs)
        {
            try
            {
                var parameters = new NavigationParameters
                {
                    { ContentKey.SELECTED_SETTING,  sender.Title }
                };

                await CoreMethods.PushViewModel<SettingDetailViewModel>(parameters);
            }
            catch (Exception e)
            {
                await CoreMethods.DisplayAlert(TranslateExtension.GetValue("alert_title_error"), e.GetBaseException().Message, TranslateExtension.GetValue("alert_message_ok"));
            }

            tcs.SetResult(true);
        }
    }
}