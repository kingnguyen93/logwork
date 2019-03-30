using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Extensions
{
    [Preserve(AllMembers = true)]
    [ContentProperty("Text")]
    public class TranslateExtension : BindableObject, IMarkupExtension
    {
        #region BindableObject

        public static readonly BindableProperty KeyProperty = BindableProperty.Create(nameof(Key), typeof(string), typeof(string), null, BindingMode.TwoWay, null);

        public string Key
        {
            get { return (string)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        public static readonly BindableProperty ResourceProperty = BindableProperty.Create(nameof(Resource), typeof(string), typeof(string), null, BindingMode.TwoWay, null);

        public string Resource
        {
            get { return (string)GetValue(ResourceProperty); }
            set { SetValue(ResourceProperty, value); }
        }

        #endregion BindableObject

        #region Init

        public static Assembly Assembly;
        public static string DefaultResource;
        public static CultureInfo CurrentCultureInfo;
        public static ResourceManager CurrentResourceManager;

        /// <summary>
        /// Init TranslateExtension
        /// </summary>
        /// <param name="defaultResource"></param>
        /// <param name="currentCultureInfo"></param>
        /// <param name="assembly"></param>
        public static void InitTranslateExtension(string defaultResource, CultureInfo currentCultureInfo, Assembly assembly)
        {
            Assembly = assembly;
            DefaultResource = defaultResource;
            CurrentCultureInfo = currentCultureInfo;
        }

        #endregion Init

        #region IMarkupExtension Implementation

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            var strAssemblyName = Assembly.GetName().Name;
            if (string.IsNullOrEmpty(Resource))
            {
                CurrentResourceManager = new ResourceManager($"{strAssemblyName}.{DefaultResource}", Assembly);
            }
            else
            {
                CurrentResourceManager = new ResourceManager($"{strAssemblyName}.{Resource}", Assembly);
            }
            var translation = CurrentResourceManager.GetString(Key, CurrentCultureInfo);
            if (translation == null)
            {
                translation = Key;
            }
            return translation;
        }

        #endregion IMarkupExtension Implementation

        public static string GetValue(string key, string resource = null)
        {
            var strAssemblyName = Assembly.GetName().Name;
            if (string.IsNullOrEmpty(resource))
            {
                CurrentResourceManager = new ResourceManager($"{strAssemblyName}.{DefaultResource}", Assembly);
            }
            else
            {
                CurrentResourceManager = new ResourceManager($"{strAssemblyName}.{resource}", Assembly);
            }
            var translation = CurrentResourceManager.GetString(key, CurrentCultureInfo);
            if (translation == null)
            {
                translation = key;
            }
            return translation;
        }
    }
}