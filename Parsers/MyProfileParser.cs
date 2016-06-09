using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPTabunClient.Parsers
{
    class MyProfileParser : AbstractParser
    {
        HtmlNode rootNode;

        public async Task<bool> loadPage()
        {
#if (LOCALMIRROR)
            string addr = "http://" + siteDomain + "/tabun.html";
#else
            string addr = "http://" + siteDomain + "/";
#endif

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
            var profileNode = getFirstDescendantWithAttribute(rootNode, "div", "class", "dropdown-user");
            var imagelink = getFirstDescendantWithAttribute(profileNode, "img", "class", "avatar").Attributes["src"].Value;

            SoftwareBitmapSource source = new SoftwareBitmapSource();
            var image = await webManager.getCachedImageAsync(normalizeImageUriDebug(imagelink));
            await source.SetBitmapAsync(image);
            return source;
        }

        public string getLogin()
        {
            var profileNode = getFirstDescendantWithAttribute(rootNode, "div", "class", "dropdown-user");
            return getInnerTextFromFirstDescendantWithAttribute(profileNode, "a", "class", "username");
        }

        public string getStrength()
        {
            var profileNode = getFirstDescendantWithAttribute(rootNode, "div", "class", "dropdown-user");
            return getInnerTextFromFirstDescendantWithAttribute(profileNode, "span", "class", "strength");
        }

        public string getRating()
        {
            var profileNode = getFirstDescendantWithAttribute(rootNode, "div", "class", "dropdown-user");
            return getInnerTextFromFirstDescendantWithAttribute(profileNode, "span", "class", "rating ");
        }
    }
}
