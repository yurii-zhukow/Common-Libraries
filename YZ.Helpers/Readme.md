# YZ.Helpers

YZ.Helpers is a collection of extension methods and small types that simplify routine .NET development tasks.

## Installation

```bash
 dotnet add package YZ.Helpers
```

## Features

- String parsing and formatting
- Numeric calculations
- Date and time utilities
- Geometry and geolocation helpers
- File and network tools
- JSON serialization based on Newtonsoft.Json
- Logging and diagnostic helpers

## Quick Start

### Strings and numbers

```csharp
using YZ;

var text = "Value: 15";
var value = text.AsInt(min: 0, max: 100);
var rounded = 12.345.RoundTo(0.5);
```

### JSON serialization

```csharp
using YZ;

var user = new { userId = 7, userName = "Alice" };
var json = Helpers.JsonSerialize(user);
var clone = Helpers.JsonDeserializeWithDefault<User>(json, () => new User(0, "unknown"));
```

```csharp
public record User(int userId, string userName);
```

### Custom helper

```csharp
using YZ;

string Encode(string text) => text.Md5();
var code = Encode("secret");
```

### Sample JSON

```json
{
  "user-id": 7,
  "user-name": "Alice"
}
```

## API Reference

### Attributes.cs

**Classes:** ToleranceAttribute, ConsoleColorAttribute, DeepCopyAttribute, EditableAttribute, BriefDescriptionAttribute, SuffixAttribute, SeverityAttribute, IsFatalAttribute, NormalizeAttribute

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **Tolerance** | *double* | _TODO_ |
| **Color** | *ConsoleColor* | _TODO_ |
| **DeepCopy** | *bool* | _TODO_ |
| **Editable** | *bool* | _TODO_ |
| **BriefDescription** | *string* | _TODO_ |
| **Suffix** | *string* | _TODO_ |
| **Severity** | *Severity* | _TODO_ |
| **IsFatal** | *bool* | _TODO_ |
| **To** | *double* | _TODO_ |
| **From** | *double* | _TODO_ |
| **ToString()** | *string* | _TODO_ |
| **ToString()** | *string* | _TODO_ |
| **SeverityAttribute(Severity.NotError)** | *readonly SeverityAttribute NotError =* | _TODO_ |
| **NormalizeTo( double value )** | *double* | _TODO_ |
| **NormalizeFrom( double value )** | *double* | _TODO_ |

### Enums.cs

**Enums:** Severity, RussianCase, RussianCount, TimeUnit, LogPrefix, SpeedUnits


### Helpers.App.cs

**Classes:** Helpers

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **GetFullAppPath( this string relativePath )** | *string* | _TODO_ |

### Helpers.Attributes.cs

**Classes:** Helpers

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **GetAttr<TAttr>(this Type type, bool inherit, Func<TAttr> dflt = null)** | *TAttr* | _TODO_ |
| **TValue>(this Type type, bool inherit, Func<TAttr, TValue> getValue, Func<TAttr> dflt = null)** | *TValue GetAttr<TAttr,* | _TODO_ |
| **GetAttr<TAttr>(this object value, bool inherit, Func<object, TAttr> dflt = null)** | *TAttr* | _TODO_ |
| **TValue>(this object value, bool inherit, Func<TAttr, TValue> getValue, Func<object, TAttr> dflt = null)** | *TValue GetAttr<TAttr,* | _TODO_ |
| **TAttr>(this T value, bool inherit, Func<T, TAttr> dflt = null)** | *TAttr GetAttr<T,* | _TODO_ |
| **TAttr>(this T value, bool inherit)** | *TAttr[] GetAttrs<T,* | _TODO_ |
| **TAttr>(this T value, string propName, bool inherit, Func<T, string, TAttr> dflt = null)** | *TAttr GetAttr<T,* | _TODO_ |
| **TAttr>(this T value, string propName, bool inherit)** | *TAttr[] GetAttrs<T,* | _TODO_ |
| **TValue>(this T value, bool inherit, Func<TAttr, TValue> getValue)** | *TValue[] GetAttrs<T, TAttr,* | _TODO_ |
| **TValue>(this T value, string propName, bool inherit, Func<TAttr, TValue> getValue)** | *TValue[] GetAttrs<T, TField, TAttr,* | _TODO_ |
| **TValue>(this T value, bool inherit, Func<TAttr, TValue> getValue, Func<T, TAttr> dflt = null)** | *TValue GetAttr<T, TAttr,* | _TODO_ |
| **TValue>(this T value, string propName, bool inherit, Func<TAttr, TValue> getValue, Func<T, string, TAttr> dflt = null)** | *TValue GetAttr<T, TField, TAttr,* | _TODO_ |
| **GetDescription<TType>(this TType value, bool inherit = false)** | *string* | _TODO_ |
| **GetBriefDescription<TType>(this TType value, bool inherit = false)** | *string* | _TODO_ |
| **GetDescription<TType>(this TType value, GetDescriptionMode mode, bool inherit = false)** | *string* | _TODO_ |
| **GetDescription<TType>(this TType value, string propName, bool inherit = false)** | *string* | _TODO_ |
| **GetBriefDescription<TType>(this TType value, string propName, bool inherit = false)** | *string* | _TODO_ |
| **GetDescription<TType>(this TType value, string propName, GetDescriptionMode mode, bool inherit = false)** | *string* | _TODO_ |

### Helpers.Compare.cs

**Classes:** GenericComparer, GenericEqualityComparer, GenericEqualityComparer

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **Compare(T x, T y)** | *int* | _TODO_ |
| **Equals(T x, T y)** | *bool* | _TODO_ |
| **GetHashCode(T obj)** | *int* | _TODO_ |
| **Equals(T x, T y)** | *bool* | _TODO_ |
| **GetHashCode(T obj)** | *int* | _TODO_ |

### Helpers.Converters.cs

**Classes:** Helpers

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **Not<T>(Func<T, bool> fn)** | *Func<T, bool>* | _TODO_ |

### Helpers.Encryption.cs

**Classes:** Helpers, Crypt

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **Key** | *string* | _TODO_ |
| **Md5(this string s)** | *string* | _TODO_ |
| **GeneratePassword()** | *string* | _TODO_ |
| **GenerateKey(int deep = 3)** | *string* | _TODO_ |
| **Encrypt(byte[] data)** | *byte[]* | _TODO_ |
| **Encrypt(string data)** | *byte[]* | _TODO_ |
| **EncryptString(string data)** | *string* | _TODO_ |
| **Decrypt(byte[] data)** | *byte[]* | _TODO_ |
| **DecryptString(byte[] data)** | *string* | _TODO_ |
| **DecryptString(string data)** | *string* | _TODO_ |

### Helpers.Enum.cs

