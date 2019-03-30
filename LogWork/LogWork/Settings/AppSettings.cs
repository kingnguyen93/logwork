using LogWork.Models;
using System;
using System.Globalization;
using Xamarin.Forms.Extensions;

namespace LogWork
{
    public class AppSettings
    {
        public static bool IsSuperUser => Settings.CurrentUser.Rang == "22";
        public static bool IsAdmin => Settings.CurrentUser.Rang == "10";
        public static bool IsModerator => Settings.CurrentUser.Rang == "11";
        public static bool IsUser => Settings.CurrentUser.Rang == "12";
        public static bool IsInvoicing => Settings.CurrentUser.Rang == "13";

        public static string AppLanguage { get; set; }
        public static double FontSize { get; set; }
        public static int MediaMaxSize { get; set; }
        public static string PurgeData { get; set; }

        // Master Menu
        public static bool MenuShowNonAssignedIntervention { get; set; }

        public static bool MenuShowClientAddress { get; set; }
        public static bool ChatIsActive { get; set; }
        public static bool DealIsActive { get; set; }

        // List Intervention
        public static bool MobileShowNotDone { get; set; } // enable/disable feature to display interventions of previous days not finished yet

        public static bool MobileShowPriority { get; set; }
        public static bool MobileShowTitle { get; set; }
        public static bool MobileShowClient { get; set; }
        public static bool MobileShowAddress { get; set; }
        public static bool MobileShowToggleProgress { get; set; }

        // View Intervention
        public static bool MobileShowMapButton { get; set; }

        public static bool MobileShowSendMail { get; set; }
        public static bool MobileActiveTakePhoto { get; set; }
        public static bool MobileActivePickPhoto { get; set; }
        public static bool MobileActiveSignature { get; set; }
        public static bool MobileEnableProduct { get; set; }

        // Edit Intervention
        public static bool MobileDisplayCheckboxIsDone { get; set; }

        public static bool MobileCanEditClientAddress { get; set; }
        public static bool MobileDisplayTitle { get; set; }
        public static bool MobileCanEditUsers { get; set; }
        public static bool MobileCanEditDates { get; set; }
        public static bool MobileCanEditHours { get; set; }
        public static bool MobileCanSearchProduct { get; set; }

        // Hours Worked
        public static bool MobileHourStartEnable { get; set; }

        public static string MobileHourStartDefault { get; set; }
        public static string MobileHourStartPauseDefault { get; set; }
        public static string MobileHourEndDefault { get; set; }
        public static string MobileHourEndPauseDefault { get; set; }

        // Notification
        public static bool NewMessageNotification { get; set; }

        public static bool NewInterventionNotification { get; set; }

        public static bool StopAccessSetting { get; set; }

        public static bool SyncSimple { get; set; }
        public static bool SyncUnite { get; set; }
        public static bool SyncWithDbClient { get; set; }
        public static bool SyncAnonymousClient { get; set; }
        public static bool SynchroInterventionLockNew { get; set; }

        public static bool MobileActiveMultiplyHourTasksWhenMultiUsers { get; set; }
        public static bool MobilePreremplirPlanningDate { get; set; }
        public static bool MobileLocationIsActive { get; set; } // enable/disable the location on mobile
        public static bool MobileDistanceGeo { get; set; }

        public static bool TrackingAskGeo { get; set; }

        public static bool AddressIsBonDePassageActive { get; set; }
        public static bool AddressIsCleActive { get; set; }
        public static bool AddressIsHoraireActive { get; set; }
        public static bool AddressIsPlannedInterventionActive { get; set; }

        public static bool PushNotifIsActive { get; set; }
        public static bool PushcrewTimeLive { get; set; }
        public static bool PushcrewAutohide { get; set; }

        public static void ReloadSetting()
        {
            AppLanguage = App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("APP_LANGUAGE"))?.Value ?? "fr_FR";

