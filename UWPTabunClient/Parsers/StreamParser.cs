using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWPTabunClient.Models;

namespace UWPTabunClient.Parsers
{
    class StreamParser : AbstractParser
    {
        HtmlNode rootNode;

        private async Task<bool> loadPage(string uri)
        {
            //rootNode = await getRootNodeOfPage(addr);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(await WebManager.getAjaxAsync(uri));
            rootNode = doc.DocumentNode;
            if (rootNode == null)
                return false;
            else
                return true;

        }

        public async Task<List<StreamElement>> getStreamElements(bool isComment)
        {
            if (isComment)
                await loadPage("http://tabun.everypony.ru/ajax/stream/comment/");
            else
                await loadPage("http://tabun.everypony.ru/ajax/stream/topic/");

            List<StreamElement> resultList = new List<StreamElement>();

            var streamNode = getFirstDescendantWithAttribute(rootNode, "ul", "class", "latest-list");
            var streamElements = getArrayDescendants(streamNode, "li");

            foreach (HtmlNode node in streamElements)
            {
                var nodeAuthor = getInnerTextFromFirstDescendantWithAttribute(node, "a", "class", "author");
                var nodeBlog = getInnerTextFromFirstDescendantWithAttribute(node, "a", "class", "stream-blog");
                var nodeTopic = getInnerTextFromFirstDescendantWithAttribute(node, "a", "class", "stream-topic");
                nodeTopic = HtmlEntity.DeEntitize(nodeTopic);
                var nodeCommentsCount = int.Parse(getInnerTextFromFirstDescendantWithAttribute(node, "span", "class", "block-item-comments"));

                var nodeLink = getFirstDescendantWithAttribute(node, "a", "class", "stream-topic").Attributes["href"].Value;
                var nodeUri = new Uri(nodeLink);


                resultList.Add(new StreamElement
                {
                    author = nodeAuthor,
                    blog = nodeBlog,
                    topic = nodeTopic,
                    comments_count = nodeCommentsCount,
                    link = nodeUri.Segments.Last().TrimEnd('#').Replace(".html", String.Empty),
                });
            }

            return resultList;
        }
    }
}
