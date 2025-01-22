using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace YZ {


    public static partial class Helpers {

        public static string Params(this string template, params object[] parms) => string.Format(template, parms);

        public static int AsInt(this string s, int? min = null, int? max = null, int? outrangeDefault = null) {
            var res = outrangeDefault ?? min ?? max ?? 0;

            while (true) {
                s = Regex.Replace(s ?? "", "[^0-9\\-]+", "");
                if (String.IsNullOrWhiteSpace(s)) if (outrangeDefault.HasValue) return outrangeDefault.Value; else break;
                try { res = System.Convert.ToInt32(s); } catch { if (outrangeDefault.HasValue) return outrangeDefault.Value; }
                break;
            }

            return res.Constraint(min, max, outrangeDefault);
        }

        public static long AsLong(this string s, long? min = null, long? max = null, long? outrangeDefault = null) {
            var res = outrangeDefault ?? min ?? max ?? 0;

            while (true) {
                s = Regex.Replace(s ?? "", "[^0-9\\-]+", "");
                if (String.IsNullOrWhiteSpace(s)) if (outrangeDefault.HasValue) return outrangeDefault.Value; else break;
                try { res = System.Convert.ToInt64(s); } catch { if (outrangeDefault.HasValue) return outrangeDefault.Value; }
                break;
            }

            return res.Constraint(min, max, outrangeDefault);
        }

        public static double AsDouble(this string s, double? min = null, double? max = null, double? outrangeDefault = null) {
            var res = outrangeDefault ?? min ?? max ?? 0.0;

            while (true) {
                s = Regex.Replace(s ?? "", "[^0-9\\.\\-]+", "");
                if (string.IsNullOrWhiteSpace(s)) if (outrangeDefault.HasValue) return outrangeDefault.Value; else break;
                if (!double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out res) && outrangeDefault.HasValue) return outrangeDefault.Value;
                break;
            }

            return res.Constraint(min, max, outrangeDefault);
        }

        public static bool AsBool(this string s, params string[] trueValues) {
            var res = AsBool(s);
            if (res != null) return res.Value;
            if (trueValues.Length == 0) return false;
            if (s == null) return false;
            return trueValues.Contains(s);
        }

        public static bool AsBool(this string s, bool deflt) => AsBool(s) ?? deflt;

        public static bool? AsBool(this string s) {
            if (string.IsNullOrEmpty(s)) return null;
            s = s.Trim().ToLower();
            if (s == "true" || s == "yes" || s == "on") return true;
            if (s == "false" || s == "no" || s == "off") return false;
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out var res)) return res > 0;
            return null;
        }

        public static int FromHex(this string s) {
            s = Regex.Replace(s ?? "", "[^0-9A-F]+", "");
            if (String.IsNullOrWhiteSpace(s)) return 0;

            try {
                return int.Parse(s, System.Globalization.NumberStyles.HexNumber);
            } catch {
                return 0;
            }
        }

        public static string ToString(this bool? b, string trueValue, string falseValue, string nullValue = null) => b?.ToString(trueValue, falseValue) ?? nullValue ?? falseValue;
        public static string ToString(this bool b, string trueValue, string falseValue) => b ? trueValue : falseValue;

        public static string Repeat(this string s, int count) {
            if (string.IsNullOrEmpty(s) || count <= 0) return "";
            if (count == 1) return s;
            var sb = new StringBuilder(s.Length * count);
            for (var i = 0; i < count; i++) sb.Append(s);
            return sb.ToString();
        }

        public static string Ellipsis(this string s, int maxLength) {
            if (string.IsNullOrEmpty(s) || maxLength <= 0) return "";
            if (s.Length <= maxLength) return s;
            if (maxLength < 4) return ".".Repeat(maxLength);
            return s.Substring(0, maxLength - 3) + "...";
        }

    }


}
