using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;

namespace Xamarin.Forms.Converters
{
    public class HandleDataTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try { return JToken.Load(reader).ToObject(objectType); }
            catch (Exception ex) { Debug.WriteLine("Deserialize Error: " + ex); }
            return objectType.IsValueType ? Activator.CreateInstance(objectType) : null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}