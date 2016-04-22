using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using UWPTabunClient.Models;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Controls;

namespace UWPTabunClient.Parsers
{
    class MainpageParser : AbstractParser
    {
        HtmlNode rootNode;
        public HtmlParser htmlParser;
        public int PageNumber { get; private set; }

        public MainpageParser()
        {
            htmlParser = new HtmlParser();
        }

        public async Task<bool> loadPage(int pagenumber = 1)
        {
            if (pagenumber < 1)         // Номер страницы не может быть меньше 1
                PageNumber = 1;
            else
                PageNumber = pagenumber;

#if (LOCALMIRROR)
            string addr = "http://" + siteDomain + "/tabun.html";
#else
            string addr = "http://" + siteDomain + "/index/page" + PageNumber + "/";
#endif
            rootNode = await getRootNodeOfPage(addr);
            if (rootNode == null)
                return false;
            else
                return true;
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

                var articleBlogId = articleHeader.Descendants().Where(x => isAttributeValueContains(x.Attributes, "class", "topic-blog"))
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
