using System;
using System.Linq;

using Newtonsoft.Json;

using YZ;

namespace YZ.Geo {
    [Serializable/*, JsonConverter(typeof(GeoCoordJsonConverter))*/]
    public struct GeoCoord {
        public GeoCoord( (double lat, double lon) src ) : this( src.lat, src.lon ) { }

        [JsonConstructor]
        public GeoCoord( string latLon ) : this( Parse( latLon ) ) { }

        public GeoCoord( double lat, double lon ) {
            Lat = lat;
            Lon = lon;
        }

        public readonly double Lat, Lon;

        [JsonIgnore]
        public bool IsValid => !double.IsNaN( Lat ) && !double.IsNaN( Lon ) && !double.IsInfinity( Lat ) && !double.IsInfinity( Lon );
        public static bool operator ==( GeoCoord a, GeoCoord b ) => ( a - b ).Distance < 0.1;
        public static bool operator !=( GeoCoord a, GeoCoord b ) => ( a - b ).Distance >= 0.1;

        public static implicit operator string( GeoCoord a ) => $"{a.Lat:0.0000###}, {a.Lon:0.0000###}";

        public static implicit operator GeoCoord( string a ) => FromString( a );
        public static GeoCoord FromString( string latLon ) => new GeoCoord( latLon );
        public GeoCoord Constraint( GeoCoord? min = null, GeoCoord? max = null ) => new( Lat.Constraint( min?.Lat, max?.Lat ), Lon.Constraint( min?.Lon, max?.Lon ) );

        public static (double lat, double lon) Parse( string latNon ) {
            var t = latNon?.Split(',').Take(2).Select(t => t.Trim().AsDouble()) ?? Array.Empty<double>();
            if ( t.Count() < 2 ) return (0.0, 0.0);
            return (t.First(), t.Last());
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

        public override string ToString() => $"{Lat:0.0000###}, {Lon:0.0000###}";

    }

}
