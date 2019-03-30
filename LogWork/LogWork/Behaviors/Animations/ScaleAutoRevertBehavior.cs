using System.Threading.Tasks;

namespace Xamarin.Forms.Behaviors
{
    public class ScaleAutoRevertBehavior : AnimationBaseBehavior
    {
        public static readonly BindableProperty FinalScaleProperty = BindableProperty.Create(nameof(FinalScale), typeof(double), typeof(ScaleAutoRevertBehavior), 1);
        public static readonly BindableProperty IsRelativeProperty = BindableProperty.Create(nameof(IsRelative), typeof(bool), typeof(ScaleBehavior), false);

        /// <summary>
        /// Final scale, default: 1
        /// </summary>
        public double FinalScale
        {
            get { return (double)GetValue(FinalScaleProperty); }
            set { SetValue(FinalScaleProperty, value); }
        }

        /// <summary>
        /// Use relative or absolute scaling, default absolute
        /// </summary>
        public bool IsRelative
        {
            get { return (bool)GetValue(IsRelativeProperty); }
            set { SetValue(IsRelativeProperty, value); }
        }

        protected override async Task DoAnimation(View element)
        {
            if (IsRelative)
            {
                await element.RelScaleTo(FinalScale, (uint)Duration, GetEasingMethodFromEnumerator());
                await element.RelScaleTo(1, (uint)Duration, GetEasingMethodFromEnumerator());
            }
            else
            {
                await element.ScaleTo(FinalScale, (uint)Duration, GetEasingMethodFromEnumerator());
                await element.ScaleTo(1, (uint)Duration, GetEasingMethodFromEnumerator());
            }
        }
    }
}