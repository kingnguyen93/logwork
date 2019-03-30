using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyMVVM.IoC;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace TinyMVVM
{
    public class TabbedNavigationContainer : Xamarin.Forms.TabbedPage, INavigationService
    {
        public string NavigationServiceName { get; private set; }

        private Dictionary<Page, (Color BarBackgroundColor, Color BarTextColor)> _tabs = new Dictionary<Page, (Color BarBackgroundColor, Color TextColor)>();

        public IEnumerable<Page> TabbedPages { get => _tabs.Keys; }

        public TabbedNavigationContainer(bool bottomToolBar = false) : this(Constants.DefaultNavigationServiceName, bottomToolBar)
        {
        }

        public TabbedNavigationContainer(string navigationServiceName, bool bottomToolBar = false)
        {
            if (bottomToolBar)
                On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);

            NavigationServiceName = navigationServiceName;
            RegisterNavigation();
        }

        protected void RegisterNavigation()
        {
            TinyIOC.Container.Register<INavigationService>(this, NavigationServiceName);
        }

        public virtual Page AddTab<T>(string title, string icon, NavigationParameters parameters = null) where T : TinyViewModel
        {
            var page = ViewModelResolver.ResolveViewModel<T>(parameters);
            return AddTab(page, title, icon);
        }

        public virtual Page AddTab(Type pageType, string title, string icon, NavigationParameters parameters = null)
        {
            var page = ViewModelResolver.ResolveViewModel(pageType, parameters);
            return AddTab(page, title, icon);
        }

        private Page AddTab(Page page, string title, string icon)
        {
            var viewModel = page.GetModel();
            viewModel.CurrentNavigationServiceName = NavigationServiceName;

            _tabs.Add(page, (BarBackgroundColor, BarTextColor));

            var navigationContainer = CreateContainerPageSafe(page);
            navigationContainer.Title = title;

            if (!string.IsNullOrWhiteSpace(icon))
                navigationContainer.Icon = icon;

            Children.Add(navigationContainer);

            viewModel.OnPushed(new NavigationParameters());

            return navigationContainer;
        }

        internal Page CreateContainerPageSafe(Page page)
        {
            if (page is NavigationPage || page is MasterDetailPage || page is Xamarin.Forms.TabbedPage)
                return page;

            return CreateContainerPage(page);
        }

        protected virtual Page CreateContainerPage(Page page)
        {
            return new NavigationPage(page);
        }

        public Task PushPage(Page page, bool modal = false, bool animate = true)
        {
            if (modal)
                return CurrentPage.Navigation.PushModalAsync(CreateContainerPageSafe(page));
            return CurrentPage.Navigation.PushAsync(page);
        }

        public Task PopPage(bool modal = false, bool animate = true)
        {
            if (modal)
                return CurrentPage.Navigation.PopModalAsync(animate);
            return CurrentPage.Navigation.PopAsync(animate);
        }

        public Task PopToRoot(bool animate = true)
        {
            return CurrentPage.Navigation.PopToRootAsync(animate);
        }

        public void NotifyChildrenPageWasPopped()
        {
            foreach (var page in Children)
            {
                if (page is NavigationPage)
                    ((NavigationPage)page).NotifyAllChildrenPopped();
            }
        }

        public Task<TinyViewModel> SwitchSelectedRootViewModel<T>() where T : TinyViewModel
        {
            var page = _tabs.ToList().FindIndex(o => o.Key.GetModel().GetType().FullName == typeof(T).FullName);

            if (page > -1)
            {
                CurrentPage = Children[page];
                var topOfStack = CurrentPage.Navigation.NavigationStack.LastOrDefault();
                if (topOfStack != null)
                    return Task.FromResult(topOfStack.GetModel());
            }
            return null;
        }

        protected override void OnCurrentPageChanged()
        {
            /*var color = _tabs.FirstOrDefault(x => x.Key.GetType().FullName == CurrentPage.GetType().FullName).Value;

            BarBackgroundColor = color.BarBackgroundColor;
            BarTextColor = color.BarTextColor;*/

            base.OnCurrentPageChanged();
        }
    }
}