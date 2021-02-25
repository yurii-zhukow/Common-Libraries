﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;
using YZ;

namespace YZ {


    public static partial class Helpers {

        public static readonly DateTime Jan2001 = new DateTime(2001, 01, 01);
        public static DateTime TrimSeconds(this DateTime d) => d.Subtract(TimeSpan.FromSeconds(d.Second));

        private const long TICKS_TO_SECONDS_DIVIDER = 10000 * 1000;
        private const long TICKS_TO_MINUTES_DIVIDER = TICKS_TO_SECONDS_DIVIDER * 60;
        public static readonly DateTime Year2020Start = new DateTime(2020, 01, 01);
        public static readonly DateTime Year1970Start = new DateTime(1970, 01, 01);
        public static readonly long Year2020StartSecondId = Year2020Start.Ticks / TICKS_TO_SECONDS_DIVIDER;
        public static readonly long Year2020StartMinuteId = Year2020Start.Ticks / TICKS_TO_MINUTES_DIVIDER;


        /// <summary>
        /// Convert number of seconds since mindinght 2019-01-01
        /// </summary>
        /// <param name="v">total seconds from 1 Jan 1970</param>
        /// <returns>Absolute DateTime</returns>

        public static DateTime FromJan1970(this long seconds) => Year1970Start.AddSeconds(seconds);
        public static long FromJan1970(this DateTime d) => (long)((d - Year1970Start).TotalSeconds);


        /// <summary>
        /// Convert DateTime to number of minutes since mindinght 2019-01-01
        /// </summary>
        /// <param name="d">Desired DateTime</param>
        /// <returns>Number of minutes since mindinght 2020-01-01</returns>
        public static long GetMinuteId(this DateTime d) => d.Ticks / TICKS_TO_MINUTES_DIVIDER - Year2020StartMinuteId;

        /// <summary>
        /// Convert DateTime to number of seconds since mindinght 2019-01-01
        /// </summary>
        /// <param name="d">Desired DateTime</param>
        /// <returns>Number of seconds since mindinght 2020-01-01</returns>
        public static long GetSecondId(this DateTime d) => d.Ticks / TICKS_TO_SECONDS_DIVIDER - Year2020StartSecondId;

        /// <summary>
        /// Converts minutes passed from 2020-01-01 00:00:00 to DateTime
        /// </summary>
        /// <param name="d">Number of minutes passed from 2020-01-01 00:00:00</param>
        /// <returns>Date and time rounded to minutes</returns>
        public static DateTime GetDateTimeFromMinuteId(this long d) => Year2020Start + TimeSpan.FromMinutes(d);
        /// <summary>
        /// Converts seconds passed from 2020-01-01 00:00:00 to DateTime
        /// </summary>
        /// <param name="d">Number of seconds passed from 2020-01-01 00:00:00</param>
        /// <returns>Date and time rounded to seconds</returns>
        public static DateTime GetDateTimeFromSecondId(this long d) => Year2020Start + TimeSpan.FromSeconds(d);


        public static TimeSpan GetOffsetFromGMTString(this string tz) {
            var sign = tz.Contains('+') ? 1 : tz.Contains('-') ? -1 : 0;
            var tta = tz.Split('+', '-');
            var res = TimeSpan.Parse($"{(sign < 0 ? "-" : "")}{tta[1]}");
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(tta[0]);
            res += tzi?.BaseUtcOffset ?? TimeSpan.Zero;

            //switch (tta[0].ToUpper()) {
            //    case "CST":
            //        res += TimeSpan.FromHours(-6);
            //        break;
            //    case "EST":
            //        res += TimeSpan.FromHours(-5);
            //        break;
            //    case "PST":
            //        res += TimeSpan.FromHours(-8);
            //        break;
            //    case "CET":
            //        res += TimeSpan.FromHours(+1);
            //        break;
            //    default: break;

            //}
            return res;
        }

        public static string AsOffsetForDateTimeOffset(this TimeSpan offset) => $"{(offset.TotalMinutes < 0 ? "-" : "+")}{offset:hh\\:mm}";

        public static int AsInt(this DayOfWeek dow) {
            switch (dow) {
                case DayOfWeek.Sunday: return 7;
                default: return (int)dow;
            }
        }

