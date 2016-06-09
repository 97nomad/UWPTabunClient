using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWPTabunClient.Managers;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPTabunClient.Parsers
{
    class MyProfileParser : AbstractParser
    {
        HtmlNode rootNode;

        public async Task<bool> loadPage()
        {
            string addr = GlobalVariables.linkRoot;

            rootNode = await getRootNodeOfPage(addr);

            if (rootNode == null)
                return false;
            else
                return true;
        }

        public bool isUserLoggedIn()
        {
            try
            {
                getFirstDescendantWithAttribute(rootNode, "div", "class", "dropdown-user");
                return true;
            } catch (Exception)
            {
                return false;
            }
        }

        public async Task<SoftwareBitmapSource> getProfileImage()
        {
            var profileNode = rootNode.SelectSingleNode(".//div[@class='dropdown-user']");
            var imagelink = profileNode.SelectSingleNode(".//img[@class='avatar']").Attributes["src"].Value;

            SoftwareBitmapSource source = new SoftwareBitmapSource();
            var image = await webManager.getCachedImageAsync(normalizeImageUriDebug(imagelink));
            await source.SetBitmapAsync(image);
            return source;
        }

        public string getLogin()
        {
            var profileNode = rootNode.SelectSingleNode(".//div[@class='dropdown-user']");
            return rootNode.SelectSingleNode(".//a[@class='username']").InnerText;
        }

        public string getStrength()
        {
            var profileNode = rootNode.SelectSingleNode(".//div[@class='dropdown-user']");
            return profileNode.SelectSingleNode(".//span[@class='strength']").InnerText;
        }

        public string getRating()
        {
            var profileNode = rootNode.SelectSingleNode(".//div[@class='dropdown-user']");
            return profileNode.SelectSingleNode(".//span[@class='rating ']").InnerText;
        }
    }
}