**Classes:** Helpers

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **ToTypes<TEnumType>()** | *KeyValuePair<string, Dictionary<string, string>>* | _TODO_ |
| **EnumToArray<TEnumType>()** | *TEnumType[]* | _TODO_ |
| **TResult>(Func<TEnumType, TResult> fn)** | *TResult[] EnumToArray<TEnumType,* | _TODO_ |
| **TResult>(bool inherit, Func<TEnumType, TAttr, TResult> fn)** | *TResult[] EnumToArray<TEnumType, TAttr,* | _TODO_ |
| **TResult>(Func<TEnumType, TResult> resFunc)** | *Dictionary<TEnumType, TResult> EnumToDictionary<TEnumType,* | _TODO_ |
| **TResult>(bool inherit, Func<TAttr, TResult> resFunc, Func<TEnumType, TResult> dflt)** | *Dictionary<TEnumType, TResult> EnumToDictionary<TEnumType, TAttr,* | _TODO_ |
| **ToKeyValuePairs<TEnumType>()** | *Dictionary<TEnumType, string>* | _TODO_ |
| **TResult>(this TEnumType value, bool inherit, Func<TEnumType, TAttr, TResult> fn, Func<TEnumType, TAttr> dflt)** | *TResult GetEnumAttr<TEnumType, TAttr,* | _TODO_ |
| **TAttr>(this TEnumType value, bool inherit, Func<TEnumType, TAttr> dflt = null)** | *TAttr GetEnumAttr<TEnumType,* | _TODO_ |
| **TResult>(this TEnumType value, bool inherit, Func<TAttr, TResult> fn)** | *TResult[] GetEnumAttrs<TEnumType, TAttr,* | _TODO_ |
| **TAttr>(this TEnumType value, bool inherit)** | *TAttr[] GetEnumAttrs<TEnumType,* | _TODO_ |
| **Next<T>(this T value, out bool overflow, params T[] skipValues)** | *T* | _TODO_ |
| **Prev<T>(this T value, out bool overflow, params T[] skipValues)** | *T* | _TODO_ |
| **First<T>(this T value, params T[] skipValues)** | *T* | _TODO_ |
| **Last<T>(this T value, params T[] skipValues)** | *T* | _TODO_ |
| **Is<T>(this T value, params T[] allowed)** | *bool* | _TODO_ |
| **GetEnumDescription<TEnum>(this TEnum value, GetDescriptionMode mode = GetDescriptionMode.Full)** | *string* | _TODO_ |
| **GetEnumDescription(this Enum value, GetDescriptionMode mode = GetDescriptionMode.Full)** | *string* | _TODO_ |
| **SafeCast<TEnum>(this string value, TEnum deflt = default, bool ignoreCase = false)** | *TEnum* | _TODO_ |
| **SafeCast<TEnum>(this int value, TEnum deflt = default)** | *TEnum* | _TODO_ |
| **SafeCast<TEnum>(this uint value, TEnum deflt = default)** | *TEnum* | _TODO_ |
| **SafeCast<TEnum>(this byte value, TEnum deflt = default)** | *TEnum* | _TODO_ |
| **SafeCast<TEnum>(this decimal? value, TEnum deflt = default)** | *TEnum* | _TODO_ |

### Helpers.Eval.cs

**Classes:** Helpers


### Helpers.Exceptions.cs

**Classes:** Helpers

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **GetInner(this Exception ex, int maxDepth = 5)** | *IEnumerable<string>* | _TODO_ |
| **GetInner(this Exception ex, string separator, int maxDepth = 5)** | *string* | _TODO_ |
| **GetInner<TException>(this Exception ex)** | *TException* | _TODO_ |
| **GetInner(this Exception ex, Func<Exception, bool> selector)** | *Exception* | _TODO_ |
| **TResult>(this TSource src, Func<TSource, TResult> fn)** | *TResult SafeCall<TSource,* | _TODO_ |
| **TResult>(this TSource src, Func<TSource, TResult> fn, TResult dflt)** | *TResult SafeCall<TSource,* | _TODO_ |
| **TResult>(this TSource src, Func<TSource, TResult> fn, TResult dflt, out Exception ex)** | *TResult SafeCall<TSource,* | _TODO_ |
| **TResult>(this TSource src, Func<TSource, TResult> fn, Func<TResult> dflt)** | *TResult SafeCall<TSource,* | _TODO_ |
| **TResult>(this TSource src, Func<TSource, TResult> fn, Func<TResult> dflt, out Exception ex)** | *TResult SafeCall<TSource,* | _TODO_ |
| **SafeCall<TSource>(this TSource src, Action<TSource> fn)** | *bool* | _TODO_ |
| **SafeCall<TSource>(this TSource src, Action<TSource> fn, out Exception ex)** | *bool* | _TODO_ |
| **SafeCall(this Action fn)** | *bool* | _TODO_ |
| **SafeCall(this Action fn, out Exception ex)** | *bool* | _TODO_ |
| **SafeCall<TResult>(this Func<TResult> fn, out Exception ex, Func<TResult> dflt)** | *TResult* | _TODO_ |
| **SafeCall<TResult>(this Func<TResult> fn, TResult dflt)** | *TResult* | _TODO_ |
| **SafeCall<TResult>(this Func<TResult> fn, out Exception ex, TResult @default = default)** | *TResult* | _TODO_ |

### Helpers.Files.cs

**Classes:** Helpers

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **GetPathEnding( this string path, int lastPathParts )** | *string* | _TODO_ |

### Helpers.Geo.Angle.cs

**Classes:** AngleJsonConverter

**Structs:** Angle

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **Degrees** | *double* | _TODO_ |
| **Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )** | *Angle* | _TODO_ |
| **Write( Utf8JsonWriter writer, Angle angleValue, JsonSerializerOptions options )** | *void* | _TODO_ |
| **new(0)** | *readonly Angle Zero =* | _TODO_ |
| **new(90)** | *readonly Angle Deg90 =* | _TODO_ |
| **new(180)** | *readonly Angle Deg180 =* | _TODO_ |
| **new(270)** | *readonly Angle Deg270 =* | _TODO_ |
| **new(epsilon)** | *readonly Angle Epsilon =* | _TODO_ |
| **FromDegrees( double deg )** | *Angle* | _TODO_ |
| **FromRadians( double rad )** | *Angle* | _TODO_ |
| **-( Angle a, Angle b )** | *Angle operator* | _TODO_ |
| **+( Angle a, Angle b )** | *Angle operator* | _TODO_ |
| **==( Angle a, Angle b )** | *bool operator* | _TODO_ |
| **!=( Angle a, Angle b )** | *bool operator* | _TODO_ |
| **<( Angle a, Angle b )** | *bool operator* | _TODO_ |
| **>( Angle a, Angle b )** | *bool operator* | _TODO_ |
| **IsSame( Angle b, Angle? maxDiff = null )** | *bool* | _TODO_ |
| **RoundTo( Angle step )** | *Angle* | _TODO_ |
| **Average( params Angle[] a )** | *Angle* | _TODO_ |
| **Average( params double[] a )** | *Angle* | _TODO_ |
| **Diff( Angle a, Angle b )** | *Angle* | _TODO_ |
| **Equals( object that )** | *bool* | _TODO_ |
| **GetHashCode()** | *int* | _TODO_ |
| **ToString()** | *string* | _TODO_ |
| **Parse( string s )** | *Angle* | _TODO_ |

### Helpers.Geo.Coord.cs

**Classes:** GeoCoordJsonConverter

