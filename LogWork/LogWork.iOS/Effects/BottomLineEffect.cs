using CoreAnimation;
using LogWork.iOS.Effects;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(BottomLineEffect), "UnderlineEntryEffect")]

namespace LogWork.iOS.Effects
{
    public class BottomLineEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                if (Element is Entry entry)
                {
                    var borderLayer = new CALayer
                    {
                        MasksToBounds = true,
                        Frame = new CoreGraphics.CGRect(0f, entry.Height - 1, entry.Width, 1f),
                        BorderColor = Color.DimGray.ToCGColor(),
                        BorderWidth = 1.0f
                    };
                    Control.Layer.AddSublayer(borderLayer);
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