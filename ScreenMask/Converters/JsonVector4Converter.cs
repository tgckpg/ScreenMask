using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ScreenMask.Converters
{
	class JsonVector4Converter : JsonConverter<Vector4>
	{
		public override Vector4 Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
		{
			string V = reader.GetString();
			IEnumerator<float> V4Float = V.Split( " " ).Select( x => float.Parse( x ) ).GetEnumerator();
			Vector4 V4 = new Vector4();
			V4Float.MoveNext();
			V4.X = V4Float.Current;
			V4Float.MoveNext();
			V4.Y = V4Float.Current;
			V4Float.MoveNext();
			V4.Z = V4Float.Current;
			V4Float.MoveNext();
			V4.W = V4Float.Current;
			return V4;
		}

		public override void Write( Utf8JsonWriter writer, Vector4 value, JsonSerializerOptions options )
		{
			writer.WriteStringValue( $"{value.X} {value.Y} {value.Z} {value.W}" );
		}
	}
}
