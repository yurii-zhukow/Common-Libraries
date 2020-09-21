using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace YZ {

    public static partial class Helpers {

        public static Uri ReplaceHost(this Uri uri, string host) => new UriBuilder(uri) { Host = host }.Uri;

        public static bool IsValidIpV4(this string ip) {
            var parts = ip.Split('.');
            if (parts.Length != 4) return false;

            try {
                for (int i = 0; i < parts.Length; i++) {
                    var b = System.Convert.ToByte(parts[i]);
                    parts[i] = b.ToString();
                }

                ip = parts.ToString(".");
                if (ip == "0.0.0.0" || ip == "255.255.255.255") return false;
            } catch {
                return false;
            }

            return true;
        }

    }
}
