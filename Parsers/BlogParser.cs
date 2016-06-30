using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using UWPTabunClient.Models;
using UWPTabunClient.Managers;

namespace UWPTabunClient.Parsers
{
    class BlogParser : AbstractParser
    {
        HtmlNode rootNode;
        public HtmlParser htmlParser;

        public BlogParser()
        {
            htmlParser = new HtmlParser();
        }

        public async Task<bool> loadPage(string name)
        {
            string addr = GlobalVariables.linkBlog + name + "/";

            rootNode = await getRootNodeOfPage(addr);
            if (rootNode == null)
                return false;
            else
                return true;
        }

        public string getBlogName()
        {
            var blog_top = rootNode.SelectSingleNode(".//div[@class='blog-top']");
            return blog_top.SelectSingleNode(".//h2[@class='page-header']").InnerText;
        }

        public async Task<List<Post>> getPosts()
        {
            List<Post> resultPosts = new List<Post>();

            if (rootNode == null)
            {
                return null;
            }

            var articles = rootNode.SelectNodes(".//article");

            foreach (HtmlNode article in articles)
            {
                var articleHeader = article.SelectSingleNode(".//header");

                var articleTitle = articleHeader.SelectSingleNode(".//h1/a")
                    .InnerText;

                var articleUri = new Uri(articleHeader.SelectSingleNode(".//h1/a")
                    .Attributes["href"].Value);

                var articleId = Int32.Parse(articleUri
                    .Segments.Last()
                    .Replace(".html", String.Empty));

                var articleRating = articleHeader.SelectSingleNode(".//span/i").InnerText;

                var articleAuthor = articleHeader.SelectSingleNode(".//a[@rel]")
                    .InnerText;

                var articleAuthorImageUri = articleHeader.SelectSingleNode(".//img")
                    .Attributes["src"].Value;

                var articleBlog = articleHeader.SelectSingleNode(".//a[@class]")
                    .InnerText;

                var articleBlogId = article.SelectSingleNode(".//*[@class='topic-blog']")
                    .Attributes["href"].Value;

                articleBlogId = UriParser.getLastPart(articleBlogId);

                var articleBody = await htmlParser.convertNodeToParagraph(
                    article.SelectSingleNode(".//div[@class='topic-content text']"));

                var articleFooter = article.SelectSingleNode(".//footer[@class='topic-footer']");

                var articleTags_tmp = articleFooter.SelectNodes(".//a[@rel]");
                string articleTags = "";
                foreach (HtmlNode node in articleTags_tmp)
                {
                    articleTags += node.InnerText + " ";
                }

                var articleDatatime = articleFooter.SelectSingleNode(".//time").InnerText;

                var articleCommentsCount = articleFooter.SelectSingleNode(".//li[@class='topic-info-comments']").InnerText.Trim();

                SoftwareBitmapSource source = new SoftwareBitmapSource();
                await source.SetBitmapAsync(
                    await webManager.getCachedImageAsync(normalizeImageUriDebug(articleAuthorImageUri)));

                resultPosts.Add(new Post
                {
                    id = articleId,
                    title = articleTitle,
                    author = articleAuthor,
                    author_image = source,
                    blog = " " + articleBlog, // Чтобы не сливался с "в блоге"
                    blog_id = articleBlogId,
                    rating = articleRating,
                    text = articleBody,
                    //text = HtmlEntity.DeEntitize(articleBody).Trim(),
                    tags = articleTags,
                    datatime = articleDatatime,
                    commentsCount = articleCommentsCount,
                });
            }

            return resultPosts;
        }
    }
}