**Structs:** GeoCoord

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )** | *GeoCoord* | _TODO_ |
| **Write( Utf8JsonWriter writer, GeoCoord angleValue, JsonSerializerOptions options )** | *void* | _TODO_ |
| **==( GeoCoord a, GeoCoord b )** | *bool operator* | _TODO_ |
| **!=( GeoCoord a, GeoCoord b )** | *bool operator* | _TODO_ |
| **string( GeoCoord a )** | *implicit operator* | _TODO_ |
| **GeoCoord( string a )** | *implicit operator* | _TODO_ |
| **Constraint( GeoCoord? min = null, GeoCoord? max = null )** | *GeoCoord* | _TODO_ |
| **Parse( string latNon )** | *GeoCoord* | _TODO_ |
| **+( GeoCoord coord, GeoOffset offs )** | *GeoCoord operator* | _TODO_ |
| **-( GeoCoord coord, GeoOffset offs )** | *GeoCoord operator* | _TODO_ |
| **-( GeoCoord a, GeoCoord b )** | *GeoOffset operator* | _TODO_ |
| **&( GeoCoord a, GeoCoord b )** | *GeoCoord operator* | _TODO_ |
| **Average( GeoCoord[] a )** | *GeoCoord* | _TODO_ |
| **Approximate( GeoCoord a, GeoCoord b, double offs )** | *GeoCoord* | _TODO_ |
| **ToString()** | *string* | _TODO_ |
| **Equals( object that )** | *bool* | _TODO_ |
| **GetHashCode()** | *int* | _TODO_ |

### Helpers.Geo.Distance.cs

**Classes:** GeoDistanceJsonConverter

**Structs:** GeoDistance

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **CanConvert( Type objectType )** | *bool* | _TODO_ |
| **ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer )** | *object* | _TODO_ |
| **WriteJson( JsonWriter writer, object value, JsonSerializer serializer )** | *void* | _TODO_ |
| **/( GeoDistance a, double b )** | *GeoDistance operator* | _TODO_ |
| **/( GeoDistance a, TimeSpan b )** | *Speed operator* | _TODO_ |
| ***( GeoDistance a, double b )** | *GeoDistance operator* | _TODO_ |
| **+( GeoDistance a, GeoDistance b )** | *GeoDistance operator* | _TODO_ |
| **-( GeoDistance a, GeoDistance b )** | *GeoDistance operator* | _TODO_ |
| **-( GeoDistance a )** | *GeoDistance operator* | _TODO_ |
| **>( GeoDistance a, GeoDistance b )** | *bool operator* | _TODO_ |
| **<( GeoDistance a, GeoDistance b )** | *bool operator* | _TODO_ |
| **>=( GeoDistance a, GeoDistance b )** | *bool operator* | _TODO_ |
| **<=( GeoDistance a, GeoDistance b )** | *bool operator* | _TODO_ |
| **==( GeoDistance a, GeoDistance b )** | *bool operator* | _TODO_ |
| **!=( GeoDistance a, GeoDistance b )** | *bool operator* | _TODO_ |
| **FromMeters( double meters )** | *GeoDistance* | _TODO_ |
| **FromKm( double km )** | *GeoDistance* | _TODO_ |
| **GeoDistance(0)** | *readonly GeoDistance Zero =* | _TODO_ |
| **CompareTo( GeoDistance other )** | *int* | _TODO_ |
| **double( GeoDistance d )** | *implicit operator* | _TODO_ |
| **GeoDistance( double meters )** | *implicit operator* | _TODO_ |
| **Equals( object obj )** | *bool* | _TODO_ |
| **GetHashCode()** | *int* | _TODO_ |
| **ToString()** | *string* | _TODO_ |

### Helpers.Geo.GpsInfo.cs

**Structs:** GpsInfo

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **OffsetTime( TimeSpan offset )** | *GpsInfo* | _TODO_ |
| **ToString()** | *string* | _TODO_ |
| **Average( GpsInfo a, GpsInfo b )** | *GpsInfo* | _TODO_ |
| **Average( GpsInfo[] a )** | *GpsInfo* | _TODO_ |
| **Copy( GeoCoord? position = null, double? speed = null, double? acceleration = null, Angle? angle = null, bool? isValid = null, bool? isVirtual = null, DateTime? date = null, string label = null, TimeSpan? duration = null )** | *GpsInfo* | _TODO_ |
| **Approximate( [NotNull] GpsInfo[] a, DateTime t )** | *GpsInfo* | _TODO_ |
| **SplitBy( GpsInfo a, GpsInfo b, TimeSpan t )** | *IEnumerable<GpsInfo>* | _TODO_ |

### Helpers.Geo.Helpers.cs

**Classes:** Helpers

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **Sum( this IEnumerable<GeoDistance> distances )** | *GeoDistance* | _TODO_ |
| **Sum<T>( this IEnumerable<T> src, Func<T, GeoDistance> field )** | *GeoDistance* | _TODO_ |
| **MakePath( this IEnumerable<GeoLine> src )** | *IEnumerable<GeoCoord>* | _TODO_ |
| **GetViewport( this IEnumerable<GeoCoord> all )** | *GeoRect* | _TODO_ |
| **GetViewport( this IEnumerable<GeoRect> all )** | *GeoRect* | _TODO_ |

### Helpers.Geo.Line.cs

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **GetProjection( GeoCoord p )** | *GeoCoord* | _TODO_ |
| **IsProjected( GeoCoord p )** | *readonly bool* | _TODO_ |
| **DistanceTo( GeoCoord p )** | *readonly GeoDistance* | _TODO_ |
| **Translate( Angle a, GeoDistance d )** | *GeoLine* | _TODO_ |
| **Translate( GeoOffset offs )** | *GeoLine* | _TODO_ |
| **Scale( double b )** | *GeoLine* | _TODO_ |
| **Intersect( GeoLine b )** | *GeoCoord* | _TODO_ |
| **/( GeoLine a, double b )** | *GeoLine operator* | _TODO_ |
| ***( GeoLine a, double b )** | *GeoLine operator* | _TODO_ |
| **+( GeoLine a, GeoOffset b )** | *GeoLine operator* | _TODO_ |
| **-( GeoLine a, GeoOffset b )** | *GeoLine operator* | _TODO_ |
| **==( GeoLine a, GeoLine b )** | *bool operator* | _TODO_ |
| **!=( GeoLine a, GeoLine b )** | *bool operator* | _TODO_ |
| **&( GeoLine a, GeoLine b )** | *GeoCoord operator* | _TODO_ |
| **Equals( object obj )** | *bool* | _TODO_ |
| **Equals( GeoLine other )** | *bool* | _TODO_ |
| **GetHashCode()** | *int* | _TODO_ |
| **ToString()** | *string* | _TODO_ |

### Helpers.Geo.Offset.cs

**Structs:** GeoOffset

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **+( GeoOffset a, GeoOffset b )** | *GeoOffset operator* | _TODO_ |
| **+( GeoOffset a, Angle b )** | *GeoOffset operator* | _TODO_ |
| **+( GeoOffset a, GeoDistance b )** | *GeoOffset operator* | _TODO_ |
| **-( GeoOffset a )** | *GeoOffset operator* | _TODO_ |
| ***( GeoOffset a, double b )** | *GeoOffset operator* | _TODO_ |
| **/( GeoOffset a, double b )** | *GeoOffset operator* | _TODO_ |
| ***( GeoOffset a, (double xLat, double xLon) b )** | *GeoOffset operator* | _TODO_ |
| **GeoDistance( GeoOffset a )** | *implicit operator* | _TODO_ |
| **ToString()** | *string* | _TODO_ |
| **CompareTo( GeoOffset other )** | *int* | _TODO_ |

### Helpers.Geo.Rect.cs

