using Android.Content;
using Android.OS;
using LogWork.Droid.Renderers;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ExtendedSearchBar), typeof(ExtendedSearchBarRenderer))]

namespace LogWork.Droid.Renderers
{
    public class ExtendedSearchBarRenderer : SearchBarRenderer
    {
        public ExtendedSearchBarRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null && Element != null)
            {
                // WorkAround to searchBar not appearing in newer android versions
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                    Element.HeightRequest = 40;

                if (Element is ExtendedSearchBar searchBar)
                {
                    Element.BackgroundColor = searchBar.BarTintColor;
                    //Control.SetBackgroundColor(searchBar.BarTintColor.ToAndroid());
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == ExtendedSearchBar.BarTintColorProperty.PropertyName)
            {
                if (Element is ExtendedSearchBar searchBar)
                {
                    Element.BackgroundColor = searchBar.BarTintColor;
                    //Control.SetBackgroundColor(searchBar.BarTintColor.ToAndroid());
                }
            }
        }
    }
}