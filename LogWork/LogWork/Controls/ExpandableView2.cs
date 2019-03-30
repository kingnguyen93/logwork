using System;

namespace Xamarin.Forms.Controls
{
    public class ExpandableView2 : StackLayout
    {
        public const string ExpandAnimationName = "expandAnimation";

        public static readonly BindableProperty ContentProperty = BindableProperty.Create(nameof(Content), typeof(DataTemplate), typeof(ExpandableView2), null, propertyChanged: OnContentPropertyChanged);

        public static readonly BindableProperty IsExpandedProperty = BindableProperty.Create(nameof(IsExpanded), typeof(bool), typeof(ExpandableView2), default(bool), BindingMode.TwoWay, propertyChanged: OnIsExpandedPropertyChanged);

        private bool _shouldIgnoreAnimation;
        private double _lastVisibleHeight = -1;
        private double _startHeight;
        private double _endHeight;
        private View _secondaryView;

        public DataTemplate Content
        {
            get => GetValue(ContentProperty) as DataTemplate;
            set => SetValue(ContentProperty, value);
        }

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        public View SecondaryView
        {
            get => _secondaryView;
            private set
            {
                if (_secondaryView != null)
                {
                    _secondaryView.SizeChanged -= OnSecondaryViewSizeChanged;
                    Children.Remove(_secondaryView);
                }

                if (value == null) return;

                if (value is Layout layout)
                {
                    layout.IsClippedToBounds = true;
                }

                value.HeightRequest = 0;
                value.IsVisible = false;
                Children.Add(_secondaryView = value);
            }
        }

        private static void OnContentPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var view = (ExpandableView2)bindable;
            view.SetSecondaryView(true);
            view.OnIsExpandedChanged();
        }

        private static void OnIsExpandedPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var view = (ExpandableView2)bindable;
            view.SetSecondaryView();
            view.OnIsExpandedChanged();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            _lastVisibleHeight = -1;
        }

        private void OnIsExpandedChanged()
        {
            if (SecondaryView == null)
            {
                return;
            }

            SecondaryView.SizeChanged -= OnSecondaryViewSizeChanged;

            var isExpanding = SecondaryView.AnimationIsRunning(ExpandAnimationName);
            SecondaryView.AbortAnimation(ExpandAnimationName);

            if (IsExpanded)
            {
                SecondaryView.IsVisible = true;
            }

            _startHeight = 0;
            _endHeight = _lastVisibleHeight;

            var shouldInvokeAnimation = true;

            if (IsExpanded)
            {
                if (_endHeight <= 0)
                {
                    shouldInvokeAnimation = false;
                    SecondaryView.HeightRequest = -1;
                    SecondaryView.SizeChanged += OnSecondaryViewSizeChanged;
                }
            }
            else
            {
                _lastVisibleHeight = _startHeight = !isExpanding
                                                    ? SecondaryView.Height
                                                    : _lastVisibleHeight;
                _endHeight = 0;
            }

            _shouldIgnoreAnimation = Height < 0;

            if (shouldInvokeAnimation)
            {
                InvokeAnimation();
            }
        }

        private void SetSecondaryView(bool forceUpdate = false)
        {
            if (IsExpanded && (SecondaryView == null || forceUpdate))
            {
                SecondaryView = CreateSecondaryView();
            }
        }

        private View CreateSecondaryView()
        {
            var template = Content;
            if (template is DataTemplateSelector selector)
            {
                template = selector.SelectTemplate(BindingContext, this);
            }
            return template?.CreateContent() as View;
        }

        private void OnSecondaryViewSizeChanged(object sender, EventArgs e)
        {
            if (SecondaryView.Height <= 0) return;
            SecondaryView.SizeChanged -= OnSecondaryViewSizeChanged;
            SecondaryView.HeightRequest = 0;
            _endHeight = SecondaryView.Height;
            InvokeAnimation();
        }

        private void InvokeAnimation()
        {
            if (_shouldIgnoreAnimation)
            {
                SecondaryView.HeightRequest = _endHeight;
                SecondaryView.IsVisible = IsExpanded;
                return;
            }

            new Animation(value => SecondaryView.HeightRequest = value, _startHeight, _endHeight)
                .Commit(SecondaryView, ExpandAnimationName, length: 150, easing: Easing.CubicOut, finished: OnAnimationFinished);
        }

        private void OnAnimationFinished(double arg1, bool arg2)
        {
            MessagingCenter.Send<View>(this, "ForceUpdateSize");

            if (IsExpanded) return;

            SecondaryView.IsVisible = false;
        }
    }
}