using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace YZ {
    public static partial class Helpers {

        public static string NormalizeIp(this string ip, string deflt = "127.0.0.1") => string.IsNullOrWhiteSpace(ip)
            ? deflt
            : ip.Split('.', 4)
               .Select(s => s.AsInt().Constraint(0, 255).ToString())
               .Take(4).ToString(".");

        public static string NormalizeMac(this string mac, string deflt = null) => string.IsNullOrWhiteSpace(mac)
            ? deflt
            : Regex.Replace(mac.ToUpper(), "[^0-9A-F]+", ":")
                .Split(':', StringSplitOptions.RemoveEmptyEntries)
                .Select(h => h.FromHex().Constraint(0, 255).ToString("X2"))
                .Take(8)
                .ToString(":");


        public static IPAddress GetLocalIp(AddressFamily addressFamily = AddressFamily.InterNetwork) => IPAddress.Parse(GetLocalIpAddress(addressFamily));
        public static string GetLocalIpAddress(AddressFamily addressFamily = AddressFamily.InterNetwork) {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList) {
                if (ip.AddressFamily == addressFamily) {
                    return ip.ToString();
                }
            }
            throw new Exception($"Can`t obtain local IP address for {addressFamily}");
        }
    }
}
