using System;
using System.Diagnostics;

namespace Xamarin.Forms.Controls
{
    public class HideableToolbarItem : ToolbarItem
    {
        public static readonly BindableProperty IsVisibleProperty = BindableProperty.Create(nameof(IsVisible), typeof(bool), typeof(HideableToolbarItem), default(bool), BindingMode.TwoWay, propertyChanged: OnIsVisibleChanged);

        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        public HideableToolbarItem()
        {
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            InitVisibility();
        }

        private void InitVisibility()
        {
            OnIsVisibleChanged(this, false, IsVisible);
        }

        private static void OnIsVisibleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            try
            {
                if (!(bindable is HideableToolbarItem toolbarItem))
                    return;

                if (!(toolbarItem.Parent is Page parent))
                    return;

                var newValueBool = (bool)newValue;

                if (newValueBool && !parent.ToolbarItems.Contains(toolbarItem))
                {
                    parent.ToolbarItems.Add(toolbarItem);
                }
                else if (!newValueBool && parent.ToolbarItems.Contains(toolbarItem))
                {
                    parent.ToolbarItems.Remove(toolbarItem);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}