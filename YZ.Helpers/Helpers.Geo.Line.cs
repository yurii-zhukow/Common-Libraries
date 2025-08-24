using System;

using Newtonsoft.Json;

namespace YZ {

    [Serializable]
    [method: JsonConstructor]
    public readonly record struct GeoLine( GeoCoord Start, GeoCoord End ) : IEquatable<GeoLine> {

        [JsonIgnore]
        public readonly GeoDistance Length => Offset.Distance;
        [JsonIgnore]
        public readonly double Meters => Length.Meters;
        [JsonIgnore]
        public readonly double Km => Length.Meters / 1000.0;
        [JsonIgnore]
        public readonly double Mi => Length.Meters / 1609.34;
        [JsonIgnore]
        public readonly GeoOffset Offset => End - Start;
        [JsonIgnore]
        public readonly Angle Angle => Offset.Target;


        /// <summary>
        /// Прямая через две точки: A·x + B·y = C, где x = Lon, y = Lat
        /// </summary>
        /// <returns>Коэффициенты A,B,C</returns>
        public readonly (double A, double B, double C) GetABC() {
            var a = End.Lat - Start.Lat;      // A = y2 - y1
            var b = Start.Lon - End.Lon;      // B = x1 - x2
            var c = a * Start.Lon + b * Start.Lat; // C = A·x1 + B·y1
            return (a, b, c);
        }

        public GeoCoord GetProjection( GeoCoord p ) {
            var m = (End.Lon - Start.Lon) / (End.Lat - Start.Lat);
            var b = Start.Lon - m * Start.Lat;
            var lat = (m * p.Lon + p.Lat - m * b) / (m * m + 1);
            var lon = (m * m * p.Lon + m * p.Lat + b) / (m * m + 1);
            return new( lat, lon );
        }
        public readonly bool IsProjected( GeoCoord p ) {
            var res = GetProjection(p);
            if ( !res.IsValid ) return false;
            return Start - res < Length && End - res < Length;
        }
        public readonly GeoDistance DistanceTo( GeoCoord p ) {
            var proj = GetProjection(p);
            var isProj = proj.IsValid && Start - proj < Length && End - proj < Length;
            if ( isProj ) return p - proj;
            return Math.Min( ( Start - p ).Distance, ( End - p ).Distance );
        }

        public GeoLine Translate( Angle a, GeoDistance d ) => Translate( new( a, d ) );
        public GeoLine Translate( GeoOffset offs ) => new( Start + offs, End + offs );
        public GeoLine Scale( double b ) => new GeoLine( Start, new GeoCoord( ( End.Lat - Start.Lat ) * b + Start.Lat, ( End.Lon - Start.Lon ) * b + Start.Lon ) );
        public GeoCoord Intersect( GeoLine b ) => Tools.Intersect( this, b );

        public static GeoLine operator /( GeoLine a, double b ) => a.Scale( 1 / b );
        public static GeoLine operator *( GeoLine a, double b ) => a.Scale( b );
        public static GeoLine operator +( GeoLine a, GeoOffset b ) => a.Translate( b );
        public static GeoLine operator -( GeoLine a, GeoOffset b ) => a.Translate( -b );
        //public static bool operator ==( GeoLine a, GeoLine b ) => a.Equals( b );
        //public static bool operator !=( GeoLine a, GeoLine b ) => !a.Equals( b );

        public static GeoCoord operator &( GeoLine a, GeoLine b ) => a.Intersect( b );

        //public override bool Equals( object obj ) => obj is GeoLine d && d.Equals( this );
        public readonly bool Equals( GeoLine other ) => Start.Equals( other.Start ) && End.Equals( other.End );
        public override int GetHashCode() => HashCode.Combine( Start, End );

        public override string ToString() {
            return $"start=[{Start}]; end=[{End}]; tgt={Angle}; len={Length}; offs=[{Offset}];";
        }


    }

}
