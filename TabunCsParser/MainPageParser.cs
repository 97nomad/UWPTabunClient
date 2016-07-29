using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabunCsParser
{
    public class MainPageParser : AbstractParser
    {
        HtmlNode RootNode;

        public MainPageParser()
        {
        }

        public async Task<MainPage> Parse(string page)
        {
            RootNode = GetRootNodeOfPage(page);

            return new MainPage
            {
                Page = GetPage(),
                Posts = GetPosts(),
            };
        }

        private int GetPage()
        {
            int Result = 0;
            int.TryParse(RootNode.SelectSingleNode("//div[@class='pagination']//li[@class='active']").InnerText, out Result);
            return Result;
        }

        private List<string> GetPosts()
        {
            List<string> Result = new List<string>();

            HtmlNodeCollection Articles = RootNode.SelectNodes("//article");

            foreach (HtmlNode Article in Articles)
            {
                Result.Add(Article.InnerHtml);
            }
            return Result;
        }
    }
}
