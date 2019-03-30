using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Xamarin.Forms.Extensions
{
    public static class JsonExtension
    {
        private static string[] IgnorePropertyName = new string[]
        {
            "IsBusy",
            "AccountId",
            "Nonce",
            "IsToSync"
        };

        public static JToken RemoveEmptyChildren(this JToken token, string propertyIgnore = null)
        {
            if (token.Type == JTokenType.Object)
            {
                JObject copy = new JObject();
                foreach (JProperty prop in token.Children<JProperty>())
                {
                    JToken child = prop.Value;
                    if (child.HasValues)
                    {
                        child = child.RemoveEmptyChildren();
                    }
                    if (!IgnorePropertyName.Contains(prop.Name) && (!propertyIgnore?.Contains(prop.Name) ?? true) && !child.IsNullOrEmpty())
                    {
                        copy.Add(prop.Name, child);
                    }
                }
                return copy;
            }
            else if (token.Type == JTokenType.Array)
            {
                JArray copy = new JArray();
                foreach (JToken item in token.Children())
                {
                    JToken child = item;
                    if (child.HasValues)
                    {
                        child = child.RemoveEmptyChildren();
                    }
                    if (!child.IsNullOrEmpty())
                    {
                        copy.Add(child);
                    }
                }
                return copy;
            }
            return token;
        }

        public static bool IsNullOrEmpty(this JToken token)
        {
            return (token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.Guid && token.ToString() == Guid.Empty.ToString()) ||
                   (token.Type == JTokenType.String && token.ToString() == string.Empty) ||
                   (token.Type == JTokenType.Null);
        }
    }
}