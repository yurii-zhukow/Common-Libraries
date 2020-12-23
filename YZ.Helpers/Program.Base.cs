using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Threading;
using YZ;

namespace YZ {

    public abstract partial class ProgramBase {

        static ProgramBase() {

            if (CultureInfo.InvariantCulture.Clone() is CultureInfo ci) {
                ci.NumberFormat.NumberDecimalSeparator = ".";
                ci.DateTimeFormat.ShortDatePattern = "dd.MM.yyyy";
                ci.DateTimeFormat.LongDatePattern = "dd.MM.yyyy";
                CultureInfo.CurrentCulture = ci;
                CultureInfo.CurrentUICulture = ci;
                CultureInfo.DefaultThreadCurrentCulture = ci;
                CultureInfo.DefaultThreadCurrentUICulture = ci;
                Thread.CurrentThread.CurrentCulture = ci;
                Thread.CurrentThread.CurrentUICulture = ci;
            }


        }



    }
}
