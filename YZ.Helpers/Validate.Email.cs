using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YZ.Validate {
    public static class Email {

        public static string FilterEmail(this string email) => Filter(email);
        public static string Filter(string email) => IsValid(email) ? email.ToLower() : "";

        public static bool IsValidEmail(this string s) => IsValid(s);
        public static bool IsValid(string s) {
            if (String.IsNullOrEmpty(s))
                return false;
            try {
                s = Regex.Replace(s, @"(@)(.+)$", DomainMapper,
                    RegexOptions.None, TimeSpan.FromMilliseconds(500));
            }
            catch (RegexMatchTimeoutException) {
                return false;
            }
            catch {
                return false;
            }
            try {
                return Regex.IsMatch(s,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(500));
            }
            catch (RegexMatchTimeoutException) {
                return false;
            }
        }

        private static string DomainMapper(Match match) {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();
            string domainName = match.Groups[2].Value;
            domainName = idn.GetAscii(domainName);
            return match.Groups[1].Value + domainName;
        }

    }
}
