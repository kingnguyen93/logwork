using Newtonsoft.Json;
using LogWork.Helpers;
using LogWork.IServices;
using LogWork.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TinyMVVM;

using Xamarin.Forms;
using Xamarin.Forms.Extensions;

namespace LogWork.ViewModels
{
    public class SyncViewModel : TinyViewModel
    {
        private readonly ISyncDataService syncService;

        private readonly int CurrentUserId = Settings.CurrentUserId;

        public SyncViewModel(ISyncDataService syncService)
        {
            this.syncService = syncService;
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();

            InitLocalSettings();
            GetSync();
        }

        private void InitLocalSettings()
        {
            // Read Settings
            var file = AssetsExtension.GetFile("setting.json");

            if (file == null)
                return;

            try
            {
                using (var reader = new StreamReader(file))
                {
                    var defaultSetting = JsonConvert.DeserializeObject<List<DefaultSetting>>(reader.ReadToEnd(), App.DefaultDeserializeSettings);

                    foreach (var settings in defaultSetting)
                    {
                        foreach (var item in settings.Items)
                        {
                            if (App.LocalDb.Table<Setting>().ToList().FirstOrDefault(se => se.Name.Equals(item.Value.Name)) is Setting setting)
                            {
                                setting.UserId = CurrentUserId;
                                setting.Category = item.Value.Category;
                                setting.DefaultValue = item.Value.DefaultValue;
                                setting.Type = item.Value.Type;
                                setting.Arrange = item.Value.Arrange;
                                setting.Message = item.Value.Message;
                                setting.Description = item.Value.Description;
                                setting.Order = item.Value.Order;
                                setting.EditDate = DateTime.Now;

                                App.LocalDb.Update(setting);
                            }
                            else
                            {
                                App.LocalDb.Insert(new Setting
                                {
                                    Id = Guid.NewGuid(),
                                    UserId = CurrentUserId,
                                    Name = item.Value.Name,
                                    Category = item.Value.Category,
                                    Value = item.Value.DefaultValue,
                                    DefaultValue = item.Value.DefaultValue,
                                    Type = item.Value.Type,
                                    Arrange = item.Value.Arrange,
                                    Message = item.Value.Message,
                                    Description = item.Value.Description,
                                    Order = item.Value.Order,
                                    IsActif = 1,
                                    AddDate = DateTime.Now,
                                    EditDate = DateTime.Now
                                });
                            }

                            if (item.Value.Arrange != null && item.Value.Arrange.Count > 0)
                            {
                                foreach (var oldArr in App.LocalDb.Table<SettingItem>().ToList().FindAll(si => si.Name.Equals(item.Value.Name) && !item.Value.Arrange.Contains(si.Value)))
                                {
                                    App.LocalDb.Delete(oldArr);
                                }

                                foreach (var arr in item.Value.Arrange)
                                {
                                    if (App.LocalDb.Table<SettingItem>().ToList().FirstOrDefault(si => si.Name.Equals(item.Value.Name) && si.Value.Equals(arr)) == null)
                                    {
                                        App.LocalDb.InsertOrReplace(new SettingItem
                                        {
                                            Id = Guid.NewGuid(),
                                            Name = item.Value.Name,
                                            Value = arr
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("INIT_LOCAL_SETTINGS_ERROR:" + ex.Message);
            }
        }

        private void GetSync()
        {
            IsBusy = true;

            syncService.SyncFromServer(method: 1, onSuccess: OnSyncCompleted, onError: (error) => OnSyncCompleted(), showOverlay: false);
        }

        private async void OnSyncCompleted()
        {
            IsBusy = false;

            try
            {
                AppSettings.ReloadSetting();

                SyncHelper.Instance.StartAutoSync(15);
            }
            catch (Exception ex)
            {
                await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), ex.Message, TranslateExtension.GetValue("error"));

                Debug.WriteLine("ERROR:" + ex.Message);
            }

            Application.Current.MainPage = new MainPage();
        }
    }
}