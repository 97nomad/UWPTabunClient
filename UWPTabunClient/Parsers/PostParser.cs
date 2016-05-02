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

namespace UWPTabunClient.Parsers
{
    class PostParser : AbstractParser
    {
        HtmlNode rootNode;
        public HtmlParser htmlParser;
        public int postId;

        public PostParser()
        {
            htmlParser = new HtmlParser();
            postId = 1;
        }

        public async Task<bool> loadPage(string name)
        {
            int.TryParse(name, out postId);
            rootNode = await getRootNodeOfPage(GlobalVariables.linkRoot + name + ".html");
            if (rootNode == null)
                return false;
            else
                return true;
        }

        public async Task<Post> getPost()
        {
            Post resultPost = new Post();

            if (rootNode == null)
                return new Post();

            var article = getFirstDescendant(rootNode, "article");
            var articleHeader = getFirstDescendant(rootNode, "header");
            var articleFooter = getFirstDescendant(rootNode, "footer");

            resultPost.id = postId;

            resultPost.title = getInnerTextFromFirstDescendantWithAttribute(article, "h1", "class", "topic-title word-wrap");

            resultPost.author = getInnerTextFromFirstDescendantWithAttribute(article, "a", "rel", "author");

            await resultPost.author_image.SetBitmapAsync(
                await webManager.getCachedImageAsync(
                    normalizeImageUriDebug(
                        getFirstDescendantWithAttribute(article, "img", "class", "avatar")
                        .Attributes["src"].Value)));

            var blogNode = article.Descendants().Where(x => isAttributeValueContains(x.Attributes, "class", "topic-blog"))
                .First();

            resultPost.blog = blogNode.InnerText;

            resultPost.blog_id = UriParser.getLastPart(blogNode
                .Attributes["href"].Value);

            resultPost.rating = getInnerTextFromFirstDescendantWithAttribute(article, "div", "class", "vote-item vote-count");
            resultPost.votesTotal = getFirstDescendantWithAttribute(article, "div", "class", "vote-item vote-count")
                .Attributes["title"].Value;

            resultPost.text = await htmlParser.convertNodeToParagraph(
                getFirstDescendantWithAttribute(article, "div", "class", "topic-content text"));

            resultPost.datatime = getInnerTextFromFirstDescendant(article, "time");

            foreach (HtmlNode node in getArrayDescendantsWithAttribute(article, "a", "rel", "tag"))
            {
                resultPost.tags += HtmlEntity.DeEntitize(node.InnerText) + " ";
            }

            resultPost.commentsCount = getInnerTextFromFirstDescendantWithAttribute(rootNode, "span", "id", "count-comments");

            return resultPost;
        }

        public async Task<Comment> getComments()
        {
            Comment resultComments = new Comment();

            if (rootNode == null)
                return new Comment();

            var commentSection = getFirstDescendantWithAttribute(rootNode, "div", "class", "comments");

            await parseLevel(commentSection, resultComments);

            return resultComments;
        }

        private async Task<int> parseLevel(HtmlNode node, Comment comm)
        {
            foreach (HtmlNode n in node.ChildNodes
                .Where(x => x.Name == "div" && isAttributeValueEquals(x.Attributes, "class", "comment-wrapper")))
            {
                HtmlNode section = n.ChildNodes.Where(x => x.Name == "section").First();

                Comment comment = new Comment();
                if (!isAttributeValueContains(section.Attributes, "class", "comment-bad"))
                {

                    SoftwareBitmapSource source = new SoftwareBitmapSource();
                    await source.SetBitmapAsync(
                        await webManager.getCachedImageAsync(normalizeImageUriDebug(
                            section.Descendants("ul")
                            .Where(x => isAttributeValueEquals(x.Attributes, "class", "comment-info"))
                            .First()
                            .Descendants("img")
                            .Where(x => isAttributeValueEquals(x.Attributes, "class", "comment-avatar"))
                            .First()
                            .Attributes["src"]
                            .Value)));

                    comment.id = Int32.Parse(section.Attributes["data-id"].Value);
                    comment.text = await htmlParser.convertNodeToParagraph(getFirstDescendantWithAttribute(section, "div", "class", "text"));

                    comment.author = section.Descendants("ul")
                        .Where(x => isAttributeValueEquals(x.Attributes, "class", "comment-info"))
                        .First()
                        .Descendants("li")
                        .First()
                        .InnerText
                        .Trim();
                    comment.author_image = source;

                    comment.datetime = getInnerTextFromFirstDescendant(section, "time");

                    comment.rating = Int32.Parse(getInnerTextFromFirstDescendantWithAttribute(section, "span", "class", "vote-count"));

                    comment.parentNode = comm;

                    comm.childNodes.Add(comment);
                } else
                {
                    comment.id = Int32.Parse(section.Attributes["data-id"].Value);
                    comment.text = await htmlParser.convertNodeToParagraph(getFirstDescendantWithAttribute(section, "div", "class", "text"));

                    comment.parentNode = comm;
                    comm.childNodes.Add(comment);
                }

                if (n.HasChildNodes)
                    await parseLevel(n, comment);
            }
            return 1;
        }
    }
}
