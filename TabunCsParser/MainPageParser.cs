﻿using HtmlAgilityPack;
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
                Page = GetPageNumber(),
                Posts = GetPosts(),
            };
        }

        private int GetPageNumber()
        {
            int Result = 0;
            int.TryParse(RootNode.SelectSingleNode("//div[@class='pagination']//li[@class='active']").InnerText, out Result);
            return Result;
        }

        private List<PostPreview> GetPosts()
        {
            List<PostPreview> Result = new List<PostPreview>();
            PostPreviwParser Parser = new PostPreviwParser();

            HtmlNodeCollection Articles = RootNode.SelectNodes("//article");

            foreach (HtmlNode Article in Articles)
            {
                Parser.Parse(Article.InnerText);
            }
            return Result;
        }
    }
}
