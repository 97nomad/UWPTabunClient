using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using UWPTabunClient.Parsers;
using TabunCsParser;
using TabunCsLibruary;
using Windows.UI.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using UWPTabunClient.Managers;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPTabunClient.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainpagePage : Page
    {
        HtmlParser Parser;

        public MainpagePage()
        {
            Parser = new HtmlParser();
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            LoadingBar.IsEnabled = true;

            int PageNumber = 1;
            if (e.Parameter != null)
                int.TryParse(e.Parameter.ToString(), out PageNumber);

            string RawPage = await new TabunAPI().GetMainPage(PageNumber);
            TabunCsParser.MainPage Page = new MainPageParser().Parse(RawPage);
            foreach (var P in Page.Posts)
            {
                await AddPostToList(P);
            }

            PostsList.ItemClick += (s, ev) =>
            {
                var item = (ev.ClickedItem as Grid);
                Frame.Navigate(typeof(PostPage), int.Parse(item.Tag.ToString()));
            };
            BackButton.Click += (s, ev) =>
            {
                Frame.Navigate(typeof(MainpagePage), PageNumber - 1);
            };
            ForwardButton.Click += (s, ev) =>
            {
                Frame.Navigate(typeof(MainpagePage), PageNumber + 1);
            };

            ForwardButton.Visibility = Visibility.Visible;
            if (PageNumber != 1)
                BackButton.Visibility = Visibility.Visible;

            LoadingBar.IsEnabled = false;
            LoadingBar.IsIndeterminate = false;
            PostsList.Visibility = Visibility.Visible;
        }

        private async Task<bool> AddPostToList(PostPreview Post)    // Возвращает ничего не значащий bool чтобы посты в нужном порядке шли
        { // За это безобразие винить кретина, который создал XAML
            Grid PostGrid = new Grid();
            RowDefinition RowDef0 = new RowDefinition();
            RowDefinition RowDef1 = new RowDefinition();
            RowDefinition RowDef2 = new RowDefinition();
            RowDefinition RowDef3 = new RowDefinition();
            RowDefinition RowDef4 = new RowDefinition();
            PostGrid.RowDefinitions.Add(RowDef0);
            PostGrid.RowDefinitions.Add(RowDef1);
            PostGrid.RowDefinitions.Add(RowDef2);
            PostGrid.RowDefinitions.Add(RowDef3);
            PostGrid.RowDefinitions.Add(RowDef4);

            PostGrid.VerticalAlignment = VerticalAlignment.Top;
            PostGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
            PostGrid.Tag = Post.Id;

            // Заголовок
            TextBlock Title = new TextBlock();
            Title.Text = Post.Title;
            Title.TextWrapping = TextWrapping.WrapWholeWords;
            Title.HorizontalAlignment = HorizontalAlignment.Left;
            Title.FontWeight = FontWeights.SemiBold;
            Title.FontSize = 18;
            Grid.SetRow(Title, 0);  // О_О
            PostGrid.Children.Add(Title);

            // Рейтинг, автор и блог
            StackPanel HeadPanel = new StackPanel();
            HeadPanel.Orientation = Orientation.Horizontal;
            Grid.SetRow(HeadPanel, 1);
            PostGrid.Children.Add(HeadPanel);

            TextBlock RatingBlock = new TextBlock();
            RatingBlock.Text = Post.Rating;
            RatingBlock.Margin = new Thickness(0, 0, 10, 0);
            RatingBlock.FontWeight = FontWeights.SemiBold;
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
            AuthorImage.Source = await WebManager.Instance.GetCachedImageSource(Post.AuthorImage);
            AuthorPanel.Children.Add(AuthorImage);

            TextBlock AuthorName = new TextBlock();
            AuthorName.Text = Post.Author;
            AuthorPanel.Children.Add(AuthorName);

            HeadPanel.Children.Add(AuthorButton);

            TextBlock InBlog = new TextBlock();
            InBlog.Text = "в блоге";
            InBlog.Margin = new Thickness(5, 0, 5, 0);
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
            RichTextBlock ArticleContentBlock = new RichTextBlock();
            ArticleContentBlock.Style = App.Current.Resources["StandartHtmlView"] as Style;
            ArticleContentBlock.Blocks.Add(await Parser.ConvertHTMLTextToParagraph(Post.Text));
            Grid.SetRow(ArticleContentBlock, 2);
            PostGrid.Children.Add(ArticleContentBlock);

            // Теги
            RichTextBlock TagsContentBlock = new RichTextBlock();
            TagsContentBlock.Style = App.Current.Resources["StandartHtmlView"] as Style;
            // TODO: Добавить отрисовку содержимого
            Grid.SetRow(TagsContentBlock, 3);
            PostGrid.Children.Add(TagsContentBlock);

            // Время публикации и количество комментариев
            StackPanel FooterPanel = new StackPanel();
            FooterPanel.Orientation = Orientation.Horizontal;
            Grid.SetRow(FooterPanel, 4);
            PostGrid.Children.Add(FooterPanel);

            TextBlock DateTimeBlock = new TextBlock();
            DateTimeBlock.Text = Post.DateTime;
            FooterPanel.Children.Add(DateTimeBlock);

            TextBlock CommentsCountBlock = new TextBlock();
            CommentsCountBlock.Text = Post.CommentsCount + " Комментариев";
            FooterPanel.Children.Add(CommentsCountBlock);

            PostsList.Items.Add(PostGrid);
            return true;
        }
    }
}