        public static DateTime Constraint(this DateTime src, DateTime? min = null, DateTime? max = null, DateTime? outrangeDefault = null) {
            if (min.HasValue && src < min.Value) return outrangeDefault ?? min.Value;
            if (max.HasValue && src > max.Value) return outrangeDefault ?? max.Value;
            return src;
        }

        public static TimeSpan Constraint(this TimeSpan src, TimeSpan? min = null, TimeSpan? max = null, TimeSpan? outrangeDefault = null) {
            if (min.HasValue && src < min.Value) return outrangeDefault ?? min.Value;
            if (max.HasValue && src > max.Value) return outrangeDefault ?? max.Value;
            return src;
        }

        public static TimeSpan Sum(this IEnumerable<TimeSpan> src) => TimeSpan.FromMinutes(src.Sum(t => t.TotalMinutes));
        public static TimeSpan Sum<T>(this IEnumerable<T> src, Func<T, TimeSpan> select) => TimeSpan.FromMinutes(src.Sum(t => select(t).TotalMinutes));


        public static double Age(this DateTime date) => (DateTime.Now - date).TotalDays / 365.242;
        public static bool IsToday(this DateTime d) => d >= DateTime.Today && d < DateTime.Today.AddDays(1);
        public static bool IsCurrentWeek(this DateTime d) => d >= DateTime.Now.StartOfWeek() && d <= DateTime.Now.EndOfWeek();
        public static bool IsCurrentMonth(this DateTime d) => d >= DateTime.Now.StartOfMonth() && d <= DateTime.Now.EndOfMonth();
        public static bool IsCurrentYear(this DateTime d) => d >= DateTime.Now.StartOfYear() && d <= DateTime.Now.EndOfYear();
        public static bool IsWeekend(this DateTime d) => d.DayOfWeek == DayOfWeek.Sunday || d.DayOfWeek == DayOfWeek.Saturday;

        public static DateTime RoundMinutes(this DateTime src, double minutes) => src.Date.AddMinutes(src.TimeOfDay.TotalMinutes.RoundTo(minutes));
        public static DateTime NearestHour(this DateTime src) => src.Date.AddHours(((int)src.TimeOfDay.TotalHours) + 1);
        public static TimeSpan Constraint(this TimeSpan src) => src.Constraint(TimeSpan.Zero, TimeSpan.FromDays(1).Subtract(TimeSpan.FromMilliseconds(1)));

        public static string ToHoursMinutes(this TimeSpan t, bool ignoreNegative = false) {
            var h = (int)Math.Abs(t.TotalHours);
            var m = t.Minutes;
            return $"{(!ignoreNegative && t.TotalHours < 0 ? "-" : "")}{(h > 0 ? h.ToString("часов ", "час ", "часа ") : "")}{(m > 0 || h == 0 ? m.ToString("минут", "минута", "минуты") : "")}".Trim();
        }

        public static string ToHoursMinutes2(this TimeSpan t, bool ignoreNegative = false) {
            var h = (int)Math.Abs(t.TotalHours);
            var m = t.Minutes;
            return $"{(!ignoreNegative && t.TotalHours < 0 ? "-" : "")}{(h > 0 ? h.ToString("часов ", "часа ", "часов ") : "")}{(m > 0 || h == 0 ? m.ToString("минут", "минуты", "минут") : "")}".Trim();
        }


        public static DateTime StartOfWeek(this DateTime src) {
            var dow = (int)src.DayOfWeek;
            return src.Date.AddDays(dow == 0 ? -6 : (1 - dow));
        }

        public static DateTime LastDayOfWeek(this DateTime src) => src.StartOfWeek().AddDays(6);
        public static DateTime EndOfWeek(this DateTime src) => src.StartOfWeek().AddDays(7).AddTicks(-1);
        public static DateTime StartOfMonth(this DateTime src) => new DateTime(src.Year, src.Month, 1);
        public static DateTime EndOfMonth(this DateTime src) => src.StartOfMonth().AddMonths(1).AddTicks(-1);
        public static DateTime LastDayOfMonth(this DateTime src) => src.StartOfMonth().AddMonths(1).AddDays(-1);
        public static DateTime StartOfYear(this DateTime src) => new DateTime(src.Year, 1, 1);
        public static DateTime EndOfYear(this DateTime src) => new DateTime(src.Year + 1, 1, 1).AddTicks(-1);
        public static DateTime LastDayOfYear(this DateTime src) => new DateTime(src.Year + 1, 1, 1).AddDays(-1);

