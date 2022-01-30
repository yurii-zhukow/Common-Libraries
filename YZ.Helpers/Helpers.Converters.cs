using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;


namespace YZ {


    public static partial class Helpers {
        public static (T1 Item1, T2 Item2) Explode<T, T1, T2>(this T src, Func<T, T1> f1, Func<T, T2> f2) => (f1(src), f2(src));
        public static Func<T, bool> Not<T>(Func<T, bool> fn) => (T src) => !fn(src);
    }

}
