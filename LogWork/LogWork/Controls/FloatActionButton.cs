using System;
using System.Windows.Input;

namespace Xamarin.Forms.Controls
{
    public class FloatActionButton : Frame
    {
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(FloatActionButton), null);

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(FloatActionButton), null);

        public static readonly BindableProperty SizeProperty = BindableProperty.Create(nameof(Size), typeof(double), typeof(FloatActionButton), 55d, propertyChanged: (b, o, n) => ((FloatActionButton)b).OnSizeChanged(b, o, n));

        public static readonly BindableProperty IconProperty = BindableProperty.Create(nameof(Icon), typeof(string), typeof(FloatActionButton), null, propertyChanged: (b, o, n) => ((FloatActionButton)b).OnIconChanged(b, o, n));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public double Size
        {
            get { return (double)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public event EventHandler Clicked;

        public FloatActionButton()
        {
            HasShadow = false;
            Margin = new Thickness(16);
            Padding = new Thickness(0);

            Init();
        }

        private void Init()
        {
            InitSize();

            var tap = new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await this.ScaleTo(.85, 150, easing: Easing.Linear);
                        await this.ScaleTo(1, 150, easing: Easing.Linear);

                        var param = CommandParameter ?? null;

                        if (Command != null && Command.CanExecute(param))
                        {
                            Command.Execute(param);
                        }

                        Clicked?.Invoke(this, EventArgs.Empty);
                    });
                })
            };

            GestureRecognizers.Add(tap);
        }

        private void InitSize()
        {
            HeightRequest = Size;
            WidthRequest = Size;
            CornerRadius = (float)Size / 2;
        }

        private void OnSizeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            InitSize();
        }

        private void OnIconChanged(BindableObject bindable, object oldValue, object newValue)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var image = new Image()
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Margin = new Thickness(10),
                    BackgroundColor = Color.Transparent,
                    Source = ImageSource.FromResource(Icon)
                };
                Content = image;
            });
        }
    }
}