**Structs:** GeoRect

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **Combine( GeoRect other )** | *GeoRect* | _TODO_ |
| **ExpandTo( GeoCoord pt )** | *GeoRect* | _TODO_ |
| **Expand( GeoDistance d )** | *GeoRect* | _TODO_ |
| **Constraint( GeoOffset? min = null, GeoOffset? max = null, bool keepAspect = true )** | *GeoRect* | _TODO_ |
| **Scale( double xLat, double xLon )** | *GeoRect* | _TODO_ |
| ***( GeoRect a, (double xLat, double xLon) b )** | *GeoRect operator* | _TODO_ |
| **ConstraintMin( GeoOffset min, bool keepAspect = true )** | *GeoRect* | _TODO_ |
| **ConstraintMax( GeoOffset max, bool keepAspect = true )** | *GeoRect* | _TODO_ |
| **Equals( object obj )** | *bool* | _TODO_ |
| **Equals( GeoRect other )** | *bool* | _TODO_ |
| **GetHashCode()** | *int* | _TODO_ |
| **ToString()** | *string* | _TODO_ |
| **new(GeoCoord(), GeoCoord())** | *readonly GeoRect Empty =* | _TODO_ |

### Helpers.Geo.Speed.cs

**Classes:** SpeedJsonConverter

**Structs:** Speed

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **MetersPerSecond** | *double* | _TODO_ |
| **Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )** | *Speed* | _TODO_ |
| **Write( Utf8JsonWriter writer, Speed speedValue, JsonSerializerOptions options )** | *void* | _TODO_ |
| **new(0)** | *readonly Speed Zero =* | _TODO_ |
| **new(1)** | *readonly Speed MS1 =* | _TODO_ |
| **FromKilometersPerHour(60)** | *readonly Speed KmPH60 =* | _TODO_ |
| **FromKilometersPerHour(60)** | *readonly Speed KmPH90 =* | _TODO_ |
| **new(epsilon)** | *readonly Speed Epsilon =* | _TODO_ |
| **FromUnits( double value, SpeedUnits units = SpeedUnits.MetersPerSecond )** | *Speed* | _TODO_ |
| **FromMilesPerHour( double mph )** | *Speed* | _TODO_ |
| **FromMetersPerSecond( double metersPerSecond )** | *Speed* | _TODO_ |
| **FromKilometersPerHour( double kilometersPerHour )** | *Speed* | _TODO_ |
| **FromKnotsPerHour( double knotsPerHour )** | *Speed* | _TODO_ |
| **Speed( double a )** | *implicit operator* | _TODO_ |
| **-( Speed a, Speed b )** | *Speed operator* | _TODO_ |
| **+( Speed a, Speed b )** | *Speed operator* | _TODO_ |
| **==( Speed a, Speed b )** | *bool operator* | _TODO_ |
| **!=( Speed a, Speed b )** | *bool operator* | _TODO_ |
| **<( Speed a, Speed b )** | *bool operator* | _TODO_ |
| **>( Speed a, Speed b )** | *bool operator* | _TODO_ |
| ***( Speed a, TimeSpan b )** | *GeoDistance operator* | _TODO_ |
| ***( TimeSpan b, Speed a )** | *GeoDistance operator* | _TODO_ |
| **/( GeoDistance a, Speed b )** | *TimeSpan operator* | _TODO_ |
| **IsSame( Speed b, Speed? maxDiff = null )** | *bool* | _TODO_ |
| **RoundTo( Speed step )** | *Speed* | _TODO_ |
| **Average( params (Speed spd, GeoDistance dist)[] a )** | *Speed* | _TODO_ |
| **Average( params (Speed spd, TimeSpan time)[] a )** | *Speed* | _TODO_ |
| **Diff( Speed a, Speed b )** | *Speed* | _TODO_ |
| **Equals( object that )** | *bool* | _TODO_ |
| **GetHashCode()** | *int* | _TODO_ |
| **ToString()** | *string* | _TODO_ |
| **Parse( string src )** | *Speed* | _TODO_ |

### Helpers.Geo.Tools.cs

**Classes:** Tools

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **NormalizeDeg( double deg )** | *double* | _TODO_ |
| **ToEquator( double lat )** | *GeoDistance* | _TODO_ |
| **ToGreenwich( double lon )** | *GeoDistance* | _TODO_ |
| **Translate( GeoCoord coord, GeoOffset offs )** | *GeoCoord* | _TODO_ |
| **Intersect( GeoLine a, GeoLine b )** | *GeoCoord* | _TODO_ |

### Helpers.Geometry.cs

**Classes:** Helpers

**Structs:** LineF

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **Length** | *double* | _TODO_ |
| **ScaleToFit(this Rectangle src, Rectangle bounds)** | *Rectangle* | _TODO_ |
| **ScaleToFit(this Rectangle src, Size bounds)** | *Rectangle* | _TODO_ |
| **ScaleToFit(this Rectangle src, int width, int height)** | *Rectangle* | _TODO_ |
| **Translate(this Rectangle src, Point offs)** | *Rectangle* | _TODO_ |
| **Translate(this Rectangle src, int x, int y)** | *Rectangle* | _TODO_ |
| **Translate(this Point src, Point offs)** | *Point* | _TODO_ |
| **Translate(this Point src, int x, int y)** | *Point* | _TODO_ |
| **ScaleToFit(this RectangleF src, RectangleF bounds)** | *RectangleF* | _TODO_ |
| **ScaleToFit(this RectangleF src, Size bounds)** | *RectangleF* | _TODO_ |
| **ScaleToFit(this RectangleF src, float width, float height)** | *RectangleF* | _TODO_ |
| **Translate(this RectangleF src, PointF offs)** | *RectangleF* | _TODO_ |
| **Translate(this RectangleF src, float x, float y)** | *RectangleF* | _TODO_ |
| **Translate(this PointF src, PointF offs)** | *PointF* | _TODO_ |
| **Translate(this PointF src, float x, float y)** | *PointF* | _TODO_ |
| **Contains(PointF p, bool strict = true)** | *bool* | _TODO_ |
| **IsInside(this PointF pt, IEnumerable<PointF> shape)** | *bool* | _TODO_ |

### Helpers.Guid.cs

**Classes:** Helpers

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **IsEmptyOrSame(this Guid guid, Guid target)** | *bool* | _TODO_ |

### Helpers.Http.cs

**Classes:** Helpers

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **GetJsonAsync<T>(this HttpClient http, string host, CancellationToken cancellationToken, params (string key, object value)[] args)** | *Task<T>* | _TODO_ |
| **GetJsonAsync<T>(this HttpClient http, string host, string path, CancellationToken cancellationToken, params (string key, object value)[] args)** | *Task<T>* | _TODO_ |

### Helpers.Lists.cs

