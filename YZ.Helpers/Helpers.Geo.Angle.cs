using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

using YZ;

namespace YZ {
    public class AngleJsonConverter : JsonConverter<Angle> {
        public override Angle Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options ) => Angle.Parse( reader.GetString() );
        public override void Write( Utf8JsonWriter writer, Angle angleValue, JsonSerializerOptions options ) => writer.WriteStringValue( angleValue.ToString() );
    }

    [JsonConverter( typeof( AngleJsonConverter ) )]

    public struct Angle {
        const double epsilon = 0.00001;

        public static readonly Angle Zero = new(0);
        public static readonly Angle Deg90 = new(90);
        public static readonly Angle Deg180 = new(180);
        public static readonly Angle Deg270 = new(270);
        public static readonly Angle Pi = Deg180;
        public static readonly Angle Epsilon = new(epsilon);


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
        public bool IsSame( Angle b, Angle? maxDiff = null ) => Math.Abs( Diff( this, b ).Degrees ) <= Math.Abs( maxDiff?.Degrees ?? epsilon );
        public Angle RoundTo( Angle step ) => FromDegrees( Degrees.RoundTo( step.Degrees ) );

        public static Angle Average( params Angle[] a ) => a.Length == 0 ? new Angle( 0 ) : a.Length == 1 ? a[ 0 ] : Average( a.Select( t => t.Radians ).ToArray() );
        public static Angle Average( params double[] a ) => a.Length == 0 ? new Angle( 0 ) : a.Length == 1 ? FromRadians( a[ 0 ] ) : FromRadians( Math.Atan2( a.Sum( Math.Sin ) / a.Length, a.Sum( Math.Cos ) / a.Length ) );
        public static Angle Diff( Angle a, Angle b ) {
            var r = Math.Abs(a.Degrees - b.Degrees);
            return new( r > 180.0 ? 360.0 - r : r );
        }
        public override bool Equals( object that ) => that is Angle a && a == this;
        public override int GetHashCode() => Degrees.GetHashCode();
        public override string ToString() => $"{Degrees:# ##0.###} deg".Trim();

        public static Angle Parse( string s ) {
            var units = s.EndsWith("deg") ? "deg" : "rad";
            var v = s.AsDouble();
            return s.EndsWith( "deg" ) ? FromDegrees( v ) : FromRadians( v );
        }
    }

}
