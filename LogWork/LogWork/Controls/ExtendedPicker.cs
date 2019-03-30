using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Xamarin.Forms.Controls
{
    public class ExtendedPicker : Picker
    {
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ExtendedPicker), null);

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(ExtendedPicker), null);

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

        public ExtendedPicker()
        {
            Init();
        }

        private void Init()
        {
            SelectedIndexChanged += ExtendedPicker_SelectedIndexChanged; ;
        }

        private void ExtendedPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var param = CommandParameter ?? SelectedItem;

                if (Command != null && Command.CanExecute(param))
                {
                    Command.Execute(param);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}