using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LogWork.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingScreen1 : ContentView
    {
        public static readonly BindableProperty MessageProperty = BindableProperty.Create(nameof(Message), typeof(string), typeof(LoadingScreen1), null, propertyChanged: OnMessageChanged);

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public LoadingScreen1()
        {
            InitializeComponent();
        }

        private static void OnMessageChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is LoadingScreen1 control))
                return;

            if (string.IsNullOrWhiteSpace(newValue as string))
            {
                control.lblMessage.Text = "Loading...";
            }
            else
            {
                control.lblMessage.Text = newValue as string;
            }
        }
    }
}