**Classes:** Helpers, ConcurrentDictionary, ConcurrentDictionary, FolderTree, Folder, Item

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **ToString<T>( this ICollection<T> arr, string separator )** | *string* | _TODO_ |
| **ToString<T>( this IEnumerable<T> arr, string separator )** | *string* | _TODO_ |
| **ToString<T>( this IEnumerable<T> arr, string separator, Func<T, object> convert )** | *string* | _TODO_ |
| **ToString<T>( this IEnumerable<T> arr, string separator, string openTag, string closeTag )** | *string* | _TODO_ |
| **ToString( this IEnumerable<byte> arr, string separator = "", string format = "x2" )** | *string* | _TODO_ |
| **ToBase64String( this IEnumerable<byte> arr )** | *string* | _TODO_ |
| **ToBase64String( this byte[] arr )** | *string* | _TODO_ |
| **ToString<TValue>( this Dictionary<string, TValue> arr, string separator, string kvSeparator )** | *string* | _TODO_ |
| **ForEach<T>( this IEnumerable<T> src, Action<T> fn )** | *IEnumerable<T>* | _TODO_ |
| **ToArray<T>( this IEnumerable<T> src, int size, Func<T, int> index )** | *T[]* | _TODO_ |
| **TDest>( this IEnumerable<T> src, int size, Func<T, int> index, Func<T, TDest> convert )** | *TDest[] ToArray<T,* | _TODO_ |
| **TValue>()** | *Func<KeyValuePair<TKey, TValue>, TKey> KeyFn<TKey,* | _TODO_ |
| **TValue>()** | *Func<KeyValuePair<TKey, TValue>, TValue> ValueFn<TKey,* | _TODO_ |
| **Fill<T>( this T[] src, T value, int start = 0, int count = 0 )** | *void* | _TODO_ |
| **Nearest<T>( this IEnumerable<T> src, T value, Func<T, T, decimal> distance, [CallerArgumentExpression( nameof( src ) )] string expr = "" )** | *T* | _TODO_ |
| **Nearest<T>( this IEnumerable<T> src, T value, Func<T, T, uint> distance, [CallerArgumentExpression( nameof( src ) )] string expr = "" )** | *T* | _TODO_ |
| **Nearest<T>( this IEnumerable<T> src, Func<T, decimal> distance, [CallerArgumentExpression( nameof( src ) )] string expr = "" )** | *T* | _TODO_ |
| **Nearest<T>( this IEnumerable<T> src, Func<T, uint> distance, [CallerArgumentExpression( nameof( src ) )] string expr = "" )** | *T* | _TODO_ |
| **TDiff>( this T[] src, TProp value, Func<T, TProp> fld, Func<TProp, TProp, TDiff> diff )** | *int NearestIndex<T, TProp,* | _TODO_ |
| **SplitBy<T>( this IEnumerable<T> src, int size )** | *IEnumerable<IEnumerable<T>>* | _TODO_ |
| **Generate<T>( this int count, Func<int, T> generator )** | *T[]* | _TODO_ |
| **Shuffle<T>( this IEnumerable<T> src )** | *IEnumerable<T>* | _TODO_ |
| **TValue>( this IEnumerable<KeyValuePair<TKey, TValue>> src )** | *Dictionary<TKey, TValue> ToDictionary<TKey,* | _TODO_ |
| **AddOrUpdate( TKey key, TSubKey subkey, TSubSubKey subsubkey, Func<TKey, TSubKey, TSubSubKey, TValue> addValueFactory, Func<TKey, TSubKey, TSubSubKey, TValue, TValue> updateValueFactory )** | *TValue* | _TODO_ |
| **TryGetValue( TKey key, TSubKey subkey, TSubSubKey subsubkey, out TValue value )** | *bool* | _TODO_ |
| **TryAdd( TKey key, TSubKey subkey, TSubSubKey subsubkey, TValue value )** | *bool* | _TODO_ |
| **TryRemove( TKey key, TSubKey subkey, TSubSubKey subsubkey, out TValue value )** | *bool* | _TODO_ |
| **AddOrUpdate( TKey key, TSubKey subkey, Func<TKey, TSubKey, TValue> addValueFactory, Func<TKey, TSubKey, TValue, TValue> updateValueFactory )** | *TValue* | _TODO_ |
| **TryGetValue( TKey key, TSubKey subkey, out TValue value )** | *bool* | _TODO_ |
| **TryAdd( TKey key, TSubKey subkey, TValue value )** | *bool* | _TODO_ |
| **TryRemove( TKey key, TSubKey subkey, out TValue value )** | *bool* | _TODO_ |
| **ToList<T>( Func<TKey, TSubKey, TValue, T> convert )** | *List<T>* | _TODO_ |
| **ForEach( Action<TKey, TSubKey, TValue> fn )** | *void* | _TODO_ |
| **ForEach( TKey key, Action<TKey, TSubKey, TValue> fn )** | *void* | _TODO_ |
| **ForEach( IEnumerable<TKey> keys, Action<TKey, TSubKey, TValue> fn )** | *void* | _TODO_ |
| **ForEach( IEnumerable<TKey> keys, IEnumerable<TSubKey> subKeys, Action<TKey, TSubKey, TValue> fn )** | *void* | _TODO_ |

### Helpers.Logs.cs

**Classes:** LogStorage, Item, LoggerClient

**Structs:** Dto

**Enums:** Verbosity

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **Prefix** | *string* | _TODO_ |
| **Verbosity** | *Verbosity* | _TODO_ |
| **FileWriteVerbosity** | *Verbosity* | _TODO_ |
| **Verbosity** | *Verbosity* | _TODO_ |
| **Prefix** | *string* | _TODO_ |
| **Index** | *ulong* | _TODO_ |
| **Date** | *DateTime* | _TODO_ |
| **Message** | *string* | _TODO_ |
| **ILoggerClient** | *interface* | _TODO_ |
| **Storage** | *LogStorage* | _TODO_ |
| **Prefix** | *string* | _TODO_ |
| **ConsoleColor>(true, resFunc: a)** | *readonly Dictionary<Verbosity, ConsoleColor> VerbosityConsoleColors = Helpers.EnumToDictionary<Verbosity, ConsoleColorAttribute,* | _TODO_ |
| **string>(true, resFunc: a)** | *readonly Dictionary<Verbosity, string> VerbosityDescriptions = Helpers.EnumToDictionary<Verbosity, DescriptionAttribute,* | _TODO_ |
| **DefaultLogEvent( LogStorage sender, Item message )** | *void* | _TODO_ |
| **LogEvent( LogStorage sender, Item message )** | *delegate void* | _TODO_ |
| **GetItems( ulong startFromId )** | *List<Item.Dto>* | _TODO_ |
| **GetItemsError( string err )** | *List<Item.Dto>* | _TODO_ |
| **Log( string s, Verbosity v = Verbosity.Info, string prefix = "" )** | *void* | _TODO_ |
| **Error( Exception ex, Verbosity v = Verbosity.Error, string prefix = "" )** | *void* | _TODO_ |
| **Error( string s, Verbosity v = Verbosity.Error, string prefix = "" )** | *void* | _TODO_ |
| **Dispose()** | *void* | _TODO_ |
| **Log( string s, Verbosity v = Verbosity.Info, string prefix = "", [CallerMemberName] string callerName = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = -1 )** | *void* | _TODO_ |
| **Hint( string s, string prefix = "", [CallerMemberName] string callerName = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = -1 )** | *void* | _TODO_ |
| **Info( string s, string prefix = "", [CallerMemberName] string callerName = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = -1 )** | *void* | _TODO_ |
| **Warning( string s, string prefix = "", [CallerMemberName] string callerName = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = -1 )** | *void* | _TODO_ |
| **Fatal( string s, string prefix = "", [CallerMemberName] string callerName = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = -1 )** | *void* | _TODO_ |
| **Critical( string s, string prefix = "", [CallerMemberName] string callerName = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = -1 )** | *void* | _TODO_ |
| **Important( string s, string prefix = "", [CallerMemberName] string callerName = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = -1 )** | *void* | _TODO_ |
| **Error( Exception ex, Verbosity v = Verbosity.Error, string prefix = "", [CallerMemberName] string callerName = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = -1 )** | *void* | _TODO_ |
| **Error( string s, Verbosity v = Verbosity.Error, string prefix = "", [CallerMemberName] string callerName = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = -1 )** | *void* | _TODO_ |
| **InfoTemp( string s, string prefix = "", [CallerMemberName] string callerName = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = -1 )** | *void* | _TODO_ |
| **Dispose()** | *void* | _TODO_ |

