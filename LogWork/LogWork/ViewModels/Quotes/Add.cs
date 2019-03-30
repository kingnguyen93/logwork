using System;

using Xamarin.Forms;

namespace Organilog.ViewModels.Quote
{
    public class Add : ContentPage
    {
        public Add()
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Hello ContentPage" }
                }
            };
        }
    }
}

