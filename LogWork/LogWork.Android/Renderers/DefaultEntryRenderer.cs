using Android.Content;
using Android.Text;
using LogWork.Droid.Renderers;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Entry), typeof(DefaultEntryRenderer))]

namespace LogWork.Droid.Renderers
{
    public class DefaultEntryRenderer : EntryRenderer
    {
        public DefaultEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null && Element != null)
            {
                if (Element.Keyboard == Keyboard.Numeric && Control != null)
                {
                    Control.InputType = InputTypes.ClassNumber | InputTypes.NumberFlagSigned | InputTypes.NumberFlagDecimal;
                }
            }
        }
    }
}