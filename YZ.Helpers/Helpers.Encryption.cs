using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;
using System.Text;

namespace YZ
{

    public static partial class Helpers
    {
        public static string Md5(this string s)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            return md5.ComputeHash(Encoding.UTF8.GetBytes(s)).ToString(format: "x2");
        }

        public static string GeneratePassword()
        {
            var rnd = new Random();
            var data = new byte[6];
            rnd.NextBytes(data);
            return System.Convert.ToBase64String(data);
        }
    }
}