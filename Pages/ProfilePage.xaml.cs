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
using HtmlAgilityPack;
using Windows.UI.Xaml.Documents;
using UWPTabunClient.Pages;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPTabunClient.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class ProfilePage : Page
    {
        Profile profile;
        List<Friend> friends;
        ProfileParser parser;

        public ProfilePage()
        {
            this.InitializeComponent();
            parser = new ProfileParser();
        }

        private void LoadProfile()
        {
            NicknameBlock.Text = profile.nickname;
            NameBlock.Text = profile.name;
            ForceBlock.Text = profile.force;
            RatingBlock.Text = profile.rating;
            VotesBlock.Text = profile.votes;
            AboutBlock.Blocks.Add(profile.about);
            //AboutBlock.DataContext = profile.about;
            SexBlock.Text = profile.sex;
            DateOfBirdthBlock.Text = profile.dateOfBirdth;
            PlaceBlock.Text = profile.place;
            DateOfRegistrationBlock.Text = profile.dateOfRegistration;
            DateOfLastVisiteBlock.Text = profile.dateOfLastVisite;

            AvatarImage.Source = profile.avatar_100x100;
            BigAvatarImage.Source = profile.profile_photo;

            RatingBlock.Foreground = new SolidColorBrush(profile.ratingColor);
            friends = profile.friends;

            string tmp = "";
            foreach (string contact in profile.contacts)
                tmp += contact + "\n";
            ContactsBlock.Text = tmp;

            /*Paragraph tmpParagraph = new Paragraph();
            foreach (KeyValuePair<string, string> blog in profile.blogsConsistIn)
            {
                InlineUIContainer container = new InlineUIContainer();
                Button hpButton = new Button();
                hpButton.Style = App.Current.Resources["HyperLinkInvisibleButton"] as Style;
                hpButton.Margin = new Thickness(5, 0, 5, 0);

                hpButton.Content = blog.Key;
                hpButton.Tag = blog.Value;

                container.Child = hpButton;
                tmpParagraph.Inlines.Add(container);
            }*/
            ConsistsBlock.Blocks.Add(profile.blogsConsistIn);

            //CrutchManager.crutchForButtonsInRichTextBlock(Frame, ConsistsBlock);
            //CrutchManager.crutchForButtonsInRichTextBlock(Frame, AboutBlock);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            parser.htmlParser.frame = Frame;

            LoadingBar.IsEnabled = true;
            if (await parser.loadPage(e.Parameter.ToString()))
            {
                try {
                    profile = await parser.getProfile();
                    LoadProfile();
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
            MainGrid.Visibility = Visibility.Visible;
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as Friend;
            Frame.Navigate(typeof(ProfilePage), item.name);
        }
    }
}
