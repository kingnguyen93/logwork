using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Input;

namespace Xamarin.Forms.Controls
{
    public class ExtendedListView : ListView
    {
        public static readonly BindableProperty SelectedItemColorProperty = BindableProperty.Create(nameof(SelectedItemColor), typeof(Color), typeof(ExtendedListView), Color.Default);

        public static readonly BindableProperty ItemSelectedCommandProperty = BindableProperty.Create(nameof(ItemSelectedCommand), typeof(ICommand), typeof(ExtendedListView), default(ICommand));

        public static readonly BindableProperty ItemSelectedCommandParameterProperty = BindableProperty.Create(nameof(ItemSelectedCommandParameter), typeof(object), typeof(ExtendedListView), default);

        public static readonly BindableProperty ItemTappedCommandProperty = BindableProperty.Create(nameof(ItemTappedCommand), typeof(ICommand), typeof(ExtendedListView), default(ICommand));

        public static readonly BindableProperty ItemTappedCommandParameterProperty = BindableProperty.Create(nameof(ItemTappedCommandParameter), typeof(object), typeof(ExtendedListView), default);

        public static readonly BindableProperty LoadMoreCommandProperty = BindableProperty.Create(nameof(LoadMoreCommand), typeof(ICommand), typeof(ExtendedListView), default(ICommand));

        public static readonly BindableProperty LoadMoreCommandParameterProperty = BindableProperty.Create(nameof(LoadMoreCommandParameter), typeof(object), typeof(ExtendedListView), default);

        public static readonly BindableProperty LoadPositionOffsetProperty = BindableProperty.Create(nameof(LoadPositionOffset), typeof(int), typeof(ExtendedListView), 1);

        public static readonly BindableProperty CanLoadMoreProperty = BindableProperty.Create(nameof(CanLoadMore), typeof(bool), typeof(ExtendedListView), default(bool), propertyChanged: (b, o, n) => ((ExtendedListView)b)?.OnCanLoadMoreChanged(b, o, n));

        public Color SelectedItemColor
        {
            get { return (Color)GetValue(SelectedItemColorProperty); }
            set { SetValue(SelectedItemColorProperty, value); }
        }

        public ICommand ItemSelectedCommand
        {
            get { return (ICommand)GetValue(ItemTappedCommandProperty); }
            set { SetValue(ItemTappedCommandProperty, value); }
        }

        public object ItemSelectedCommandParameter
        {
            get { return GetValue(ItemTappedCommandParameterProperty); }
            set { SetValue(ItemTappedCommandParameterProperty, value); }
        }

        public ICommand ItemTappedCommand
        {
            get { return (ICommand)GetValue(ItemTappedCommandProperty); }
            set { SetValue(ItemTappedCommandProperty, value); }
        }

        public object ItemTappedCommandParameter
        {
            get { return GetValue(ItemTappedCommandParameterProperty); }
            set { SetValue(ItemTappedCommandParameterProperty, value); }
        }

        public ICommand LoadMoreCommand
        {
            get { return (ICommand)GetValue(LoadMoreCommandProperty); }
            set { SetValue(LoadMoreCommandProperty, value); }
        }

        public object LoadMoreCommandParameter
        {
            get { return GetValue(LoadMoreCommandParameterProperty); }
            set { SetValue(LoadMoreCommandParameterProperty, value); }
        }

        public int LoadPositionOffset
        {
            get { return (int)GetValue(LoadPositionOffsetProperty); }
            set { SetValue(LoadPositionOffsetProperty, value); }
        }

        public bool CanLoadMore
        {
            get { return (bool)GetValue(CanLoadMoreProperty); }
            set { SetValue(CanLoadMoreProperty, value); }
        }

        public ExtendedListView() : base(ListViewCachingStrategy.RecycleElement)
        {
            Init();
        }

        public ExtendedListView(ListViewCachingStrategy cachingStrategy) : base(cachingStrategy)
        {
            Init();
        }

        private void Init()
        {
            ItemSelected += ExtendedListView_ItemSelected;
            ItemTapped += ExtendedListView_ItemTapped;
        }

        private void ExtendedListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                var param = ItemSelectedCommandParameter ?? e.SelectedItem;

                if (ItemSelectedCommand != null && ItemSelectedCommand.CanExecute(param))
                {
                    ItemSelectedCommand.Execute(param);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void ExtendedListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                var param = ItemTappedCommandParameter ?? e.Item;

                if (ItemTappedCommand != null && ItemTappedCommand.CanExecute(param))
                {
                    ItemTappedCommand.Execute(param);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void OnCanLoadMoreChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ExtendedListView control)
            {
                if (oldValue.Equals(newValue))
                    return;

                if (newValue.Equals(true))
                    control.ItemAppearing += ExtendedListView_ItemAppearing;
                else
                    control.ItemAppearing -= ExtendedListView_ItemAppearing;
            }
        }

        private void ExtendedListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            try
            {
                if (ItemsSource is IList items && e.Item.Equals(items[items.Count - LoadPositionOffset]))
                {
                    var param = LoadMoreCommandParameter ?? e.Item;

                    if (LoadMoreCommand != null && LoadMoreCommand.CanExecute(param))
                    {
                        LoadMoreCommand.Execute(param);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}