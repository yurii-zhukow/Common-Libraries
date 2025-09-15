using System;
using System.Collections;
using System.Collections.Generic;
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
        enum AngleUnits { Degrees, Radians }
        const double epsilon = 0.00001;

        public static readonly Angle Zero = new(0);
        public static readonly Angle Deg90 = new(90);
        public static readonly Angle Deg180 = new(180);
        public static readonly Angle Deg270 = new(270);
        public static readonly Angle Pi = Deg180;
        public static readonly Angle Epsilon = new(epsilon);
        readonly AngleUnits baseUnits = AngleUnits.Degrees;

        Angle( double deg, AngleUnits baseUnits = AngleUnits.Degrees ) {
            Degrees = Tools.NormalizeDeg( deg );
            this.baseUnits = baseUnits;
        }

        public readonly double Degrees { get; }
        public readonly double Radians => d2r( Degrees );
        static double r2d( double rad ) => rad * 180.0 / Math.PI;
        static double d2r( double deg ) => deg * Math.PI / 180.0;
        static Angle fromBaseUntis( double deg, AngleUnits baseUnits = AngleUnits.Degrees ) => new( deg, baseUnits );
        public static Angle FromDegrees( double deg ) => fromBaseUntis( deg );
        public static Angle FromRadians( double rad ) => fromBaseUntis( rad / Math.PI * 180.0, AngleUnits.Radians );
        public static Angle operator -( Angle a, Angle b ) => new( a.Degrees - b.Degrees, a.baseUnits );
        public static Angle operator +( Angle a, Angle b ) => new( a.Degrees + b.Degrees, a.baseUnits );
        public static bool operator ==( Angle a, Angle b ) => Math.Abs( a.Degrees - b.Degrees ) <= epsilon;
        public static bool operator !=( Angle a, Angle b ) => Math.Abs( a.Degrees - b.Degrees ) > epsilon;
        public static bool operator <( Angle a, Angle b ) => Math.Abs( a.Degrees ) < Math.Abs( b.Degrees ) - epsilon;
        public static bool operator >( Angle a, Angle b ) => Math.Abs( a.Degrees ) > Math.Abs( b.Degrees ) + epsilon;
        public readonly bool IsSame( Angle b, Angle? maxDiff = null ) => Math.Abs( Diff( this, b ).Degrees ) <= Math.Abs( maxDiff?.Degrees ?? epsilon );
        public readonly Angle RoundTo( Angle step ) => new( Degrees.RoundTo( step.Degrees ), baseUnits );
        static double[] rad( params ReadOnlySpan<Angle> a ) => a.Length == 1 ? [ a[ 0 ].Radians ] : a.Length == 0 ? [] : [ .. a.ToArray().Select( t => t.Radians ) ];
        static Angle avg( IEnumerable<double> rad, AngleUnits baseUnits ) => ( rad?.Any() ?? false ) ? rad.Count() == 1 ? FromRadians( rad.First() ) : FromRadians( Math.Atan2( rad.Sum( Math.Sin ) / rad.Count(), rad.Sum( Math.Cos ) / rad.Count() ) ) : Zero;

        public static Angle Average( IEnumerable<Angle> a ) => ( a?.Any() ?? false ) ? avg( a.Select( t => t.Radians ), a.First().baseUnits ) : Zero;
        public static Angle Average( params ReadOnlySpan<Angle> a ) => a.Length == 1 ? a[ 0 ] : a.Length == 0 ? Zero : avg( rad( a ), a[ 0 ].baseUnits );


        public static Angle Diff( Angle a, Angle b ) {
            var r = Math.Abs(a.Degrees - b.Degrees);
            return new( r > 180.0 ? 360.0 - r : r, a.baseUnits );
        }
        public override readonly bool Equals( object that ) => that is Angle a && a == this;
        public override readonly int GetHashCode() => Degrees.GetHashCode();
        public override readonly string ToString() => baseUnits switch {
            AngleUnits.Radians => $"{ToStringRad()} rad",
            _ => $"{ToStringDeg()} deg"
        };
        public readonly string ToStringDeg() => Degrees.ToString( "##0.###", System.Globalization.CultureInfo.InvariantCulture );
        public readonly string ToStringRad() => Radians.ToString( "##0.#####", System.Globalization.CultureInfo.InvariantCulture );

        public static Angle Parse( string s ) {
            var units = s.EndsWith("deg") ? "deg" : "rad";
            var v = s.AsDouble();
            return s.EndsWith( "deg" ) ? FromDegrees( v ) : FromRadians( v );
        }
    }

}
