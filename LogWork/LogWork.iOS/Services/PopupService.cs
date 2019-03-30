using LogWork.iOS.Services;
using LogWork.IServices;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: Dependency(typeof(PopupService))]

namespace LogWork.iOS.Services
{
    public class PopupService : IPopupService
    {
        private UIView contentNativeView;

        public void ShowContent(View content, bool mathParent = true)
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

            contentNativeView = renderer?.NativeView;

            // showing the native loading page
            UIApplication.SharedApplication.KeyWindow.AddSubview(contentNativeView);
        }

        public void HideContent()
        {
            // Hide the page
            contentNativeView?.RemoveFromSuperview();
        }
    }

    internal static class PlatformExtension
    {
        public static IVisualElementRenderer GetOrCreateRenderer(this VisualElement bindable)
        {
            var renderer = Platform.GetRenderer(bindable);
            if (renderer == null)
            {
                renderer = Platform.CreateRenderer(bindable);
                Platform.SetRenderer(bindable, renderer);
            }
            return renderer;
        }
    }
}