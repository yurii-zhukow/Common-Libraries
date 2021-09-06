using System;
using System.Linq;

namespace YZ {


    public static partial class Helpers {

        /// <summary>
        /// Представление числа в произвольной системе счисления
        /// </summary>
        /// <param name="value">значение</param>
        /// <param name="digits">Алфавит цифр</param>
        /// <returns></returns>
        public static string Convert(this int value, char[] digits) {
            const int valueBits = 64;
            int i = valueBits;
            char[] buffer = new char[i];
            int targetBase = digits.Length;

            do {
                buffer[--i] = digits[value % targetBase];
                value = value / targetBase;
            } while (value > 0);

            char[] result = new char[valueBits - i];
            Array.Copy(buffer, i, result, 0, valueBits - i);
            return new string(result);
        }

        public static double RoundTo(this double x, double step) => Math.Round(x / step) * step;
        public static double RoundTo(this double x, double step, int decimals) => Math.Round(x.RoundTo(step), decimals);
        public static decimal RoundTo(this decimal x, decimal step) => Math.Round(x / step) * step;

        public static bool InRange(this int x, int min, int max) => x >= min && x <= max;
        public static bool InRange(this double x, double min, double max) => x >= min && x <= max;
        public static bool InRange<T>(this T x, T min, T max) where T : IComparable<T> => x.CompareTo(min) >= 0 && x.CompareTo(max) <= 0;

        public static T Constraint<T>(this T src, T? min = null, T? max = null, T? outrangeDefault = null) where T : struct, IComparable {
            if (min.HasValue && src.CompareTo(min.Value) < 0) return outrangeDefault ?? min.Value;
            if (max.HasValue && src.CompareTo(max.Value) > 0) return outrangeDefault ?? max.Value;
            return src;
        }

        public static T Constraint<T>(this T src, (T min, T max) minmax, T? outrangeDefault = null) where T : struct, IComparable {
            if (src.CompareTo(minmax.min) < 0) return outrangeDefault ?? minmax.min;
            if (src.CompareTo(minmax.max) > 0) return outrangeDefault ?? minmax.max;
            return src;
        }

        public static T Constraint<T>(this T src, T min = null, T max = null, T outrangeDefault = null) where T : class, IComparable {
            if (min != null && (src == null || src.CompareTo(min) < 0)) return outrangeDefault ?? min;
            if (max != null && (src == null || src.CompareTo(max) > 0)) return outrangeDefault ?? max;
            return src;
        }

        public static T Constraint<T>(this T src, (T min, T max) minmax, T outrangeDefault = null) where T : class, IComparable {
            if (src.CompareTo(minmax.min) < 0) return outrangeDefault ?? minmax.min;
            if (src.CompareTo(minmax.max) > 0) return outrangeDefault ?? minmax.max;
            return src;
        }

        public static string ToString(this double value, double roundTo, string e0, string e1, string e2) {
            var t = value.RoundTo(roundTo);
            var x = (int)t;

            var v100 = x % 100;
            var v10 = x % 10;

            var e = v100 > 4 && v100 < 21 || v10 % 10 > 4 || v10 == 0 ? e0 : v10 == 1 ? e1 : e2;
            return $"{t}{e}";
        }

        public static string ToString(this int value, string e0, string e1, string e2) {
            var v100 = value % 100;
            var v10 = value % 10;
            var e = v100 > 4 && v100 < 21 || v10 % 10 > 4 || v10 == 0 ? e0 : v10 == 1 ? e1 : e2;
            return $"{value}{e}";
        }
        public static readonly string[] BytesNativeSuffixRu = new[] { " байт", " кБ", " МБ", " ГБ", " ТБ", " ПБ" };
        public static readonly string[] BytesNativeSuffixEn = new[] { " bytes", " Kb", " Mb", " Gb", " Tb", " Pb" };
        public static string ToBytesNative(this int value, params string[] suffix) => ToBytesNative((double)value, suffix);
        public static string ToBytesNative(this uint value, params string[] suffix) => ToBytesNative((double)value, suffix);

        public static string ToBytesNative(this long value, params string[] suffix) => ToBytesNative((double)value, suffix);
        public static string ToBytesNative(this ulong value, params string[] suffix) => ToBytesNative((double)value, suffix);
        public static string ToBytesNative(this double value, params string[] suffix) {
            if (suffix.Length == 0) suffix = BytesNativeSuffixEn;
            var ix = 0;
            while (value >= 700.0 && ix < suffix.Length - 1) {
                ix++;
                value /= 1024.0;
            }
            return $"{value:#,##0.##}{suffix[ix]}";
        }


    }


}
