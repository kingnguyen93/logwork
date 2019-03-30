using CoreGraphics;
using LogWork.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Entry), typeof(DefaultEntryRenderer))]

namespace LogWork.iOS.Renderers
{
    public class DefaultEntryRenderer : EntryRenderer
    {
        public DefaultEntryRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null && Element != null)
            {
                // Check only for Numeric keyboard
                if (Element.Keyboard == Keyboard.Numeric && Control != null)
                {
                    Control.KeyboardType = UIKeyboardType.DecimalPad | UIKeyboardType.NumbersAndPunctuation;
                    AddDoneButton();
                }
            }
        }

        /// <summary>
        /// <para>Add toolbar with Done button</para>
        /// </summary>
        protected void AddDoneButton()
        {
            var toolbar = new UIToolbar(new CGRect(0.0f, 0.0f, Control.Frame.Size.Width, 44.0f))
            {
                Items = new UIBarButtonItem[] {
                    new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
                    new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
                    {
                        Control.ResignFirstResponder();
                        ((IEntryController)Element)?.SendCompleted();
                    })
                }
            };
            Control.InputAccessoryView = toolbar;
        }
    }
}