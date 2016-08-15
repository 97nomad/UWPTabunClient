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
using TabunCsLibruary;
using TabunCsParser;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPTabunClient.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class StreamPage : Page
    {
        List<StreamElement> Comments;
        List<StreamElement> Topics;
        TabunAPI TAPI;
        StreamParser Parser;

        public StreamPage()
        {
            this.InitializeComponent();
            Comments = new List<StreamElement>();
            Topics = new List<StreamElement>();
            TAPI = new TabunAPI();
            Parser = new StreamParser();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void StreamList_ItemClick(object sender, ItemClickEventArgs e)
        {
            //var item = (StreamElement)e.ClickedItem;
            //Frame.Navigate(typeof(PostPage), item.Link);
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
                
                string RawComments = await TAPI.GetStreamNewComments(new Dictionary<string, string>
                {
                    {"security_ls_key", Windows.Storage.ApplicationData.Current.LocalSettings.Values["livestreet_security_key"] as string }
                });
                Comments = Parser.Parse(RawComments);
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
                string RawTopics = await TAPI.GetStreamNewTopics(new Dictionary<string, string>
                {
                    {"security_ls_key", Windows.Storage.ApplicationData.Current.LocalSettings.Values["livestreet_security_key"] as string }
                });
                Topics = Parser.Parse(RawTopics);
                Bindings.Update();
            } catch (Exception exc)
            {
                Frame.Navigate(typeof(ErrorPage), exc.Message);
            }
        }
    }
}
