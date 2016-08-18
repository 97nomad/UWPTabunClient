using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TabunCsLibruary;
using TabunCsParser;
using UWPTabunClient.Managers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPTabunClient.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            LoadingBar.IsEnabled = true;

            TabunAPI API = new TabunAPI();
            ProfileParser Parser = new ProfileParser();

            string RawProfile = await API.GetProfile(e.Parameter.ToString());
            Profile ParsedProfile = Parser.Parse(RawProfile);

            DrawProfile(ParsedProfile);

            LoadingBar.IsEnabled = false;
            LoadingBar.IsIndeterminate = false;
            MainGrid.Visibility = Visibility.Visible;
        }

        private async void DrawProfile(Profile Profile)
        {
            // Ник, имя, циферки всякие
            Grid HeaderPanel = new Grid();
            RowDefinition RowDef0 = new RowDefinition();
            RowDefinition RowDef1 = new RowDefinition();
            ColumnDefinition ColDef0 = new ColumnDefinition();
            ColumnDefinition ColDef1 = new ColumnDefinition();
            ColumnDefinition ColDef2 = new ColumnDefinition();
            ColumnDefinition ColDef3 = new ColumnDefinition();
            ColDef2.Width = GridLength.Auto;
            ColDef3.Width = GridLength.Auto;
            HeaderPanel.RowDefinitions.Add(RowDef0);
            HeaderPanel.RowDefinitions.Add(RowDef1);
            HeaderPanel.ColumnDefinitions.Add(ColDef0);
            HeaderPanel.ColumnDefinitions.Add(ColDef1);
            HeaderPanel.ColumnDefinitions.Add(ColDef2);
            HeaderPanel.ColumnDefinitions.Add(ColDef3);
            Grid.SetRow(HeaderPanel, 0);
            MainGrid.Children.Add(HeaderPanel);

            TextBlock NicknameBlock = new TextBlock();
            NicknameBlock.Text = Profile.Nickname;
            NicknameBlock.FontSize = 24;
            NicknameBlock.FontWeight = FontWeights.Bold;
            Grid.SetRow(NicknameBlock, 0);
            Grid.SetColumn(NicknameBlock, 0);
            HeaderPanel.Children.Add(NicknameBlock);

            TextBlock NameBlock = new TextBlock();
            NameBlock.Text = Profile.Name;
            NameBlock.FontSize = 14;
            Grid.SetRow(NameBlock, 1);
            Grid.SetColumn(NameBlock, 0);
            HeaderPanel.Children.Add(NameBlock);

            TextBlock ForceBlock = new TextBlock();
            ForceBlock.Text = Profile.Force.ToString();
            ForceBlock.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetRow(ForceBlock, 0);
            Grid.SetColumn(ForceBlock, 2);
            HeaderPanel.Children.Add(ForceBlock);

            TextBlock ForceDescriptionBlock = new TextBlock();
            ForceDescriptionBlock.HorizontalAlignment = HorizontalAlignment.Center;
            ForceDescriptionBlock.Text = "сила";
            Grid.SetRow(ForceDescriptionBlock, 1);
            Grid.SetColumn(ForceDescriptionBlock, 2);
            HeaderPanel.Children.Add(ForceDescriptionBlock);

            TextBlock RatingBlock = new TextBlock();
            RatingBlock.Text = Profile.Rating.ToString();
            RatingBlock.HorizontalAlignment = HorizontalAlignment.Center;
            RatingBlock.Margin = new Thickness(0, 0, 20, 0);
            Grid.SetRow(RatingBlock, 0);
            Grid.SetColumn(RatingBlock, 3);
            HeaderPanel.Children.Add(RatingBlock);

            TextBlock RatingDescriptionBlock = new TextBlock();
            RatingDescriptionBlock.Text = "голосов: " + Profile.Votes;
            RatingDescriptionBlock.HorizontalAlignment = HorizontalAlignment.Center;
            RatingDescriptionBlock.Margin = new Thickness(0, 0, 20, 0);
            Grid.SetRow(RatingDescriptionBlock, 1);
            Grid.SetColumn(RatingDescriptionBlock, 3);
            HeaderPanel.Children.Add(RatingDescriptionBlock);

            // Маленькая аватарка и "О Себе"
            StackPanel AvatarAndAboutBlock = new StackPanel();
            AvatarAndAboutBlock.Orientation = Orientation.Horizontal;
            Grid.SetRow(AvatarAndAboutBlock, 1);
            MainGrid.Children.Add(AvatarAndAboutBlock);

            Image Avatar100x100Image = new Image();
            Avatar100x100Image.Source = await WebManager.Instance.GetCachedImageSource(Profile.Avatar100x100);
            AvatarAndAboutBlock.Children.Add(Avatar100x100Image);

            StackPanel AboutBlock = new StackPanel();
            AboutBlock.Orientation = Orientation.Vertical;
            AvatarAndAboutBlock.Children.Add(AboutBlock);

            TextBlock AboutText = new TextBlock();
            AboutText.Text = "О себе";
            AboutText.FontSize = 18;
            AboutText.FontWeight = FontWeights.SemiBold;
            AboutBlock.Children.Add(AboutText);

            Parsers.HtmlParser Parser = new Parsers.HtmlParser();
            RichTextBlock RichAboutBlock = new RichTextBlock();
            RichAboutBlock.Style = App.Current.Resources["StandartHtmlView"] as Style;
            RichAboutBlock.Blocks.Add(await Parser.ConvertHTMLTextToParagraph(Profile.About));
            AboutBlock.Children.Add(RichAboutBlock);

            // Пол, дата рождения и локация
            Grid SexDateLocationPanel = new Grid();
            RowDefinition SDLRowDef0 = new RowDefinition();
            RowDefinition SDLRowDef1 = new RowDefinition();
            RowDefinition SDLRowDef2 = new RowDefinition();
            RowDefinition SDLRowDef3 = new RowDefinition();
            RowDefinition SDLRowDef4 = new RowDefinition();
            RowDefinition SDLRowDef5 = new RowDefinition();
            RowDefinition SDLRowDef6 = new RowDefinition();
            RowDefinition SDLRowDef7 = new RowDefinition();
            SexDateLocationPanel.RowDefinitions.Add(SDLRowDef0);
            SexDateLocationPanel.RowDefinitions.Add(SDLRowDef1);
            SexDateLocationPanel.RowDefinitions.Add(SDLRowDef2);
            SexDateLocationPanel.RowDefinitions.Add(SDLRowDef3);
            SexDateLocationPanel.RowDefinitions.Add(SDLRowDef4);
            SexDateLocationPanel.RowDefinitions.Add(SDLRowDef5);
            SexDateLocationPanel.RowDefinitions.Add(SDLRowDef6);
            SexDateLocationPanel.RowDefinitions.Add(SDLRowDef7);
            ColumnDefinition SDLColDef0 = new ColumnDefinition();
            ColumnDefinition SDLColDef1 = new ColumnDefinition();
            SexDateLocationPanel.ColumnDefinitions.Add(SDLColDef0);
            SexDateLocationPanel.ColumnDefinitions.Add(SDLColDef1);
            Grid.SetRow(SexDateLocationPanel, 2);
            MainGrid.Children.Add(SexDateLocationPanel);

            // Личное
            TextBlock PrivateTextBlock = new TextBlock();
            PrivateTextBlock.Text = "Личное";
            PrivateTextBlock.FontSize = 18;
            PrivateTextBlock.FontWeight = FontWeights.SemiBold;
            Grid.SetRow(PrivateTextBlock, 0);
            Grid.SetColumnSpan(PrivateTextBlock, 2);
            SexDateLocationPanel.Children.Add(PrivateTextBlock);

            TextBlock SexTextBlock = new TextBlock();
            SexTextBlock.Text = "Пол: ";
            Grid.SetRow(SexTextBlock, 1);
            Grid.SetColumn(SexTextBlock, 0);
            SexDateLocationPanel.Children.Add(SexTextBlock);

            TextBlock SexBlock = new TextBlock();
            SexBlock.Text = Profile.Sex;
            Grid.SetRow(SexBlock, 1);
            Grid.SetColumn(SexBlock, 1);
            SexDateLocationPanel.Children.Add(SexBlock);

            TextBlock DateOfBirdthTextBlock = new TextBlock();
            DateOfBirdthTextBlock.Text = "Дата рождения";
            Grid.SetRow(DateOfBirdthTextBlock, 2);
            Grid.SetColumn(DateOfBirdthTextBlock, 0);
            SexDateLocationPanel.Children.Add(DateOfBirdthTextBlock);

            TextBlock DateOfBirdthBlock = new TextBlock();
            DateOfBirdthBlock.Text = Profile.DateOfBirdth;
            Grid.SetRow(DateOfBirdthBlock, 2);
            Grid.SetColumn(DateOfBirdthBlock, 1);
            SexDateLocationPanel.Children.Add(DateOfBirdthBlock);

            TextBlock LocationTextBlock = new TextBlock();
            LocationTextBlock.Text = "Местоположение: ";
            Grid.SetRow(LocationTextBlock, 3);
            Grid.SetColumn(LocationTextBlock, 0);
            SexDateLocationPanel.Children.Add(LocationTextBlock);

            TextBlock LocationBlock = new TextBlock();
            LocationBlock.Text = Profile.Place;
            Grid.SetRow(LocationBlock, 3);
            Grid.SetColumn(LocationBlock, 1);
            SexDateLocationPanel.Children.Add(LocationBlock);

            // Активность
            TextBlock ActivityTextBlock = new TextBlock();
            ActivityTextBlock.Text = "Активность";
            ActivityTextBlock.FontSize = 18;
            ActivityTextBlock.FontWeight = FontWeights.SemiBold;
            Grid.SetRow(ActivityTextBlock, 4);
            Grid.SetColumnSpan(ActivityTextBlock, 2);
            SexDateLocationPanel.Children.Add(ActivityTextBlock);

            TextBlock ConsistInTextBlock = new TextBlock();
            ConsistInTextBlock.Text = "Состоит в: ";
            Grid.SetRow(ConsistInTextBlock, 5);
            Grid.SetColumn(ConsistInTextBlock, 0);
            SexDateLocationPanel.Children.Add(ConsistInTextBlock);

            RichTextBlock ConsistInBlock = new RichTextBlock();
            ConsistInBlock.TextWrapping = TextWrapping.WrapWholeWords;
            Paragraph ConsistInParagraph = new Paragraph();
            foreach (KeyValuePair<Uri, string> Blog in Profile.Blogs)
            {
                Hyperlink Hyperlink = new Hyperlink();
                Hyperlink.Inlines.Add(new Run { Text = Blog.Value });
                Hyperlink.Click += (s, e) =>
                {
                    Frame.Navigate(typeof(BlogPage), Blog.Key.Segments.Last());
                };
                ConsistInParagraph.Inlines.Add(Hyperlink);
                ConsistInParagraph.Inlines.Add(new Run { Text = ", " });
            }
            ConsistInBlock.Blocks.Add(ConsistInParagraph);
            Grid.SetRow(ConsistInBlock, 5);
            Grid.SetColumn(ConsistInBlock, 1);
            SexDateLocationPanel.Children.Add(ConsistInBlock);

            TextBlock DateOfRegistrationTextBlock = new TextBlock();
            DateOfRegistrationTextBlock.Text = "Зарегистрирован: ";
            Grid.SetRow(DateOfRegistrationTextBlock, 6);
            Grid.SetColumn(DateOfRegistrationTextBlock, 0);
            SexDateLocationPanel.Children.Add(DateOfRegistrationTextBlock);

            TextBlock DateOfRegistrationBlock = new TextBlock();
            DateOfRegistrationBlock.Text = Profile.DateOfRegistration;
            Grid.SetRow(DateOfRegistrationBlock, 6);
            Grid.SetColumn(DateOfRegistrationBlock, 1);
            SexDateLocationPanel.Children.Add(DateOfRegistrationBlock);

            TextBlock DateOfLastVisiteTextBlock = new TextBlock();
            DateOfLastVisiteTextBlock.Text = "Последний визит: ";
            Grid.SetRow(DateOfLastVisiteTextBlock, 7);
            Grid.SetColumn(DateOfLastVisiteTextBlock, 0);
            SexDateLocationPanel.Children.Add(DateOfLastVisiteTextBlock);

            TextBlock DateOfLastVisiteBlock = new TextBlock();
            DateOfLastVisiteBlock.Text = Profile.DateOfLastVisite;
            Grid.SetRow(DateOfLastVisiteBlock, 7);
            Grid.SetColumn(DateOfLastVisiteBlock, 1);

            TextBlock FriendsTextBlock = new TextBlock();
            FriendsTextBlock.Text = "Друзья";
            FriendsTextBlock.FontSize = 18;
            FriendsTextBlock.FontWeight = FontWeights.SemiBold;
            Grid.SetRow(FriendsTextBlock, 3);
            MainGrid.Children.Add(FriendsTextBlock);

            GridView FriendsGrid = new GridView();
            FriendsGrid.IsItemClickEnabled = true;
            foreach(KeyValuePair<string, Uri> Friend in Profile.Friends)
            {
                StackPanel FriendPanel = new StackPanel();
                FriendPanel.Orientation = Orientation.Vertical;

                Image FriendAvatar = new Image();
                //TODO: Добавить загрузку изображения
                FriendPanel.Children.Add(FriendAvatar);

                TextBlock FriendName = new TextBlock();
                FriendName.Text = Friend.Key;
                FriendPanel.Children.Add(FriendName);

                FriendPanel.Tag = Friend.Key;
                FriendsGrid.Items.Add(FriendPanel);
            }
            FriendsGrid.ItemClick += (s, e) =>
            {
                Frame.Navigate(typeof(ProfilePage), (e.ClickedItem as StackPanel).Tag.ToString());
            };
            Grid.SetRow(FriendsGrid, 4);
            MainGrid.Children.Add(FriendsGrid);
        }

    }
}
