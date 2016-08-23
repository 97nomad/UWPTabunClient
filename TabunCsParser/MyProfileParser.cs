using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabunCsParser
{
    public class MyProfileParser : AbstractParser
    {
        HtmlNode RootNode;

        public MyProfile Parse(string Page)
        {
            RootNode = GetRootNodeOfPage(Page);

            if (RootNode.SelectSingleNode("//ul[@class='auth']") == null)
            {
                return new MyProfile()
                {
                    Avatar = GetAvatar(),
                    Nickname = GetNickname(),
                    Force = GetForce(),
                    Rating = GetRating(),
                    IsLoggedIn = true,
                };
            } else
            {
                return new MyProfile()
                {
                    Avatar = new Uri("https://cdn.everypony.ru/static/local/avatar_male_48x48.png"), // Магическая ссылка на стандартную аватарку
                    Nickname = "Guest",
                    Force = 0.0f,
                    Rating = 0.0f,
                    IsLoggedIn = false,
                };
            }
        }

        private Uri GetAvatar()
        {
            return new Uri(RootNode.SelectSingleNode("//img[@class='avatar']").Attributes["src"].Value);
        }

        private string GetNickname()
        {
            return RootNode.SelectSingleNode("//a[@class='username']").InnerText;
        }

        private float GetForce()
        {
            CultureInfo CI = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            CI.NumberFormat.CurrencyDecimalSeparator = ".";
            return float.Parse(RootNode.SelectSingleNode("//span[@class='strength']").InnerText.Trim(), NumberStyles.Any, CI);
        }

        private float GetRating()
        {
            CultureInfo CI = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            CI.NumberFormat.CurrencyDecimalSeparator = ".";
            return float.Parse(RootNode.SelectSingleNode("//span[@class='rating ']").InnerText.Trim(), NumberStyles.Any, CI);
        }
    }
}
