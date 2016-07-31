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
using UWPTabunClient.Models;
using HtmlAgilityPack;
using UWPTabunClient.Parsers;
using Windows.UI.Xaml.Documents;
using System.Diagnostics;
using UWPTabunClient.Pages;
using TabunCsParser;
using TabunCsLibruary;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPTabunClient.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainpagePage : Page
    {
        private List<PostPreview> Posts;

        public MainpagePage()
        {
            this.InitializeComponent();
        }

        private void AuthorBlock_Click(object sender, RoutedEventArgs e)
        {
            string author = (sender as Button).Tag.ToString();
            Frame.Navigate(typeof(ProfilePage), author);
        }

        private void PostsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = (e.ClickedItem as Post);
            Frame.Navigate(typeof(PostPage), item.id);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            LoadingBar.IsEnabled = true;

            int PageNumber = 1;
            if (e.Parameter != null)
                int.TryParse(e.Parameter.ToString(), out PageNumber);

            string RawPage = await new TabunMainPage().GetPage(PageNumber);
            TabunCsParser.MainPage Page = new MainPageParser().Parse(RawPage);
            Posts = Page.Posts;
            Bindings.Update();

            ForwardButton.Visibility = Visibility.Visible;
            if (PageNumber != 1)
                BackButton.Visibility = Visibility.Visible;

            LoadingBar.IsEnabled = false;
            LoadingBar.IsIndeterminate = false;
            PostsList.Visibility = Visibility.Visible;
        }

        // Один огромный костыль
        private void RichTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            var rtb = sender as RichTextBlock;
            rtb.Blocks.Add(rtb.DataContext as Paragraph);
        }

        private void BlogButton_Click(object sender, RoutedEventArgs e)
        {
            string blogId = (sender as Button).Tag.ToString();
            Frame.Navigate(typeof(BlogPage), blogId);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            //Frame.Navigate(typeof(MainpagePage), parser.PageNumber - 1);
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            //Frame.Navigate(typeof(MainpagePage), parser.PageNumber + 1);
        }
    }
}
