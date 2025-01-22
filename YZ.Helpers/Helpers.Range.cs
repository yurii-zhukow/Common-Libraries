using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

using Microsoft.VisualBasic.CompilerServices;

using Newtonsoft.Json.Linq;

using YZ;

namespace YZ {
    public partial class Helpers {

        public static IEnumerable<Range<T>> Recombine<T>(this IEnumerable<Range<T>> src) where T : IComparable<T> => Range<T>.Recombine(src);
        public static IEnumerable<T2> Recombine<T, T2>(this IEnumerable<T2> src, Func<Range<T>, T2> convert) where T : IComparable<T> where T2 : Range<T> => Range<T>.Recombine<T2>(src, convert);
        public static TimeSpan Sum(this IEnumerable<DateRange> src) => src.Sum(t => t.Length);
        public static TimeSpan Sum(this IEnumerable<Range<DateTime>> src) => src.Sum(t => t.Stop - t.Start);
    }


    [Serializable]

    public class Range<T> : IFormattable, IComparable<Range<T>>, IEquatable<Range<T>> where T : IComparable<T> {


        public T Start { get; set; }
        public T Stop { get; set; }

        public Range() : this(default, default) { }
        public Range(T startStop) : this(startStop, startStop) { }

        public Range(T start, T stop) {
            if (start.CompareTo(stop) <= 0) {
                Start = start;
                Stop = stop;
            }
            else {
                Stop = start;
                Start = stop;
            }
        }



        public int CompareTo(Range<T> other) {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(other, null)) return 1;
            if (ReferenceEquals(this, null)) return -1;
            var res = Start.CompareTo(other.Start);
            if (res != 0) return res;
            return Stop.CompareTo(other.Stop);
        }

        public bool Equals(Range<T> other) => ReferenceEquals(this, other) || (!ReferenceEquals(this, null) && CompareTo(other) == 0);

        public (T, T) Deconstruct() => (Start, Stop);

        public override string ToString() => ToString("-");
        public string ToString(string separator) => Start.CompareTo(Stop) == 0 ? Start.ToString() : $"{Start}{separator}{Stop}";
        public string ToString(string format, IFormatProvider formatProvider) => string.IsNullOrWhiteSpace(format) || formatProvider == null ? ToString() : Start.CompareTo(Stop) == 0 ? string.Format(formatProvider, format, Start) : string.Format(formatProvider, format, Start) + "-" + string.Format(formatProvider, format, Stop);

        public static Range<T> Parse(string s, Func<string, T> convert, T dflt = default, string separator = "-") {
            T cvt(string v) {
                try { return convert(v); } catch { return dflt; }
            }

            s = s.Trim();
            separator = separator.Trim();
            var a = s.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            if (a.Length < 1) return new Range<T>(dflt, dflt);
            var r = cvt(a[0]);
            if (a.Length == 1) return new Range<T>(r, r);
            var min = r;
            var max = r;
            foreach (var t in a.Skip(1).ToList()) {
                var r1 = cvt(t);
                if (r1.CompareTo(min) < 0) min = r1;
                if (r1.CompareTo(max) > 0) max = r1;
            }

            return new Range<T>(min, max);
        }

        public List<T> ToList(Func<T, T> succ) {
            var t = Start;
            var res = new List<T>();

            while (true) {
                if (t.CompareTo(Stop) > 0) return res;
                res.Add(t);
                t = succ(t);
            }
        }

        public virtual List<T> ToList() {
            var t = Start;
            var res = new List<T>();
            while (true) {
                if (t.CompareTo(Stop) > 0) return res;
                res.Add(t);
                var t2 = Succ(t);
                if (t.CompareTo(Stop) <= 0) return res;
                t = t2;
            }
        }


        static T min(T a, T b) => a.CompareTo(b) <= 0 ? a : b;
        static T max(T a, T b) => a.CompareTo(b) >= 0 ? a : b;

        public Range<T> CombineWith(Range<T> other) => new Range<T>(min(Start, other.Start), max(Stop, other.Stop));
        public Range<T> IntersectWith(Range<T> other) => !IntersectsWith(other) ? null : other.FullyContains(this) ? this : this.FullyContains(other) ? other : Constraint(other);

        public bool IntersectsWith(Range<T> other) => other.Start.CompareTo(Stop) <= 0 && other.Stop.CompareTo(Start) >= 0;
        public bool FullyContains(Range<T> other) => other.Start.CompareTo(Start) >= 0 && other.Stop.CompareTo(Stop) <= 0;

