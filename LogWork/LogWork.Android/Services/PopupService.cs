using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Views;
using LogWork.Droid.Services;
using LogWork.IServices;
using Plugin.CurrentActivity;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: Dependency(typeof(PopupService))]

namespace LogWork.Droid.Services
{
    public class PopupService : IPopupService
    {
        private Android.Views.View contentNativeView;

        private Dialog contentDialog;

        public void ShowContent(Xamarin.Forms.View content, bool mathParent = true)
        {
            // build the loading page with native base
            content.Parent = Xamarin.Forms.Application.Current.MainPage;

            if (mathParent)
            {
                content.Layout(new Rectangle(0, 0,
                    Xamarin.Forms.Application.Current.MainPage.Width,
                    Xamarin.Forms.Application.Current.MainPage.Height));
            }
            else
            {
                content.Layout(new Rectangle(Xamarin.Forms.Application.Current.MainPage.Width - content.WidthRequest, Xamarin.Forms.Application.Current.MainPage.Height - content.HeightRequest,
                    content.WidthRequest,
                    content.HeightRequest));
            }

            var renderer = content.GetOrCreateRenderer();

            contentNativeView = renderer?.View;

            contentDialog = new Dialog(CrossCurrentActivity.Current.Activity);
            contentDialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
            if (mathParent)
            {
                contentDialog.SetCanceledOnTouchOutside(false);
                contentDialog.SetOnKeyListener(new DialogInterfaceOnKeyListener());
            }
            else
            {
                contentDialog.SetCanceledOnTouchOutside(true);
            }
            contentDialog.SetContentView(contentNativeView);

            Window window = contentDialog.Window;
            window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            window.ClearFlags(WindowManagerFlags.DimBehind);
            window.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.Transparent));

            // Show Popup
            contentDialog?.Show();
        }

        private class DialogInterfaceOnKeyListener : Java.Lang.Object, IDialogInterfaceOnKeyListener
        {
            public bool OnKey(IDialogInterface dialog, [GeneratedEnum] Keycode keyCode, KeyEvent e)
            {
                return keyCode == Keycode.Back;
            }
        }

        public void HideContent()
        {
            // Hide Popup
            contentDialog?.Dismiss();
        }
    }

    internal static class PlatformExtension
    {
        public static IVisualElementRenderer GetOrCreateRenderer(this VisualElement bindable)
        {
            var renderer = Platform.GetRenderer(bindable);
            if (renderer == null)
            {
                renderer = Platform.CreateRendererWithContext(bindable, CrossCurrentActivity.Current.Activity);
                Platform.SetRenderer(bindable, renderer);
            }
            return renderer;
        }
    }
}