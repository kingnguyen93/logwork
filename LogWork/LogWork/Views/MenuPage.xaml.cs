using LogWork.Constants;
using LogWork.Models;
using LogWork.Models.Response;
using LogWork.Services;
using LogWork.ViewModels.Interventions;
using LogWork.ViewModels.SystemSettings;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using Xamarin.Forms.Xaml;

namespace LogWork
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        private readonly LoginResponse CurrentUser = Settings.CurrentUser;

        private MainPage RootPage => Application.Current.MainPage as MainPage;

        private List<HomeMenuItem> menuItems;

        public MenuPage()
        {
            InitializeComponent();

            lblUserAccount.Text = Settings.CurrentAccount;
            lblUserName.Text = Settings.CurrentUser?.FullName;

            InitMenu();
            OnInterventionChanged(null);

            ListViewMenu.ItemTapped += async (sender, e) =>
            {
                if (e.Item == null)
                    return;

                var id = (int)((HomeMenuItem)e.Item).Id;
                await RootPage.NavigateFromMenu(id);
            };
            
            MessagingCenter.Unsubscribe<SettingDetailViewModel>(this, MessageKey.SETTINGS_CHANGED);
            MessagingCenter.Subscribe<SettingDetailViewModel>(this, MessageKey.SETTINGS_CHANGED, (sender) =>
            {
                InitMenu();
            });
            
            MessagingCenter.Unsubscribe<SyncDataService>(this, MessageKey.INTERVENTION_CHANGED);
            MessagingCenter.Subscribe<SyncDataService>(this, MessageKey.INTERVENTION_CHANGED, OnInterventionChanged);
            
            MessagingCenter.Unsubscribe<InterventionDetailViewModel>(this, MessageKey.INTERVENTION_CHANGED);
            MessagingCenter.Subscribe<InterventionDetailViewModel>(this, MessageKey.INTERVENTION_CHANGED, OnInterventionChanged);
        }

        private void InitMenu()
        {
            menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem {Id = MenuItemType.Interventions, Title = TranslateExtension.GetValue("home_menu_title_interventions"), Icon = "ic_menu_interventions_black.png" }
            };

            if (AppSettings.MenuShowNonAssignedIntervention)
            {
                menuItems.Add(new HomeMenuItem { Id = MenuItemType.InterventionsNotAssigned, Title = TranslateExtension.GetValue("home_menu_title_interventions_not_assigned"), Icon = "ic_menu_interventions_not_assigned_black.png" });
            }

            if (AppSettings.IsAdmin || AppSettings.IsModerator)
            {
                menuItems.Add(new HomeMenuItem { Id = MenuItemType.Quote, Title = TranslateExtension.GetValue("home_menu_title_quote"), Icon = "ic_local_atm_black.png" });
                menuItems.Add(new HomeMenuItem { Id = MenuItemType.Invoice, Title = TranslateExtension.GetValue("home_menu_title_invoice"), Icon = "ic_local_atm_black.png" });
                //menuItems.Add(new HomeMenuItem { Id = MenuItemType.Invoice, Title = TranslateExtension.GetValue("home_menu_title_invoice"), Icon = "ic_assignment_black.png" });
            }

            if (AppSettings.IsAdmin || AppSettings.IsModerator || AppSettings.MenuShowClientAddress)
            {
                menuItems.Add(new HomeMenuItem { Id = MenuItemType.Customers, Title = TranslateExtension.GetValue("home_menu_title_customers"), Icon = "ic_menu_customers_black.png" });
                menuItems.Add(new HomeMenuItem { Id = MenuItemType.Addresses, Title = TranslateExtension.GetValue("home_menu_title_addresses"), Icon = "ic_menu_addresses_black.png" });
            }

            menuItems.AddRange(new List<HomeMenuItem>()
            {
                new HomeMenuItem {Id = MenuItemType.Messages, Title = TranslateExtension.GetValue("home_menu_title_messages"), Icon = "ic_menu_messages_black.png" },
                new HomeMenuItem {Id = MenuItemType.Tracking, Title = TranslateExtension.GetValue("home_menu_title_tracking"), Icon = "ic_menu_tracking_black.png" },
                new HomeMenuItem {Id = MenuItemType.Settings, Title = TranslateExtension.GetValue("home_menu_title_settings"), Icon = "ic_menu_settings_black.png" },
                new HomeMenuItem {Id = MenuItemType.About, Title = TranslateExtension.GetValue("home_menu_title_about"), Icon = "ic_menu_info_black.png" },
                new HomeMenuItem {Id = MenuItemType.LogOut, Title = TranslateExtension.GetValue("home_menu_title_log_out"), Icon = "ic_menu_log_out_black.png" }
            });

            ListViewMenu.ItemsSource = null;
            ListViewMenu.ItemsSource = menuItems;
        }

        private void OnInterventionChanged(object sender)
        {
            if (ListViewMenu.ItemsSource is List<HomeMenuItem> menuItems && menuItems.Find(m => m.Id == MenuItemType.InterventionsNotAssigned) is HomeMenuItem item)
            {
                int count = App.LocalDb.Table<Intervention>().Count(i => i.UserId == CurrentUser.Id && (i.FkUserAppId.Equals(Guid.Empty) && i.FkUserServerlId == 0) && (!i.FkClientAppId.Equals(Guid.Empty) || i.FkClientServerId != 0) && i.IsActif == 1 && i.LastViewDate == null);
                if (count == 0)
                {
                    item.Badge = null;
                }
                else
                {
                    item.Badge = count > 100 ? "100+" : count.ToString();
                    //item.Badge = count.ToString();
                }
                item.OnPropertyChanged(nameof(HomeMenuItem.Badge));
            }
        }
    }
}