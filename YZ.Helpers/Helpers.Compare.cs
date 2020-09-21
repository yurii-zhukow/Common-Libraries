using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;
using System.Text;

namespace YZ {

       public class GenericComparer<T> : IComparer<T> {
            readonly Func<T, T, int> cmp;
            public GenericComparer(Func<T, T, int> cmp) { this.cmp = cmp; }
            public int Compare(T x, T y) => cmp(x, y);

        }

        public class GenericEqualityComparer<T> : IEqualityComparer<T> {
            readonly Func<T, T, bool> cmp;
            readonly Func<T, int> hash;

            public GenericEqualityComparer(Func<T, T, bool> cmp, Func<T, int> hash) {
                this.cmp = cmp;
                this.hash = hash;
            }
            public bool Equals(T x, T y) {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;
                return cmp(x, y);
            }

            public int GetHashCode(T obj) => obj == null ? 0 : hash(obj);
        }
        public class GenericEqualityComparer<T, TProp> : IEqualityComparer<T> {
            readonly Func<T, IEquatable<TProp>> prop;
            public GenericEqualityComparer(Func<T, IEquatable<TProp>> prop) { this.prop = prop; }

            public bool Equals(T x, T y) => prop(x).Equals(prop(y));
            public int GetHashCode(T obj) => prop(obj).GetHashCode();
        }

}