### Helpers.Network.cs

**Classes:** Helpers

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **NormalizeIp(this string ip, string deflt = "127.0.0.1")** | *string* | _TODO_ |
| **NormalizeMac(this string mac, string deflt = null)** | *string* | _TODO_ |
| **GetLocalIp(AddressFamily addressFamily = AddressFamily.InterNetwork)** | *IPAddress* | _TODO_ |
| **GetLocalIpAddress(AddressFamily addressFamily = AddressFamily.InterNetwork)** | *string* | _TODO_ |

### Helpers.Numbers.cs

**Classes:** Helpers

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **Convert(this int value, char[] digits)** | *string* | _TODO_ |
| **RoundTo(this double x, double step)** | *double* | _TODO_ |
| **RoundTo(this double x, double step, int decimals)** | *double* | _TODO_ |
| **RoundTo(this decimal x, decimal step)** | *decimal* | _TODO_ |
| **InRange(this int x, int min, int max)** | *bool* | _TODO_ |
| **InRange(this double x, double min, double max)** | *bool* | _TODO_ |
| **InRange<T>(this T x, T min, T max)** | *bool* | _TODO_ |
| **Constraint<T>(this T src, T? min = null, T? max = null, T? outrangeDefault = null)** | *T* | _TODO_ |
| **Constraint<T>(this T src, (T min, T max) minmax, T? outrangeDefault = null)** | *T* | _TODO_ |
| **Constraint<T>(this T src, T min = null, T max = null, T outrangeDefault = null)** | *T* | _TODO_ |
| **Constraint<T>(this T src, (T min, T max) minmax, T outrangeDefault = null)** | *T* | _TODO_ |
| **ToString(this double value, double roundTo, string e0, string e1, string e2)** | *string* | _TODO_ |
| **ToString(this int value, string e0, string e1, string e2)** | *string* | _TODO_ |
| **ToBytesNative(this int value, params string[] suffix)** | *string* | _TODO_ |
| **ToBytesNative(this uint value, params string[] suffix)** | *string* | _TODO_ |
| **ToBytesNative(this long value, params string[] suffix)** | *string* | _TODO_ |
| **ToBytesNative(this ulong value, params string[] suffix)** | *string* | _TODO_ |
| **ToBytesNative(this double value, params string[] suffix)** | *string* | _TODO_ |

### Helpers.Objects.cs

**Classes:** Helpers

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **ListEqual(this IList src, IList dst, bool deep)** | *bool* | _TODO_ |
| **DictionaryEqual(this IDictionary src, IDictionary dst, bool deep)** | *bool* | _TODO_ |
| **ObjectEqual(this object src, object dst)** | *bool* | _TODO_ |

### Helpers.Range.cs

**Classes:** Helpers, Range, Range, LongRange

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **Start** | *T* | _TODO_ |
| **Stop** | *T* | _TODO_ |
| **Recombine<T>(this IEnumerable<Range<T>> src)** | *IEnumerable<Range<T>>* | _TODO_ |
| **T2>(this IEnumerable<T2> src, Func<Range<T>, T2> convert)** | *IEnumerable<T2> Recombine<T,* | _TODO_ |
| **Sum(this IEnumerable<DateRange> src)** | *TimeSpan* | _TODO_ |
| **Sum(this IEnumerable<Range<DateTime>> src)** | *TimeSpan* | _TODO_ |
| **CompareTo(Range<T> other)** | *int* | _TODO_ |
| **Equals(Range<T> other)** | *bool* | _TODO_ |
| **ToString()** | *string* | _TODO_ |
| **ToString(string separator)** | *string* | _TODO_ |
| **ToString(string format, IFormatProvider formatProvider)** | *string* | _TODO_ |
| **Parse(string s, Func<string, T> convert, T dflt = default, string separator = "-")** | *Range<T>* | _TODO_ |
| **ToList(Func<T, T> succ)** | *List<T>* | _TODO_ |
| **ToList()** | *List<T>* | _TODO_ |
| **CombineWith(Range<T> other)** | *Range<T>* | _TODO_ |
| **IntersectWith(Range<T> other)** | *Range<T>* | _TODO_ |
| **IntersectsWith(Range<T> other)** | *bool* | _TODO_ |
| **FullyContains(Range<T> other)** | *bool* | _TODO_ |
| **Contains(T other)** | *bool* | _TODO_ |
| **Constraint(T min, T max)** | *Range<T>* | _TODO_ |
| **Constraint(Range<T> limit)** | *Range<T>* | _TODO_ |
| **Expand(T start, T stop)** | *Range<T>* | _TODO_ |
| **Expand(Range<T> other)** | *Range<T>* | _TODO_ |
| **Range<T>(T src)** | *implicit operator* | _TODO_ |
| **Sort(IEnumerable<Range<T>> src)** | *IEnumerable<Range<T>>* | _TODO_ |
| **Add(params Range<T>[] items)** | *IEnumerable<Range<T>>* | _TODO_ |
| **Add(T item)** | *IEnumerable<Range<T>>* | _TODO_ |
| **As<TDest>()** | *TDest* | _TODO_ |
| **Subtract(Range<T> other)** | *IEnumerable<Range<T>>* | _TODO_ |
| **Subtract(T item)** | *IEnumerable<Range<T>>* | _TODO_ |
| **Subtract(params Range<T>[] items)** | *IEnumerable<Range<T>>* | _TODO_ |
| **==(Range<T> a, Range<T> b)** | *bool operator* | _TODO_ |
| **!=(Range<T> a, Range<T> b)** | *bool operator* | _TODO_ |
| **+(Range<T> a, IEnumerable<Range<T>> b)** | *IEnumerable<Range<T>> operator* | _TODO_ |
| **+(Range<T> a, Range<T> b)** | *IEnumerable<Range<T>> operator* | _TODO_ |
| **+(Range<T> a, T b)** | *IEnumerable<Range<T>> operator* | _TODO_ |
| **-(Range<T> a, IEnumerable<Range<T>> b)** | *IEnumerable<Range<T>> operator* | _TODO_ |
| **-(Range<T> a, Range<T> b)** | *IEnumerable<Range<T>> operator* | _TODO_ |
| **-(Range<T> a, T b)** | *IEnumerable<Range<T>> operator* | _TODO_ |
| ***(Range<T> a, Range<T> b)** | *Range<T> operator* | _TODO_ |
| **GetHashCode()** | *int* | _TODO_ |
| **Recombine<T2>(IEnumerable<T2> src, Func<Range<T>, T2> convert)** | *IEnumerable<T2>* | _TODO_ |
| **Recombine(IEnumerable<Range<T>> src)** | *IEnumerable<Range<T>>* | _TODO_ |
| **Equals(object obj)** | *bool* | _TODO_ |
| **<(Range<T> left, Range<T> right)** | *bool operator* | _TODO_ |
| **<=(Range<T> left, Range<T> right)** | *bool operator* | _TODO_ |
| **>(Range<T> left, Range<T> right)** | *bool operator* | _TODO_ |
| **>=(Range<T> left, Range<T> right)** | *bool operator* | _TODO_ |
| **ConvertTo(Func<T, TDest> converter)** | *Range<TDest>* | _TODO_ |
| **ConvertTo()** | *Range<T>* | _TODO_ |
| **ConvertFrom(Range<TDest> src, Func<TDest, T> converter)** | *Range<T, TDest>* | _TODO_ |
| **TDest>(T src)** | *implicit operator Range<T,* | _TODO_ |
| **ToList()** | *List<int>* | _TODO_ |
| **Recombine(params int[] value)** | *IntRange[]* | _TODO_ |
| **Recombine(IEnumerable<IntRange> values)** | *IntRange[]* | _TODO_ |
| **TakeFirst(out IEnumerable<IntRange> left)** | *int* | _TODO_ |
| **Random()** | *int* | _TODO_ |
| **TakeRandom(out IEnumerable<IntRange> left)** | *int* | _TODO_ |
| **ExtractFirst()** | *int* | _TODO_ |
| **ToList()** | *List<IntRange>* | _TODO_ |
| **Add(int item)** | *void* | _TODO_ |
| **AddRange(IntRange t)** | *void* | _TODO_ |
| **ExtractRandom()** | *int* | _TODO_ |
| **IntRangeSet(IntRange[] src)** | *implicit operator* | _TODO_ |
| **IntRangeSet(List<IntRange> src)** | *implicit operator* | _TODO_ |
| **DateRange(DateTime src)** | *implicit operator* | _TODO_ |
| **FullMonthFromDate(DateTime src)** | *DateRange* | _TODO_ |
| **Expand(TimeSpan margin)** | *DateRange* | _TODO_ |
| **Equals(DateRange b, TimeSpan epsilon)** | *bool* | _TODO_ |
| **SplitByDay()** | *DateRange[]* | _TODO_ |
| **SplitByMonth()** | *DateRange[]* | _TODO_ |
| **SplitByYear()** | *DateRange[]* | _TODO_ |
| **SplitByHour()** | *DateRange[]* | _TODO_ |
| **SplitByHalfHour()** | *DateRange[]* | _TODO_ |
| **SplitByQuarterHour()** | *DateRange[]* | _TODO_ |
| **SplitByPoints(IEnumerable<DateTime> pts)** | *IEnumerable<DateRange>* | _TODO_ |

