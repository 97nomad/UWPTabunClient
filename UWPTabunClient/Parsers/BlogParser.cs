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
            var blog_top = getFirstDescendantWithAttribute(rootNode, "div", "class", "blog-top");
            return getInnerTextFromFirstDescendantWithAttribute(blog_top, "h2", "class", "page-header");
        }

        public async Task<List<Post>> getPosts()
        {
            List<Post> resultPosts = new List<Post>();

            if (rootNode == null)
            {
                return null;
            }

            var articles = getArrayDescendants(rootNode, "article");

            foreach (HtmlNode article in articles)
            {
                var articleHeader = getFirstDescendant(article, "header");

                var articleTitle = articleHeader
                    .Descendants("h1").First()
                    .Descendants("a").First()
                    .InnerText;

                var articleUri = new Uri(articleHeader
                    .Descendants("h1").First()
                    .Descendants("a").First()
                    .Attributes["href"].Value);

                var articleId = Int32.Parse(articleUri
                    .Segments.Last()
                    .Replace(".html", String.Empty));

                var articleRating = articleHeader
                    .Descendants("span").First()
                    .Descendants("i").First()
                    .InnerText;

                var articleAuthor = articleHeader
                    .Descendants("a").Where(x => x.Attributes.Contains("rel")).First()
                    .InnerText;

                var articleAuthorImageUri = getFirstDescendant(articleHeader, "img")
                    .Attributes["src"].Value;

                var articleBlog = articleHeader
                    .Descendants("a").Where(x => x.Attributes.Contains("class")).First()
                    .InnerText;

                var articleBlogId = article
                    .Descendants().Where(x => isAttributeValueContains(x.Attributes, "class", "topic-blog"))
                    .First().Attributes["href"].Value;

                articleBlogId = UriParser.getLastPart(articleBlogId);

                var articleBody = await htmlParser.convertNodeToParagraph(
                    getFirstDescendantWithAttribute(article, "div", "class", "topic-content text"));

                var articleFooter = getFirstDescendantWithAttribute(article, "footer", "class", "topic-footer");

                var articleTags_tmp = articleFooter
                    .Descendants("a").Where(x => x.Attributes.Contains("rel")).ToArray();
                string articleTags = "";
                foreach (HtmlNode node in articleTags_tmp)
                {
                    articleTags += node.InnerText + " ";
                }

                //var articleDatatime = getFirstDescendant(articleFooter, "time")
                //    .Attributes["datetime"].Value;
                var articleDatatime = getInnerTextFromFirstDescendant(articleFooter, "time");

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
                    datatime = articleDatatime
                });
            }

            return resultPosts;
        }
    }
}
