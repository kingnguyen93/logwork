using System;
using System.ComponentModel;
using LogWork.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(DatePicker), typeof(DatePickerLocalRenderer))]
namespace LogWork.iOS.Renderers
{
    public class DatePickerLocalRenderer: DatePickerRenderer
    {
        public DatePickerLocalRenderer()
        {
        }
        protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
        {
            base.OnElementChanged(e);
            var date = (UIDatePicker)Control.InputView;
            date.Locale = new Foundation.NSLocale("fr-FR");
           
        }
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            Control.Layer.BorderWidth = 0;
            Control.BorderStyle = UITextBorderStyle.None;
        }
    }
}
