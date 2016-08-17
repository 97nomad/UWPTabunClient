using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabunCsParser
{
    public class StreamParser : AbstractParser
    {
        HtmlNode RootNode;
        public StreamParser()
        {

        }

        public List<StreamElement> Parse(string Page)
        {
            var Json = JsonConvert.DeserializeObject<JsonResponse>(Page).sText;

            RootNode = GetRootNodeOfPage(Json);
            List<StreamElement> Result = new List<StreamElement>();

            HtmlNodeCollection RawElements = RootNode.SelectNodes("//ul[@class='latest-list']/li");
            foreach (HtmlNode Node in RawElements)
            {
                Result.Add(new StreamElement
                {
                    Author = Node.SelectSingleNode(".//a[@class='author']").InnerText,
                    Blog = HtmlEntity.DeEntitize(Node.SelectSingleNode(".//a[@class='stream-blog']").InnerText),
                    TopicName = HtmlEntity.DeEntitize(Node.SelectSingleNode(".//a[@class='stream-topic']").InnerText),
                    CommentsCount = int.Parse(Node.SelectSingleNode(".//span[@class='block-item-comments']").InnerText),
                    Link = Node.SelectSingleNode(".//a[@class='stream-topic']").Attributes["href"].Value,
                });
            }

            return Result;
        }
    }
}
