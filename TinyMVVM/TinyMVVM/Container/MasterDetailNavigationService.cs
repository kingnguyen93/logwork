using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TinyMVVM.IoC;
using Xamarin.Forms;

namespace TinyMVVM
{
    public class MasterDetailNavigationContainer : MasterDetailPage, INavigationService
    {
        public string NavigationServiceName { get; private set; }

        private ContentPage menuPage;
        private ListView listView = new ListView();
        private List<Page> pagesInner = new List<Page>();

        public Dictionary<string, Page> Pages { get; } = new Dictionary<string, Page>();

        protected ObservableCollection<string> PageNames { get; } = new ObservableCollection<string>();

        public MasterDetailNavigationContainer() : this(Constants.DefaultNavigationServiceName)
        {
        }

        public MasterDetailNavigationContainer(string navigationServiceName)
        {
            listView.SelectionMode = ListViewSelectionMode.None;

            NavigationServiceName = navigationServiceName;
            RegisterNavigation();
        }

        public void Init(string menuTitle, string menuIcon = null)
        {
            CreateMenuPage(menuTitle, menuIcon);
            RegisterNavigation();
        }

        protected virtual void RegisterNavigation()
        {
            TinyIOC.Container.Register<INavigationService>(this, NavigationServiceName);
        }

        protected virtual void CreateMenuPage(string menuPageTitle, string menuIcon = null)
        {
            listView.ItemsSource = PageNames;

            listView.ItemTapped += (sender, args) =>
            {
                if (Pages.ContainsKey((string)args.Item))
                {
                    var page = Pages[(string)args.Item];
                    Detail = page;
                    page.GetModel().OnPushed(new NavigationParameters());
                }

                IsPresented = false;
            };

            menuPage = new ContentPage
            {
                Title = menuPageTitle,
                Content = listView
            };

            var navPage = new NavigationPage(menuPage) { Title = "Menu" };

            if (!string.IsNullOrEmpty(menuIcon))
                navPage.Icon = menuIcon;

            Master = navPage;
        }

        public virtual void AddPage<T>(string title, NavigationParameters parameters = null) where T : TinyViewModel
        {
            var page = ViewModelResolver.ResolveViewModel<T>(parameters);
            AddPage(page, title);
        }

        public virtual void AddPage<T>(string title, string icon, NavigationParameters parameters = null) where T : TinyViewModel
        {
            var page = ViewModelResolver.ResolveViewModel<T>(parameters);
            AddPage(page, title, icon);
        }

        public virtual void AddPage(Type pageType, string title, NavigationParameters parameters = null)
        {
            var page = ViewModelResolver.ResolveViewModel(pageType, parameters);
            AddPage(page, title);
        }

        public virtual void AddPage(Type pageType, string title, string icon, NavigationParameters parameters = null)
        {
            var page = ViewModelResolver.ResolveViewModel(pageType, parameters);
            AddPage(page, title, icon);
        }

        private void AddPage(Page page, string title, string icon = null)
        {
            page.GetModel().CurrentNavigationServiceName = NavigationServiceName;
            pagesInner.Add(page);
            var navigationContainer = CreateContainerPage(page);
            Pages.Add(title, navigationContainer);
            PageNames.Add(title);
            if (Pages.Count == 1)
                Detail = navigationContainer;
        }

        internal Page CreateContainerPageSafe(Page root)
        {
            if (root is NavigationPage || root is MasterDetailPage || root is TabbedPage)
                return root;

            return CreateContainerPage(root);
        }

        protected virtual Page CreateContainerPage(Page root)
        {
            return new NavigationPage(root);
        }

        public Task PushPage(Page page, TinyViewModel model, bool modal = false, bool animate = true)
        {
            if (modal)
                return Navigation.PushModalAsync(CreateContainerPageSafe(page));
            return (Detail as NavigationPage).PushAsync(page, animate); //TODO: make this better
        }

        public Task PushPage(Page page, bool modal = false, bool animate = true)
        {
            if (modal)
                return Navigation.PushModalAsync(CreateContainerPageSafe(page));
            return (Detail as NavigationPage).PushAsync(page, animate); //TODO: make this better
        }

        public Task PopPage(bool modal = false, bool animate = true)
        {
            if (modal)
                return Navigation.PopModalAsync(animate);
            return (Detail as NavigationPage).PopAsync(animate); //TODO: make this better
        }

        public Task PopToRoot(bool animate = true)
        {
            return (Detail as NavigationPage).PopToRootAsync(animate);
        }

        public void NotifyChildrenPageWasPopped()
        {
            if (Master is NavigationPage)
                ((NavigationPage)Master).NotifyAllChildrenPopped();

            if (Master is INavigationService)
                ((INavigationService)Master).NotifyChildrenPageWasPopped();

            foreach (var page in Pages.Values)
            {
                if (page is NavigationPage)
                    ((NavigationPage)page).NotifyAllChildrenPopped();

                if (page is INavigationService)
                    ((INavigationService)page).NotifyChildrenPageWasPopped();
            }

            if (Pages != null && !Pages.ContainsValue(Detail) && Detail is NavigationPage)
                ((NavigationPage)Detail).NotifyAllChildrenPopped();

            if (Pages != null && !Pages.ContainsValue(Detail) && Detail is INavigationService)
                ((INavigationService)Detail).NotifyChildrenPageWasPopped();
        }

        public Task<TinyViewModel> SwitchSelectedRootViewModel<T>() where T : TinyViewModel
        {
            var tabIndex = pagesInner.FindIndex(o => o.GetModel().GetType().FullName == typeof(T).FullName);

            listView.SelectedItem = PageNames[tabIndex];

            return Task.FromResult((Detail as NavigationPage).CurrentPage.GetModel());
        }
    }
}