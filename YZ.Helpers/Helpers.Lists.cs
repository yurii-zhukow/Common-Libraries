using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace YZ {
    public static partial class Helpers {
        public static string ToString<T>(this ICollection<T> arr, string separator) => !arr.Any() ? "" : string.Join(separator, arr.Select(e => e.ToString()).ToArray());
        public static string ToString<T>(this IEnumerable<T> arr, string separator) => !arr.Any() ? "" : string.Join(separator, arr.Select(e => e.ToString()).ToArray());
        public static string ToString<T>(this IEnumerable<T> arr, string separator, Func<T, object> convert) => !arr.Any() ? "" : string.Join(separator, arr.Select(convert).ToArray());
        public static string ToString<T>(this IEnumerable<T> arr, string separator, string openTag, string closeTag) => !arr.Any() ? "" : string.Join(separator, arr.Select(e => $"{openTag}{e}{closeTag}").ToArray());
        public static string ToString(this IEnumerable<byte> arr, string separator = "", string format = "x2") => !arr.Any() ? "" : string.Join(separator, arr.Select(e => e.ToString(format)).ToArray());
        public static string ToBase64String(this IEnumerable<byte> arr) => !arr.Any() ? "" : System.Convert.ToBase64String(arr.ToArray());
        public static string ToBase64String(this byte[] arr) => !arr.Any() ? "" : System.Convert.ToBase64String(arr);
        public static string ToString<TValue>(this Dictionary<string, TValue> arr, string separator, string kvSeparator) => string.Join(separator, arr.Select(kv => $"{kv.Key}{kvSeparator}{kv.Value}"));

        public static T[] ToArray<T>(this IEnumerable<T> src, int size, Func<T, int> index) {
            var res = Enumerable.Repeat(Activator.CreateInstance<T>(), size).ToArray();
            src.Where(c => index(c) >= 0 && index(c) < size).ToList().ForEach(c => res[index(c)] = c);
            return res;
        }



        public static TDest[] ToArray<T, TDest>(this IEnumerable<T> src, int size, Func<T, int> index, Func<T, TDest> convert) {
            var res = Enumerable.Repeat(Activator.CreateInstance<T>(), size).ToList();
            src.Where(c => index(c) >= 0 && index(c) < size).ToList().ForEach(c => res[index(c)] = c);
            return res.Select(convert).ToArray();
        }


        public static Func<KeyValuePair<TKey, TValue>, TKey> KeyFn<TKey, TValue>() => kv => kv.Key;
        public static Func<KeyValuePair<TKey, TValue>, TValue> ValueFn<TKey, TValue>() => kv => kv.Value;

        public static void Fill<T>(this T[] src, T value, int start = 0, int count = 0) {
            if (start < 0) start = 0;
            if (count < 1) count = src.Length - start;
            if (count < 1) return;
            var end = (start + count).Constraint(start, src.Length - 1);
            for (var i = start; i <= end; i++) src[i] = value;
        }


        public static IEnumerable<IEnumerable<T>> SplitBy<T>(this IEnumerable<T> src, int size) {
            src = src.ToList();
            if (size == 0 || size >= src.Count()) return new[] { src };
            var res = new List<IEnumerable<T>>();
            while (src.Any()) {
                res.Add(src.Take(size).ToArray());
                src = src.Skip(size);
            }
            return res.ToArray();
        }

        public static T[] Generate<T>(this int count, Func<int, T> generator) {
            var res = new T[count];
            for (int i = 0; i < res.Length; i++) res[i] = generator(i);
            return res;
        }


        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> src) => src.ToDictionary(t => t.Key, t => t.Value);

    }


    public class ConcurrentDictionary<TKey, TSubKey, TSubSubKey, TValue> : ConcurrentDictionary<TKey, ConcurrentDictionary<TSubKey, TSubSubKey, TValue>> {
        object locker = new object();
        public TValue AddOrUpdate(TKey key, TSubKey subkey, TSubSubKey subsubkey, Func<TKey, TSubKey, TSubSubKey, TValue> addValueFactory, Func<TKey, TSubKey, TSubSubKey, TValue, TValue> updateValueFactory) {
            lock (locker) {
                var r1 = AddOrUpdate(key, k => new ConcurrentDictionary<TSubKey, TSubSubKey, TValue>(), (k, old) => old);
                return r1.AddOrUpdate(subkey, subsubkey, (k, k2) => addValueFactory(key, k, k2), (k, k2, old) => updateValueFactory(key, k, k2, old));
            }
        }

        public bool TryGetValue(TKey key, TSubKey subkey, TSubSubKey subsubkey, out TValue value) {
            value = default;
            if (!TryGetValue(key, out var value1)) return false;
            return value1.TryGetValue(subkey, subsubkey, out value);
        }

        public bool TryAdd(TKey key, TSubKey subkey, TSubSubKey subsubkey, TValue value) {
            var v = new ConcurrentDictionary<TSubKey, TSubSubKey, TValue>();
            return TryAdd(key, v);
        }


        public bool TryRemove(TKey key, TSubKey subkey, TSubSubKey subsubkey, out TValue value) {
            lock (locker) {
                value = default;
                if (!TryGetValue(key, out var value1)) return false;
                if (!value1.TryRemove(subkey, subsubkey, out value)) return false;
                if (value1.IsEmpty) this.TryRemove(key, out value1);
                return true;
            }
        }
        public List<(TKey Key, TSubKey SubKey, TSubSubKey SubSubKey, TValue Value)> ToList() => this.SelectMany(kv1 => kv1.Value.SelectMany(kv2 => kv2.Value.Select(kv3 => (kv1.Key, kv2.Key, kv3.Key, kv3.Value)))).ToList();
        public List<(TSubKey SubKey, TSubSubKey SubSubKey, TValue Value)> ToList(TKey key) {
            if (!TryGetValue(key, out var r)) return new List<(TSubKey SubKey, TSubSubKey SubSubKey, TValue Value)>();
            return r.ToList();
        }
    }
    public class ConcurrentDictionary<TKey, TSubKey, TValue> : ConcurrentDictionary<TKey, ConcurrentDictionary<TSubKey, TValue>> {
        object locker = new object();
        public TValue AddOrUpdate(TKey key, TSubKey subkey, Func<TKey, TSubKey, TValue> addValueFactory, Func<TKey, TSubKey, TValue, TValue> updateValueFactory) {
            lock (locker) {
                var r1 = AddOrUpdate(key, k => new ConcurrentDictionary<TSubKey, TValue>(), (k2, old) => old);
                return r1.AddOrUpdate(subkey, k => addValueFactory(key, k), (k, old) => updateValueFactory(key, k, old));
            }
        }

        public bool TryGetValue(TKey key, TSubKey subkey, out TValue value) {
            value = default;
            if (!TryGetValue(key, out var value1)) return false;
            return value1.TryGetValue(subkey, out value);
        }

        public bool TryAdd(TKey key, TSubKey subkey, TValue value) {
            var v = new ConcurrentDictionary<TSubKey, TValue>();
            if (!v.TryAdd(subkey, value)) return false;
            return TryAdd(key, v);
        }


        public bool TryRemove(TKey key, TSubKey subkey, out TValue value) {
            lock (locker) {
                value = default;
                if (!TryGetValue(key, out var value1)) return false;
                if (!value1.TryRemove(subkey, out value)) return false;
                if (value1.IsEmpty) this.TryRemove(key, out value1);
                return true;
            }
        }


        public List<(TKey Key, TSubKey SubKey, TValue Value)> ToList() => this.SelectMany(kv => kv.Value.Select(kv1 => (Key: kv.Key, SubKey: kv1.Key, Value: kv1.Value))).ToList();
        public List<T> ToList<T>(Func<TKey, TSubKey, TValue, T> convert) => this.SelectMany(kv => kv.Value.Select(kv1 => convert(kv.Key, kv1.Key, kv1.Value))).ToList();
        public static Func<TKey, TSubKey, TValue, TValue> ValueExtractor => (k, k2, v) => v;

        public new IEnumerable<TValue> Values => ToList(ValueExtractor);
        public void ForEach(Action<TKey, TSubKey, TValue> fn) {
            foreach (var k in Keys.ToList()) if (TryGetValue(k, out var v)) foreach (var kk in v.Keys.ToList()) if (v.TryGetValue(kk, out var vv)) fn(k, kk, vv);
        }
        public void ForEach(TKey key, Action<TKey, TSubKey, TValue> fn) {
            if (TryGetValue(key, out var v)) foreach (var k in v.Keys.ToList()) if (v.TryGetValue(k, out var vv)) fn(key, k, vv);
        }
        public void ForEach(IEnumerable<TKey> keys, Action<TKey, TSubKey, TValue> fn) {
            foreach (var key in keys) if (TryGetValue(key, out var v)) foreach (var subKey in v.Keys.ToList()) if (v.TryGetValue(subKey, out var vv)) fn(key, subKey, vv);
        }

        public void ForEach(IEnumerable<TKey> keys, IEnumerable<TSubKey> subKeys, Action<TKey, TSubKey, TValue> fn) {
            foreach (var key in keys) if (TryGetValue(key, out var v)) foreach (var subKey in subKeys) if (v.TryGetValue(subKey, out var vv)) fn(key, subKey, vv);
        }
    }


    public class FolderTree<T> {
        public class Folder {
            public List<Folder> Folders { get; } = new List<Folder>();
        }
        public class Item { }

        public Folder Root { get; } = new Folder();


    }

}