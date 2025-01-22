using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Newtonsoft.Json;

using YZ;

namespace YZ.Geo {
    public struct GpsInfo {
        public readonly GeoCoord Position;
        public readonly double Speed;
        public readonly double Acceleration;

        [JsonProperty(nameof(Angle))]
        public readonly double AngleDeg;
        [JsonIgnore]
        public readonly Angle Angle;

        public readonly DateTime Date;
        [JsonProperty("Valid")]
        public readonly bool IsValid;
        [JsonProperty("Virtual")]
        public readonly bool IsVirtual;

        [JsonIgnore]
        public readonly string Label;
        [JsonIgnore]
        public readonly TimeSpan Duration;

        public GpsInfo( GeoCoord position, double speed, double acceleration, Angle angle, bool isValid, bool isVirtual, DateTime? date = null, string label = "", TimeSpan duration = default ) : this() {
            Position = position;
            Speed = speed;
            Angle = angle;
            AngleDeg = angle.Degrees;
            Acceleration = acceleration;
            IsValid = isValid;
            IsVirtual = isVirtual;
            Date = date ?? DateTime.Now;
            Label = label;
            Duration = duration;
        }
        public GpsInfo OffsetTime( TimeSpan offset ) => new( Position, Speed, Acceleration, Angle, IsValid, IsVirtual, Date + offset, Label, Duration );

        public override string ToString() => $"{Date:g} [{Position}] {Speed:0.0} km/h | {AngleDeg} deg | {( IsValid ? "Valid" : "Invalid" )}{( IsVirtual ? " Virtual" : "" )}";

        public static (GeoDistance Distance, TimeSpan Time) operator -( GpsInfo a, GpsInfo b ) => (Distance: a.Position - b.Position, Time: a.Date - b.Date);

        const double deg2rad = Math.PI / 180.0;
        const double rad2deg = 180.0 / Math.PI;

        static DateTime averageDate( DateTime a, DateTime b ) => a > b ? averageDate( b, a ) : a + ( b - a ) / 2.0;
        public static GpsInfo Average( GpsInfo a, GpsInfo b ) => new GpsInfo( a.Position & b.Position, ( a.Speed + b.Speed ) / 2.0, ( a.Acceleration + b.Acceleration ) / 2.0, Angle.Average( a.Angle, b.Angle ), a.IsValid && b.IsValid, a.IsVirtual || b.IsVirtual, averageDate( a.Date, b.Date ) );
        public static GpsInfo Average( GpsInfo[] a ) => ( a?.Length ?? 0 ) > 1 ? new GpsInfo( GeoCoord.Average( a.Select( t => t.Position ).ToArray() ), a.Sum( t => t.Speed ) / a.Length, a.Sum( t => t.Acceleration ) / a.Length, a.OrderByDescending( t => t.Date ).First().Angle, a.All( t => t.IsValid ), a.Any( t => t.IsVirtual ), a.Min( t => t.Date ), a.ToString( "; ", t => t.Label ), a.Max( t => t.Date ) - a.Min( t => t.Date ) ) : ( a?.Length ?? 0 ) == 1 ? a[ 0 ] : new GpsInfo();
        public GpsInfo Copy( GeoCoord? position = null, double? speed = null, double? acceleration = null, Angle? angle = null, bool? isValid = null, bool? isVirtual = null, DateTime? date = null, string label = null, TimeSpan? duration = null ) => new( position ?? Position, speed ?? Speed, acceleration ?? Acceleration, angle ?? Angle, isValid ?? IsValid, isVirtual ?? IsVirtual, date ?? Date, label ?? Label, duration ?? Duration );

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
            var spdDelta = b.Speed - a.Speed;
            var spdDeltaA = spdDelta * offs;
            var spdDeltaB = spdDelta - spdDeltaA;

            var spdMid = a.Speed + spdDelta * offs;
            var pthA = (a.Speed + spdDeltaA / 2.0) * offs;
            var pthB = (spdMid + spdDeltaB / 2.0) * (1.0 - offs);
            var pth = pthB + pthA;
            var asp = pth > 0 ? pthA / pth : 0;

            return new( GeoCoord.Approximate( a.Position, b.Position, asp ), spdMid, a.Acceleration + ( b.Acceleration - a.Acceleration ) * offs, Angle.Average( a.Angle, b.Angle ), a.IsValid && b.IsValid, true, a.Date + ( b.Date - a.Date ) * offs );
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
