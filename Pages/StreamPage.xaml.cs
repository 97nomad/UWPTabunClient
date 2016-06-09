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
using UWPTabunClient.Parsers;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPTabunClient.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class StreamPage : Page
    {
        List<StreamElement> streamElementsComments;
        List<StreamElement> streamElementsTopics;
        StreamParser parser;

        public StreamPage()
        {
            this.InitializeComponent();
            streamElementsComments = new List<StreamElement>();
            streamElementsTopics = new List<StreamElement>();
            parser = new StreamParser();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void StreamList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as StreamElement;
            Frame.Navigate(typeof(PostPage), item.link);
        }

        private void AuthorButton_Click(object sender, RoutedEventArgs e)
        {
            var authorfield = (sender as Button).Tag.ToString();
            Frame.Navigate(typeof(ProfilePage), authorfield);
        }

        private async void CommentsItem_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                streamElementsComments = await parser.getStreamElements(true);
                Bindings.Update();
            } catch (Exception exc)
            {
                Frame.Navigate(typeof(ErrorPage), exc.Message);
            }
        }

        private async void TopicsItem_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                streamElementsTopics = await parser.getStreamElements(false);
                Bindings.Update();
            } catch (Exception exc)
            {
                Frame.Navigate(typeof(ErrorPage), exc.Message);
            }
        }
    }
}
