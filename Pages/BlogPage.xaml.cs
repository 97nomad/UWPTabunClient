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
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using UWPTabunClient.Models;
using UWPTabunClient.Parsers;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPTabunClient.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class BlogPage : Page
    {
        BlogParser parser;
        List<Post> posts;

        public BlogPage()
        {
            this.InitializeComponent();
            parser = new BlogParser();
            posts = new List<Post>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            parser.htmlParser.frame = Frame;

            LoadingBar.IsEnabled = true;
            if (await parser.loadPage((e.Parameter as string).TrimEnd('/')))
            {
                try {
                    BlogName.Text = parser.getBlogName();
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
            LoadingBar.IsEnabled = false;
            LoadingBar.IsIndeterminate = false;
            HeaderPanel.Visibility = Visibility.Visible;
            BlogPostsList.Visibility = Visibility.Visible;

        }

        private void BlogPostsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = (e.ClickedItem as Post);
            Frame.Navigate(typeof(PostPage), item.id);
        }

        private void AuthorBlock_Click(object sender, RoutedEventArgs e)
        {
            string author = (sender as Button).Tag.ToString();
            Frame.Navigate(typeof(ProfilePage), author);
        }

        private void RichTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            var rtb = sender as RichTextBlock;
            rtb.Blocks.Add(rtb.DataContext as Paragraph);
        }
    }
}
