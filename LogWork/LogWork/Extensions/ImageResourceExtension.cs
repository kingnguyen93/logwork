using System;
using System.Reflection;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Extensions
{
    [Preserve(AllMembers = true)]
    [ContentProperty("Source")]
    public class ImageResourceExtension : BindableObject, IMarkupExtension
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

        /// <summary>
        /// Init ImageResourceExtension
        /// </summary>
        /// <param name="defaultResource"></param>
        /// <param name="assembly"></param>
        public static void InitImageResourceExtension(string defaultResource, Assembly assembly)
        {
            Assembly = assembly;
            DefaultResource = defaultResource;
        }

        #endregion Init

        #region IMarkupExtension Implementation

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Key == null)
                return null;

            var strAssemblyName = Assembly.GetName().Name;
            var resource = string.IsNullOrEmpty(Resource) ? DefaultResource : Resource;
            var imageSource = ImageSource.FromResource($"{strAssemblyName}.{resource}.{Key}", Assembly);

            return imageSource;
        }

        #endregion IMarkupExtension Implementation
    }
}