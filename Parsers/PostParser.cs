using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWPTabunClient.Models;
using HtmlAgilityPack;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Controls;
using UWPTabunClient.Managers;
using Newtonsoft.Json;

namespace UWPTabunClient.Parsers
{
    class PostParser : AbstractParser
    {
        HtmlNode rootNode;
        public HtmlParser htmlParser;
        public int postId;

        private int lastComment;

        public PostParser()
        {
            htmlParser = new HtmlParser();
            postId = 1;
            lastComment = 0;
        }

        public async Task<bool> loadPage(string name)
        {
            int.TryParse(name, out postId);
            rootNode = await getRootNodeOfPage(GlobalVariables.linkRoot + name + ".html"); // TODO: Разобраться, какого чёрта тут не взлетает HTTPS
            if (rootNode == null)
                return false;
            else
                return true;
        }

        public async Task<List<KeyValuePair<int, Comment>>> refreshComments()
        {
            Dictionary<string, string> parameters = new Dictionary<string,string>
            {
                { "idCommentLast", lastComment.ToString()},
                { "idTarget", postId.ToString() },
                { "typeTarget", "topic" }
            };

            var jsonText = await webManager.getPostAsync(GlobalVariables.linkAjaxResponseComment, parameters);
            var json = JsonConvert.DeserializeObject<JsonResponseComment>(jsonText);

            List<KeyValuePair<int, Comment>> resultPairs = new List<KeyValuePair<int, Comment>>();

            if (json.aComments.Count == 0)
                return null;

            lastComment = json.iMaxIdComment;

            foreach (Dictionary<string, string> dic in json.aComments)
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(dic["html"].ToString());
                int idParent = 0;
                int.TryParse(dic["idParent"], out idParent);
                resultPairs.Add(new KeyValuePair<int, Comment>(idParent, await parseSingleComment(doc.DocumentNode)));
            }

            return resultPairs;
        }

        public async Task<Post> getPost()
        {
            Post resultPost = new Post();

            if (rootNode == null)
                return new Post();

            var article = rootNode.SelectSingleNode("//article");
            var articleHeader = rootNode.SelectSingleNode("//header");
            var articleFooter = rootNode.SelectSingleNode("//footer");

            resultPost.id = postId;

            resultPost.title = article.SelectSingleNode(".//h1[@class='topic-title word-wrap']").InnerText;

            resultPost.author = article.SelectSingleNode(".//a[@rel='author']").InnerText;

            await resultPost.author_image.SetBitmapAsync(
                await webManager.getCachedImageAsync(
                    normalizeImageUriDebug(
                        article.SelectSingleNode(".//img[@class='avatar']")
                        .Attributes["src"].Value)));

            var blogNode = article.SelectSingleNode(".//a[@class='topic-blog'] | .//a[@class='topic-blog private-blog']");

            resultPost.blog = blogNode.InnerText;

            resultPost.blog_id = UriParser.getLastPart(blogNode
                .Attributes["href"].Value);

            resultPost.rating = article.SelectSingleNode(".//div[@class='vote-item vote-count']").InnerText;
            resultPost.votesTotal = article.SelectSingleNode(".//div[@class='vote-item vote-count']")
                .Attributes["title"].Value;

            resultPost.text = await htmlParser.convertNodeToParagraph(
                article.SelectSingleNode(".//div[@class='topic-content text']"));

            resultPost.datatime = article.SelectSingleNode(".//time").InnerText;

            foreach (HtmlNode node in article.SelectNodes(".//a[@rel='tag']"))
            {
                resultPost.tags += HtmlEntity.DeEntitize(node.InnerText) + " ";
            }

            resultPost.commentsCount = rootNode.SelectSingleNode(".//span[@id='count-comments']").InnerText;

            var new_comments_counter = rootNode.SelectSingleNode(".//div[@id='new_comments_counter']");
            if (new_comments_counter != null) {
                int.TryParse(new_comments_counter.Attributes["data-id-comment-last"].Value, out lastComment);
                resultPost.lastComment = lastComment;
            }

            return resultPost;
        }

        public async Task<Comment> getComments()
        {
            Comment resultComments = new Comment();

            if (rootNode == null)
                return new Comment();

            var commentSection = rootNode.SelectSingleNode(".//div[@class='comments']");

            await parseLevel(commentSection, resultComments);

            return resultComments;
        }

        private async Task<Comment> parseSingleComment(HtmlNode node)
        {
            HtmlNode commentSection = node.SelectSingleNode(".//section");

            Comment resultComment = new Comment();
            if (!isAttributeValueContains(commentSection.Attributes, "class", "comment-bad"))
            {
                SoftwareBitmapSource source = new SoftwareBitmapSource();
                await source.SetBitmapAsync(
                    await webManager.getCachedImageAsync(normalizeImageUriDebug(
                        commentSection.SelectSingleNode(".//ul[@class='comment-info']")
                        .SelectSingleNode(".//img[@class='comment-avatar']")
                        .Attributes["src"].Value)));

                resultComment.id = int.Parse(commentSection.Attributes["data-id"].Value);

                if (isAttributeValueContains(commentSection.Attributes, "class", "comment-new"))
                    resultComment.isRead = false;

                resultComment.text = await htmlParser.convertNodeToParagraph(commentSection.SelectSingleNode(".//div[@class='text']"));

                resultComment.author = commentSection.SelectSingleNode(".//ul[@class='comment-info']")
                    .SelectSingleNode("./li")
                    .InnerText
                    .Trim();

                resultComment.author_image = source;

                resultComment.datetime = commentSection.SelectSingleNode(".//time").InnerText;

                resultComment.rating = int.Parse(commentSection.SelectSingleNode(".//span[@class='vote-count']").InnerText);
            }
            else
            {
                resultComment.id = int.Parse(commentSection.Attributes["data-id"].Value);
                resultComment.text = await htmlParser.convertNodeToParagraph(commentSection.SelectSingleNode(".//div[@class='text']"));
            }

            return resultComment;
        }

        private async Task<int> parseLevel(HtmlNode node, Comment comm)
        {
            foreach (HtmlNode n in node.ChildNodes
                .Where(x => x.Name == "div" && isAttributeValueEquals(x.Attributes, "class", "comment-wrapper")))   // TODO: Переписать на XPath
            {
                var comment = await parseSingleComment(n);

                    comment.parentNode = comm;
                    comm.childNodes.Add(comment);

                if (n.HasChildNodes)
                    await parseLevel(n, comment);
            }
            return 1;
        }
    }
}
