using System;

namespace YZ {
    public struct GeoOffset : IComparable<GeoOffset> {

        public GeoOffset( GeoDistance lat, GeoDistance lon ) {
            Lat = lat;
            Lon = lon;
        }
        public GeoOffset( GeoCoord offs ) {
            Lat = Tools.ToEquator( offs.Lat );
            Lon = Tools.ToGreenwich( offs.Lon );
        }
        public GeoOffset( Angle target, GeoDistance dist ) {
            initialTarget = target;

            Lat = Math.Sin( target.Radians ) * dist;
            Lon = Math.Cos( target.Radians ) * dist * 1.44;
        }
        public readonly GeoDistance Lat;
        public readonly GeoDistance Lon;

        Angle initialTarget = Angle.Zero;
        public readonly Angle Target => Angle.FromRadians( Math.Atan2( Lat.Km, Lon.Km / 1.44 ) );
        public readonly GeoDistance Distance => Math.Sqrt( Lat * Lat + Lon * Lon );

        public static GeoOffset operator +( GeoOffset a, GeoOffset b ) => new GeoOffset( a.Lat + b.Lat, a.Lon + b.Lon );
        public static GeoOffset operator +( GeoOffset a, Angle b ) => new GeoOffset( a.Target + b, a.Distance );
        public static GeoOffset operator +( GeoOffset a, GeoDistance b ) => new GeoOffset( a.Target, a.Distance + b );
        public static GeoOffset operator -( GeoOffset a ) => new GeoOffset( a.Target + Angle.FromDegrees( 180 ), a.Distance );
        public static GeoOffset operator *( GeoOffset a, double b ) => new GeoOffset( a.Target, a.Distance * b );
        public static GeoOffset operator /( GeoOffset a, double b ) => new GeoOffset( a.Target, a.Distance / b );
        public static GeoOffset operator *( GeoOffset a, (double xLat, double xLon) b ) => new GeoOffset( a.Lat * b.xLat, a.Lon * b.xLon );


        public static implicit operator GeoDistance( GeoOffset a ) => a.Distance;

        public override string ToString() {
            return $"lat={Lat}; lon={Lon}; tgt={Target}; d={Distance};{( initialTarget == Target ? "" : $" (init tgt={initialTarget})" )}";
        }

        public int CompareTo( GeoOffset other ) => Distance.CompareTo( other.Distance );
    }

}
