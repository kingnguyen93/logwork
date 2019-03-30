using System.ComponentModel;
using LogWork.iOS.Renderers;

using Xamarin.Forms;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExtendedSearchBar), typeof(ExtendedSearchBarRenderer))]

namespace LogWork.iOS.Renderers
{
    public class ExtendedSearchBarRenderer : SearchBarRenderer
    {
        public ExtendedSearchBarRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null && Element != null)
            {
                if (Element is ExtendedSearchBar searchBar)
                {
                    if (searchBar.BarTintColor != Color.Default)
                    {
                        //Control.SearchBarStyle = UIKit.UISearchBarStyle.Minimal;
                        Control.BarTintColor = searchBar.BarTintColor.ToUIColor();
                    }

                    Control.ShowsCancelButton = false;
                    Control.ShowsSearchResultsButton = false;
                    Control.EnablesReturnKeyAutomatically = false;

                    Control.SearchButtonClicked += (sender, arg) =>
                    {
                        Element.SearchCommand?.Execute(Element.Text);
                    };
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == SearchBar.TextColorProperty.PropertyName)
            {
                Control.ShowsCancelButton = false;
            }
            else if (e.PropertyName == ExtendedSearchBar.BarTintColorProperty.PropertyName)
            {
                if (Element is ExtendedSearchBar searchBar)
                {
                    if (searchBar.BarTintColor != Color.Default)
                    {
                        Control.BarTintColor = searchBar.BarTintColor.ToUIColor();
                    }
                }
            }

        }
    }
}