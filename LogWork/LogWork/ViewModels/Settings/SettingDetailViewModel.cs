using LogWork.Constants;
using LogWork.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using TinyMVVM;

namespace LogWork.ViewModels.SystemSettings
{
    public class SettingDetailViewModel : TinyViewModel
    {
        private readonly int CurrentUserId = Settings.CurrentUserId;

        private bool settingsChanged;

        private string category;

        private List<Setting> listSetting;
        public List<Setting> ListSetting { get => listSetting; set => SetProperty(ref listSetting, value); }

        public SettingDetailViewModel()
        {
        }

        public override void Init(NavigationParameters parameters)
        {
            base.Init(parameters);

            category = parameters?.GetValue<string>(ContentKey.SELECTED_SETTING);

            Title = TranslateExtension.GetValue(category);

            if (App.IsDeveloperMode && category.Equals("CATEGORY_NO_CATEGORY"))
            {
                ListSetting = App.LocalDb.Table<Setting>().ToList().FindAll(se => se.UserId == CurrentUserId && string.IsNullOrWhiteSpace(se.Category)).OrderBy(se => se.Order).ToList();
            }
            else
            {
                ListSetting = App.LocalDb.Table<Setting>().ToList().FindAll(se => se.UserId == CurrentUserId && (!string.IsNullOrWhiteSpace(se.Category) && se.Category.Equals(category)) && se.IsActif == 1).OrderBy(se => se.Order).ToList();
            }

            foreach (var set in ListSetting)
            {
                set.Arrange = App.LocalDb.Table<SettingItem>().ToList().FindAll(si => si.Name.Equals(set.Name))?.Select(si => si.Value).ToList();
                set.PropertyChanged += Set_PropertyChanged;
            }
        }

        protected override void ViewIsDisappearing(object sender, EventArgs e)
        {
            base.ViewIsDisappearing(sender, e);

            if (settingsChanged)
            {
                AppSettings.ReloadSetting();

                MessagingCenter.Send(this, MessageKey.SETTINGS_CHANGED);
            }
        }

        private bool isBusy;

        private void Set_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (isBusy)
                return;

            if (sender is Setting setting)
            {
                if (setting.Name.Equals("APP_RESET_DATA"))
                {
                    isBusy = true;

                    Settings.LastSync = "0";
                    if (ListSetting.Find(se => se.Name.Equals("APP_LAST_SYNCHRO")) is Setting lastSync)
                    {
                        lastSync.Value = "0";
                        App.LocalDb.Update(lastSync);
                    }
                    Settings.LastSyncProduct = 0;
                    if (ListSetting.Find(se => se.Name.Equals("APP_LAST_SYNCHRO_PRODUCT")) is Setting lastSyncProduct)
                    {
                        lastSyncProduct.Value = "0";
                        App.LocalDb.Update(lastSyncProduct);
                    }
                    Settings.LastSyncInvoice = 0;
                    if (ListSetting.Find(se => se.Name.Equals("APP_LAST_SYNCHRO_INVOICE")) is Setting lastSyncInvoice)
                    {
                        lastSyncInvoice.Value = "0";
                        App.LocalDb.Update(lastSyncInvoice);
                    }
                    ClearAllData();

                    isBusy = false;

                    settingsChanged = true;

                    return;
                }

                App.LocalDb.Update(setting);
                settingsChanged = true;
            }
        }

        private void ClearAllData()
        {
            App.LocalDb.DropTable<Filiale>();
            App.LocalDb.DropTable<Intervention>();
            App.LocalDb.DropTable<Client>();
            App.LocalDb.DropTable<Address>();
            App.LocalDb.DropTable<Chemin>();
            App.LocalDb.DropTable<UniteLink>();
            App.LocalDb.DropTable<Unite>();
            App.LocalDb.DropTable<UniteItem>();
            App.LocalDb.DropTable<LinkInterventionTask>();
            App.LocalDb.DropTable<Tasks>();
            App.LocalDb.DropTable<MediaLink>();
            App.LocalDb.DropTable<Media>();
            App.LocalDb.DropTable<LinkInterventionProduct>();
            App.LocalDb.DropTable<Product>();
            App.LocalDb.DropTable<CategoryTracking>();
            App.LocalDb.DropTable<Location>();
            App.LocalDb.DropTable<Models.Tracking>();
            App.LocalDb.DropTable<User>();
            App.LocalDb.DropTable<Message>();
            App.LocalDb.DropTable<Invoice>();
            App.LocalDb.DropTable<InvoiceProduct>();
            //App.LocalDb.DropTable<Setting>();
            //App.LocalDb.DropTable<SettingItem>();

            App.LocalDb.CreateTable<Filiale>();
            App.LocalDb.CreateTable<Intervention>();
            App.LocalDb.CreateTable<Client>();
            App.LocalDb.CreateTable<Address>();
            App.LocalDb.CreateTable<Chemin>();
            App.LocalDb.CreateTable<UniteLink>();
            App.LocalDb.CreateTable<Unite>();
            App.LocalDb.CreateTable<UniteItem>();
            App.LocalDb.CreateTable<LinkInterventionTask>();
            App.LocalDb.CreateTable<Tasks>();
            App.LocalDb.CreateTable<MediaLink>();
            App.LocalDb.CreateTable<Media>();
            App.LocalDb.CreateTable<LinkInterventionProduct>();
            App.LocalDb.CreateTable<Product>();
            App.LocalDb.CreateTable<CategoryTracking>();
            App.LocalDb.CreateTable<Location>();
            App.LocalDb.CreateTable<Models.Tracking>();
            App.LocalDb.CreateTable<User>();
            App.LocalDb.CreateTable<Message>();
            App.LocalDb.CreateTable<Invoice>();
            App.LocalDb.CreateTable<InvoiceProduct>();
            //App.LocalDb.CreateTable<Setting>();
            //App.LocalDb.CreateTable<SettingItem>();
        }
    }
}