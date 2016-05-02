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

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPTabunClient.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainpagePage : Page
    {
        private List<Post> posts;
        private MainpageParser parser;

        public MainpagePage()
        {
            this.InitializeComponent();
            parser = new MainpageParser();
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
            parser.htmlParser.frame = Frame;

            LoadingBar.IsEnabled = true;

            int page = 0;
            if (e.Parameter != null)
                int.TryParse(e.Parameter.ToString(), out page);

            if (await parser.loadPage(page))
            {
                try {
                    posts = await parser.getPosts();
                    Bindings.Update();
                } catch (Exception exp)
                {
                    Frame.Navigate(typeof(ErrorPage), exp.Message);
                }
            } else
            {
                Frame.Navigate(typeof(ErrorPage), "Ошибка при загрузке страницы");
            }

            ForwardButton.Visibility = Visibility.Visible;
            if (parser.PageNumber != 1)
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
            Frame.Navigate(typeof(MainpagePage), parser.PageNumber - 1);
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainpagePage), parser.PageNumber + 1);
        }
    }
}
