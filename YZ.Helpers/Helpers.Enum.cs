﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;


namespace YZ {

    public enum Severity {
        NotError = 0,
        Hint = 1,
        Warning = 2,
        Error = 3,
        Critical = 4,
        Fatal = 5
    }



    public static partial class Helpers {

        public static KeyValuePair<string, Dictionary<string, string>> ToTypes<TEnumType>() where TEnumType : Enum, IConvertible {
            var type = typeof(TEnumType);
            var t = Enum.GetValues(type).Cast<TEnumType>().ToArray();
            var d1 = t.ToDictionary(v => v.ToString(), v => v.GetDescription());

            foreach (var tp in t) {
                var x = System.Convert.ChangeType(tp, tp.GetTypeCode()).ToString();
                d1.Add(x, d1[tp.ToString()]);
            }

            return new KeyValuePair<string, Dictionary<string, string>>(type.Name, d1);
        }

        public static TEnumType[] EnumToArray<TEnumType>() where TEnumType : Enum => Enum.GetValues(typeof(TEnumType)).Cast<TEnumType>().ToArray();
        public static TResult[] EnumToArray<TEnumType, TResult>(Func<TEnumType, TResult> fn) where TEnumType : Enum => EnumToArray<TEnumType>().Select(fn).ToArray();
        public static TResult[] EnumToArray<TEnumType, TAttr, TResult>(bool inherit, Func<TEnumType, TAttr, TResult> fn) where TEnumType : Enum where TAttr : Attribute => EnumToArray<TEnumType>().Select(t => t.GetEnumAttr<TEnumType, TAttr, TResult>(inherit, fn, t => default)).ToArray();

        public static Dictionary<TEnumType, TResult> EnumToDictionary<TEnumType, TResult>(Func<TEnumType, TResult> resFunc) where TEnumType : Enum {
            var type = typeof(TEnumType);
            var keys = Enum.GetValues(type).Cast<TEnumType>();
            return keys.ToDictionary(v => v, v => resFunc(v));
        }

        public static Dictionary<TEnumType, TResult> EnumToDictionary<TEnumType, TAttr, TResult>(bool inherit, Func<TAttr, TResult> resFunc, Func<TEnumType, TResult> dflt) where TEnumType : Enum, IConvertible where TAttr : Attribute {
            var type = typeof(TEnumType);
            var keys = Enum.GetValues(type).Cast<TEnumType>();
            return keys.ToDictionary(v => v, v => v == null ? dflt.Invoke(v) : resFunc(v.GetEnumAttr<TEnumType, TAttr>(inherit, e => default)));
        }

        public static Dictionary<TEnumType, string> ToKeyValuePairs<TEnumType>() where TEnumType : Enum, IConvertible {
            var type = typeof(TEnumType);
            return Enum.GetValues(type).Cast<TEnumType>().ToDictionary(v => v, v => v.GetDescription());
        }
        public static TResult GetEnumAttr<TEnumType, TAttr, TResult>(this TEnumType value, bool inherit, Func<TEnumType, TAttr, TResult> fn, Func<TEnumType, TAttr> dflt) where TEnumType : Enum, IConvertible where TAttr : Attribute => fn(value, value.GetEnumAttr<TEnumType, TAttr>(inherit, dflt));
        public static TAttr GetEnumAttr<TEnumType, TAttr>(this TEnumType value, bool inherit, Func<TEnumType, TAttr> dflt = null) where TEnumType : Enum, IConvertible where TAttr : Attribute {
            dflt ??= ((a) => default);
            var type = typeof(TEnumType);
            var name = Enum.GetName(type, value);
            if (name == null) return dflt?.Invoke(value);
            var field = type.GetField(name);
            if (field == null) return dflt?.Invoke(value);
            return (TAttr)Attribute.GetCustomAttribute(field, typeof(TAttr), inherit) ?? dflt?.Invoke(value);
        }


        public static TResult[] GetEnumAttrs<TEnumType, TAttr, TResult>(this TEnumType value, bool inherit, Func<TAttr, TResult> fn) where TEnumType : Enum, IConvertible where TAttr : Attribute => value.GetEnumAttrs<TEnumType, TAttr>(inherit).Select(fn).ToArray();
        public static TAttr[] GetEnumAttrs<TEnumType, TAttr>(this TEnumType value, bool inherit) where TEnumType : Enum, IConvertible where TAttr : Attribute {
            var type = typeof(TEnumType);
            var name = Enum.GetName(type, value);
            if (name == null) return new TAttr[0];
            var field = type.GetField(name);
            if (field == null) return new TAttr[0];
            return Attribute.GetCustomAttributes(field, typeof(TAttr), inherit).Select(t => (TAttr)t).ToArray();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static T next<T>(T value, int vector, out bool overflow, params T[] skipValues) where T : Enum {
            overflow = false;

            var a = (T[])Enum.GetValues(value.GetType());
            var l = a.Length;
            var c = l;
            var x = Array.IndexOf(a, value);
            while (c >= 0) {
                x += vector;
                if (x < 0 || x >= l) {
                    overflow = true;
                    x = x < 0 ? a.Length - 1 : 0;
                }

                if (Array.IndexOf(skipValues, a[x]) < 0)
                    break;
                c--;
            }

            return c <= 0 ? value : a[x];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static T fromIndex<T>(T value, int index, int vector, params T[] skipValues) where T : Enum {
            var a = (T[])Enum.GetValues(value.GetType());
            var l = a.Length;
            index -= vector; // next should finish with current if all is ok;
            if (index < 0)
                index = l - 1;
            if (index >= l)
                index = 0;
            return next(a[index], vector, out var o, skipValues);
        }

        public static T Next<T>(this T value, out bool overflow, params T[] skipValues) where T : Enum =>
            next(value, 1, out overflow, skipValues);

        public static T Prev<T>(this T value, out bool overflow, params T[] skipValues) where T : Enum =>
            next(value, -1, out overflow, skipValues);

        public static T First<T>(this T value, params T[] skipValues) where T : Enum => fromIndex(value, 0, 1, skipValues);

        public static T Last<T>(this T value, params T[] skipValues) where T : Enum => fromIndex(value, -1, -1, skipValues);

        public static bool Is<T>(this T value, params T[] allowed) where T : Enum => allowed.Contains(value);


        public static string GetDescription<TEnumType>(this TEnumType value) where TEnumType : Enum, IConvertible => GetEnumAttr<TEnumType, DescriptionAttribute>(value, false)?.Description ?? "";
        public static TEnum SafeCast<TEnum>(this string value, TEnum deflt = default) where TEnum : struct => Enum.IsDefined(typeof(TEnum), value) ? Enum.Parse<TEnum>(value) : deflt;
        public static TEnum SafeCast<TEnum>(this int value, TEnum deflt = default) where TEnum : Enum => Enum.IsDefined(typeof(TEnum), value) ? (TEnum)(object)value : deflt;
        public static TEnum SafeCast<TEnum>(this uint value, TEnum deflt = default) where TEnum : Enum => Enum.IsDefined(typeof(TEnum), value) ? (TEnum)(object)value : deflt;
        public static TEnum SafeCast<TEnum>(this byte value, TEnum deflt = default) where TEnum : Enum => SafeCast<TEnum>((int)value, deflt);
        public static TEnum SafeCast<TEnum>(this decimal? value, TEnum deflt = default) where TEnum : Enum => SafeCast<TEnum>((int)(value ?? 0), deflt);

    }


}