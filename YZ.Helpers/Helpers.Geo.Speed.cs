using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

using YZ;

namespace YZ {

    public class SpeedJsonConverter : JsonConverter<Speed> {
        public override Speed Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options ) => Speed.Parse( reader.GetString() );
        public override void Write( Utf8JsonWriter writer, Speed speedValue, JsonSerializerOptions options ) => writer.WriteStringValue( speedValue.ToString() );
    }

    [JsonConverter( typeof( SpeedJsonConverter ) )]
    public readonly struct Speed {
        const double epsilon = 0.000001;
        public static readonly Speed Zero = new(0);
        public static readonly Speed MS1 = new(1);
        public static readonly Speed KmPH60 = FromKilometersPerHour(60);
        public static readonly Speed KmPH90 = FromKilometersPerHour(90);
        public static readonly Speed Epsilon = new(epsilon);


        readonly SpeedUnits baseUnits;
        Speed( double v, SpeedUnits srcUnits = SpeedUnits.MetersPerSecond ) => (MetersPerSecond, baseUnits) = (normalizeTo( v, srcUnits ), srcUnits);
        public static Speed FromUnits( double value, SpeedUnits units = SpeedUnits.MetersPerSecond ) => new( value, units );
        public static Speed FromMilesPerHour( double mph ) => FromUnits( mph, SpeedUnits.MilesPerHour );
        public static Speed FromMetersPerSecond( double metersPerSecond ) => FromUnits( metersPerSecond, SpeedUnits.MetersPerSecond );
        public static Speed FromKilometersPerHour( double kilometersPerHour ) => FromUnits( kilometersPerHour, SpeedUnits.KilometersPerHour );
        public static Speed FromKnots( double knotsPerHour ) => FromUnits( knotsPerHour, SpeedUnits.Knots );


        static double normalizeTo( double valueInUnits, SpeedUnits units ) => units.GetEnumAttr( false, v => new NormalizeAttribute( 1, 1 ) ).NormalizeTo( valueInUnits );
        static double normalizeFrom( double baseUnits, SpeedUnits units ) => units.GetEnumAttr( false, v => new NormalizeAttribute( 1, 1 ) ).NormalizeFrom( baseUnits );

        public readonly double MetersPerSecond { get; }
        public readonly double Knots => normalizeFrom( MetersPerSecond, SpeedUnits.Knots );
        public readonly double MilesPerHour => normalizeFrom( MetersPerSecond, SpeedUnits.MilesPerHour );
        public readonly double KilometersPerHour => normalizeFrom( MetersPerSecond, SpeedUnits.KilometersPerHour );


        public static implicit operator Speed( double a ) => new( a );
        public static Speed operator -( Speed a, Speed b ) => new( a.MetersPerSecond - b.MetersPerSecond );
        public static Speed operator +( Speed a, Speed b ) => new( a.MetersPerSecond + b.MetersPerSecond );
        public static bool operator ==( Speed a, Speed b ) => Math.Abs( a.MetersPerSecond - b.MetersPerSecond ) <= epsilon;
        public static bool operator !=( Speed a, Speed b ) => Math.Abs( a.MetersPerSecond - b.MetersPerSecond ) > epsilon;
        public static bool operator <( Speed a, Speed b ) => Math.Abs( a.MetersPerSecond ) < Math.Abs( b.MetersPerSecond ) - epsilon;
        public static bool operator >( Speed a, Speed b ) => Math.Abs( a.MetersPerSecond ) > Math.Abs( b.MetersPerSecond ) + epsilon;

        public static GeoDistance operator *( Speed a, TimeSpan b ) => GeoDistance.FromMeters( a.MetersPerSecond * b.TotalSeconds );
        public static GeoDistance operator *( TimeSpan b, Speed a ) => GeoDistance.FromMeters( a.MetersPerSecond * b.TotalSeconds );
        public static TimeSpan operator /( GeoDistance a, Speed b ) => b.IsSame( Speed.Zero ) ? throw new DivideByZeroException( "Speed cannot be 0." ) : TimeSpan.FromSeconds( a.Meters / b.MetersPerSecond );
        public static Speed operator *( Speed a, double b ) => normalizeFrom( a.MetersPerSecond * b, a.baseUnits );
        public static Speed operator /( Speed a, double b ) => b == 0 ? throw new DivideByZeroException( "Coeff cannot be Zero" ) : normalizeFrom( a.MetersPerSecond / b, a.baseUnits );


        public readonly bool IsSame( Speed b, Speed? maxDiff = null ) => Math.Abs( Diff( this, b ).MetersPerSecond ) <= Math.Abs( maxDiff?.MetersPerSecond ?? epsilon );
        public readonly Speed RoundTo( Speed step ) => FromMetersPerSecond( MetersPerSecond.RoundTo( step.MetersPerSecond ) );

        public static Speed Average( IEnumerable<Speed> spd ) => !spd.Any() ? Zero: spd.Count() == 1 || spd.All( t => t == 0 ) ? spd.First() : new( spd.Sum( t => t.MetersPerSecond ) / spd.Count( t => t > 0 ) );
        public static Speed Average( params Speed[] spd ) => spd.Length == 0 ? Zero : spd.Length == 1 || spd.All( t => t == 0 ) ? spd[ 0 ] : new( spd.Sum( t => t.MetersPerSecond ) / spd.Count( t => t > 0 ) );
        public static Speed Average( params (Speed spd, GeoDistance dist)[] a ) => a.Length == 0 ? new Speed( 0 ) : a.Length == 1 ? a[ 0 ].spd : a.Where( t => t.spd > 0 ).Sum( t => t.dist ) / a.Where( t => t.spd > 0 ).Sum( t => t.dist / t.spd );
        public static Speed Average( params (Speed spd, TimeSpan time)[] a ) => a.Length == 0 ? new Speed( 0 ) : a.Length == 1 ? a[ 0 ].spd : a.Sum( t => t.time * t.spd ) / a.Sum( t => t.time );
        public static Speed Diff( Speed a, Speed b ) => new Speed( Math.Abs( a.MetersPerSecond - b.MetersPerSecond ) );



        public readonly bool Equals( Speed that ) => Math.Abs( this.MetersPerSecond - that.MetersPerSecond ) <= epsilon;
        public override readonly bool Equals( object that ) => that is Speed a && Equals( a );
        public override readonly int GetHashCode() => MetersPerSecond.GetHashCode();
        readonly string v2s => normalizeFrom( MetersPerSecond, baseUnits ).ToString( "##0.###", CultureInfo.InvariantCulture );
        public override readonly string ToString() => $"{v2s} {baseUnits.GetEnumAttr( false, ( v, a ) => a.Suffix, v => new SuffixAttribute( "" ) )}".Trim();
        public static Speed Parse( string src ) {
            src = src.Replace( " ", "" ).Trim().ToLower();
            var units = Enum.GetValues<SpeedUnits>().Select(t=>(k:t,suffix: t.GetEnumAttr( false, ( v, a ) => a.Suffix, v => new SuffixAttribute( "" ) ).ToLower())).Where(t=> src.EndsWith(t.suffix));
            var u = units.FirstOrDefault((k:SpeedUnits.MetersPerSecond,suffix:""));
            //if ( u.suffix.Length>0) src = src.
            return new( src.AsDouble(), u.k );
        }

    }

}
