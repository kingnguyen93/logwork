namespace Xamarin.Forms.Controls
{
    public class ExtendedViewCell : ViewCell
    {
        public static readonly BindableProperty SelectedBackgroundColorProperty = BindableProperty.Create(nameof(SelectedBackgroundColor), typeof(Color), typeof(ExtendedViewCell), Color.Default);

        public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(ExtendedViewCell), default);

        public Color SelectedBackgroundColor
        {
            get { return (Color)GetValue(SelectedBackgroundColorProperty); }
            set { SetValue(SelectedBackgroundColorProperty, value); }
        }

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public delegate void ChangeBackgroundDelegate(bool selected);

        public ChangeBackgroundDelegate Selected { get; set; }

        public ExtendedViewCell()
        {
        }
    }
}