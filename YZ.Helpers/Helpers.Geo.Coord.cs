using System;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

using YZ;

namespace YZ {

    public class GeoCoordJsonConverter : JsonConverter<GeoCoord> {
        public override GeoCoord Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options ) => GeoCoord.Parse( reader.GetString() );
        public override void Write( Utf8JsonWriter writer, GeoCoord angleValue, JsonSerializerOptions options ) => writer.WriteStringValue( angleValue.ToString() );
    }

    [JsonConverter( typeof( GeoCoordJsonConverter ) )]
    public readonly struct GeoCoord( double lat, double lon ) {
        public GeoCoord( (double lat, double lon) src ) : this( src.lat, src.lon ) { }

        public readonly double Lat = lat, Lon = lon;

        [JsonIgnore]
        public bool IsValid => !double.IsNaN( Lat ) && !double.IsNaN( Lon ) && !double.IsInfinity( Lat ) && !double.IsInfinity( Lon );
        public static bool operator ==( GeoCoord a, GeoCoord b ) => ( a - b ).Distance < 0.1;
        public static bool operator !=( GeoCoord a, GeoCoord b ) => ( a - b ).Distance >= 0.1;
        static string v2s( double v ) => v.ToString( "#00.0000###", CultureInfo.InvariantCulture );

        public static implicit operator string( GeoCoord a ) => $"{v2s( a.Lat )}, {v2s( a.Lon )}";

        public static implicit operator GeoCoord( string a ) => Parse( a );
        public static implicit operator GeoCoord( (double lat, double lon) src ) => new( src );
        public GeoCoord Constraint( GeoCoord? min = null, GeoCoord? max = null ) => new( Lat.Constraint( min?.Lat, max?.Lat ), Lon.Constraint( min?.Lon, max?.Lon ) );

        public static double Parse( string v, params string[] negSymbols ) {
            var neg = negSymbols.Any(  v.Contains  ) ;
            var r = v.AsDouble();
            return neg? -r : r;
        }
        public static GeoCoord Parse( string latNon ) {
            var t = latNon?.Split(',').Take(2).Select(t => t.Trim()) ?? [];
            if ( t.Count() < 2 ) return new( 0.0, 0.0 );

            return new( Parse( t.First(), "S", "s" ), Parse( t.Last(), "W", "w" ) );
        }

        public static GeoCoord operator +( GeoCoord coord, GeoOffset offs ) => Tools.Translate( coord, offs );
        public static GeoCoord operator -( GeoCoord coord, GeoOffset offs ) => Tools.Translate( coord, -offs );
        public static GeoOffset operator -( GeoCoord a, GeoCoord b ) => new( new GeoCoord( a.Lat - b.Lat, a.Lon - b.Lon ) );

        public static GeoCoord operator &( GeoCoord a, GeoCoord b ) => new( ( a.Lat + b.Lat ) / 2.0, ( a.Lon + b.Lon ) / 2.0 );
        public static GeoCoord Average( GeoCoord[] a ) => a?.Length == 0 ? new() : new( a.Sum( t => t.Lat ) / a.Length, a.Sum( t => t.Lon ) / a.Length );
        public static GeoCoord Approximate( GeoCoord a, GeoCoord b, double offs ) {
            var lat = a.Lat + (b.Lat - a.Lat) * offs;
            var lon = a.Lon + (b.Lon - a.Lon) * offs;
            if ( double.IsNaN( lat ) || double.IsNaN( lon ) ) return a;
            return new( lat, lon );
        }

        public override readonly string ToString() => $"{v2s( Lat )}, {v2s( Lon )}";

        public override readonly bool Equals( object that ) => that is GeoCoord a && a == this;
        public override readonly int GetHashCode() => HashCode.Combine( Lat, Lon );
    }

}
