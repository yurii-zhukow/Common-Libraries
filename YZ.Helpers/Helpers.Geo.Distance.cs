using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace YZ {

    public class GeoDistanceJsonConverter : JsonConverter<GeoDistance> {
        public override GeoDistance Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => GeoDistance.Parse(reader.GetString());
        public override void Write(Utf8JsonWriter writer, GeoDistance distanceValue, JsonSerializerOptions options) => writer.WriteStringValue(distanceValue.ToString());
    }

    [JsonConverter(typeof(GeoDistanceJsonConverter))]
    public readonly record struct GeoDistance : IComparable<GeoDistance> {
        const double epsilon = 0.00001;
        public static readonly GeoDistance Zero = new(0);

        readonly DistanceUnits baseUnits;
        GeoDistance(double v, DistanceUnits srcUnits = DistanceUnits.Meters) => (Meters, baseUnits) = (normalizeTo(v, srcUnits), srcUnits);
        public static GeoDistance FromUnits(double value, DistanceUnits units = DistanceUnits.Meters) => new(value, units);
        public static GeoDistance FromMiles(double miles) => FromUnits(miles, DistanceUnits.Miles);
        public static GeoDistance FromMeters(double meters) => FromUnits(meters, DistanceUnits.Meters);
        public static GeoDistance FromKilometers(double kilometers) => FromUnits(kilometers, DistanceUnits.Kilometers);
        public static GeoDistance FromKm(double km) => FromKilometers(km);

        static double normalizeTo(double valueInUnits, DistanceUnits units) => units.GetEnumAttr(false, v => new NormalizeAttribute(1,1)).NormalizeTo(valueInUnits);
        static double normalizeFrom(double baseUnits, DistanceUnits units) => units.GetEnumAttr(false, v => new NormalizeAttribute(1,1)).NormalizeFrom(baseUnits);

        public readonly double Meters { get; }
        public readonly double Kilometers => normalizeFrom(Meters, DistanceUnits.Kilometers);
        public readonly double Km => Kilometers;
        public readonly double Miles => normalizeFrom(Meters, DistanceUnits.Miles);
        public readonly double Mi => Miles;

        public static implicit operator GeoDistance(double meters) => FromMeters(meters);
        public static implicit operator double(GeoDistance d) => d.Meters;
        public static GeoDistance operator /(GeoDistance a, double b) => FromMeters(a.Meters / b);
        public static Speed operator /(GeoDistance a, TimeSpan b) => Speed.FromMetersPerSecond(a.Meters / b.TotalSeconds);
        public static GeoDistance operator *(GeoDistance a, double b) => FromMeters(a.Meters * b);
        public static GeoDistance operator +(GeoDistance a, GeoDistance b) => FromMeters(a.Meters + b.Meters);
        public static GeoDistance operator -(GeoDistance a, GeoDistance b) => FromMeters(a.Meters - b.Meters);
        public static GeoDistance operator -(GeoDistance a) => FromMeters(-a.Meters);
        public static bool operator >(GeoDistance a, GeoDistance b) => Math.Abs(a.Meters) > Math.Abs(b.Meters) + epsilon;
        public static bool operator <(GeoDistance a, GeoDistance b) => Math.Abs(a.Meters) < Math.Abs(b.Meters) - epsilon;
        public static bool operator >=(GeoDistance a, GeoDistance b) => Math.Abs(a.Meters) >= Math.Abs(b.Meters) - epsilon;
        public static bool operator <=(GeoDistance a, GeoDistance b) => Math.Abs(a.Meters) <= Math.Abs(b.Meters) + epsilon;
        public static bool operator ==(GeoDistance a, GeoDistance b) => Math.Abs(a.Meters - b.Meters) <= epsilon;
        public static bool operator !=(GeoDistance a, GeoDistance b) => Math.Abs(a.Meters - b.Meters) > epsilon;

        public int CompareTo(GeoDistance other) => Math.Abs(Meters).CompareTo(Math.Abs(other.Meters));

        public override bool Equals(object that) => that is GeoDistance d && d == this;
        public override int GetHashCode() => Meters.GetHashCode();
        public override string ToString() => $"{normalizeFrom(Meters, baseUnits):# ##0.###} {baseUnits.GetEnumAttr(false, (v,a) => a.Suffix, v => new SuffixAttribute(""))}".Trim();
        public static GeoDistance Parse(string src) {
            src = src.Replace(" ", "").Trim().ToLower();
            var units = Enum.GetValues<DistanceUnits>().Select(t => (k: t, suffix: t.GetEnumAttr(false, (v,a) => a.Suffix, v => new SuffixAttribute("" )).ToLower())).Where(t => src.EndsWith(t.suffix));
            var u = units.FirstOrDefault((k: DistanceUnits.Meters, suffix: ""));
            return new(src.AsDouble(), u.k);
        }
    }
}

