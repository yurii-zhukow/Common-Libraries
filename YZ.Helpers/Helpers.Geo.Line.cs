using System;

using Newtonsoft.Json;

namespace YZ {

    [Serializable]
    [method: JsonConstructor]
    public readonly struct GeoLine( GeoCoord StartPoint, GeoCoord EndPoint ) : IEquatable<GeoLine> {

        [JsonIgnore]
        public readonly GeoDistance Length => Offset.Distance;
        [JsonIgnore]
        public readonly double Meters => Length.Meters;
        [JsonIgnore]
        public readonly double Km => Length.Meters / 1000.0;
        [JsonIgnore]
        public readonly double Mi => Length.Meters / 1609.34;
        [JsonIgnore]
        public readonly GeoOffset Offset => EndPoint - StartPoint;
        [JsonIgnore]
        public readonly Angle Angle => Offset.Target;


        /// <summary>
        /// Прямая через две точки: A·x + B·y = C, где x = Lon, y = Lat
        /// </summary>
        /// <returns>Коэффициенты A,B,C</returns>
        public readonly (double A, double B, double C) GetABC() {
            var a = EndPoint.Lat - StartPoint.Lat;      // A = y2 - y1
            var b = StartPoint.Lon - EndPoint.Lon;      // B = x1 - x2
            var c = a * StartPoint.Lon + b * StartPoint.Lat; // C = A·x1 + B·y1
            return (a, b, c);
        }
       
        public GeoCoord GetProjection( GeoCoord p ) {
            var m = (EndPoint.Lon - StartPoint.Lon) / (EndPoint.Lat - StartPoint.Lat);
            var b = StartPoint.Lon - m * StartPoint.Lat;
            var lat = (m * p.Lon + p.Lat - m * b) / (m * m + 1);
            var lon = (m * m * p.Lon + m * p.Lat + b) / (m * m + 1);
            return new( lat, lon );
        }
        public readonly bool IsProjected( GeoCoord p ) {
            var res = GetProjection(p);
            if ( !res.IsValid ) return false;
            return StartPoint - res < Length && EndPoint - res < Length;
        }
        public readonly GeoDistance DistanceTo( GeoCoord p ) {
            var proj = GetProjection(p);
            var isProj = proj.IsValid && StartPoint - proj < Length && EndPoint - proj < Length;
            if ( isProj ) return p - proj;
            return Math.Min( ( StartPoint - p ).Distance, ( EndPoint - p ).Distance );
        }

        public GeoLine Translate( Angle a, GeoDistance d ) => Translate( new( a, d ) );
        public GeoLine Translate( GeoOffset offs ) => new( StartPoint + offs, EndPoint + offs );
        public GeoLine Scale( double b ) => new GeoLine( StartPoint, new GeoCoord( ( EndPoint.Lat - StartPoint.Lat ) * b + StartPoint.Lat, ( EndPoint.Lon - StartPoint.Lon ) * b + StartPoint.Lon ) );
        public GeoCoord Intersect( GeoLine b ) => Tools.Intersect( this, b );

        public static GeoLine operator /( GeoLine a, double b ) => a.Scale( 1 / b );
        public static GeoLine operator *( GeoLine a, double b ) => a.Scale( b );
        public static GeoLine operator +( GeoLine a, GeoOffset b ) => a.Translate( b );
        public static GeoLine operator -( GeoLine a, GeoOffset b ) => a.Translate( -b );
        public static bool operator ==( GeoLine a, GeoLine b ) => a.Equals( b );
        public static bool operator !=( GeoLine a, GeoLine b ) => !a.Equals( b );

        public static GeoCoord operator &( GeoLine a, GeoLine b ) => a.Intersect( b );

        public override bool Equals( object obj ) => obj is GeoLine d && d.Equals( this );
        public bool Equals( GeoLine other ) => StartPoint == other.StartPoint && EndPoint == other.EndPoint;
        public override int GetHashCode() => HashCode.Combine( StartPoint, EndPoint );

        public override string ToString() {
            return $"start=[{StartPoint}]; end=[{EndPoint}]; tgt={Angle}; len={Length}; offs=[{Offset}];";
        }


    }

}
