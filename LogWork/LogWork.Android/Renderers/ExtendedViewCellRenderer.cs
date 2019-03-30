using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using LogWork.Droid.Renderers;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ExtendedViewCell), typeof(ExtendedViewCellRenderer))]

namespace LogWork.Droid.Renderers
{
    public class ExtendedViewCellRenderer : ViewCellRenderer
    {
        private Android.Views.View cellCore;
        private Drawable unselectedBackground;

        protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, ViewGroup parent, Context context)
        {
            cellCore = base.GetCellCore(item, convertView, parent, context);
            unselectedBackground = cellCore.Background;
            return cellCore;
        }

        protected override void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnCellPropertyChanged(sender, e);

            if (e.PropertyName == ExtendedViewCell.IsSelectedProperty.PropertyName)
            {
                if (sender is ExtendedViewCell extendedViewCell)
                {
                    if (extendedViewCell.IsSelected)
                    {
                        cellCore.SetBackgroundColor(extendedViewCell.SelectedBackgroundColor.ToAndroid());
                    }
                    else
                    {
                        cellCore.SetBackground(unselectedBackground);
                    }
                }
            }
        }
    }
}