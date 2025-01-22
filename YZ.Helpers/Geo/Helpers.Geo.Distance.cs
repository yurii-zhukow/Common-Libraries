using System;

using Newtonsoft.Json;

namespace YZ.Geo {


    public class GeoDistanceJsonConverter : JsonConverter {
        public override bool CanConvert( Type objectType ) => objectType == typeof( GeoDistance );
        public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer ) => GeoDistance.FromMeters( reader.Value.SafeCall( Convert.ToDouble, 0.0 ) );
        public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer ) => writer.WriteValue( value is GeoDistance g ? g.Meters : 0.0 );
        public override bool CanRead => true;
        public override bool CanWrite => true;
    }


    [Serializable, JsonConverter( typeof( GeoDistanceJsonConverter ) )]
    public struct GeoDistance : IComparable<GeoDistance> {
        [JsonConstructor]
        GeoDistance( double meters ) => Meters = meters;
        public readonly double Meters;
        [JsonIgnore]
        public readonly double Km => Meters / 1000.0;
        [JsonIgnore]
        public readonly double Mi => Meters / 1609.34;

        public static GeoDistance operator /( GeoDistance a, double b ) => new( a.Meters / b );
        public static GeoDistance operator *( GeoDistance a, double b ) => new( a.Meters * b );
        public static GeoDistance operator +( GeoDistance a, GeoDistance b ) => new( a.Meters + b.Meters );
        public static GeoDistance operator -( GeoDistance a, GeoDistance b ) => new( a.Meters - b.Meters );
        public static GeoDistance operator -( GeoDistance a ) => new( -a.Meters );

        public static bool operator >( GeoDistance a, GeoDistance b ) => Math.Abs( a.Meters ) > Math.Abs( b.Meters );
        public static bool operator <( GeoDistance a, GeoDistance b ) => Math.Abs( a.Meters ) < Math.Abs( b.Meters );
        public static bool operator >=( GeoDistance a, GeoDistance b ) => Math.Abs( a.Meters ) >= Math.Abs( b.Meters );
        public static bool operator <=( GeoDistance a, GeoDistance b ) => Math.Abs( a.Meters ) <= Math.Abs( b.Meters );
        public static bool operator ==( GeoDistance a, GeoDistance b ) => Math.Abs( a.Meters ) == Math.Abs( b.Meters );
        public static bool operator !=( GeoDistance a, GeoDistance b ) => Math.Abs( a.Meters ) != Math.Abs( b.Meters );
        public static GeoDistance FromMeters( double meters ) => new GeoDistance( meters );
        public static GeoDistance FromKm( double km ) => new GeoDistance( km * 1000.0 );
        public static readonly GeoDistance Zero = new GeoDistance(0);

        public int CompareTo( GeoDistance other ) => Math.Abs( Meters ).CompareTo( Math.Abs( other.Meters ) );

        public static implicit operator double( GeoDistance d ) => d.Meters;
        public static implicit operator GeoDistance( double meters ) => new GeoDistance( meters );

        public override bool Equals( object obj ) => obj is GeoDistance d && d.Meters == Meters;

        public override int GetHashCode() => Meters.GetHashCode();
        public override string ToString() {
            return $"{Meters:0.0} m";
        }

    }

}
