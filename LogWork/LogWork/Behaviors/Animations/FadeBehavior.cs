using System.Threading.Tasks;

namespace Xamarin.Forms.Behaviors
{
    public class FadeBehavior : AnimationBaseBehavior
    {
        public static readonly BindableProperty FinalOpacityProperty = BindableProperty.Create(nameof(FinalOpacity), typeof(double), typeof(FadeBehavior), 1);

        /// <summary>
        /// Final opacity, default: 1
        /// </summary>
        public double FinalOpacity
        {
            get { return (double)GetValue(FinalOpacityProperty); }
            set { SetValue(FinalOpacityProperty, value); }
        }

        protected override async Task DoAnimation(View element)
        {
            await element.FadeTo(FinalOpacity, (uint)Duration, GetEasingMethodFromEnumerator());
        }
    }
}