using System.Linq;

namespace Xamarin.Forms.Extensions
{
    public static class PageExtension
    {
        public static bool IsModal(this Page page)
        {
            for (int i = 0; i < page.Navigation.ModalStack.Count(); i++)
            {
                if (page == page.Navigation.ModalStack[i])
                    return true;
            }
            return false;
        }
    }
}