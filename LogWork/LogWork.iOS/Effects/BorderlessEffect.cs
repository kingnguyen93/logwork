using LogWork.iOS.Effects;
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("LogWork")]
[assembly: ExportEffect(typeof(BorderlessEffect), "BorderlessEffect")]

namespace LogWork.iOS.Effects
{
    public class BorderlessEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                Control.Layer.BorderWidth = 0;
                if (Control is UITextField entry)
                {
                    entry.BorderStyle = UITextBorderStyle.None;
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