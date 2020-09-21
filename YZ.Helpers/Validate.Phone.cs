using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;
using YZ;

namespace YZ.Validate {
    public class Phone {

        public static string Format(string phone, bool format) { return string.IsNullOrWhiteSpace(phone) ? "" : new Phone(phone).ToString(format); }

        public string CountryCode { get; set; }
        public string AreaCode { get; set; }
        public string Number { get; set; }
        public bool IsValid => CountryCode != "" && AreaCode != "" && Number != "";

        public Phone(string s) {
            Parse(s);
        }
        public void Clear() {
            CountryCode = "";
            AreaCode = "";
            Number = "";
        }


        public void Parse(string s) {
            Clear();
            if (String.IsNullOrEmpty(s)) return;
            CountryCode = "7";
            AreaCode = "812";

            s = s.Trim();
            var hasPlus = s.StartsWith("+");
            s = Regex.Replace(s, "[^0-9]", "");
            if (s.StartsWith("00")) {
                hasPlus = true;
                s = s.Substring(2);
            }
            else if ((s.StartsWith("7") || s.StartsWith("8")) && s.Length > 10 && !hasPlus) {
                CountryCode = "7";
                s = s.Substring(1);
            }
            if (hasPlus) CountryCode = ExtractCountryCode(ref s, 3);
            if (hasPlus || s.Length >= 8) AreaCode = ExtractAreaCode(ref s, CountryCode);
            Number = s;
        }
        private static string ExtractAreaCode(ref string s, string countryCode) {
            var t = s;
            var cc = countryCode.AsInt();
            if (cc > 0 && KnownAreaCodes.ContainsKey(cc)) {
                var candidates = KnownAreaCodes[cc].Where(c => t.StartsWith(c)).ToArray();
                if (candidates.Length > 0) {
                    var c = candidates[0];
                    s = s.Substring(c.Length);
                    return c;
                }
            }
            var ccSize = countryCode.Length;
            var acSize = Math.Max(1, Math.Min(4 - ccSize, s.Length - 5));
            t = s.Substring(0, acSize);
            s = s.Substring(acSize);
            return t;
        }

        private static string ExtractCountryCode(ref string s, int digits) {
            if (s == "" || digits < 1) return "";
            if (s.Length < digits) return ExtractCountryCode(ref s, s.Length);
            try {
                var n = s.Substring(0, digits).AsInt();
                if (CountryCodes.ContainsKey(n)) {
                    s = s.Substring(digits);
                    return n.ToString();
                }
            }
            catch { }
            return ExtractCountryCode(ref s, digits - 1);
        }

        private static void SplitNumber(out List<string> acc, string s) {
            acc = new List<string>();
            while (true) {
                if (s.Length <= 3) {
                    acc.Add(s);
                    return;
                }
                var n = s.Length == 4 ? 2 : 3;
                acc.Add(s.Substring(0, n));
                s = s.Substring(n);
            }
        }

        private string AddSpaces(string s) {
            SplitNumber(out var acc, s);
            return acc.ToString("-");
        }

        public string ToString(bool format) {
            var cc = "+" + (String.IsNullOrWhiteSpace(CountryCode) ? "7" : CountryCode);
            var ac = String.IsNullOrWhiteSpace(AreaCode) ? "812" : AreaCode;
            return format ? (cc + " (" + ac + ") " + AddSpaces(Number)).Trim() : cc + ac + Number;
        }

        public override string ToString() {
            return ToString(true);
        }

        private static readonly Dictionary<int, string[]> KnownAreaCodes = new Dictionary<int, string[]>() {
            {7, new [] {"81378"}}
        };

