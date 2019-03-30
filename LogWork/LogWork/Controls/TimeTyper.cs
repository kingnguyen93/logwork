using LogWork.Effects;
using LogWork.IServices;
using System;

namespace Xamarin.Forms.Controls
{
    public class TimeTyper : Entry
    {
        private StackLayout _controlLayout;
        private AbsoluteLayout _typerPopup;
        private Frame _typerFrame;
        private StackLayout _typerContent;
        private Entry _hourInput, _minuteInput;

        public TimeTyper()
        {
            Initialize();
        }

        private void Initialize()
        {
            Focused += TimeTyper_Focused;
        }

        private void TimeTyper_Focused(object sender, FocusEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Unfocus();

                DependencyService.Get<IPopupService>()?.ShowContent(RenderTyperPopup());

                _hourInput.Unfocus();
                _minuteInput.Unfocus();
            });
        }

        private View RenderTyperPopup()
        {
            _controlLayout = new StackLayout();

            _typerContent = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };

            var _hourTyper = new StackLayout();
            var _hourTitle = new Label()
            {
                FontSize = 14,
                TextColor = Color.Black,
                Text = "Hours"
            };
            _hourInput = new Entry()
            {
                FontSize = 13,
                WidthRequest = 50,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Color.Black,
                Text = "0",
                Keyboard = Keyboard.Numeric
            };
            _hourInput.Effects.Add(new RoundedEffect());
            _hourTyper.Children.Add(_hourTitle);
            _hourTyper.Children.Add(_hourInput);

            var _minuteTyper = new StackLayout();
            var _minuteTitle = new Label()
            {
                FontSize = 13,
                TextColor = Color.Black,
                Text = "Minutes"
            };
            _minuteInput = new Entry()
            {
                FontSize = 14,
                WidthRequest = 50,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Color.Black,
                Text = "0",
                Keyboard = Keyboard.Numeric
            };
            _minuteInput.Effects.Add(new RoundedEffect());
            _minuteTyper.Children.Add(_minuteTitle);
            _minuteTyper.Children.Add(_minuteInput);

            _typerContent.Children.Add(_hourTyper);
            _typerContent.Children.Add(_minuteTyper);

            var bottomControl = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.End
            };

            var cancelLabel = new Label()
            {
                FontSize = 14,
                HeightRequest = 30,
                TextColor = Color.DeepSkyBlue,
                Text = "Cancel",
                VerticalTextAlignment = TextAlignment.Center
            };
            cancelLabel.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command((o) =>
                {
                    DependencyService.Get<IPopupService>()?.HideContent();
                })
            });

            var okLabel = new Label()
            {
                FontSize = 14,
                HeightRequest = 30,
                WidthRequest = 50,
                TextColor = Color.DeepSkyBlue,
                Text = "Ok",
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
            okLabel.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command((o) =>
                {
                    Text = string.Format("{0:00}:{1:00}", Convert.ToInt32(_hourInput.Text), Convert.ToInt32(_minuteInput.Text));

                    DependencyService.Get<IPopupService>()?.HideContent();
                })
            });

            bottomControl.Children.Add(cancelLabel);
            bottomControl.Children.Add(okLabel);

            _controlLayout.Children.Add(_typerContent);
            _controlLayout.Children.Add(bottomControl);

            _typerFrame = new Frame()
            {
                BackgroundColor = Color.White,
                CornerRadius = 10,
                Padding = 10,
                HasShadow = false,
                Content = _controlLayout
            };

            _typerPopup = new AbsoluteLayout()
            {
                BackgroundColor = Color.Transparent
            };

            var background = new ContentView()
            {
                BackgroundColor = Color.Black,
                Opacity = 0.6
            };

            _typerPopup.Children.Add(background, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
            _typerPopup.Children.Add(_typerFrame, new Rectangle(.5, .5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize), AbsoluteLayoutFlags.PositionProportional);

            return _typerPopup;
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged();

            if (propertyName == null)
                return;
        }
    }
}