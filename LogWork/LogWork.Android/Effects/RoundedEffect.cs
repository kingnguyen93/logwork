using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using LogWork.Droid.Effects;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(RoundedEffect), "RoundedEffect")]

namespace LogWork.Droid.Effects
{
    public class RoundedEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                if (Element is SearchBar searchBar)
                {
                    searchBar.HeightRequest = 55;

                    var plate = Control.FindViewById(Control.Resources.GetIdentifier("android:id/search_plate", null, null));
                    if (plate != null)
                        plate.Background = new ColorDrawable(Android.Graphics.Color.Transparent);

                    Control.Background = ContextCompat.GetDrawable(Android.App.Application.Context, Resource.Drawable.RoundedSearchBarBackground);
                    Control.SetForegroundGravity(Android.Views.GravityFlags.CenterVertical);

                    return;
                }

                Control.SetPadding(20, 18, 20, 18);

                Control.Background = ContextCompat.GetDrawable(Android.App.Application.Context, Resource.Drawable.RoundedBackground);
                Control.SetForegroundGravity(Android.Views.GravityFlags.CenterVertical);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot set property on attached control. Error: ", ex.Message);
            }
        }

        protected override void OnDetached()
        {
        }
    }
}