        public static DateTime StartOfHour(this DateTime src, double addHours = 0) => src.Date.AddHours(src.Hour + addHours);
        public static DateTime EndOfHour(this DateTime src, double addHours = 0) => src.StartOfHour().AddHours(1 + addHours).AddTicks(-1);

        public static DateTime StartOfHalfHour(this DateTime src, double addHours = 0) => src.Date.AddHours(src.Hour + addHours).AddMinutes(src.Minute >= 30 ? 30 : 0);
        public static DateTime EndOfHalfHour(this DateTime src, double addHours = 0) => src.StartOfHalfHour(addHours).AddMinutes(30).AddTicks(-1);

        public static DateTime StartOfQuarterHour(this DateTime src, double addHours = 0) => src.Date.AddHours(src.Hour + addHours).AddMinutes(src.Minute >= 45 ? 45 : src.Minute >= 30 ? 30 : src.Minute >= 15 ? 15 : 0);
        public static DateTime EndOfQuarterHour(this DateTime src, double addHours = 0) => src.StartOfQuarterHour(addHours).AddMinutes(15).AddTicks(-1);


        public static DateTime StartOfMinute(this DateTime src, double addMinutes = 0) => src.Date.Add(new TimeSpan(src.Hour, src.Minute, 0)).AddMinutes(addMinutes);
        public static DateTime EndOfMinute(this DateTime src, double addMinutes = 0) => src.StartOfMinute(addMinutes).AddMinutes(1).AddTicks(-1);


        public static bool IsYesterday(this DateTime src, int offset = 1) => src.Date.AddDays(offset).IsToday();
        public static bool IsTomorrow(this DateTime src, int offset = 1) => src >= Year2020Start && src.Date.AddDays(-offset).IsToday();

        public static string ToNativeDate(this DateTime src, string timeFormat = null) => src < Year1970Start || src == DateTime.MaxValue ? ""
            : (src.IsToday() ? "сегодня"
            : src.IsYesterday() ? "вчера"
            : src.IsYesterday(2) ? "позавчера"
            : src.IsTomorrow() ? "завтра"
            : src.IsTomorrow(2) ? "послезавтра"
            : src.ToShortDateString())
            + (string.IsNullOrEmpty(timeFormat) ? "" : src.ToString(timeFormat));

        public static string ToNativeDateShort(this DateTime src) => src.IsCurrentYear() ? src.ToString("dd.MM") : src.ToString("dd.MM.yyyy");

        public static string ToFriendlyString(this TimeSpan ts) {

            if (ts.TotalDays > 365) return ToString(ts.TotalDays / 365, 1, " лет", " год", " года");
            if (ts.TotalDays > 30) return ToString(ts.TotalDays / 30, 1, " месяцев", " месяц", " месяца");
            if (ts.TotalDays > 14) return ToString(ts.TotalDays / 7, 1, " недель", " неделя", " недели");
            if (ts.TotalDays > 2) return ToString(ts.TotalDays, 1, " дней", " день", " дня");
            if (ts.TotalHours >= 4) return ToString(ts.TotalHours, 1, " часов", " час", " часа");
            if (ts.TotalHours >= 1) return ToString(ts.TotalHours, 1, " часов", " час", " часа");
            if (ts.TotalMinutes >= 5) return ToString(ts.TotalMinutes, 1, " минут", " минута", " минуты");
            if (ts.TotalMinutes >= 1) return ToString(ts.TotalMinutes, 1, " минут", " минута", " минуты");
            if (ts.TotalSeconds >= 15) return ToString(ts.TotalSeconds, 1, " секунд", " секунда", " секунды");
            if (ts.TotalSeconds >= 5) return ToString(ts.TotalSeconds, 1, " секунд", " секунда", " секунды");
            return ToString(ts.TotalSeconds, 1, " секунд", " секунда", " секунды");
        }
    }
}