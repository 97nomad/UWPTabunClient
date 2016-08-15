using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabunCsParser
{
    public class BlogParser : AbstractParser
    {
        HtmlNode RootNode;

        public Blog Parse(string page)
        {
            RootNode = GetRootNodeOfPage(page);

            return new Blog
            {
                Posts = GetPosts(),
                Description = GetDescription(),
                Name = GetName(),
                Rating = GetRating(),
                Votes = GetVotes(),
            };
        }

        private List<PostPreview> GetPosts()
        {
            List<PostPreview> Result = new List<PostPreview>();
            PostPreviewParser Parser = new PostPreviewParser();

            HtmlNodeCollection Articles = RootNode.SelectNodes("//article");

            foreach (HtmlNode Article in Articles)
            {
                Result.Add(Parser.Parse(Article.InnerHtml));
            }
            return Result;
        }

        private string GetDescription()
        {
            return RootNode.SelectSingleNode("//div[@class='blog-content text']").InnerHtml;
        }

        public string GetName()
        {
            return HtmlEntity.DeEntitize(RootNode.SelectSingleNode("//h2[@class='page-header']").InnerText.Trim());
        }

        private float GetRating()
        {
            CultureInfo CI = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            CI.NumberFormat.CurrencyDecimalSeparator = ".";
            return float.Parse(RootNode.SelectSingleNode("//span[contains(@id, 'vote_total_blog')]").InnerText, NumberStyles.Any, CI);
        }

        private int GetVotes()
        {
            return int.Parse(RootNode.SelectSingleNode("//div[@class='vote-item vote-count']")
                .Attributes["title"].Value.Trim()
                .Replace("голосов: ", String.Empty));
        }
    }
}
