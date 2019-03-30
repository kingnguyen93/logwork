using System;
using System.Diagnostics;

namespace Xamarin.Forms.Behaviors
{
    public class ZoomInAnimationBehavior : BindableBehavior<View>
    {
        protected override void OnAttachedTo(View visualElement)
        {
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += OnElementTapped;
            visualElement.GestureRecognizers.Add(tapGestureRecognizer);

            base.OnAttachedTo(visualElement);
        }

        protected override void OnDetachingFrom(View visualElement)
        {
            visualElement.GestureRecognizers.Clear();

            base.OnDetachingFrom(visualElement);
        }

        private async void OnElementTapped(object sender, EventArgs e)
        {
            try
            {
                if (sender is View visualElement)
                {
                    await visualElement.ScaleTo(.9, 150);
                    await visualElement.ScaleTo(1, 150);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}