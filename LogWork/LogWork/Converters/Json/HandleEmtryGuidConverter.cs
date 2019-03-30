using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Xamarin.Forms.Converters
{
    public class HandleEmtryGuidConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(Guid));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try { return JToken.Load(reader).ToObject(objectType); }
            catch { }
            return default;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.Equals(Guid.Empty) ? "" : value);
        }
    }
}