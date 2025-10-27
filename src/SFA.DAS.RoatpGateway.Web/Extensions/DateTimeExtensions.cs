using System;

namespace SFA.DAS.RoatpGateway.Web.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToSfaShortDateString(this DateTime time) => time.ToString("dd MMMM yyyy");

        public static string ToSfaShortDateString(this DateTime? time) => time?.ToString("dd MMMM yyyy");

        public static string ToSfaShorterDateString(this DateTime time) => time.ToString("dd MMM yyyy");

        public static string ToSfaShorterDateString(this DateTime? time) => time?.ToString("dd MMM yyyy");

        public static string ToSfaShortestDateString(this DateTime? time)
        {
            return time.HasValue ? time.Value.ToString("dd MMM yy") : string.Empty;
        }

        public static string ToShortNumericFormatString(this DateTime? time)
        {
            return time.HasValue ? time.Value.ToString("dd.MM.yyyy") : string.Empty;
        }

        public static string ToSfaShortMonthDateString(this DateTime? time)
        {
            return time?.ToString("dd MMM yyyy");
        }

        public static DateTime UtcFromTimeZoneTime(this DateTime time, string timeZoneId = "GMT Standard Time")
        {
            TimeZoneInfo systemTimeZoneById;
            try
            {
                systemTimeZoneById = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
                return time;
            }
            catch (InvalidTimeZoneException)
            {
                return time;
            }
            return TimeZoneInfo.ConvertTimeToUtc(time, systemTimeZoneById);
        }

        public static DateTime UtcToTimeZoneTime(this DateTime time, string timeZoneId = "GMT Standard Time")
        {
            TimeZoneInfo systemTimeZoneById;
            try
            {
                systemTimeZoneById = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
                return time;
            }
            catch (InvalidTimeZoneException)
            {
                return time;
            }
            return TimeZoneInfo.ConvertTimeFromUtc(time, systemTimeZoneById);
        }

        public static DateTime GetNextWeekday(this DateTime start, DayOfWeek day)
        {
            int num = (day - start.DayOfWeek + 7) % 7;
            return start.AddDays((double)num);
        }
    }
}
