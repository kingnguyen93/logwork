using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Xamarin.Forms.Converters
{
    public class IgnoreDataTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsValueType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try { return JToken.Load(reader).ToObject(objectType); }
            catch { }
            return objectType.IsValueType ? Activator.CreateInstance(objectType) : null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}