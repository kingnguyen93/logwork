namespace Xamarin.Forms.Controls
{
    public class Separator : View
    {
        public enum SeparatorOrientation
        {
            Vertical = 0,
            Horizontal = 1
        }

        public static readonly BindableProperty OrientationProperty = BindableProperty.Create(nameof(Orientation), typeof(SeparatorOrientation), typeof(Separator), SeparatorOrientation.Horizontal, propertyChanged: OnOrientationChanged);

        public SeparatorOrientation Orientation
        {
            get { return (SeparatorOrientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public Separator()
        {
            if (Orientation == SeparatorOrientation.Horizontal)
            {
                HeightRequest = 1;
            }
            else
            {
                WidthRequest = 1;
            }
        }

        private static void OnOrientationChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is Separator control))
                return;

            if (control.Orientation == SeparatorOrientation.Horizontal)
            {
                control.HeightRequest = 1;
            }
            else
            {
                control.WidthRequest = 1;
            }
        }
    }
}