using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Xamarin.Forms.Controls
{
    public class ExtendedSearchBar : SearchBar
    {
        public static readonly BindableProperty TextChangedCommandProperty = BindableProperty.Create(nameof(TextChangedCommand), typeof(ICommand), typeof(ExtendedSearchBar), null);

        public static readonly BindableProperty TextChangedCommandParameterProperty = BindableProperty.Create(nameof(TextChangedCommandParameter), typeof(object), typeof(ExtendedSearchBar), null);

        public static readonly BindableProperty PaddingProperty = BindableProperty.Create(nameof(Padding), typeof(Thickness), typeof(ExtendedSearchBar), new Thickness(5));

        public static readonly BindableProperty BarTintColorProperty = BindableProperty.Create(nameof(BarTintColor), typeof(Color), typeof(ExtendedSearchBar), Color.Default);

        public ICommand TextChangedCommand
        {
            get { return (ICommand)GetValue(TextChangedCommandProperty); }
            set { SetValue(TextChangedCommandProperty, value); }
        }

        public object TextChangedCommandParameter
        {
            get { return GetValue(TextChangedCommandParameterProperty); }
            set { SetValue(TextChangedCommandParameterProperty, value); }
        }

        public Color BarTintColor
        {
            get { return (Color)GetValue(BarTintColorProperty); }
            set { SetValue(BarTintColorProperty, value); }
        }

        public Thickness Padding
        {
            get { return (Thickness)GetValue(BarTintColorProperty); }
            set { SetValue(BarTintColorProperty, value); }
        }

        public ExtendedSearchBar()
        {
            Init();
        }

        private void Init()
        {
            TextChanged += ExtendedSearchBar_TextChanged;
        }

        private void ExtendedSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var param = TextChangedCommandParameter ?? e.NewTextValue;

                if (TextChangedCommand != null && TextChangedCommand.CanExecute(param))
                {
                    TextChangedCommand.Execute(param);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}