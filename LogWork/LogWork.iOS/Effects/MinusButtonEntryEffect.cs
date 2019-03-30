using LogWork.iOS.Effects;
using UIKit;
using Xamarin.Forms;

[assembly: ExportEffect(typeof(MinusButtonEntryEffect), "MinusButtonEntryEffect")]

namespace LogWork.iOS.Effects
{
    public class MinusButtonEntryEffect : PlatformEffect<UIView, UITextField>
    {
        protected override void OnAttached()
        {
            if (Control == null)
                return;

            if (!(Element is Entry element))
                return;

            UIBarButtonItem button = new UIBarButtonItem("-", UIBarButtonItemStyle.Plain, (sender, args) =>
            {
                var position = Control.SelectedTextRange.Start;
                var idx = (int)Control.GetOffsetFromPosition(Control.BeginningOfDocument, position);
                element.Text = element.Text.Insert(idx, "-");
            });
            UIToolbar toolbar = new UIToolbar()
            {
                Items = new[] { button }
            };
            Control.InputAccessoryView = toolbar;
        }

        protected override void OnDetached()
        {
            Control.InputAccessoryView = null;
        }
    }
}