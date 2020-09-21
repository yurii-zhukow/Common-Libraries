using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;


namespace YZ {

    public static partial class Helpers {

        public static string ExePath => System.Reflection.Assembly.GetExecutingAssembly().Location;
        public static string AppDir => System.IO.Path.GetDirectoryName(ExePath);

        public static string GetFullAppPath(this string relativePath) => Path.GetFullPath(relativePath, AppDir);

        static CultureInfo ru = null;
        public static CultureInfo RU => ru ??= new CultureInfo("ru");
        public static CultureInfo EN => CultureInfo.InvariantCulture;

    }
}