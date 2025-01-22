using System;
using System.Linq;

using YZ;

namespace YZ.Geo {
    public struct Angle {
        const double epsilon = 0.00001;

        public static readonly Angle Zero = new(0);
        public static readonly Angle Deg90 = new(90);
        public static readonly Angle Deg180 = new(180);
        public static readonly Angle Deg270 = new(270);
        public static readonly Angle Pi = Deg180;


        Angle( double deg ) => Degrees = Tools.NormalizeDeg( deg );
        public double Degrees { get; }
        public double Radians => Degrees * Math.PI / 180.0;
        public static Angle FromDegrees( double deg ) => new Angle( deg );
        public static Angle FromRadians( double rad ) => new Angle( rad / Math.PI * 180.0 );
        public static Angle operator -( Angle a, Angle b ) => new Angle( a.Degrees - b.Degrees );
        public static Angle operator +( Angle a, Angle b ) => new Angle( a.Degrees + b.Degrees );
        public static bool operator ==( Angle a, Angle b ) => Math.Abs( a.Degrees - b.Degrees ) <= epsilon;
        public static bool operator !=( Angle a, Angle b ) => Math.Abs( a.Degrees - b.Degrees ) > epsilon;
        public static bool operator <( Angle a, Angle b ) => Math.Abs( a.Degrees ) < Math.Abs( b.Degrees ) - epsilon;
        public static bool operator >( Angle a, Angle b ) => Math.Abs( a.Degrees ) > Math.Abs( b.Degrees ) + epsilon;
        public bool IsSame( Angle b, Angle maxDiff ) => Math.Abs( Diff( this, b ).Degrees ) <= Math.Abs( maxDiff.Degrees );
        public Angle RoundTo( Angle step ) => FromDegrees( Degrees.RoundTo( step.Degrees ) );

        public static Angle Average( params Angle[] a ) => a.Length == 0 ? new Angle( 0 ) : a.Length == 1 ? a[ 0 ] : Average( a.Select( t => t.Radians ).ToArray() );
        public static Angle Average( params double[] a ) => a.Length == 0 ? new Angle( 0 ) : a.Length == 1 ? FromRadians( a[ 0 ] ) : FromRadians( Math.Atan2( a.Sum( Math.Sin ) / a.Length, a.Sum( Math.Cos ) / a.Length ) );
        public static Angle Diff( Angle a, Angle b ) {
            var r = Math.Abs(a.Degrees - b.Degrees);
            return new( r > 180.0 ? 360.0 - r : r );
        }
        public override bool Equals( object that ) => that is Angle a && a == this;
        public override int GetHashCode() => Degrees.GetHashCode();
        public override string ToString() {
            return $"{Degrees:0.000} deg";
        }
    }

}
