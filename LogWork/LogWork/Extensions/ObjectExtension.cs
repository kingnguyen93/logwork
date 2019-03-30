using Newtonsoft.Json;

namespace Xamarin.Forms.Extensions
{
    public static class ObjectExtension
    {
        public static T DeepCopy<T>(this T other)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(other));
        }
    }
}