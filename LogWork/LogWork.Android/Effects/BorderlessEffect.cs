using LogWork.Droid.Effects;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static Android.Views.ViewGroup;

[assembly: ResolutionGroupName("LogWork")]
[assembly: ExportEffect(typeof(BorderlessEffect), "BorderlessEffect")]

namespace LogWork.Droid.Effects
{
    public class BorderlessEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                Control.Background = null;

                if (Element is DatePicker || Element is Picker || Element is TimePicker)
                {
                    var layoutParams = new MarginLayoutParams(Control.LayoutParameters);
                    layoutParams.SetMargins(0, 0, 0, 0);
                    Control.LayoutParameters = layoutParams;
                    Control.LayoutParameters = layoutParams;
                    Control.SetPadding(0, 0, 0, 0);
                    Control.SetPadding(0, 0, 0, 0);
                }
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