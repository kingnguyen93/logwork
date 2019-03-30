using Newtonsoft.Json;
using LogWork.Models.Response;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;

namespace LogWork
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters.
    /// </summary>
    public static class Settings
    {
        private static ISettings AppSettings => CrossSettings.IsSupported ? CrossSettings.Current : null;

        public static string CurrentAccount
        {
            get => AppSettings.GetValueOrDefault(nameof(CurrentAccount), default(string));
            set => AppSettings.AddOrUpdateValue(nameof(CurrentAccount), value);
        }

        public static void RemoveCurrentAccount() => AppSettings.Remove(nameof(CurrentAccount));

        public static string CurrentUserName
        {
            get => AppSettings.GetValueOrDefault(nameof(CurrentUserName), default(string));
            set => AppSettings.AddOrUpdateValue(nameof(CurrentUserName), value);
        }

        public static void RemoveCurrentUserName() => AppSettings.Remove(nameof(CurrentUserName));

        public static string CurrentPassword
        {
            get => AppSettings.GetValueOrDefault(nameof(CurrentPassword), default(string));
            set => AppSettings.AddOrUpdateValue(nameof(CurrentPassword), value);
        }

        public static void RemoveCurrentPassword() => AppSettings.Remove(nameof(CurrentPassword));

        public static int CurrentUserId
        {
            get => AppSettings.GetValueOrDefault(nameof(CurrentUserId), default(int));
            set => AppSettings.AddOrUpdateValue(nameof(CurrentUserId), value);
        }

        public static void RemoveCurrentUserId() => AppSettings.Remove(nameof(CurrentUserId));

        public static LoginResponse CurrentUser
        {
            get
            {
                string value = AppSettings.GetValueOrDefault(nameof(CurrentUser), default(string));
                return string.IsNullOrWhiteSpace(value) ? null : JsonConvert.DeserializeObject<LoginResponse>(value);
            }
            set => AppSettings.AddOrUpdateValue(nameof(CurrentUser), JsonConvert.SerializeObject(value));
        }

        public static void RemoveCurrentUser() => AppSettings.Remove(nameof(CurrentUser));

        public static bool LoggedIn
        {
            get => AppSettings.GetValueOrDefault(nameof(LoggedIn), default(bool));
            set => AppSettings.AddOrUpdateValue(nameof(LoggedIn), value);
        }

        public static void RemoveLoggedIn() => AppSettings.Remove(nameof(LoggedIn));

        public static string LastSync
        {
            get => AppSettings.GetValueOrDefault(nameof(LastSync), default(string));
            set => AppSettings.AddOrUpdateValue(nameof(LastSync), value);
        }

        public static void RemoveLastSync() => AppSettings.Remove(nameof(LastSync));

        public static long LastSyncProduct
        {
            get => AppSettings.GetValueOrDefault(nameof(LastSyncProduct), default(long));
            set => AppSettings.AddOrUpdateValue(nameof(LastSyncProduct), value);
        }

        public static void RemoveLastSyncProduct() => AppSettings.Remove(nameof(LastSyncProduct));


        //Last Sync Invoice 
        public static long LastSyncInvoice
        {
            get => AppSettings.GetValueOrDefault(nameof(LastSyncInvoice), default(long));
            set => AppSettings.AddOrUpdateValue(nameof(LastSyncInvoice), value);
        }

        public static void RemoveSyncInvoice() => AppSettings.Remove(nameof(LastSyncInvoice));
        // End Last Sync Invoice 


        public static DateTime LastDoneDate
        {
            get => AppSettings.GetValueOrDefault(nameof(LastDoneDate), DateTime.MinValue);
            set => AppSettings.AddOrUpdateValue(nameof(LastDoneDate), value);
        }

        public static void RemoveLastDoneDate() => AppSettings.Remove(nameof(LastDoneDate));

        public static string LastDoneTime
        {
            get => AppSettings.GetValueOrDefault(nameof(LastDoneTime), default(string));
            set => AppSettings.AddOrUpdateValue(nameof(LastDoneTime), value);
        }

        public static void RemoveLastDoneTime() => AppSettings.Remove(nameof(LastDoneTime));

        public static void ClearEverything()
        {
            AppSettings.Clear();
        }
    }
}