### Helpers.Serialize.cs

**Classes:** Helpers

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **JsonDeserializeWithDefault<T>(string s, Func<T> deflt = null)** | *T* | _TODO_ |
| **JsonDeserializeToAny(string s)** | *object* | _TODO_ |
| **JsonSerialize(object v)** | *string* | _TODO_ |

### Helpers.Strings.cs

**Classes:** Helpers

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **Params(this string template, params object[] parms)** | *string* | _TODO_ |
| **AsInt(this string s, int? min = null, int? max = null, int? outrangeDefault = null)** | *int* | _TODO_ |
| **AsLong(this string s, long? min = null, long? max = null, long? outrangeDefault = null)** | *long* | _TODO_ |
| **AsDouble(this string s, double? min = null, double? max = null, double? outrangeDefault = null)** | *double* | _TODO_ |
| **AsBool(this string s, params string[] trueValues)** | *bool* | _TODO_ |
| **AsBool(this string s, bool deflt)** | *bool* | _TODO_ |
| **AsBool(this string s)** | *bool?* | _TODO_ |
| **FromHex(this string s)** | *int* | _TODO_ |
| **ToString(this bool? b, string trueValue, string falseValue, string nullValue = null)** | *string* | _TODO_ |
| **ToString(this bool b, string trueValue, string falseValue)** | *string* | _TODO_ |
| **Repeat(this string s, int count)** | *string* | _TODO_ |
| **Ellipsis(this string s, int maxLength)** | *string* | _TODO_ |

### Helpers.Time.cs

**Classes:** Helpers

**Enums:** OffsetTimeUnit

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **DateTime(2001, 01, 01)** | *readonly DateTime Jan2001 =* | _TODO_ |
| **TrimSeconds(this DateTime d)** | *DateTime* | _TODO_ |
| **DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc)** | *readonly DateTime Year1970Start =* | _TODO_ |
| **DateTime(2000, 01, 01, 0, 0, 0, DateTimeKind.Utc)** | *readonly DateTime Year2000Start =* | _TODO_ |
| **DateTime(2020, 01, 01, 0, 0, 0, DateTimeKind.Utc)** | *readonly DateTime Year2020Start =* | _TODO_ |
| **ToOffsetTimeUnits(this TimeSpan t, OffsetTimeUnit mode)** | *long* | _TODO_ |
| **FromJan(this DateTime d, DateTime startOfYear, OffsetTimeUnit mode)** | *long* | _TODO_ |
| **FromJan1970(this DateTime d, OffsetTimeUnit mode)** | *long* | _TODO_ |
| **FromJan2000(this DateTime d, OffsetTimeUnit mode)** | *long* | _TODO_ |
| **FromJan2020(this DateTime d, OffsetTimeUnit mode)** | *long* | _TODO_ |
| **ToOffsetTimeUnits(this long t, OffsetTimeUnit mode)** | *TimeSpan* | _TODO_ |
| **FromJan(this long units, DateTime startOfYear, OffsetTimeUnit mode)** | *DateTime* | _TODO_ |
| **FromJan1970(this long units, OffsetTimeUnit mode)** | *DateTime* | _TODO_ |
| **FromJan2000(this long units, OffsetTimeUnit mode)** | *DateTime* | _TODO_ |
| **FromJan2020(this long units, OffsetTimeUnit mode)** | *DateTime* | _TODO_ |
| **FromUnixTimestamp(this long seconds)** | *DateTime* | _TODO_ |
| **ToUnixTimestamp(this DateTime time)** | *long* | _TODO_ |
| **GetMinuteId(this DateTime d)** | *long* | _TODO_ |
| **GetSecondId(this DateTime d)** | *long* | _TODO_ |
| **FromMinuteId(this long d)** | *DateTimeOffset* | _TODO_ |
| **FromSecondId(this long d)** | *DateTimeOffset* | _TODO_ |
| **GetOffsetFromGMTString(this string tz)** | *TimeSpan* | _TODO_ |
| **AsOffsetForDateTimeOffset(this TimeSpan offset)** | *string* | _TODO_ |
| **AsInt(this DayOfWeek dow)** | *int* | _TODO_ |
| **Constraint(this DateTime src, DateTime? min = null, DateTime? max = null, DateTime? outrangeDefault = null)** | *DateTime* | _TODO_ |
| **Constraint(this TimeSpan src, TimeSpan? min = null, TimeSpan? max = null, TimeSpan? outrangeDefault = null)** | *TimeSpan* | _TODO_ |
| **Constraint(this DateTime src, DateTime min)** | *DateTime* | _TODO_ |
| **Constraint(this TimeSpan src, TimeSpan min)** | *TimeSpan* | _TODO_ |
| **Sum(this IEnumerable<TimeSpan> src)** | *TimeSpan* | _TODO_ |
| **Sum<T>(this IEnumerable<T> src, Func<T, TimeSpan> select)** | *TimeSpan* | _TODO_ |
| **Age(this DateTime date)** | *double* | _TODO_ |
| **IsToday(this DateTime d)** | *bool* | _TODO_ |
| **IsCurrentWeek(this DateTime d)** | *bool* | _TODO_ |
| **IsCurrentMonth(this DateTime d)** | *bool* | _TODO_ |
| **IsCurrentYear(this DateTime d)** | *bool* | _TODO_ |
| **IsWeekend(this DateTime d)** | *bool* | _TODO_ |
| **RoundMinutes(this DateTime src, double minutes)** | *DateTime* | _TODO_ |
| **NearestHour(this DateTime src)** | *DateTime* | _TODO_ |
| **Constraint(this TimeSpan src)** | *TimeSpan* | _TODO_ |
| **ToHoursMinutes(this TimeSpan t, bool ignoreNegative = false)** | *string* | _TODO_ |
| **ToHoursMinutes2(this TimeSpan t, bool ignoreNegative = false)** | *string* | _TODO_ |
| **StartOfWeek(this DateTime src)** | *DateTime* | _TODO_ |
| **LastDayOfWeek(this DateTime src)** | *DateTime* | _TODO_ |
| **EndOfWeek(this DateTime src)** | *DateTime* | _TODO_ |
| **StartOfMonth(this DateTime src)** | *DateTime* | _TODO_ |
| **EndOfMonth(this DateTime src)** | *DateTime* | _TODO_ |
| **LastDayOfMonth(this DateTime src)** | *DateTime* | _TODO_ |
| **StartOfYear(this DateTime src)** | *DateTime* | _TODO_ |
| **EndOfYear(this DateTime src)** | *DateTime* | _TODO_ |
| **LastDayOfYear(this DateTime src)** | *DateTime* | _TODO_ |
| **StartOfHour(this DateTime src, double addHours = 0)** | *DateTime* | _TODO_ |
| **EndOfHour(this DateTime src, double addHours = 0)** | *DateTime* | _TODO_ |
| **StartOfNthHour(this DateTime src, int n, double addHours = 0)** | *DateTime* | _TODO_ |
| **EndOfNthHour(this DateTime src, int n, double addHours = 0)** | *DateTime* | _TODO_ |
| **StartOfHalfHour(this DateTime src, double addHours = 0)** | *DateTime* | _TODO_ |
| **EndOfHalfHour(this DateTime src, double addHours = 0)** | *DateTime* | _TODO_ |
| **StartOfQuarterHour(this DateTime src, double addHours = 0)** | *DateTime* | _TODO_ |
| **EndOfQuarterHour(this DateTime src, double addHours = 0)** | *DateTime* | _TODO_ |
| **StartOfMinute(this DateTime src, double addMinutes = 0)** | *DateTime* | _TODO_ |
| **EndOfMinute(this DateTime src, double addMinutes = 0)** | *DateTime* | _TODO_ |
| **IsYesterday(this DateTime src, int offset = 1)** | *bool* | _TODO_ |
| **IsTomorrow(this DateTime src, int offset = 1)** | *bool* | _TODO_ |
| **ToNativeDate(this DateTime src, string timeFormat = null)** | *string* | _TODO_ |
| **ToNativeDateShort(this DateTime src)** | *string* | _TODO_ |
| **ToFriendlyString(this TimeSpan ts, RussianCase rc = RussianCase.Nominative)** | *string* | _TODO_ |

