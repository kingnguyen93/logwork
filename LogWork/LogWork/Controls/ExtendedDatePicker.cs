using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Xamarin.Forms.Controls
{
    public class ExtendedDatePicker : DatePicker
    {
        public static readonly BindableProperty DateSelectedCommandProperty = BindableProperty.Create(nameof(DateSelectedCommand), typeof(ICommand), typeof(ExtendedDatePicker), null);

        public static readonly BindableProperty DateSelectedCommandParameterProperty = BindableProperty.Create(nameof(DateSelectedCommandParameter), typeof(object), typeof(ExtendedDatePicker), null);

        public static readonly BindableProperty FocusedCommandProperty = BindableProperty.Create(nameof(FocusedCommand), typeof(ICommand), typeof(ExtendedDatePicker), null);

        public static readonly BindableProperty FocusedCommandParameterProperty = BindableProperty.Create(nameof(FocusedCommandParameter), typeof(object), typeof(ExtendedDatePicker), null);

        public static readonly BindableProperty UnFocusedCommandProperty = BindableProperty.Create(nameof(UnFocusedCommand), typeof(ICommand), typeof(ExtendedDatePicker), null);

        public static readonly BindableProperty UnFocusedCommandParameterProperty = BindableProperty.Create(nameof(UnFocusedCommandParameter), typeof(object), typeof(ExtendedDatePicker), null);

        public ICommand DateSelectedCommand
        {
            get { return (ICommand)GetValue(DateSelectedCommandProperty); }
            set { SetValue(DateSelectedCommandProperty, value); }
        }

        public object DateSelectedCommandParameter
        {
            get { return GetValue(DateSelectedCommandParameterProperty); }
            set { SetValue(DateSelectedCommandParameterProperty, value); }
        }

        public ICommand FocusedCommand
        {
            get { return (ICommand)GetValue(FocusedCommandProperty); }
            set { SetValue(FocusedCommandProperty, value); }
        }

        public object FocusedCommandParameter
        {
            get { return GetValue(FocusedCommandParameterProperty); }
            set { SetValue(FocusedCommandParameterProperty, value); }
        }

        public ICommand UnFocusedCommand
        {
            get { return (ICommand)GetValue(UnFocusedCommandProperty); }
            set { SetValue(UnFocusedCommandProperty, value); }
        }

        public object UnFocusedCommandParameter
        {
            get { return GetValue(UnFocusedCommandParameterProperty); }
            set { SetValue(UnFocusedCommandParameterProperty, value); }
        }

        public ExtendedDatePicker()
        {
            Init();
        }

        private void Init()
        {
            DateSelected += ExtendedDatePicker_DateSelected;
            Focused += ExtendedDatePicker_Focused;
            Unfocused += ExtendedDatePicker_Unfocused;
        }

        private void ExtendedDatePicker_Focused(object sender, FocusEventArgs e)
        {
            try
            {
                var param = FocusedCommandParameter ?? e.IsFocused;

                if (FocusedCommand != null && FocusedCommand.CanExecute(param))
                {
                    FocusedCommand.Execute(param);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void ExtendedDatePicker_Unfocused(object sender, FocusEventArgs e)
        {
            try
            {
                var param = UnFocusedCommandParameter ?? e.IsFocused;

                if (UnFocusedCommand != null && UnFocusedCommand.CanExecute(param))
                {
                    UnFocusedCommand.Execute(param);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void ExtendedDatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            try
            {
                var param = DateSelectedCommandParameter ?? e.NewDate;

                if (DateSelectedCommand != null && DateSelectedCommand.CanExecute(param))
                {
                    DateSelectedCommand.Execute(param);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}