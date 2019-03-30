using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TinyMVVM;

namespace LogWork
{
    public static class Public
    {
        public static decimal GetDecimal(object input)
        {
            try
            {
                return Convert.ToDecimal(input);
            }
            catch
            {
                return 0;
            }
        }

        public static bool SetProperty<TSource, TValue>(this TSource source, string propertyName, TValue value) where TSource : TinyModel
        {
            try
            {
                if (source is TinyModel model)
                {
                    typeof(TSource).GetProperty(propertyName)?.SetValue(source, value);

                    model.OnPropertyChanged(propertyName);

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Update Property " + typeof(TValue).Name + " error: " + ex.Message);
                return false;
            }
        }
    }
}
