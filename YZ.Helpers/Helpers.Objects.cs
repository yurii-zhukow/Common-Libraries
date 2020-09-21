using System;
using System.Collections;
using System.Linq;
using System.Reflection;


namespace YZ {


    public static partial class Helpers {

        public static bool ListEqual(this IList src, IList dst, bool deep) {
            if (src.Count != dst.Count) return false;

            for (int i = 0; i < src.Count; i++) {
                var v1 = src[i];
                var v2 = dst[i];
                if (v1 == v2) continue;
                if (deep && v1.ObjectEqual(v2)) continue;
                return false;
            }

            return true;
        }

        public static bool DictionaryEqual(this IDictionary src, IDictionary dst, bool deep) {
            if (src.Count != dst.Count) return false;

            foreach (var k in src.Keys) {
                if (!dst.Contains(k)) return false;
                var v1 = src[k];
                var v2 = dst[k];
                if (v1 == v2) continue;
                if (deep && v1.ObjectEqual(v2)) continue;
                return false;
            }

            return true;
        }


        public static bool ObjectEqual(this object src, object dst) {
            if ((src == null) != (dst == null)) return false;
            if (src == dst) return true;
            var srcT = src.GetType();
            var dstT = dst.GetType();
            if (dstT != srcT) return false;
            if (src is IList srcI && dst is IList dstI) return srcI.ListEqual(dstI, srcT.GetCustomAttributes<DeepCopyAttribute>(true).Any(a => a.DeepCopy));
            if (src is IDictionary srcD && dst is IDictionary dstD) return srcD.DictionaryEqual(dstD, srcT.GetCustomAttributes<DeepCopyAttribute>(true).Any(a => a.DeepCopy));
            var props = srcT.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
               .Cast<PropertyInfo>()
               .Intersect<PropertyInfo>(dstT.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Cast<PropertyInfo>());

            foreach (var prop in props) {
                object v = prop.GetValue(src, null);
                object v2 = prop.GetValue(dst, null);
                if ((v == null) != (v2 == null)) return false;

                if ((prop.PropertyType.IsValueType || prop.PropertyType.IsEnum || prop.PropertyType == typeof(string))) {
                    if (v != null && v2 != null && prop.PropertyType.IsPrimitive) {
                        var tolerance = prop.GetCustomAttributes<ToleranceAttribute>(true).FirstOrDefault(t => t.Tolerance > 0.0);

                        if (tolerance != null) {
                            try {
                                var d1 = System.Convert.ToDouble(v);
                                var d2 = System.Convert.ToDouble(v2);
                                if (Math.Abs(d1 - d2) > tolerance.Tolerance) return false;
                                continue;
                            } catch { }
                        }
                    }

                    if (!(v?.Equals(v2) ?? true)) return false;
                    continue;
                }

                if (!v?.ObjectEqual(v2) ?? true) return false;
            }

            return true;
        }

    }


}
