using System;

namespace YZ {

    public static class Tools {
        const double DEG2RAD = Math.PI / 180.0;
        const double EPSILON = 0.000000001;
        static double deg2rad( double deg ) => deg * DEG2RAD;
        static double rad2deg( double rad ) => rad / DEG2RAD;
        static double dist2coord( double km ) => rad2deg( km / Helpers.EARTH_RADIUS );

        public static double NormalizeDeg( double deg ) {
            var t = Math.Abs(deg) / 360.0;
            t = Math.Sign( deg ) * ( t - Math.Floor( t ) );
            t = t <= -0.5 ? t + 1 : t > 0.5 ? t - 1 : t;
            return t * 360.0;
        }

        public static GeoDistance ToEquator( double lat ) {
            lat = NormalizeDeg( lat );
            var dLat = deg2rad(lat);  // deg2rad below
            var sinLat2 = Math.Sin(dLat / 2);
            var x = sinLat2 * sinLat2;
            var c = 2 * Math.Atan2(Math.Sqrt(x), Math.Sqrt(1 - x));
            var d = Helpers.EARTH_RADIUS * c; // Distance in km
            return GeoDistance.FromKm( Math.Sign( lat ) * d );
        }

        public static GeoDistance ToGreenwich( double lon ) {
            lon = NormalizeDeg( lon );
            var dLon2 = deg2rad(lon) / 2;
            var sinLon2 = Math.Sin(dLon2);
            var x = sinLon2 * sinLon2;
            var c = 2 * Math.Atan2(Math.Sqrt(x), Math.Sqrt(1 - x));
            var d = Helpers.EARTH_RADIUS * c; // Distance in km
            return GeoDistance.FromKm( Math.Sign( lon ) * d );
        }

        public static GeoCoord Translate( GeoCoord coord, GeoOffset offs ) => new( coord.Lat + dist2coord( offs.Lat.Km ), coord.Lon + dist2coord( offs.Lon.Km ) );

        public static GeoCoord Intersect( GeoLine a, GeoLine b ) {
            var (A1, B1, C1) = a.GetABC();
            var (A2, B2, C2) = b.GetABC();
            var det = A1 * B2 - A2 * B1;
            return Math.Abs( det ) < EPSILON ? a.EndPoint & b.EndPoint : new( ( A1 * C2 - A2 * C1 ) / det, ( B2 * C1 - B1 * C2 ) / det );
        }
    }
}