using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Xamarin.Forms.Extensions;

namespace Xamarin.Forms.Converters
{
    public class DateToUnixConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(DateTime)) || objectType.Equals(typeof(DateTime?));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try { return JToken.Load(reader).ToObject(objectType); }
            catch { }
            return default;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime?)
                serializer.Serialize(writer, ((DateTime?)value)?.ToUnixTimeSeconds(true));
            else
                serializer.Serialize(writer, ((DateTime)value).ToUnixTimeSeconds(true));
        }
    }
}