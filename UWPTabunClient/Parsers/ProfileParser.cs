using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWPTabunClient.Models;
using HtmlAgilityPack;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using UWPTabunClient.Managers;

namespace UWPTabunClient.Parsers
{
    class ProfileParser : AbstractParser
    {
        HtmlNode rootNode;
        public HtmlParser htmlParser;

        public ProfileParser()
        {
            htmlParser = new HtmlParser();
        }

        public async Task<bool> loadPage(string page)
        {
            string addr = GlobalVariables.linkProfile + page + "/";
            rootNode = await getRootNodeOfPage(addr);
            if (rootNode == null)
                return false;
            else
                return true;

        }

        public async Task<Profile> getProfile()
        {
            var profile = getFirstDescendantWithAttribute(rootNode, "div", "class", "profile");
            var profileLeft = getFirstDescendantWithAttribute(rootNode, "div", "class", "profile-left");
            var profileRight = getFirstDescendantWithAttribute(rootNode, "div", "class", "profile-right");
                
            var profileNickname = getInnerTextFromFirstDescendant(profile, "h2");
            var profileName = getInnerTextFromFirstDescendant(profile, "p");

            var profileForce = getInnerTextFromFirstDescendantWithAttribute(profile, "div", "class", "count");
            var profileRating = getInnerTextFromFirstDescendantWithAttribute(profile, "div", "class", "vote-item vote-count");

            var profileVotes = getInnerTextFromFirstDescendantWithAttribute(profile, "div", "class", "vote-label");


            // Жестокий парсинг голого HTML
            var profileAbout = new Paragraph();
            try {
                var profileAboutNodes = rootNode
                    .Descendants("div").Where(x => isAttributeValueEquals(x.Attributes, "class", "profile-info-about")).First()
                    .Descendants("div").Where(x => isAttributeValueEquals(x.Attributes, "class", "text")).First();
                profileAbout = await htmlParser.convertNodeToParagraph(profileAboutNodes);
            } catch (Exception)
            {
            }

            var profileDotLists = getArrayDescendantsWithAttribute(profileLeft, "ul", "class", "profile-dotted-list");

            string profileSex = "";
            string profileDateOfBirdth = "";
            string profilePlace = "";
            HtmlNode profileCreated = null;
            HtmlNode profileAdministrated = null;
            HtmlNode profileConsistsIn = null;
            string profileDateOfRegistration = "";
            string profileLastVisite = "";

            foreach (HtmlNode dottedList in profileDotLists) {
                var list = getArrayDescendants(dottedList, "li");
                foreach (HtmlNode node in list)
                {
                    var span = getInnerTextFromFirstDescendant(node, "span");
                    if (span.Contains("Пол:"))
                        profileSex = getInnerTextFromFirstDescendant(node, "strong");
                    if (span.Contains("Дата рождения:"))
                        profileDateOfBirdth = getInnerTextFromFirstDescendant(node, "strong");
                    if (span.Contains("Местоположение:"))
                        profilePlace = getInnerTextFromFirstDescendant(node, "strong");
                    if (span.Contains("Создал:"))
                        profileCreated = getFirstDescendant(node, "strong");
                    if (span.Contains("Администрирует:"))
                        profileAdministrated = getFirstDescendant(node, "strong");
                    if (span.Contains("Состоит в:"))
                        profileConsistsIn = getFirstDescendant(node, "strong");
                    if (span.Contains("Зарегистрирован:"))
                        profileDateOfRegistration = getInnerTextFromFirstDescendant(node, "strong");
                    if (span.Contains("Последний визит:"))
                        profileLastVisite = getInnerTextFromFirstDescendant(node, "strong");
                }
            }

            var profileBlogConsistIn = await htmlParser.convertNodeToParagraph(profileConsistsIn);

            HtmlNode profileFriendsNode = getFirstDescendantWithAttribute(profileLeft, "ul", "class", "user-list-avatar");
            List<Friend> profileFriends = new List<Friend>();
            foreach (HtmlNode node in getArrayDescendants(profileFriendsNode, "a"))
            {
                var friendAvatarPath = getFirstDescendant(node, "img")
                    .Attributes["src"].Value;
                var friendName = node.InnerText.Trim();
                SoftwareBitmapSource source = new SoftwareBitmapSource();
                await source.SetBitmapAsync(
                    await webManager.getCachedImageAsync(normalizeImageUriDebug(friendAvatarPath)));
                profileFriends.Add(new Friend
                {
                    avatar_100x100 = source,
                    name = friendName,
                });
            }

            var profileContactLists = getArrayDescendantsWithAttribute(profileRight, "ul", "class", "profile-contact-list");
            List<string> profileContacts = new List<string>();
            foreach (HtmlNode list in profileContactLists)
            {
                foreach (HtmlNode node in getArrayDescendants(list, "a"))
                {
                    profileContacts.Add(node.InnerText);
                }
            }

            SoftwareBitmapSource profileBigPhoto = new SoftwareBitmapSource();
            await profileBigPhoto.SetBitmapAsync(
                await webManager.getCachedImageAsync(
                    normalizeImageUriDebug(
                        getFirstDescendantWithAttribute(rootNode, "img", "class", "profile-photo")
                        .Attributes["src"].Value)));

            SoftwareBitmapSource profileAvatar_100x100 = new SoftwareBitmapSource();
            await profileAvatar_100x100.SetBitmapAsync(
                await webManager.getCachedImageAsync(
                    normalizeImageUriDebug(
                        getFirstDescendantWithAttribute(rootNode, "img", "itemprop", "photo")
                        .Attributes["src"].Value)));

            Profile resultProfile = new Profile
            {
                nickname = profileNickname,
                name = profileName,
                force = profileForce,
                rating = profileRating,
                votes = profileVotes,
                about = profileAbout,
                sex = profileSex,
                dateOfBirdth = profileDateOfBirdth,
                place = profilePlace,
                profile_photo = profileBigPhoto,
                avatar_100x100 = profileAvatar_100x100,
                dateOfRegistration = profileDateOfRegistration,
                dateOfLastVisite = profileLastVisite,
                friends = profileFriends,
                contacts = profileContacts,
                blogsConsistIn = profileBlogConsistIn,
            };

            return resultProfile;
        }

    }
}
