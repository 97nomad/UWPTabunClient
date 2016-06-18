using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using UWPTabunClient.Pages;
using UWPTabunClient.Parsers;

// Документацию по шаблону элемента "Пустая страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPTabunClient
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            currentView.BackRequested += MainPage_BackRequested;

            MainFrame.Navigate(typeof(MainpagePage));
            ProfileFrame.Navigate(typeof(MyProfilePage));

            MainFrame.Navigated += MainFrame_Navigated;
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            PostPage page = MainFrame.Content as PostPage;
            if (page == null)
            {
                CommentRefreshButton.Visibility = Visibility.Collapsed;
            } else if ((ProfileFrame.Content as MyProfilePage).isLoggedIn)
            {
                CommentRefreshButton.Visibility = Visibility.Visible;
            } else
            {
                CommentRefreshButton.Visibility = Visibility.Collapsed;
            }
        }

        private void PanelButton_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.IsPaneOpen = !MainMenu.IsPaneOpen;
            PostPage post = MainFrame.Content as PostPage;
        }

        private void PaneList_ItemClick(object sender, ItemClickEventArgs e)
        {
            StackPanel item = (e.ClickedItem as StackPanel);
            if (item.Name == "Home")
                MainFrame.Navigate(typeof(MainpagePage));
            if (item.Name == "Stream")
                MainFrame.Navigate(typeof(StreamPage)); 
            MainMenu.IsPaneOpen = false;
        }

        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                e.Handled = true;
                MainFrame.GoBack();
            }
        }

        private async void CommentRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            PostPage page = MainFrame.Content as PostPage;
            CommentRefreshButton.IsEnabled = false;
            await page.refreshComments();
            CommentRefreshButton.IsEnabled = true;
        }
    }
}