        public virtual bool Contains(T other) => other.CompareTo(Start) >= 0 && other.CompareTo(Stop) <= 0;

        public Range<T> Constraint(T min, T max) => new Range<T>(this.Start.CompareTo(min) < 0 ? min : this.Start, this.Stop.CompareTo(max) > 0 ? max : this.Stop);
        public Range<T> Constraint(Range<T> limit) => Constraint(limit.Start, limit.Stop);

        public Range<T> Expand(T start, T stop) => new Range<T>(this.Start.CompareTo(start) > 0 ? start : this.Start, this.Stop.CompareTo(stop) < 0 ? stop : this.Stop);
        public Range<T> Expand(Range<T> other) => Expand(other.Start, other.Stop);

        public static implicit operator Range<T>(T src) => new Range<T>(src);
        public static IEnumerable<Range<T>> Sort(IEnumerable<Range<T>> src) { return (src?.Count() ?? 0) < 2 ? src : src.OrderBy(t => t).Distinct(); }

        public IEnumerable<Range<T>> Add(params Range<T>[] items) => new[] { this }.Concat(items.Where(t => t != null)).Recombine();
        public IEnumerable<Range<T>> Add(T item) => item.CompareTo(Start) >= 0 && item.CompareTo(Stop) <= 0 ? new[] { this } : Add(new Range<T>(item));

        public TDest As<TDest>() where TDest : Range<T> => (TDest)Activator.CreateInstance(typeof(TDest), Start, Stop);

        protected virtual T Succ(T value) => value;
        protected virtual T Pred(T value) => value;

        public virtual IEnumerable<Range<T>> Subtract(Range<T> other) {
            if (other.FullyContains(this)) return new Range<T>[0];
            if (!IntersectsWith(other)) return new[] { this };
            if (FullyContains(other)) return new[] { new Range<T>(Start, Pred(other.Start)), new Range<T>(Succ(other.Stop), Stop) };
            if (other.Start.CompareTo(Start) > 0) return new[] { new Range<T>(Start, Pred(other.Start)) };
            return new[] { new Range<T>(Succ(other.Stop), Stop) };
        }
        public virtual IEnumerable<Range<T>> Subtract(T item) => item.CompareTo(Start) < 0 || item.CompareTo(Stop) > 0 ? new[] { this } : Subtract(new Range<T>(item));

        public IEnumerable<Range<T>> Subtract(params Range<T>[] items) => items.Aggregate(new[] { this }, (acc, t) => acc.SelectMany(t2 => t2 - t).ToArray(), Recombine);

        public static bool operator ==(Range<T> a, Range<T> b) => ReferenceEquals(a, b) || (!ReferenceEquals(a, null) && a.Equals(b));
        public static bool operator !=(Range<T> a, Range<T> b) => !(a == b);
        public static IEnumerable<Range<T>> operator +(Range<T> a, IEnumerable<Range<T>> b) => a.Add(b.ToArray());
        public static IEnumerable<Range<T>> operator +(Range<T> a, Range<T> b) => ReferenceEquals(a, null) ? new[] { b } : a.Add(b);
        public static IEnumerable<Range<T>> operator +(Range<T> a, T b) => ReferenceEquals(a, null) ? new Range<T>[] { b } : a.Add(b);
        public static IEnumerable<Range<T>> operator -(Range<T> a, IEnumerable<Range<T>> b) => ReferenceEquals(a, null) ? new Range<T>[0] : a.Subtract(b.ToArray());
        public static IEnumerable<Range<T>> operator -(Range<T> a, Range<T> b) => ReferenceEquals(a, null) ? new Range<T>[0] : a.Subtract(b);
        public static IEnumerable<Range<T>> operator -(Range<T> a, T b) => ReferenceEquals(a, null) ? new Range<T>[0] : a.Subtract(b);

        public static Range<T> operator *(Range<T> a, Range<T> b) => ReferenceEquals(a, null) || ReferenceEquals(b, null) ? null : a.IntersectWith(b);

        public override int GetHashCode() => Start.GetHashCode() ^ Stop.GetHashCode();

        public static IEnumerable<T2> Recombine<T2>(IEnumerable<T2> src, Func<Range<T>, T2> convert) where T2 : Range<T> => Recombine(src).Select(convert).ToList();
        public static IEnumerable<Range<T>> Recombine(IEnumerable<Range<T>> src) {
            if (src == null) return new Range<T>[0];
            if (src.Count() < 2) return src;
            var a = Sort(src);
            var (l, m) = a.Skip(1)
                .Aggregate(
                    new TAcc(new List<Range<T>>(), a.First()),
                    (acc, t) => acc.Item2.IntersectsWith(t)
                        ? new TAcc(acc.Item1, acc.Item2.CombineWith(t))
                        : new TAcc(acc.Item1.Append(acc.Item2), t)
                );
            return l.Append(m).ToList();
        }


