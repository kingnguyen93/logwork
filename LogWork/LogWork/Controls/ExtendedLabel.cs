using System;
using System.Windows.Input;

namespace Xamarin.Forms.Controls
{
    internal class ExtendedLabel : Label
    {
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ExtendedLabel), null);

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(ExtendedLabel), null);

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

        public event EventHandler ItemTapped;

        public ExtendedLabel()
        {
            Initialize();
        }

        public void Initialize()
        {
            GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = TransitionCommand
            });
        }

        private ICommand TransitionCommand
        {
            get
            {
                return new Command(() =>
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        AnchorX = 0.48;
                        AnchorY = 0.48;

                        await this.ScaleTo(0.95, 50, Easing.Linear);
                        await this.ScaleTo(1, 50, Easing.Linear);

                        Command?.Execute(CommandParameter);

                        ItemTapped?.Invoke(this, EventArgs.Empty);
                    });
                });
            }
        }
    }
}