using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Reflection;


namespace YZ {

    public static partial class Helpers {

        public static TAttr GetAttr<TAttr>(this Type type, bool inherit, Func<TAttr> dflt = null) where TAttr : Attribute {
            if (type == null) return dflt?.Invoke();
            return (TAttr)Attribute.GetCustomAttribute(type, typeof(TAttr), inherit) ?? dflt?.Invoke();
        }
        public static TValue GetAttr<TAttr, TValue>(this Type type, bool inherit, Func<TAttr, TValue> getValue, Func<TAttr> dflt = null) where TAttr : Attribute => getValue(GetAttr(type, inherit, dflt));


        public static TAttr GetAttr<TAttr>(this object value, bool inherit, Func<object, TAttr> dflt = null) where TAttr : Attribute {
            if (value == null) return dflt?.Invoke(default);
            return (TAttr)Attribute.GetCustomAttribute(value.GetType(), typeof(TAttr), inherit) ?? dflt?.Invoke(value);
        }
        public static TValue GetAttr<TAttr, TValue>(this object value, bool inherit, Func<TAttr, TValue> getValue, Func<object, TAttr> dflt = null) where TAttr : Attribute => getValue(GetAttr(value, inherit, dflt));

        public static TAttr GetAttr<T, TAttr>(this T value, bool inherit, Func<T, TAttr> dflt = null) where TAttr : Attribute {
            if (value == null) return dflt?.Invoke(default);
            return (TAttr)Attribute.GetCustomAttribute(value.GetType(), typeof(TAttr), inherit) ?? dflt?.Invoke(value);
        }
        public static TValue GetAttr<T, TAttr, TValue>(this T value, bool inherit, Func<TAttr, TValue> getValue, Func<T, TAttr> dflt = null) where TAttr : Attribute => getValue(GetAttr(value, inherit, dflt));

        public static string GetDescription<TType>(this TType value, bool inherit = false) => GetAttr<TType, DescriptionAttribute>(value, inherit)?.Description ?? "";
        public static string GetBriefDescription<TType>(this TType value, bool inherit = false) => GetAttr<TType, BriefDescriptionAttribute>(value, inherit)?.BriefDescription ?? value.GetDescription(inherit);

        public static string GetDescription<TType>(this TType value, GetDescriptionMode mode, bool inherit = false) {
            var brief = mode.HasFlag(GetDescriptionMode.Brief);

            if (brief) {
                var res = GetBriefDescription(value, inherit);
                return mode.HasFlag(GetDescriptionMode.Full) && string.IsNullOrEmpty(res) ? GetDescription(value, GetDescriptionMode.Full, inherit) : res;
            }
            return GetDescription(value, inherit);
        }

    }
}