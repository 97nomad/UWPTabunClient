using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabunCsParser
{
    class CommentParser : AbstractParser
    {
        HtmlNode RootNode;

        public Comment Parse(string RawComment)
        {
            RootNode = GetRootNodeOfPage(RawComment);

            return new Comment
            {
                Id = GetId(),
                Author = GetAuthor(),
                AuthorImage = GetAuthorImage(),
                DateTime = GetDateTime(),
                IsRead = GetIsRead(),
                Rating = GetRating(),
                ParentId = GetParentId(),
                Text = GetText(),
            };
        }

        private int GetId()
        {
            return int.Parse(RootNode.Attributes["data-id"].Value);
        }

        private string GetAuthor()
        {
            return RootNode.SelectSingleNode("//li[@class='comment-author ']").InnerText.Trim();
        }

        private Uri GetAuthorImage()
        {
            return new Uri(RootNode.SelectSingleNode("//img[@class='comment-avatar']").Attributes["src"].Value);
        }

        private string GetDateTime()
        {
            return RootNode.SelectSingleNode("//time").InnerText.Trim();
        }

        private bool GetIsRead()
        {
            return !RootNode.Attributes["class"].Value.Contains("comment-new");
        }

        private int GetRating()
        {
            return int.Parse(RootNode.SelectSingleNode("//span[@class='vote-count']").InnerText.Trim());
        }

        private int GetParentId()
        {
            HtmlNode Node = RootNode.SelectSingleNode("//li[@class='goto goto-comment-parent']/a");
            if (Node != null)
            {
                Uri FullLink = new Uri(Node.Attributes["href"].Value);
                return int.Parse(FullLink.Segments.Last());
            }
            return 0;
        }

        private string GetText()
        {
            return RootNode.SelectSingleNode("//div[@class='text']").InnerHtml;
        }
    }
}
