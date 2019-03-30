using System;
using System.Diagnostics;

namespace Xamarin.Forms.Behaviors
{
    public class ButtonZoomInAnimationBehavior : BindableBehavior<Button>
    {
        protected override void OnAttachedTo(Button bindable)
        {
            bindable.Clicked += OnButtonClicked;

            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(Button bindable)
        {
            bindable.Clicked -= OnButtonClicked;

            base.OnDetachingFrom(bindable);
        }

        private async void OnButtonClicked(object sender, EventArgs e)
        {
            try
            {
                if (sender is Button button)
                {
                    await button.ScaleTo(.9, 150);
                    await button.ScaleTo(1, 150);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}