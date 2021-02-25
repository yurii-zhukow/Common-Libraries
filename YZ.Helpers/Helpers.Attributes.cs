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

        public static TAttr[] GetAttrs<T, TAttr>(this T value, bool inherit) where TAttr : Attribute {
            if (value == null) return new TAttr[0];
            return Attribute.GetCustomAttributes(value.GetType(), typeof(TAttr), inherit)?.OfType<TAttr>().ToArray() ?? new TAttr[0];
        }


        public static TAttr GetAttr<T, TAttr>(this T value, string propName, bool inherit, Func<T, string, TAttr> dflt = null) where TAttr : Attribute {
            if (value == null) return dflt?.Invoke(default, propName);
            var fi = typeof(T).GetMember(propName);
            if (fi == null) return dflt?.Invoke(value, propName);
            return fi.Select(t => t.GetCustomAttribute<TAttr>(inherit)).Where(t => t != null).FirstOrDefault() ?? dflt?.Invoke(value, propName);
        }

        public static TAttr[] GetAttrs<T, TAttr>(this T value, string propName, bool inherit) where TAttr : Attribute {
            if (value == null) return new TAttr[0];
            var fi = typeof(T).GetMember(propName);
            if (fi == null) return new TAttr[0];
            return fi.SelectMany(t => t.GetCustomAttributes<TAttr>(inherit).OfType<TAttr>()).Where(t => t != null).ToArray() ?? new TAttr[0];
        }


        public static TValue[] GetAttrs<T, TAttr, TValue>(this T value, bool inherit, Func<TAttr, TValue> getValue) where TAttr : Attribute => GetAttrs<T, TAttr>(value, inherit).Select(getValue).ToArray();
        public static TValue[] GetAttrs<T, TField, TAttr, TValue>(this T value, string propName, bool inherit, Func<TAttr, TValue> getValue) where TAttr : Attribute => GetAttrs<T, TAttr>(value, propName, inherit).Select(getValue).ToArray();

        public static TValue GetAttr<T, TAttr, TValue>(this T value, bool inherit, Func<TAttr, TValue> getValue, Func<T, TAttr> dflt = null) where TAttr : Attribute => getValue(GetAttr(value, inherit, dflt));
        public static TValue GetAttr<T, TField, TAttr, TValue>(this T value, string propName, bool inherit, Func<TAttr, TValue> getValue, Func<T, string, TAttr> dflt = null) where TAttr : Attribute => getValue(GetAttr(value, propName, inherit, dflt));


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

        public static string GetDescription<TType>(this TType value, string propName, bool inherit = false) => GetAttr<TType, DescriptionAttribute>(value, propName, inherit)?.Description ?? "";
        public static string GetBriefDescription<TType>(this TType value, string propName, bool inherit = false) => GetAttr<TType, BriefDescriptionAttribute>(value, propName, inherit)?.BriefDescription ?? value.GetDescription(inherit);

        public static string GetDescription<TType>(this TType value, string propName, GetDescriptionMode mode, bool inherit = false) {
            var brief = mode.HasFlag(GetDescriptionMode.Brief);

            if (brief) {
                var res = GetBriefDescription(value, propName, inherit);
                return mode.HasFlag(GetDescriptionMode.Full) && string.IsNullOrEmpty(res) ? GetDescription(value, propName, GetDescriptionMode.Full, inherit) : res;
            }
            return GetDescription(value, propName, inherit);
        }



    }
}