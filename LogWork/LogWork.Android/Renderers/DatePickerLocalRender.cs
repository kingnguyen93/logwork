using System;
using LogWork.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Content;
using Java.Util;

[assembly: ExportRenderer(typeof(DatePicker), typeof(DatePickerLocalRender))]
namespace LogWork.Droid.Renderers
{
    public class DatePickerLocalRender: DatePickerRenderer
    {
        public DatePickerLocalRender(Context context) : base(context)
        {

        }
        protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
        {
            base.OnElementChanged(e);
            Locale locale = new Locale("fr-FR");
            Control.TextLocale = locale;

        }
    }
}