        private static readonly Dictionary<int, string[]> CountryCodes = new Dictionary<int, string[]>(286)
           {
                { 1, new[]{"US","AG","AI","AS","BB","BM","BS","CA","DM","DO","GD","GU","JM","KN","KY","LC","MP","MS","PR","SX","TC","TT","VC","VG","VI"}},
                { 7, new[]{"RU","KZ"}},
                { 20, new[] { "EG" }},
                { 27, new[] { "ZA" }},
                { 30, new[] { "GR" }},
                { 31, new[] { "NL" }},
                { 32, new[] { "BE" }},
                { 33, new[] { "FR" }},
                { 34, new[] { "ES" }},
                { 36, new[] { "HU" }},
                { 39, new[]{"IT","VA"}},
                { 40, new[] { "RO" }},
                { 41, new[] { "CH" }},
                { 43, new[] { "AT" }},
                { 44, new[]{"GB","GG","IM","JE"}},
                { 45, new[] { "DK" }},
                { 46, new[] { "SE" }},
                { 47, new[]{"NO","SJ"}},
                { 48, new[] { "PL" }},
                { 49, new[] { "DE" }},
                { 51, new[] { "PE" }},
                { 52, new[] { "MX" }},
                { 53, new[] { "CU" }},
                { 54, new[] { "AR" }},
                { 55, new[] { "BR" }},
                { 56, new[] { "CL" }},
                { 57, new[] { "CO" }},
                { 58, new[] { "VE" }},
                { 60, new[] { "MY" }},
                { 61, new[]{"AU","CC","CX"}},
                { 62, new[] { "ID" }},
                { 63, new[] { "PH" }},
                { 64, new[] { "NZ" }},
                { 65, new[] { "SG" }},
                { 66, new[] { "TH" }},
                { 81, new[] { "JP" }},
                { 82, new[] { "KR" }},
                { 84, new[] { "VN" }},
                { 86, new[] { "CN" }},
                { 90, new[] { "TR" }},
                { 91, new[] { "IN" }},
                { 92, new[] { "PK" }},
                { 93, new[] { "AF" }},
                { 94, new[] { "LK" }},
                { 95, new[] { "MM" }},
                { 98, new[] { "IR" }},
                { 211, new[] { "SS" }},
                { 212, new[]{"MA","EH"}},
                { 213, new[] { "DZ" }},
                { 216, new[] { "TN" }},
                { 218, new[] { "LY" }},
                { 220, new[] { "GM" }},
                { 221, new[] { "SN" }},
                { 222, new[] { "MR" }},
                { 223, new[] { "ML" }},
                { 224, new[] { "GN" }},
                { 225, new[] { "CI" }},
                { 226, new[] { "BF" }},
                { 227, new[] { "NE" }},
                { 228, new[] { "TG" }},
                { 229, new[] { "BJ" }},
                { 230, new[] { "MU" }},
                { 231, new[] { "LR" }},
                { 232, new[] { "SL" }},
                { 233, new[] { "GH" }},
                { 234, new[] { "NG" }},
                { 235, new[] { "TD" }},
                { 236, new[] { "CF" }},
                { 237, new[] { "CM" }},
                { 238, new[] { "CV" }},
                { 239, new[] { "ST" }},
                { 240, new[] { "GQ" }},
                { 241, new[] { "GA" }},
                { 242, new[] { "CG" }},
                { 243, new[] { "CD" }},
                { 244, new[] { "AO" }},
                { 245, new[] { "GW" }},
                { 246, new[] { "IO" }},
                { 247, new[] { "AC" }},
                { 248, new[] { "SC" }},
                { 249, new[] { "SD" }},
                { 250, new[] { "RW" }},
                { 251, new[] { "ET" }},
                { 252, new[] { "SO" }},
                { 253, new[] { "DJ" }},
                { 254, new[] { "KE" }},
                { 255, new[] { "TZ" }},
                { 256, new[] { "UG" }},
                { 257, new[] { "BI" }},
                { 258, new[] { "MZ" }},
                { 260, new[] { "ZM" }},
                { 261, new[] { "MG" }},
                { 262, new[]{"RE","YT"}},
                { 263, new[] { "ZW" }},
                { 264, new[] { "NA" }},
                { 265, new[] { "MW" }},
                { 266, new[] { "LS" }},
                { 267, new[] { "BW" }},
                { 268, new[] { "SZ" }},
                { 269, new[] { "KM" }},
                { 290, new[]{"SH","TA"}},
                { 291, new[] { "ER" }},
                { 297, new[] { "AW" }},
                { 298, new[] { "FO" }},
                { 299, new[] { "GL" }},
                { 350, new[] { "GI" }},
                { 351, new[] { "PT" }},
                { 352, new[] { "LU" }},
                { 353, new[] { "IE" }},
                { 354, new[] { "IS" }},
                { 355, new[] { "AL" }},
                { 356, new[] { "MT" }},
                { 357, new[] { "CY" }},
                { 358, new[]{"FI","AX"}},
                { 359, new[] { "BG" }},
                { 370, new[] { "LT" }},
                { 371, new[] { "LV" }},
                { 372, new[] { "EE" }},
                { 373, new[] { "MD" }},
                { 374, new[] { "AM" }},
                { 375, new[] { "BY" }},
                { 376, new[] { "AD" }},
                { 377, new[] { "MC" }},
                { 378, new[] { "SM" }},
                { 380, new[] { "UA" }},
                { 381, new[] { "RS" }},
                { 382, new[] { "ME" }},
                { 383, new[] { "XK" }},
                { 385, new[] { "HR" }},
                { 386, new[] { "SI" }},
                { 387, new[] { "BA" }},
                { 389, new[] { "MK" }},
                { 420, new[] { "CZ" }},
                { 421, new[] { "SK" }},
                { 423, new[] { "LI" }},
                { 500, new[] { "FK" }},
                { 501, new[] { "BZ" }},
                { 502, new[] { "GT" }},
                { 503, new[] { "SV" }},
                { 504, new[] { "HN" }},
                { 505, new[] { "NI" }},
                { 506, new[] { "CR" }},
                { 507, new[] { "PA" }},
                { 508, new[] { "PM" }},
                { 509, new[] { "HT" }},
                { 590, new[]{"GP","BL","MF"}},
                { 591, new[] { "BO" }},
                { 592, new[] { "GY" }},
                { 593, new[] { "EC" }},
                { 594, new[] { "GF" }},
                { 595, new[] { "PY" }},
                { 596, new[] { "MQ" }},
                { 597, new[] { "SR" }},
                { 598, new[] { "UY" }},
                { 599, new[]{"CW","BQ"}},
                { 670, new[] { "TL" }},
                { 672, new[] { "NF" }},
                { 673, new[] { "BN" }},
                { 674, new[] { "NR" }},
                { 675, new[] { "PG" }},
                { 676, new[] { "TO" }},
                { 677, new[] { "SB" }},
                { 678, new[] { "VU" }},
                { 679, new[] { "FJ" }},
                { 680, new[] { "PW" }},
                { 681, new[] { "WF" }},
                { 682, new[] { "CK" }},
                { 683, new[] { "NU" }},
                { 685, new[] { "WS" }},
                { 686, new[] { "KI" }},
                { 687, new[] { "NC" }},
                { 688, new[] { "TV" }},
                { 689, new[] { "PF" }},
                { 690, new[] { "TK" }},
                { 691, new[] { "FM" }},
                { 692, new[] { "MH" }},
                { 800, new[] { "001" }},
                { 808, new[] { "001" }},
                { 850, new[] { "KP" }},
                { 852, new[] { "HK" }},
                { 853, new[] { "MO" }},
                { 855, new[] { "KH" }},
                { 856, new[] { "LA" }},
                { 870, new[] { "001" }},
                { 878, new[] { "001" }},
                { 880, new[] { "BD" }},
                { 881, new[] { "001" }},
                { 882, new[] { "001" }},
                { 883, new[] { "001" }},
                { 886, new[] { "TW" }},
                { 888, new[] { "001" }},
                { 960, new[] { "MV" }},
                { 961, new[] { "LB" }},
                { 962, new[] { "JO" }},
                { 963, new[] { "SY" }},
                { 964, new[] { "IQ" }},
                { 965, new[] { "KW" }},
                { 966, new[] { "SA" }},
                { 967, new[] { "YE" }},
                { 968, new[] { "OM" }},
                { 970, new[] { "PS" }},
                { 971, new[] { "AE" }},
                { 972, new[] { "IL" }},
                { 973, new[] { "BH" }},
                { 974, new[] { "QA" }},
                { 975, new[] { "BT" }},
                { 976, new[] { "MN" }},
                { 977, new[] { "NP" }},
                { 979, new[] { "001" }},
                { 992, new[] { "TJ" }},
                { 993, new[] { "TM" }},
                { 994, new[] { "AZ" }},
                { 995, new[] { "GE" }},
                { 996, new[] { "KG" }},
                { 998, new[] { "UZ" }}
           };

    }
}
