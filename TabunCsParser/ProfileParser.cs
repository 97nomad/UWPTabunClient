using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabunCsParser
{
    public class ProfileParser : AbstractParser
    {
        HtmlNode RootNode;

        public Profile Parse(string Page)
        {
            RootNode = GetRootNodeOfPage(Page);

            return new Profile()
            {
                About = GetAbout(),
                Avatar100x100 = GetAvatar100x100(),
                Blogs = GetBlogs(),
                DateOfBirdth = GetDateOfBirdth(),
                DateOfLastVisite = GetDateOfLastVisite(),
                DateOfRegistration = GetDateOfRegistration(),
                Force = GetForce(),
                Friends = GetFriends(),
                Name = GetName(),
                Nickname = GetNickname(),
                Place = GetPlace(),
                ProfilePhoto = GetProfilePhoto(),
                Rating = GetRating(),
                Sex = GetSex(),
                Votes = GetVotes(),
            };
        }

        private string GetAbout()
        {
            return RootNode.SelectSingleNode("//div[@class='profile-info-about']/div[@class='text']").InnerHtml;
        }

        private Uri GetAvatar100x100()
        {
            return new Uri(RootNode.SelectSingleNode("//img[@itemprop='photo']").Attributes["src"].Value);
        }

        private Dictionary<Uri, string> GetBlogs()
        {
            Dictionary<Uri, string> Result = new Dictionary<Uri, string>();
            HtmlNodeCollection BlogNodes = RootNode.SelectNodes("(//ul[@class='profile-dotted-list'])[2]/li[1]/strong/a");
            foreach (HtmlNode BlogNode in BlogNodes)
            {
                Result.Add(new Uri(BlogNode.Attributes["href"].Value), HtmlEntity.DeEntitize(BlogNode.InnerText));
            }
            return Result;
        }

        private string GetDateOfBirdth()
        {
            return RootNode.SelectSingleNode("(//ul[@class='profile-dotted-list'])[1]/li[2]/strong").InnerText.Trim();
        }

        private string GetDateOfLastVisite()
        {
            return RootNode.SelectSingleNode("(//ul[@class='profile-dotted-list'])[2]/li[3]/strong").InnerText.Trim();
        }

        private string GetDateOfRegistration()
        {
            return RootNode.SelectSingleNode("(//ul[@class='profile-dotted-list'])[2]/li[2]/strong").InnerText.Trim();
        }

        private float GetForce()
        {
            CultureInfo CI = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            CI.NumberFormat.CurrencyDecimalSeparator = ".";
            return float.Parse(RootNode.SelectSingleNode("//div[contains(@id, 'user_skill')]").InnerText, NumberStyles.Any, CI);
        }

        private Dictionary<string, Uri> GetFriends()
        {
            Dictionary<string, Uri> Result = new Dictionary<string, Uri>();
            HtmlNodeCollection FriendNodes = RootNode.SelectNodes("//ul[@class='user-list-avatar']/li");
            foreach (HtmlNode FriendNode in FriendNodes)
            {
                Result.Add(
                    FriendNode.InnerText.Trim(),
                    new Uri(FriendNode.SelectSingleNode(".//img[@class='avatar']").Attributes["src"].Value));
            }
            return Result;
        }

        private string GetName()
        {
            return HtmlEntity.DeEntitize(RootNode.SelectSingleNode("//p[@class='user-name']").InnerText.Trim());
        }

        private string GetNickname()
        {
            return HtmlEntity.DeEntitize(RootNode.SelectSingleNode("//h2[@itemprop='nickname']").InnerText.Trim());
        }

        private string GetPlace()
        {
            return HtmlEntity.DeEntitize(RootNode.SelectSingleNode("//strong[@itemprop='address']").InnerText.Trim());
        }

        private Uri GetProfilePhoto()
        {
            return new Uri(RootNode.SelectSingleNode("//img[@id='foto-img']").Attributes["src"].Value);
        }

        private float GetRating()
        {
            CultureInfo CI = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            CI.NumberFormat.CurrencyDecimalSeparator = ".";
            return float.Parse(RootNode.SelectSingleNode("//span[contains(@id, 'vote_total_user')]").InnerText, NumberStyles.Any, CI);
        }

        private string GetSex()
        {
            return RootNode.SelectSingleNode("(//ul[@class='profile-dotted-list'])[1]/li[1]/strong").InnerText.Trim();
        }

        private int GetVotes()
        {
            return int.Parse(RootNode.SelectSingleNode("//div[@class='vote-label']").InnerText.Trim().Replace("голосов: ", String.Empty));
        }
    }
}
