using LogWork.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExtendedViewCell), typeof(ExtendedViewCellRenderer))]

namespace LogWork.iOS.Renderers
{
    public class ExtendedViewCellRenderer : ViewCellRenderer
    {
        private UIView selectedBackgroundView;

        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            var cell = base.GetCell(item, reusableCell, tv);
            if (item is ExtendedViewCell view)
            {
                selectedBackgroundView = selectedBackgroundView ?? new UIView { BackgroundColor = view.SelectedBackgroundColor.ToUIColor() };
                cell.SelectedBackgroundView = selectedBackgroundView;
            }
            return cell;
        }
    }
}