            CultureInfo ci = AppLanguage.Equals("en_US") ? new CultureInfo("en-US") : new CultureInfo("fr-FR");

            CultureInfo.CurrentUICulture = ci;
            CultureInfo.CurrentCulture = ci;
            CultureInfo.DefaultThreadCurrentUICulture = ci;
            CultureInfo.DefaultThreadCurrentCulture = ci;
            TranslateExtension.CurrentCultureInfo = ci;

            var fontSize = App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("APP_FONT_SIZE"))?.Value ?? "14";
            FontSize = Convert.ToDouble(fontSize);

            var mediaMaxsize = App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("APP_MEDIA_SIZE_MAX"))?.Value ?? "Original";
            if (mediaMaxsize.Equals("Original"))
            {
                MediaMaxSize = 0;
            }
            else
            {
                mediaMaxsize = mediaMaxsize.Replace("px", "");
                if (int.TryParse(mediaMaxsize, out int maxSize))
                {
                    MediaMaxSize = maxSize;
                }
            }

            var lastSync = App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("APP_LAST_SYNCHRO"))?.Value ?? "0";
            Settings.LastSync = lastSync;

            var lastSyncProduct = App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("APP_LAST_SYNCHRO_PRODUCT"))?.Value ?? "0";
            Settings.LastSyncProduct = Convert.ToInt64(lastSyncProduct);

            var lastSyncInvoice = App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("APP_LAST_SYNCHRO_INVOICE"))?.Value ?? "0";
            Settings.LastSyncInvoice = Convert.ToInt64(lastSyncInvoice);

            PurgeData = App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("APP_PURGE_DATA"))?.Value ?? "+ 3 months";

            // Master Menu
            MenuShowNonAssignedIntervention = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("NOT_ASSIGNED_IS_ACTIF"))?.Value ?? "0") != "0";
            MenuShowClientAddress = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_ACTIVATE_CLIENT_ADRESSE_ON_MENU"))?.Value ?? "0") != "0";
            ChatIsActive = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("CHAT_IS_ACTIF"))?.Value ?? "0") != "0";
            DealIsActive = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("DEAL_IS_ACTIF"))?.Value ?? "0") != "0";

            // List Intervention
            MobileShowNotDone = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_REPORTER_INTERVENTIONS_DEFAULT"))?.Value ?? "1") != "0";
            MobileShowPriority = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_DISPLAY_HOME_INTERVENTION_PRIORITY"))?.Value ?? "0") != "0";
            MobileShowTitle = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_DISPLAY_HOME_INTERVENTION_TITLE"))?.Value ?? "1") != "0";
            MobileShowClient = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_DISPLAY_HOME_INTERVENTION_CLIENT"))?.Value ?? "1") != "0";
            MobileShowAddress = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_DISPLAY_HOME_INTERVENTION_ADRESSE"))?.Value ?? "1") != "0";
            MobileShowToggleProgress = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_DISPLAY_HOME_INTERVENTION_TOGGLE_PROGRESS"))?.Value ?? "0") != "0";

            // View Intervention
            MobileShowMapButton = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_DISPLAY_MAP"))?.Value ?? "1") != "0";
            MobileShowSendMail = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_SEND_MAIL"))?.Value ?? "1") != "0";
            MobileEnableProduct = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_ENABLE_PRODUCT"))?.Value ?? "0") != "0";
            MobileActiveTakePhoto = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_ACTIVATE_TAKE_PHOTO"))?.Value ?? "0") != "0";
            MobileActivePickPhoto = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_ACTIVATE_SEARCH_PHOTO"))?.Value ?? "0") != "0";
            MobileActiveSignature = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_ACTIVATE_TAKE_SIGNATURE"))?.Value ?? "0") != "0";

            MobileActiveMultiplyHourTasksWhenMultiUsers = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_ACTIVATE_MULTIPLY_HOUR_TASKS_WHEN_MULTIUSERS"))?.Value ?? "0") != "0";
            MobilePreremplirPlanningDate = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_PREREMPLIR_PLANNING_DATE"))?.Value ?? "0") != "0";
            MobileLocationIsActive = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_LOCATION_IS_ACTIF"))?.Value ?? "0") != "0";
            MobileDistanceGeo = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_DISTANCE_GEOLOC"))?.Value ?? "0") != "0";

            TrackingAskGeo = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("TRACKING_ASK_GEOLOC"))?.Value ?? "0") != "0";

            // Edit Intervention
            MobileDisplayCheckboxIsDone = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_DISPLAY_CHECKBOX_IS_DONE"))?.Value ?? "1") != "0";
            MobileCanEditClientAddress = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_CAN_EDIT_CLIENT_ADRESSE"))?.Value ?? "0") != "0";
            MobileDisplayTitle = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_DISPLAY_FIELD_TITLE"))?.Value ?? "1") != "0";
            MobileCanEditUsers = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_CAN_EDIT_USERS"))?.Value ?? "0") != "0";
            MobileCanEditDates = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_CAN_EDIT_DATES"))?.Value ?? "0") != "0";
            MobileCanEditHours = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_CAN_EDIT_HOURS"))?.Value ?? "0") != "0";
            MobileCanSearchProduct = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_CAN_SEARCH_PRODUCT"))?.Value ?? "0") != "0";

            // Hours worked
            MobileHourStartEnable = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_HOUR_START_ENABLE"))?.Value ?? "1") != "0";
            MobileHourStartDefault = App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_HOUR_START_DEFAULT"))?.Value ?? "09:00";
            MobileHourStartPauseDefault = App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_HOUR_START_PAUSE_DEFAULT"))?.Value ?? "12:30";
            MobileHourEndDefault = App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_HOUR_END_DEFAULT"))?.Value ?? "18:00";
            MobileHourEndPauseDefault = App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_HOUR_END_PAUSE_DEFAULT"))?.Value ?? "14:00";

            // Notification
            NewMessageNotification = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_NOTIF_MESSAGE"))?.Value ?? "1") != "0";
            NewInterventionNotification = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("MOBILE_NOTIF_INTERVENTION"))?.Value ?? "0") != "0";

            // Settings
            StopAccessSetting = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("STOP_ACCESS_SETTING"))?.Value ?? "0") != "0";

            // Unused
            SyncSimple = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("SYNC_SIMPLE"))?.Value ?? "0") != "0";
            SyncUnite = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("SYNC_UNITE"))?.Value ?? "0") != "0";
            SyncAnonymousClient = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("SYNC_ANONYMOUS_CLIENT"))?.Value ?? "0") != "0";
            SyncWithDbClient = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("SYNC_WITH_DB_CLIENT"))?.Value ?? "0") != "0";
            SynchroInterventionLockNew = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("SYNCHRO_INTERVENTION_LOCK_NEW"))?.Value ?? "0") != "0";

            AddressIsBonDePassageActive = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("ADRESSE_IS_BON_DE_PASSAGE_ACTIF"))?.Value ?? "0") != "0";
            AddressIsCleActive = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("ADRESSE_IS_CLE_ACTIF"))?.Value ?? "0") != "0";
            AddressIsHoraireActive = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("ADRESSE_IS_HORAIRE_ACTIF"))?.Value ?? "0") != "0";
            AddressIsPlannedInterventionActive = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("ADRESSE_IS_PLANNED_INTERVENTIONS_ACTIF"))?.Value ?? "0") != "0";

            PushNotifIsActive = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("NOTIF_PUSH_IS_ACTIF"))?.Value ?? "0") != "0";
            PushcrewTimeLive = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("PUSHCREW_TIME_TO_LIVE"))?.Value ?? "0") != "0";
            PushcrewAutohide = (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("PUSHCREW_AUTOHIDE"))?.Value ?? "0") != "0";
        }
    }
}