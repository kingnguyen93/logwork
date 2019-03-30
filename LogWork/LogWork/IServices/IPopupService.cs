using Xamarin.Forms;

namespace LogWork.IServices
{
    public interface IPopupService
    {
        void ShowContent(View content, bool mathParent = true);

        void HideContent();
    }
}