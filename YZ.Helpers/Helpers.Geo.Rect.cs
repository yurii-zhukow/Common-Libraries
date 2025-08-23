using System;

using Newtonsoft.Json;

namespace YZ {
    [Serializable]
    public struct GeoRect : IEquatable<GeoRect> {
        [JsonConstructor]
        public GeoRect( GeoCoord startPoint, GeoCoord endPoint ) {
            SouthWest = new( Math.Min( startPoint.Lat, endPoint.Lat ), Math.Min( startPoint.Lon, endPoint.Lon ) );
            NorthEast = new( Math.Max( startPoint.Lat, endPoint.Lat ), Math.Max( startPoint.Lon, endPoint.Lon ) );
        }
        public readonly GeoCoord SouthWest;
        public readonly GeoCoord NorthEast;

        [JsonIgnore]
        public GeoOffset Offset => NorthEast - SouthWest;

        [JsonIgnore]
        public GeoDistance Height => Offset.Lat;
        [JsonIgnore]
        public GeoDistance Width => Offset.Lon;

        [JsonIgnore]
        public double SquareMeters => Math.Abs( Height.Meters * Width.Meters );
        [JsonIgnore]
        public double SquareKm => Math.Abs( Height.Km * Width.Km );
        [JsonIgnore]
        public double SquareMi => Math.Abs( Height.Mi * Width.Mi );

        public GeoRect Combine( GeoRect other ) => new( other.SouthWest.Constraint( max: SouthWest ), other.NorthEast.Constraint( min: NorthEast ) );
        public GeoRect ExpandTo( GeoCoord pt ) => new( SouthWest.Constraint( max: pt ), NorthEast.Constraint( min: pt ) );
        public GeoRect Expand( GeoDistance d ) => new( SouthWest + new GeoOffset( -d, -d ), NorthEast + new GeoOffset( d, d ) );
        public GeoRect Constraint( GeoOffset? min = null, GeoOffset? max = null, bool keepAspect = true ) {
            var res = min != null ? ConstraintMin(min.Value, keepAspect) : this;
            res = max != null ? res.ConstraintMax( max.Value, keepAspect ) : res;
            return res;
        }
        public GeoCoord Center => SouthWest + Offset / 2;
        public GeoRect Scale( double xLat, double xLon ) {
            var offs = Offset * (xLat, xLon);
            return new GeoRect( Center - offs / 2, Center + offs / 2 );
        }

        public static GeoRect operator *( GeoRect a, (double xLat, double xLon) b ) => a.Scale( b.xLat, b.xLon );


        public GeoRect ConstraintMin( GeoOffset min, bool keepAspect = true ) {
            double ax = min.Lon / Width, ay = min.Lat / Height;
            if ( keepAspect ) {
                ax = Math.Max( ax, ay );
                ay = ax;
            }
            return ax > 1 || ay > 1 ? Scale( ax.Constraint( 1 ), ay.Constraint( 1 ) ) : this;

        }
        public GeoRect ConstraintMax( GeoOffset max, bool keepAspect = true ) {

            double ax = max.Lon / Width, ay = max.Lat / Height;
            if ( keepAspect ) {
                ax = Math.Min( ax, ay );
                ay = ax;
            }
            return ax < 1 || ay < 1 ? Scale( ax.Constraint( max: 1 ), ay.Constraint( max: 1 ) ) : this;

        }


        public override bool Equals( object obj ) => obj is GeoRect d && d.Equals( this );
        public bool Equals( GeoRect other ) => SouthWest == other.SouthWest && NorthEast == other.NorthEast;
        public override int GetHashCode() => HashCode.Combine( SouthWest, NorthEast );

        public override string ToString() {
            return $"start=[{SouthWest}]; end=[{NorthEast}]; offs=[{Offset}];";
        }
        public static readonly GeoRect Empty = new(new GeoCoord(), new GeoCoord());

    }

}
