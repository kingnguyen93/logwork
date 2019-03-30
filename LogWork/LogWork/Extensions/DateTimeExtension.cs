using System;
using System.Diagnostics;

namespace Xamarin.Forms.Extensions
{
    public static class DateTimeExtension
    {
        public static DateTime FirstDayOfWeek(this DateTime dt)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var diff = dt.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;
            if (diff < 0)
                diff += 7;
            return dt.AddDays(-diff).Date;
        }

        public static DateTime LastDayOfWeek(this DateTime dt)
        {
            return dt.FirstDayOfWeek().AddDays(6);
        }

        public static DateTime FirstDayOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }

        public static DateTime LastDayOfMonth(this DateTime dt)
        {
            return dt.FirstDayOfMonth().AddMonths(1).AddDays(-1);
        }

        public static DateTime FirstDayOfNextMonth(this DateTime dt)
        {
            return dt.FirstDayOfMonth().AddMonths(1);
        }

        public static bool TryParseToDateTime(this double unixTimeStamp, out DateTime result, bool isLocalTime = false)
        {
            try
            {
                // Unix timestamp is seconds past epoch
                DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, isLocalTime ? DateTimeKind.Local : DateTimeKind.Utc);
                result = dt.AddSeconds(unixTimeStamp).ToLocalTime();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException());
                result = DateTime.MinValue;
                return false;
            }
        }

        public static double ToUnixTimestamp(this DateTime dateTime) => (TimeZoneInfo.ConvertTimeToUtc(dateTime) - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

        public static long ToUnixTimeSeconds(this DateTime dateTime, bool isLocalTime = false) => ToUnixTimeMilliseconds(dateTime, isLocalTime) / TimeSpan.TicksPerSecond;

        public static long ToUnixTimeMilliseconds(this DateTime dateTime, bool isLocalTime = false) => dateTime.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, isLocalTime ? DateTimeKind.Local : DateTimeKind.Utc).Ticks;

        private static readonly long UnixEpochTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
    }
}