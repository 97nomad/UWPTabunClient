﻿using System;
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
            rootNode = await getRootNodeOfPage("http://tabun.everypony.ru/" + name + ".html"); // TODO: Разобраться, какого чёрта тут не взлетает HTTPS
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

            lastComment = int.Parse(
                getFirstDescendantWithAttribute(rootNode, "div", "id", "new_comments_counter")
                .Attributes["data-id-comment-last"].Value);
            resultPost.lastComment = lastComment;

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

        private async Task<Comment> parseSingleComment(HtmlNode node)
        {
            HtmlNode commentSection = getFirstDescendant(node, "section");

            Comment resultComment = new Comment();
            if (!isAttributeValueContains(commentSection.Attributes, "class", "comment-bad"))
            {
                SoftwareBitmapSource source = new SoftwareBitmapSource();
                await source.SetBitmapAsync(
                    await webManager.getCachedImageAsync(normalizeImageUriDebug(
                        commentSection.Descendants("ul")
                        .Where(x => isAttributeValueEquals(x.Attributes, "class", "comment-info"))
                        .First()
                        .Descendants("img")
                        .Where(x => isAttributeValueEquals(x.Attributes, "class", "comment-avatar"))
                        .First()
                        .Attributes["src"]
                        .Value)));

                resultComment.id = Int32.Parse(commentSection.Attributes["data-id"].Value);

                if (isAttributeValueContains(commentSection.Attributes, "class", "comment-new"))
                    resultComment.isRead = false;

                resultComment.text = await htmlParser.convertNodeToParagraph(getFirstDescendantWithAttribute(commentSection, "div", "class", "text"));

                resultComment.author = commentSection.Descendants("ul")
                    .Where(x => isAttributeValueEquals(x.Attributes, "class", "comment-info"))
                    .First()
                    .Descendants("li")
                    .First()
                    .InnerText
                    .Trim();
                resultComment.author_image = source;

                resultComment.datetime = getInnerTextFromFirstDescendant(commentSection, "time");

                resultComment.rating = Int32.Parse(getInnerTextFromFirstDescendantWithAttribute(commentSection, "span", "class", "vote-count"));
            }
            else
            {
                resultComment.id = Int32.Parse(commentSection.Attributes["data-id"].Value);
                resultComment.text = await htmlParser.convertNodeToParagraph(getFirstDescendantWithAttribute(commentSection, "div", "class", "text"));
            }

            return resultComment;
        }

        private async Task<int> parseLevel(HtmlNode node, Comment comm)
        {
            foreach (HtmlNode n in node.ChildNodes
                .Where(x => x.Name == "div" && isAttributeValueEquals(x.Attributes, "class", "comment-wrapper")))
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
