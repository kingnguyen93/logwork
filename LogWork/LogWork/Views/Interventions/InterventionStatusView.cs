using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace LogWork.Views.Interventions
{
    public class InterventionStatusView : ContentView
    {
        public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create(nameof(IsChecked), typeof(int), typeof(InterventionStatusView), default, BindingMode.TwoWay);

        public static readonly BindableProperty EnabledProperty = BindableProperty.Create(nameof(Enabled), typeof(bool), typeof(InterventionStatusView), true);

        public static readonly BindableProperty HasFrameProperty = BindableProperty.Create(nameof(HasFrame), typeof(bool), typeof(InterventionStatusView), default);

        public static readonly BindableProperty HasIndeterminateProperty = BindableProperty.Create(nameof(HasIndeterminate), typeof(bool), typeof(InterventionStatusView), default);

        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(InterventionStatusView), Color.Transparent);

        public static readonly BindableProperty AnimateProperty = BindableProperty.Create(nameof(Animate), typeof(bool), typeof(InterventionStatusView), default);

        public static readonly BindableProperty UnCheckedImageProperty = BindableProperty.Create(nameof(UnCheckedImage), typeof(ImageSource), typeof(InterventionStatusView), default);

        public static readonly BindableProperty IndeterminateImageProperty = BindableProperty.Create(nameof(IndeterminateImage), typeof(ImageSource), typeof(InterventionStatusView), default);

        public static readonly BindableProperty CheckedImageProperty = BindableProperty.Create(nameof(CheckedImage), typeof(ImageSource), typeof(InterventionStatusView), default);

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(InterventionStatusView), default);

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(InterventionStatusView), default);

        public int IsChecked
        {
            get { return (int)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
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

        public bool HasIndeterminate
        {
            get { return (bool)GetValue(HasIndeterminateProperty); }
            set { SetValue(HasIndeterminateProperty, value); }
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

        public ImageSource UnCheckedImage
        {
            get { return (ImageSource)GetValue(UnCheckedImageProperty); }
            set { SetValue(UnCheckedImageProperty, value); }
        }

        public ImageSource IndeterminateImage
        {
            get { return (ImageSource)GetValue(IndeterminateImageProperty); }
            set { SetValue(IndeterminateImageProperty, value); }
        }

        public ImageSource CheckedImage
        {
            get { return (ImageSource)GetValue(CheckedImageProperty); }
            set { SetValue(CheckedImageProperty, value); }
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

        public event EventHandler<int> TogleChanged;

        private Frame _frame;
        private Image _toggleImage;

        public InterventionStatusView()
        {
            Initialize();
        }

        private void Initialize()
        {
            _toggleImage = new Image
            {
                Source = GetImageSource(),
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

                    if (HasIndeterminate)
                    {
                        if (IsChecked == 0)
                            IsChecked = 2;
                        else if (IsChecked == 2)
                            IsChecked = 1;
                        else
                            IsChecked = 0;
                    }
                    else
                    {
                        if (IsChecked == 0)
                            IsChecked = 1;
                        else
                            IsChecked = 0;
                    }

                    if (Animate)
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await this.ScaleTo(0.8, 50, Easing.Linear);
                            await Task.Delay(100);
                            await this.ScaleTo(1, 50, Easing.Linear);
                        });
                    }

                    var param = CommandParameter ?? IsChecked;

                    if (Command != null && Command.CanExecute(param))
                    {
                        Command.Execute(param);
                    }

                    TogleChanged?.Invoke(this, IsChecked);
                })
            });
        }

        private ImageSource GetImageSource()
        {
            if (HasIndeterminate)
            {
                switch (IsChecked)
                {
                    case 1:
                        return CheckedImage;

                    case 2:
                        return IndeterminateImage;

                    default:
                        return UnCheckedImage;
                }
            }
            else
            {
                switch (IsChecked)
                {
                    case 1:
                        return CheckedImage;

                    default:
                        return UnCheckedImage;
                }
            }
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            _toggleImage.Source = GetImageSource();

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

            if (Equals(propertyName, nameof(IsChecked)))
            {
                _toggleImage.Source = GetImageSource();
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