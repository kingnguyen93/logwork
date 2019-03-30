namespace Xamarin.Forms.Controls
{
    public class ExpandableView : Frame
    {
        public static readonly BindableProperty HeaderTemplateProperty = BindableProperty.Create(nameof(HeaderTemplate), typeof(DataTemplate), typeof(ExpandableView), default(DataTemplate));

        public static readonly BindableProperty ContentTemplateProperty = BindableProperty.Create(nameof(ContentTemplate), typeof(DataTemplate), typeof(ExpandableView), default(DataTemplate));

        public static readonly BindableProperty SpacingProperty = BindableProperty.Create(nameof(Spacing), typeof(double), typeof(ExpandableView), default(double));

        public static readonly BindableProperty ExpandedProperty = BindableProperty.Create(nameof(Expanded), typeof(bool), typeof(ExpandableView), default(bool), BindingMode.TwoWay, propertyChanged: (b, o, n) => ((ExpandableView)b).ExpandedChanged(b, o, n));

        public DataTemplate HeaderTemplate
        {
            get => (DataTemplate)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        public DataTemplate ContentTemplate
        {
            get => (DataTemplate)GetValue(ContentTemplateProperty);
            set => SetValue(ContentTemplateProperty, value);
        }

        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        public bool Expanded
        {
            get { return (bool)GetValue(ExpandedProperty); }
            set { SetValue(ExpandedProperty, value); }
        }

        public ExpandableView()
        {
            IsClippedToBounds = true;
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            Init();
        }

        private void Init()
        {
            CreateView();
        }

        private StackLayout _container;

        private void CreateView()
        {
            _container = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Spacing = Spacing
            };

            View header = CreateHeader();
            if (header != null)
            {
                header.HorizontalOptions = LayoutOptions.FillAndExpand;
                header.VerticalOptions = LayoutOptions.Start;

                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) =>
                {
                    Expanded = !Expanded;
                };
                header.GestureRecognizers.Add(tap);

                _container.Children.Add(header);
            }

            if (Expanded)
            {
                View content = CreateContent();
                if (content != null)
                {
                    content.HorizontalOptions = LayoutOptions.FillAndExpand;
                    content.VerticalOptions = LayoutOptions.FillAndExpand;

                    _container.Children.Add(content);
                }
            }

            Content = _container;
        }

        protected virtual View CreateHeader()
        {
            View view = null;

            if (HeaderTemplate != null)
            {
                var content = HeaderTemplate.CreateContent();
                view = content is View ? content as View : ((ViewCell)content).View;
            }

            return view;
        }

        protected virtual View CreateContent()
        {
            View view = null;

            if (ContentTemplate != null)
            {
                var content = ContentTemplate.CreateContent();
                view = content is View ? content as View : ((ViewCell)content).View;
            }

            return view;
        }

        private void ExpandedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            try
            {
                CreateView();
            }
            catch { }
        }
    }
}