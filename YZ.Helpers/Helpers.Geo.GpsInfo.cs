using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Newtonsoft.Json;

using YZ;

namespace YZ {
    public readonly record struct GpsInfo( DateTimeOffset Date, GeoCoord Position, Speed Speed, Angle Angle, bool IsValid, bool IsVirtual, string Label = "", TimeSpan Duration = default ) {

        public GpsInfo OffsetTime( TimeSpan offset ) => new( Date + offset, Position, Speed, Angle, IsValid, IsVirtual, Label, Duration );

        public override string ToString() => $"{Date:g} [{Position}] {Speed} | {Angle} | {( IsValid ? "Valid" : "Invalid" )}{( IsVirtual ? " Virtual" : "" )}";

        public static (GeoDistance Distance, TimeSpan Time) operator -( GpsInfo a, GpsInfo b ) => (Distance: a.Position - b.Position, Time: a.Date - b.Date);

        const double deg2rad = Math.PI / 180.0;
        const double rad2deg = 180.0 / Math.PI;

        static DateTimeOffset averageDate( DateTimeOffset a, DateTimeOffset b ) => a > b ? averageDate( b, a ) : a + ( b - a ) / 2.0;
        static DateTimeOffset averageDate( IEnumerable<DateTimeOffset> a ) => a.Any() ? averageDate( a.Min(), a.Max() ) : DateTimeOffset.MinValue;
        public static GpsInfo Average( GpsInfo a, GpsInfo b ) => new( averageDate( a.Date, b.Date ), a.Position & b.Position, ( a.Speed + b.Speed ) / 2.0, Angle.Average( a.Angle, b.Angle ), a.IsValid && b.IsValid, a.IsVirtual || b.IsVirtual );
        public static GpsInfo Average( IEnumerable<GpsInfo> a ) => ( a?.Count() ?? 0 ) > 1 ? new GpsInfo( averageDate( a.Select( t => t.Date ) ), GeoCoord.Average( a.Select( t => t.Position ).ToArray() ), Speed.Average( a.Select( t => t.Speed ) ), Angle.Average( a.Select( t => t.Angle ) ), a.All( t => t.IsValid ), a.Any( t => t.IsVirtual ), a.Select( t => t.Label ).Distinct().ToString( "; " ), ( a.Max( t => t.Duration ) + a.Min( t => t.Duration ) ) / 2.0 ) : a.FirstOrDefault();
        public GpsInfo Copy( GeoCoord? position = null, double? speed = null, Angle? angle = null, bool? isValid = null, bool? isVirtual = null, DateTime? date = null, string label = null, TimeSpan? duration = null ) => new( date ?? Date, position ?? Position, speed ?? Speed, angle ?? Angle, isValid ?? IsValid, isVirtual ?? IsVirtual, label ?? Label, duration ?? Duration );

        public static GpsInfo Approximate( [NotNull] GpsInfo[] a, DateTime t ) {
            if ( a == null ) throw new ArgumentNullException( nameof( a ) );

            var l = a.Length;
            if ( l == 0 ) throw new ArgumentOutOfRangeException( nameof( a ) );
            if ( l == 1 ) return a[ 0 ];
            if ( t <= a[ 0 ].Date ) return a[ 0 ];
            if ( t >= a[ l - 1 ].Date ) return a[ l - 1 ];

            var (ix, p) = a.Nearest( t, t => t.Date, ( a, b ) => ( a - b ).Duration() );
            if ( ix < 0 ) new ArgumentOutOfRangeException( nameof( a ) );
            if ( ix == 0 && t <= p.Date ) return a[ 0 ];
            if ( ix >= l - 1 && t >= p.Date ) return p;

            if ( ( p.Date - t ).Duration().TotalSeconds < 1 ) return a[ ix ];
            var (left, right) = p.Date > t ? (a[ ix - 1 ], p) : (p, a[ ix + 1 ]);
            var offs = (t - left.Date) / (right.Date - left.Date);
            return approximate( left, right, offs );

        }

        /// <summary>
        /// approximates position according to speeds between two points
        /// </summary>
        /// <param name="a">left point (A)</param>
        /// <param name="b">right point (B)</param>
        /// <param name="offs">normalized offset from A (offs=0) to B (offs=1)</param>
        /// <returns>approximated value between A and B</returns>
        static GpsInfo approximate( GpsInfo a, GpsInfo b, double offs ) {

            if ( offs <= 0 ) return a;
            if ( offs >= 1 ) return b;
            var offsT = (b.Date - a.Date)/2;
            var spdDelta = b.Speed - a.Speed;
            var spdDeltaA = spdDelta * offs;
            var spdDeltaB = spdDelta - spdDeltaA;

            var spdMid = a.Speed + spdDelta * offs;
            var pthA = (a.Speed + spdDeltaA / 2.0) * offsT;
            var pthB = (spdMid + spdDeltaB / 2.0) * (1.0 - offs);
            var label = new[]{a.Label,b.Label }.Where(t=>!string.IsNullOrEmpty(t)).ToString(" - ");
            var dur = ( b.Date - a.Date ) * offs;
            return new( a.Date + dur, GeoCoord.Approximate( a.Position, b.Position, offs ), spdMid, Angle.Average( a.Angle, b.Angle ), a.IsValid && b.IsValid, true, label, dur );
        }

        public static IEnumerable<GpsInfo> SplitBy( GpsInfo a, GpsInfo b, TimeSpan t ) {
            if ( a.Date > b.Date ) (a, b) = (b, a);
            var dur = b.Date - a.Date;
            if ( dur <= t ) return new[] { a, b };
            var parts = Math.Round(dur / t);
            var partsN = (int)parts;
            t = dur / parts;

            var res = new GpsInfo[partsN + 1];
            res[ 0 ] = a;
            res[ partsN ] = b;

            for ( var i = 1; i < partsN; i++ ) res[ i ] = approximate( a, b, i / parts );
            return res;
        }

    }

}