### Helpers.Types.cs

**Classes:** Helpers


### Helpers.Uri.cs

**Classes:** Helpers

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **ReplaceHost(this Uri uri, string host)** | *Uri* | _TODO_ |
| **IsValidIpV4(this string ip)** | *bool* | _TODO_ |

### Helpers.Video.cs

**Classes:** Helpers

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **Yuv12ToRgb(IntPtr srcPtr, int w, int h, ref byte[] dst, out int stride)** | *void* | _TODO_ |

### Options.cs

**Classes:** OptionValueCollection, OptionContext, OptionException, OptionSet

**Enums:** OptionValueType

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **Count** | *int* | _TODO_ |
| **IsReadOnly** | *bool* | _TODO_ |
| **Option** | *Option* | _TODO_ |
| **OptionName** | *string* | _TODO_ |
| **OptionIndex** | *int* | _TODO_ |
| **OptionSet** | *OptionSet* | _TODO_ |
| **OptionValues** | *OptionValueCollection* | _TODO_ |
| **Prototype** | *string* | _TODO_ |
| **Description** | *string* | _TODO_ |
| **OptionValueType** | *OptionValueType* | _TODO_ |
| **MaxValueCount** | *int* | _TODO_ |
| **OptionName** | *string* | _TODO_ |
| **Add(string item)** | *void* | _TODO_ |
| **Clear()** | *void* | _TODO_ |
| **Contains(string item)** | *bool* | _TODO_ |
| **CopyTo(string[] array, int arrayIndex)** | *void* | _TODO_ |
| **Remove(string item)** | *bool* | _TODO_ |
| **GetEnumerator()** | *IEnumerator<string>* | _TODO_ |
| **IndexOf(string item)** | *int* | _TODO_ |
| **Insert(int index, string item)** | *void* | _TODO_ |
| **RemoveAt(int index)** | *void* | _TODO_ |
| **ToList()** | *List<string>* | _TODO_ |
| **ToArray()** | *string[]* | _TODO_ |
| **ToString()** | *string* | _TODO_ |
| **GetNames()** | *string[]* | _TODO_ |
| **GetValueSeparators()** | *string[]* | _TODO_ |
| **Invoke(OptionContext c)** | *void* | _TODO_ |
| **ToString()** | *string* | _TODO_ |
| **GetObjectData(SerializationInfo info, StreamingContext context)** | *void* | _TODO_ |
| **TValue>(TKey key, TValue value)** | *delegate void OptionAction<TKey,* | _TODO_ |
| **Add(Option option)** | *OptionSet* | _TODO_ |
| **Add(string prototype, Action<string> action)** | *OptionSet* | _TODO_ |
| **Add(string prototype, string description, Action<string> action)** | *OptionSet* | _TODO_ |
| **Add(string prototype, OptionAction<string, string> action)** | *OptionSet* | _TODO_ |
| **Add(string prototype, string description, OptionAction<string, string> action)** | *OptionSet* | _TODO_ |
| **Add<T>(string prototype, Action<T> action)** | *OptionSet* | _TODO_ |
| **Add<T>(string prototype, string description, Action<T> action)** | *OptionSet* | _TODO_ |
| **TValue>(string prototype, OptionAction<TKey, TValue> action)** | *OptionSet Add<TKey,* | _TODO_ |
| **TValue>(string prototype, string description, OptionAction<TKey, TValue> action)** | *OptionSet Add<TKey,* | _TODO_ |
| **Parse(IEnumerable<string> arguments)** | *List<string>* | _TODO_ |
| **Parse(IEnumerable<string> arguments)** | *List<string>* | _TODO_ |
| **WriteOptionDescriptions(TextWriter o)** | *void* | _TODO_ |
| **WriteOptionDescriptions(Action<string> line)** | *void* | _TODO_ |

### Program.Base.Logs.cs


### Program.Base.cs


### Validate.Email.cs

**Classes:** Email

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **FilterEmail(this string email)** | *string* | _TODO_ |
| **Filter(string email)** | *string* | _TODO_ |
| **IsValidEmail(this string s)** | *bool* | _TODO_ |
| **IsValid(string s)** | *bool* | _TODO_ |

### Validate.Phone.cs

**Classes:** Phone

| **Member** | *Type* | *Description* |
|-------------|-------|---------------|
| **CountryCode** | *string* | _TODO_ |
| **AreaCode** | *string* | _TODO_ |
| **Number** | *string* | _TODO_ |
| **Format(string phone, bool format)** | *string* | _TODO_ |
| **Clear()** | *void* | _TODO_ |
| **Parse(string s)** | *void* | _TODO_ |
| **ToString(bool format)** | *string* | _TODO_ |
| **ToString()** | *string* | _TODO_ |
## License

MIT