        class TAcc : Tuple<IEnumerable<Range<T>>, Range<T>> {
            public TAcc(IEnumerable<Range<T>> item1, Range<T> item2) : base(item1, item2) { }
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(this, obj))
                return true;
            if (ReferenceEquals(obj, null))
                return false;
            return obj is Range<T> r ? r.Equals(this) : false;
        }

        public static bool operator <(Range<T> left, Range<T> right) {
            return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
        }

        public static bool operator <=(Range<T> left, Range<T> right) {
            return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
        }

        public static bool operator >(Range<T> left, Range<T> right) {
            return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
        }

        public static bool operator >=(Range<T> left, Range<T> right) {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
        }
    }


    [Serializable]

    public class Range<T, TDest> : Range<T> where T : IComparable<T> where TDest : IComparable<TDest> {
        public Range(T start, T stop) : base(start, stop) { }
        public Range(T startStop) : base(startStop) { }

        public Range<TDest> ConvertTo(Func<T, TDest> converter) => new Range<TDest>(converter(Start), converter(Stop));

        public Range<T> ConvertTo() => new Range<T>(Start, Stop);

        public static Range<T, TDest> ConvertFrom(Range<TDest> src, Func<TDest, T> converter) =>
            new Range<T, TDest>(converter(src.Start), converter(src.Stop));

        public (TDest, TDest) Deconstruct(Func<T, TDest> convert) => (convert(Start), convert(Stop));

        public static implicit operator Range<T, TDest>(T src) => new Range<T, TDest>(src);

    }

    [Serializable]

    public class LongRange : Range<long> {
        public LongRange(Range<long> src) : this(src.Start, src.Stop) { }
        public LongRange(long startStop) : base(startStop) { }
        public LongRange(long start, long stop) : base(start, stop) { }

        protected override long Pred(long value) => value - 1;
        protected override long Succ(long value) => value + 1;
    }


    [Serializable]
    public sealed class IntRange : Range<int> {
        public IntRange() : this(0, 0) { }
        public IntRange(Range<int> src) : this(src.Start, src.Stop) { }
        public IntRange(int startStop) : base(startStop) { }
        public IntRange(int start, int stop) : base(start, stop) { }
        protected override int Pred(int value) => value - 1;
        protected override int Succ(int value) => value + 1;
        public override List<int> ToList() => Start >= Stop ? new List<int> { Start } : Enumerable.Range(Start, Stop - Start + 1).ToList();

        public static IntRange[] Recombine(params int[] value) => value.Length == 0 ? Array.Empty<IntRange>() : value.Select(t => new IntRange(t, t + 1)).Recombine<int, IntRange>(t => new IntRange(t.Start, t.Stop - 1)).ToArray();
        public static IntRange[] Recombine(IEnumerable<IntRange> values) => values?.Any() != true ? Array.Empty<IntRange>() : values.Select(t => new IntRange(t.Start, t.Stop + 1)).Recombine<int, IntRange>(t => new IntRange(t.Start, t.Stop - 1)).ToArray();

        public int TakeFirst(out IEnumerable<IntRange> left) {
            left = Start == Stop ? new IntRange[0] : (this - Start).Select(t => new IntRange(t)).ToArray();
            return Start;
        }

        Random rnd = null;
        public int Random() => Start == Stop ? Start : (rnd ??= new Random()).Next(Start, Stop);
        public int TakeRandom(out IEnumerable<IntRange> left) {
            var r = Random();
            left = Start == Stop ? new IntRange[0] : (this - r).Select(t => new IntRange(t)).ToArray();
            return r;
        }
    }
    public sealed class IntRangeSet {
        List<IntRange> items;
        public IntRangeSet() {
            items = new List<IntRange>();
        }
        public IntRangeSet(IntRange init) : this() {

            items = new List<IntRange> { init };
        }
        public IntRangeSet(IEnumerable<IntRange> init) : this() {
            items = IntRange.Recombine(init).ToList();
        }
        public bool Any => items?.Any() ?? false;

        public int ExtractFirst() {
            if (!Any) return 0;
            var res = items[0].TakeFirst(out var left);
            items = left.Concat(items.Skip(1)).Recombine((Range<int> t) => new IntRange(t)).ToList();
            return res;
        }
        Random rnd = null;
        public List<IntRange> ToList() => items ?? new List<IntRange>();
        public void Add(int item) {
            if (items.Any(t => t.Contains(item))) return;
            items = items.Concat(new[] { new IntRange(item) }).Recombine((Range<int> t) => new IntRange(t)).ToList();
        }
        public void AddRange(IntRange t) {
            items = items.Concat(new[] { t }).Recombine((Range<int> t) => new IntRange(t)).ToList();
        }
        public int ExtractRandom() {
            if (!Any) return 0;
            var x = items.Count == 1 ? 0 : (rnd ??= new Random()).Next(items.Count);
            var res = items[x].TakeRandom(out var left);
            items = items.Take(x).Concat(left).Concat(items.Skip(x + 1)).ToList();
            return res;
        }

        public static implicit operator IntRangeSet(IntRange[] src) => new IntRangeSet(src);
        public static implicit operator IntRangeSet(List<IntRange> src) => new IntRangeSet(src);

    }


    [Serializable]
    public sealed class DateRange : Range<DateTime> {
        public DateRange() : this(DateTime.Now) { }
        public DateRange(DateTime startStop) : this(startStop, startStop) { }
        public DateRange(DateTime start, DateTime stop) : base(start, stop) { }

        public TimeSpan Length => Stop - Start;

        public static implicit operator DateRange(DateTime src) => new DateRange(src);

        public static DateRange FullMonthFromDate(DateTime src) => new DateRange(src.StartOfMonth(), src.EndOfMonth());

        protected override DateTime Succ(DateTime value) => value.AddTicks(1);
        protected override DateTime Pred(DateTime value) => value.AddTicks(-1);

        public DateRange Expand(TimeSpan margin) => new DateRange(Start.Add(-margin), Stop.Add(margin));

        public bool Equals(DateRange b, TimeSpan epsilon) {
            var e = Math.Abs(epsilon.TotalSeconds);
            var d1 = Math.Abs((this.Start - b.Start).TotalSeconds);
            var d2 = Math.Abs((this.Stop - b.Stop).TotalSeconds);
            return d1 <= e && d2 <= e;
        }

        public DateRange[] SplitByDay() {
            if (this.Start.Date == this.Stop.Date) return new[] { this };
            var n = (int)((this.Stop.Date - this.Start.Date).TotalDays);
            var res = new DateRange[n + 1];
            res[0] = new DateRange(this.Start, this.Start.Date.AddDays(1).AddTicks(-1));
            res[n] = new DateRange(this.Stop.Date, this.Stop);
            var d0 = this.Start.Date;
            for (int i = 1; i < n; i++) {
                res[i] = new DateRange(d0.AddDays(i), d0.AddDays(i + 1).AddTicks(-1));
            }
            return res;
        }

        public DateRange[] SplitByMonth() => SplitByDay().GroupBy(t => t.Start.StartOfMonth()).SelectMany(Recombine).Select(t => (DateRange)t).ToArray();
        public DateRange[] SplitByYear() => SplitByMonth().GroupBy(t => t.Start.StartOfYear()).SelectMany(Recombine).Select(t => (DateRange)t).ToArray();


        public DateRange[] SplitByHour() {
            var t = Start.EndOfHour();
            if (t == Stop.EndOfHour()) return new[] { this };
            return (new DateRange(t.AddTicks(1), Stop).SplitByHour()).Prepend(new DateRange(Start, t)).ToArray();
        }
        public DateRange[] SplitByHalfHour() {
            var t = Start.EndOfHalfHour();
            if (t == Stop.EndOfHalfHour()) return new[] { this };
            return (new DateRange(t.AddTicks(1), Stop).SplitByHalfHour()).Prepend(new DateRange(Start, t)).ToArray();
        }

        public DateRange[] SplitByQuarterHour() {
            var t = Start.EndOfQuarterHour();
            if (t == Stop.EndOfQuarterHour()) return new[] { this };
            return (new DateRange(t.AddTicks(1), Stop).SplitByQuarterHour()).Prepend(new DateRange(Start, t)).ToArray();
        }

        public IEnumerable<DateRange> SplitByPoints(IEnumerable<DateTime> pts) {
            pts = new[] { Start }.Concat(pts.Where(t => Contains(t)).OrderBy(t => t)).Append(Stop).ToList();
            return pts.Select((t, ix) => (t, ix)).Join(pts.Skip(1).Select((t, ix) => (t, ix)), t => t.ix, t => t.ix, (start, stop) => new DateRange(start.t, stop.t));
        }


    }


}
