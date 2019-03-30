using LogWork.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ListView), typeof(ListViewCustomRenderer))]

namespace LogWork.iOS.Renderers
{
    public class ListViewCustomRenderer : ListViewRenderer
    {
        public ListViewCustomRenderer()
        {
            MessagingCenter.Subscribe<ExpandableView2>(this, "ForceUpdateSize", (args) =>
            {
                Control?.ReloadData();
            });
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
                return;

            Control.TableFooterView = new UIView();
        }
    }
}