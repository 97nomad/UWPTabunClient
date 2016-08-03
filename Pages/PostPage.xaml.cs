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
using UWPTabunClient.Pages;
using System.Diagnostics;
using System.Threading.Tasks;
using TabunCsLibruary;
using TabunCsParser;
using Windows.UI.Text;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPTabunClient.Pages
{
    public sealed partial class PostPage : Page
    {

        public PostPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            LoadingBar.IsEnabled = true;

            TabunAPI API = new TabunAPI();
            PostParser Parser = new PostParser();

            string RawPost = await API.GetPost(int.Parse(e.Parameter.ToString()));
            Post ParsedPost = Parser.Parse(RawPost);

            DrawPost(ParsedPost);

            LoadingBar.IsEnabled = false;
            LoadingBar.IsIndeterminate = false;
            MainGrid.Visibility = Visibility.Visible;
        }

        private async void DrawPost(Post Post)
        {
            TextBlock TitleBlock = new TextBlock();
            TitleBlock.Text = Post.Title;
            TitleBlock.FontSize = 18;
            TitleBlock.FontWeight = FontWeights.SemiBold;
            Grid.SetRow(TitleBlock, 0);
            MainGrid.Children.Add(TitleBlock);

            StackPanel HeadPanel = new StackPanel();
            HeadPanel.Orientation = Orientation.Horizontal;
            Grid.SetRow(HeadPanel, 1);
            MainGrid.Children.Add(HeadPanel);

            TextBlock RatingBlock = new TextBlock();
            RatingBlock.Text = Post.Rating;
            HeadPanel.Children.Add(RatingBlock);

            Button AuthorButton = new Button();
            AuthorButton.Style = App.Current.Resources["InvisibleButton"] as Style;
            AuthorButton.Click += (s, e) =>
            {
                Frame.Navigate(typeof(ProfilePage), Post.Author);
            };

            StackPanel AuthorPanel = new StackPanel();
            AuthorPanel.Orientation = Orientation.Horizontal;
            AuthorButton.Content = AuthorPanel;

            Image AuthorImage = new Image();
            AuthorPanel.Children.Add(AuthorImage);

            TextBlock AuthorName = new TextBlock();
            AuthorName.Text = Post.Author;
            AuthorPanel.Children.Add(AuthorName);

            HeadPanel.Children.Add(AuthorButton);

            TextBlock InBlog = new TextBlock();
            InBlog.Text = " в блоге ";
            HeadPanel.Children.Add(InBlog);

            Button BlogButton = new Button();
            BlogButton.Style = App.Current.Resources["InvisibleButton"] as Style;
            BlogButton.Content = Post.Blog;
            BlogButton.Click += (s, e) =>
            {
                Frame.Navigate(typeof(BlogPage), Post.BlogId);
            };
            HeadPanel.Children.Add(BlogButton);

            // Основной контент
            Parsers.HtmlParser Parser = new Parsers.HtmlParser();
            RichTextBlock ArticleContentBlock = new RichTextBlock();
            ArticleContentBlock.Style = App.Current.Resources["StandartHtmlView"] as Style;
            ArticleContentBlock.Blocks.Add(await Parser.ConvertHTMLTextToParagraph(Post.Text));
            Grid.SetRow(ArticleContentBlock, 2);
            MainGrid.Children.Add(ArticleContentBlock);

            // Теги
            RichTextBlock TagsContentBlock = new RichTextBlock();
            TagsContentBlock.Style = App.Current.Resources["StandartHtmlView"] as Style;
            // TODO: Добавить отрисовку содержимого
            Grid.SetRow(TagsContentBlock, 3);
            MainGrid.Children.Add(TagsContentBlock);

            // Время публикации и количество комментариев
            StackPanel FooterPanel = new StackPanel();
            FooterPanel.Orientation = Orientation.Horizontal;
            Grid.SetRow(FooterPanel, 4);
            MainGrid.Children.Add(FooterPanel);

            TextBlock DateTimeBlock = new TextBlock();
            DateTimeBlock.Text = Post.DateTime;
            FooterPanel.Children.Add(DateTimeBlock);

            TextBlock CommentsCountBlock = new TextBlock();
            CommentsCountBlock.Text = Post.CommentsCount + " Комментариев";
            FooterPanel.Children.Add(CommentsCountBlock);
        }

        private void AuthorButton_Click(object sender, RoutedEventArgs e)
        {
            //Frame.Navigate(typeof(ProfilePage), AuthorBlock.Text);
        }

        private void CommentAuthorButton_Click(object sender, RoutedEventArgs e)
        {
            var authorfield = (sender as Button).Tag.ToString();
            Frame.Navigate(typeof(ProfilePage), authorfield);
        }

        // Костыль, потому что RichTextBlock не умеет в DataBindings
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

        /*private async void LeaveCommentButton_Click(object sender, RoutedEventArgs e)
        {
            //LeaveCommentDialog dialog = new LeaveCommentDialog(parser.postId, int.Parse((sender as Button).Tag.ToString()));
            //await dialog.ShowAsync();
            //await refreshComments();
        }*/

        /*public async Task<bool> refreshComments()
        {
            var newComments = await parser.refreshComments();
            if (newComments != null)
            {
                foreach (KeyValuePair<int, Comment> comment in newComments)
                {
                    var parentComment = comments.FindLast(x => x.id == comment.Key);
                    comment.Value.parentNode = parentComment;
                    parentComment.childNodes.Add(comment.Value);
                    if (comment.Key != 0)
                    {
                        int insertId;

                        if (comment.Value.parentNode.childNodes.Count != 0)
                            insertId = comments.FindLastIndex(x => x.id == comment.Value.parentNode.childNodes.Last().id) + 1;
                        else
                            insertId = comments.FindLastIndex(x => x.id == comment.Key) + 1;

                        comments.Insert(insertId, comment.Value);
                        CommentsBlock.Items.Insert(insertId, comment.Value);
                    }
                    else
                    {
                        comments.Add(comment.Value);
                        CommentsBlock.Items.Add(comment.Value);
                    }
                }
            }
            return true;
        }*/
    }
}