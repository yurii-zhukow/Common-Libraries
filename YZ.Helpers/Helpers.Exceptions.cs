using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Reflection;


namespace YZ {

    public static partial class Helpers {

        public static IEnumerable<string> GetInner(this Exception ex, int maxDepth = 5) {
            if (maxDepth <= 0 || ex == null) return new string[0];
            var res = new List<string>() { ex.Message };

            if (ex is AggregateException ae) {
                res.AddRange(ae.InnerExceptions.SelectMany(aex => aex.GetInner(maxDepth - 1)));
                return res;
            }

            ex = ex.InnerException;
            while (maxDepth-- >= 0 && ex != null) {
                res.Add(ex.Message);
                ex = ex.InnerException;
            }
            return res;
        }

        public static string GetInner(this Exception ex, string separator, int maxDepth = 5) => GetInner(ex, maxDepth).ToString(separator);

        public static TException GetInner<TException>(this Exception ex) where TException : Exception {
            while (ex != null) {
                if (ex is TException y) return y;
                if (ex is AggregateException ae) {
                    foreach (var ie in ae.InnerExceptions) {
                        var r = ie.GetInner<TException>();
                        if (r != null) return r;
                    }
                }
                ex = ex.InnerException;
            }
            return null;
        }

        public static Exception GetInner(this Exception ex, Func<Exception, bool> selector) {
            while (ex != null) {
                if (selector(ex)) return ex;
                if (ex is AggregateException ae) {
                    foreach (var ie in ae.InnerExceptions) {
                        var r = ie.GetInner(selector);
                        if (r != null) return r;
                    }
                }
                ex = ex.InnerException;
            }
            return null;
        }


        public static TResult SafeCall<TSource, TResult>(this TSource src, Func<TSource, TResult> fn) => src.SafeCall(fn, default, out _);
        public static TResult SafeCall<TSource, TResult>(this TSource src, Func<TSource, TResult> fn, TResult dflt) => src.SafeCall(fn, dflt, out _);
        public static TResult SafeCall<TSource, TResult>(this TSource src, Func<TSource, TResult> fn, TResult dflt, out Exception ex) {
            try {
                ex = null;
                return fn(src);
            } catch (Exception e) {
                ex = e; return dflt;
            }
        }
        public static bool SafeCall<TSource>(this TSource src, Action<TSource> fn) => src.SafeCall(fn, out _);
        public static bool SafeCall<TSource>(this TSource src, Action<TSource> fn, out Exception ex) {
            try {
                ex = null;
                fn(src);
                return true;
            } catch (Exception e) {
                ex = e; return false;
            }
        }

        public static bool SafeCall(this Action fn) {
            try {
                fn();
                return true;
            } catch {
                return false;
            }
        }

        public static TResult SafeCall<TResult>(this Func<TResult> fn, TResult @default = default) {
            try {
                return fn();
            } catch {
                return @default;
            }
        }
        public static TResult SafeCall<TResult>(this Func<TResult> fn, out Exception ex, TResult @default = default) {
            try {
                ex = null;
                return fn();
            } catch (Exception e) {
                ex = e;
                return @default;
            }
        }

    }
}