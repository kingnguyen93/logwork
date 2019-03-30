using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Xamarin.Forms.Controls
{
    public class ToggleBox : ContentView
    {
        public static readonly BindableProperty IsToggledProperty = BindableProperty.Create(nameof(IsToggled), typeof(bool), typeof(ToggleBox), default, BindingMode.TwoWay);

        public static readonly BindableProperty EnabledProperty = BindableProperty.Create(nameof(Enabled), typeof(bool), typeof(ToggleBox), true);

        public static readonly BindableProperty HasFrameProperty = BindableProperty.Create(nameof(HasFrame), typeof(bool), typeof(ToggleBox), default);

        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(ToggleBox), Color.Transparent);

        public static readonly BindableProperty AnimateProperty = BindableProperty.Create(nameof(Animate), typeof(bool), typeof(ToggleBox), default);

        public static readonly BindableProperty CheckedImageProperty = BindableProperty.Create(nameof(CheckedImage), typeof(ImageSource), typeof(ToggleBox), default);

        public static readonly BindableProperty UnCheckedImageProperty = BindableProperty.Create(nameof(UnCheckedImage), typeof(ImageSource), typeof(ToggleBox), default);

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ToggleBox), default);

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(ToggleBox), default);

        public bool IsToggled
        {
            get { return (bool)GetValue(IsToggledProperty); }
            set { SetValue(IsToggledProperty, value); }
        }

        public bool Enabled
        {
            get { return (bool)GetValue(EnabledProperty); }
            set { SetValue(EnabledProperty, value); }
        }

        public bool HasFrame
        {
            get { return (bool)GetValue(HasFrameProperty); }
            set { SetValue(HasFrameProperty, value); }
        }

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public bool Animate
        {
            get { return (bool)GetValue(AnimateProperty); }
            set { SetValue(AnimateProperty, value); }
        }

        public ImageSource CheckedImage
        {
            get { return (ImageSource)GetValue(CheckedImageProperty); }
            set { SetValue(CheckedImageProperty, value); }
        }

        public ImageSource UnCheckedImage
        {
            get { return (ImageSource)GetValue(UnCheckedImageProperty); }
            set { SetValue(UnCheckedImageProperty, value); }
        }

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

        public event EventHandler<bool> TogleChanged;

        private Frame _frame;
        private Image _toggleImage;

        public ToggleBox()
        {
            Initialize();
        }

        private void Initialize()
        {
            _toggleImage = new Image
            {
                Source = IsToggled ? CheckedImage : UnCheckedImage,
                Aspect = Aspect.Fill,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    if (!Enabled)
                    {
                        return;
                    }

                    IsToggled = _toggleImage.Source == UnCheckedImage;

                    if (Animate)
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await this.ScaleTo(0.8, 50, Easing.Linear);
                            await Task.Delay(100);
                            await this.ScaleTo(1, 50, Easing.Linear);
                        });
                    }

                    var param = CommandParameter ?? IsToggled;

                    if (Command != null && Command.CanExecute(param))
                    {
                        Command.Execute(param);
                    }

                    TogleChanged?.Invoke(this, IsToggled);
                })
            });
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            _toggleImage.Source = IsToggled ? CheckedImage : UnCheckedImage;

            RenderContent();
        }

        private void RenderContent()
        {
            if (HasFrame)
            {
                _frame = new Frame()
                {
                    BackgroundColor = Color.White,
                    BorderColor = BorderColor,
                    Padding = Padding,
                    HasShadow = false,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Content = _toggleImage
                };

                Content = _frame;
            }
            else
            {
                Content = _toggleImage;
            }
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged();

            if (propertyName == null)
                return;

            if (Equals(propertyName, nameof(IsToggled)))
            {
                _toggleImage.Source = IsToggled ? CheckedImage : UnCheckedImage;
            }
            else if (Equals(propertyName, nameof(HasFrame)))
            {
                RenderContent();
            }
            else if (Equals(propertyName, nameof(Padding)))
            {
                _frame.Padding = Padding;
            }
        }
    }
}