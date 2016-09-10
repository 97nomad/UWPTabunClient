using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabunCsParser
{
    public class PostParser : AbstractParser
    {
        HtmlNode RootNode;

        public Post Parse(string RawText)
        {
            RootNode = GetRootNodeOfPage(RawText);

            return new Post
            {
                Id = GetId(),
                Title = GetTitle(),
                Author = GetAuthor(),
                AuthorImage = GetAuthorImage(),
                Blog = GetBlog(),
                BlogId = GetBlogId(),
                Rating = GetRating(),
                VotesTotal = GetVotesTotal(),
                Text = GetText(),
                Tags = GetTags(),
                DateTime = GetDateTime(),
                CommentsCount = GetCommentsCount(),
                LastCommentId = GetLastCommentId(),
                Comments = GetComments(),
            };
        }

        private int GetId()
        {
            Uri FullLink = new Uri(RootNode.SelectSingleNode("//link[@rel='canonical']").Attributes["href"].Value);
            return int.Parse(FullLink.Segments.Last().Split('.')[0]);
        }

        private string GetTitle()
        {
            return HtmlEntity.DeEntitize(RootNode.SelectSingleNode("//h1[@class='topic-title word-wrap']").InnerText);
        }

        private string GetAuthor()
        {
            return RootNode.SelectSingleNode("//a[@rel='author']").InnerText;
        }

        private Uri GetAuthorImage()
        {
            return new Uri(RootNode.SelectSingleNode("//div[@class='topic-info']/a/img[@class='avatar']").Attributes["src"].Value);
        }

        private string GetBlog()
        {
            return HtmlEntity.DeEntitize(   // TODO: Переписать XPath на нормальный 
                RootNode.SelectSingleNode("//a[@class='topic-blog'] | //a[@class='topic-blog private-blog']").InnerText);
        }

        private string GetBlogId()
        {
            return new Uri(
                RootNode.SelectSingleNode("//a[@class='topic-blog'] | //a[@class='topic-blog private-blog']")
                .Attributes["href"].Value).Segments.Last();
        }

        private string GetRating()
        {
            return RootNode.SelectSingleNode("//div[@class='vote-item vote-count']").InnerText.Trim();
        }

        private string GetVotesTotal()
        {
            return RootNode.SelectSingleNode("//div[@class='vote-item vote-count']").Attributes["title"].Value;
        }

        private string GetText()
        {
            return RootNode.SelectSingleNode(".//div[@class='topic-content text']").InnerHtml;
        }

        private List<string> GetTags()
        {
            List<string> Result = new List<string>();

            foreach (HtmlNode node in RootNode.SelectNodes("//a[@rel='tag']"))
            {
                Result.Add(HtmlEntity.DeEntitize(node.InnerText));
            }
            return Result;
        }

        private string GetDateTime()
        {
            return HtmlEntity.DeEntitize(RootNode.SelectSingleNode("//time").InnerText.Trim());
        }

        private string GetCommentsCount()
        {
            return RootNode.SelectSingleNode("//span[@id='count-comments']").InnerText.Trim();
        }

        private int GetLastCommentId()
        {
            int LastComment = 0;
            HtmlNode NewCommentsCounter = RootNode.SelectSingleNode("//div[@id='new_comments_counter']");
            if (NewCommentsCounter != null)
            {
                int.TryParse(NewCommentsCounter.Attributes["data-id-comment-last"].Value, out LastComment);
            }
            return LastComment;
        }

        private List<Comment> GetComments()
        {
            List<Comment> Result = new List<Comment>();
            CommentParser Parser = new CommentParser();
            HtmlNodeCollection CommentNodes = RootNode.SelectNodes("//section[@class='comment  ']");
            if (CommentNodes != null) {
                foreach (HtmlNode N in CommentNodes)
                {
                    Result.Add(Parser.Parse(N.OuterHtml));
                }
            }

            return Result;
        }
    }
}
