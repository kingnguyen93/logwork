using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xamarin.Forms.Extensions
{
    public static class PropertyExtension
    {
        public static void UpdateProperties<TSource, TValue>(TSource from, TValue to, params string[] ignore)
        {
            try
            {
                if (from != null && to != null)
                {
                    Type typeFrom = typeof(TSource);
                    Type typeTo = typeof(TValue);

                    foreach (PropertyInfo toPropertyInfo in typeTo.GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance))
                    {
                        if (!ignore.Contains(toPropertyInfo.Name) && toPropertyInfo.CanWrite)
                        {
                            object fromValue = typeFrom.GetProperty(toPropertyInfo.Name)?.GetValue(from, null);
                            object toValue = typeTo.GetProperty(toPropertyInfo.Name)?.GetValue(to, null);

                            if (fromValue != null && toValue != null && fromValue != toValue && !fromValue.Equals(toValue))
                            {
                                //toValue = fromValue;
                                toPropertyInfo.SetValue(to, Convert.ChangeType(fromValue, toValue.GetType()), default);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Update Properties of " + typeof(TValue).Name + " error: " + ex);
            }
        }

        public static void UpdatePropertiesFrom<TValue, TSource>(this TValue to, TSource from, params string[] ignore)
        {
            try
            {
                if (from != null && to != null)
                {
                    Type typeFrom = typeof(TSource);
                    Type typeTo = typeof(TValue);

                    foreach (PropertyInfo toPropertyInfo in typeTo.GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance))
                    {
                        if (!ignore.Contains(toPropertyInfo.Name) && toPropertyInfo.CanWrite)
                        {
                            object fromValue = typeFrom.GetProperty(toPropertyInfo.Name)?.GetValue(from, null);
                            object toValue = typeTo.GetProperty(toPropertyInfo.Name)?.GetValue(to, null);
                            
                            if (fromValue != null && toValue != null && fromValue != toValue && !fromValue.Equals(toValue))
                            {
                                //toValue = fromValue;
                                toPropertyInfo.SetValue(to, Convert.ChangeType(fromValue, toValue.GetType()), default);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Update Properties of " + typeof(TValue).Name + " error: " + ex);
            }
        }

        public static bool HasJsonPropertyAttribute<T>(this T source)
        {
            return Attribute.IsDefined(typeof(T), typeof(JsonProperty));
        }
    }
}