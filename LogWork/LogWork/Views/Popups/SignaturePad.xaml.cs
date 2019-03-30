using LogWork.Constants;
using SignaturePad.Forms;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LogWork.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignaturePad : ContentPage
    {
        public event EventHandler<Stream> ContentSaved;

        public SignaturePad()
        {
            InitializeComponent();
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            if (!signatureView.IsBlank)
            {
                ContentSaved?.Invoke(this, await signatureView.GetImageStreamAsync(SignatureImageFormat.Png, strokeColor: Color.Black, fillColor: Color.White));
                MessagingCenter.Send(this, MessageKey.SIGNATURE_PAD_SAVED, await signatureView.GetImageStreamAsync(SignatureImageFormat.Png, strokeColor: Color.Black, fillColor: Color.White));
            }

            await Navigation.PopModalAsync();
        }

        private async void CancelButton_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}