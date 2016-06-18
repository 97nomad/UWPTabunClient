using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UWPTabunClient.Managers;
using UWPTabunClient.Models;

namespace UWPTabunClient.Parsers
{
    class StreamParser : AbstractParser
    {
        HtmlNode rootNode;
        TabunAPIManager api;

        public StreamParser()
        {
            api = new TabunAPIManager();
        }

        private async Task<bool> loadPage(bool isComment)
        {
            HtmlDocument doc = new HtmlDocument();

            if (isComment)
                doc.LoadHtml((await api.getStreamComments()).sText);
            else
                doc.LoadHtml((await api.getStreamTopics()).sText);
            rootNode = doc.DocumentNode;

            if (rootNode == null)
                return false;
            else
                return true;
       } 

        public async Task<List<StreamElement>> getStreamElements(bool isComment)
        {
            await loadPage(isComment);

            List<StreamElement> resultList = new List<StreamElement>();

            var streamNode = rootNode.SelectSingleNode(".//ul[@class='latest-list']");
            var streamElements = streamNode.SelectNodes("./li");

            foreach (HtmlNode node in streamElements)
            {
                var nodeAuthor = node.SelectSingleNode(".//a[@class='author']").InnerText;
                var nodeBlog = node.SelectSingleNode(".//a[@class='stream-blog']").InnerText;
                var nodeTopic = node.SelectSingleNode(".//a[@class='stream-topic']").InnerText;
                nodeTopic = HtmlEntity.DeEntitize(nodeTopic);
                var nodeCommentsCount = int.Parse(node.SelectSingleNode(".//span[@class='block-item-comments']").InnerText);

                var nodeLink = node.SelectSingleNode(".//a[@class='stream-topic']").Attributes["href"].Value;
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
