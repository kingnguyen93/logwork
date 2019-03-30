using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Xamarin.Forms.Controls
{
    public class ExtendedEntry : Entry
    {
        public static readonly BindableProperty PaddingProperty = BindableProperty.Create(nameof(Padding), typeof(Thickness), typeof(ExtendedEntry), default);

        public static readonly BindableProperty OnFocusedCommandProperty = BindableProperty.Create(nameof(OnFocusedCommand), typeof(ICommand), typeof(ExtendedEntry), null);

        public static readonly BindableProperty OnFocusedCommandParameterProperty = BindableProperty.Create(nameof(OnFocusedCommandParameter), typeof(object), typeof(ExtendedEntry), null);

        public static readonly BindableProperty CanFocusProperty = BindableProperty.Create(nameof(CanFocus), typeof(bool), typeof(ExtendedEntry), true);

        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public ICommand OnFocusedCommand
        {
            get { return (ICommand)GetValue(OnFocusedCommandProperty); }
            set { SetValue(OnFocusedCommandProperty, value); }
        }

        public object OnFocusedCommandParameter
        {
            get { return GetValue(OnFocusedCommandParameterProperty); }
            set { SetValue(OnFocusedCommandParameterProperty, value); }
        }

        public bool CanFocus
        {
            get { return (bool)GetValue(CanFocusProperty); }
            set { SetValue(CanFocusProperty, value); }
        }

        public ExtendedEntry()
        {
            Init();
        }

        private void Init()
        {
            Focused += ExtendedEntry_Focused;
        }

        private void ExtendedEntry_Focused(object sender, FocusEventArgs e)
        {
            try
            {
                if (!CanFocus)
                {
                    Device.BeginInvokeOnMainThread(() => e.VisualElement.Unfocus());
                }

                var param = OnFocusedCommandParameter ?? e;

                if (OnFocusedCommand != null && OnFocusedCommand.CanExecute(param))
                {
                    OnFocusedCommand.Execute(param);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}