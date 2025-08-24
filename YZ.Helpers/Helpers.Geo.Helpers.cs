using System;
using System.Collections.Generic;
using System.Linq;

using YZ;

namespace YZ {
    public static partial class Helpers {

        /// <summary>
        /// Radius of the Earth in Km
        /// </summary>
        public const double EARTH_RADIUS = 6378.137;
        public const double LATLON_TO_KM = 111.31949079327357264771338267056;
        public const double KM_TO_LATLON = 1 / LATLON_TO_KM;



        static IEnumerable<GeoCoord> intersectAndCut( GeoLine a, GeoLine b ) {
            if ( a.Angle == b.Angle ) return new[] { a.End, b.Start };
            var x = Tools.Intersect(a, b);

            if ( a.Start - x > a.Length || a.End - x > a.Length || b.Start - x > b.Length || b.End - x > b.Length ) return new[] { a.End, b.Start };

            //return new[] { a.End, b.Start };
            return [x];
        }

        public static GeoDistance Sum( this IEnumerable<GeoDistance> distances ) => distances?.Any() == true ? GeoDistance.FromMeters( distances.Select( t => t.Meters ).Sum() ) : GeoDistance.Zero;
        public static GeoDistance Sum<T>( this IEnumerable<T> src, Func<T, GeoDistance> field ) => src.Select( t => field( t ) ).ToList().Sum();

        public static IEnumerable<GeoCoord> MakePath( this IEnumerable<GeoLine> src ) {
            if ( src?.Any() != true ) return Enumerable.Empty<GeoCoord>();
            var first = src.First();
            if ( src.Count() == 1 ) return new[] { first.Start, first.End };
            var last = src.Last();
            var inner = src.SkipLast(1).Zip(src.Skip(1)).SelectMany(t => intersectAndCut(t.First, t.Second));
            return new[] { first.Start }.Concat( inner ).Append( last.End ).ToList();
        }

        public static GeoRect GetViewport( this IEnumerable<GeoCoord> all ) {
            if ( all?.Any() != true ) return GeoRect.Empty;
            var first = all.First();
            var res = all.Skip(1).Aggregate(new GeoRect(first, first), (acc, t) => acc.ExpandTo(t));
            return res.Expand( GeoDistance.FromMeters( 100 ) );
        }
        public static GeoRect GetViewport( this IEnumerable<GeoRect> all ) {
            if ( all?.Any() != true ) return GeoRect.Empty;
            var first = all.First();
            var res = all.Skip(1).Aggregate(first, (acc, t) => acc.Combine(t));
            return res;
        }

    }
}
