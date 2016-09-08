using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Windows.Storage;
using System.IO;
using TabunCsParser;

namespace TabunCsParserTest
{
    [TestClass]
    public class ProfileParsersTesting
    {
        [TestMethod]
        public void FullProfileParseTest()
        {
            // Я ненавижу Microsoft!
            StorageFolder Folder = Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets").GetResults();
            var PageFileTask = Folder.GetFileAsync(@"97nomadProfileTest.html");
            PageFileTask.AsTask().Wait();   // Особенно вот за это.
            StorageFile PageFile = PageFileTask.GetResults();
            string RawPage = File.ReadAllText(PageFile.Path);

            ProfileParser Parser = new ProfileParser();
            Profile ParsedProfile = Parser.Parse(RawPage);

            Assert.AreEqual(ParsedProfile.About, "Фырфырфыр. Текст для теста", "О себе");

            Assert.AreEqual(ParsedProfile.Avatar100x100.ToString(), "https://cdn.everypony.ru/storage/00/62/22/2016/08/21/avatar_100x100.png", false, "Маленькая аватарка");

            Assert.IsTrue(ParsedProfile.Blogs.ContainsValue("HERP DERP"), "Содержит блог HERP DERP");
            Assert.AreEqual(ParsedProfile.Blogs.Count, 13, "Количество блогов");

            Assert.AreEqual(ParsedProfile.DateOfBirdth, "15 апреля 1997", "Дата рождения");

            Assert.AreEqual(ParsedProfile.DateOfLastVisite, "07 сентября 2016, 13:56", "Дата последнего визита");

            Assert.AreEqual(ParsedProfile.DateOfRegistration, "07 августа 2012, 00:36", "Дата регистрации");

            Assert.AreEqual(ParsedProfile.Force, 2696.88F, 0.0F, "Сила");

            Assert.AreEqual(ParsedProfile.Friends.Count, 3, "Количество друзей");
            Assert.IsTrue(ParsedProfile.Friends.ContainsKey("sparkling_feather"), "Содержит друга sparkling_feather");

            Assert.AreEqual(ParsedProfile.Name, "Иван Пономарёв", false, "Имя");

            Assert.AreEqual(ParsedProfile.Nickname, "97nomad", false, "Ник");

            Assert.AreEqual(ParsedProfile.Place, "Украина, Бахчисарай", false, "Местоположение");

            Assert.AreEqual(ParsedProfile.ProfilePhoto.ToString(), "https://cdn.everypony.ru/storage/00/62/22/2015/12/25/7228c2.jpg", "Большая аватарка");

            Assert.AreEqual(ParsedProfile.Rating, 921.25F, 0.0F, "Рейтинг");

            Assert.AreEqual(ParsedProfile.Sex, "мужской", "Пол");

            Assert.AreEqual(ParsedProfile.Votes, 25, "Количество голосов");
        }

        [TestMethod]
        public void testProfileParserTest()
        {
            StorageFolder Folder = Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets").GetResults();
            var PageFileTask = Folder.GetFileAsync(@"testProfileTest.html");
            PageFileTask.AsTask().Wait();
            StorageFile PageFile = PageFileTask.GetResults();
            string RawPage = File.ReadAllText(PageFile.Path);

            ProfileParser Parser = new ProfileParser();
            Profile ParsedProfile = Parser.Parse(RawPage);

            Assert.AreEqual(ParsedProfile.About, "", "О себе");

            Assert.AreEqual(ParsedProfile.Avatar100x100, "https://cdn.everypony.ru/static/local/avatar_male_100x100.png", "Маленькая аватарка");

            Assert.AreEqual(ParsedProfile.Blogs.Count, 0, "Количество блогов");

            Assert.AreEqual(ParsedProfile.DateOfBirdth, "", "Дата рождения");

            Assert.AreEqual(ParsedProfile.DateOfLastVisite, "", "Дата последнего визита");

            Assert.AreEqual(ParsedProfile.DateOfRegistration, "", "Дата регистрации");

            Assert.AreEqual(ParsedProfile.Force, 0.0f, "Сила");

            Assert.AreEqual(ParsedProfile.Friends.Count, 1, "Количество друзей");

            Assert.AreEqual(ParsedProfile.Name, "", "Имя");

            Assert.AreEqual(ParsedProfile.Nickname, "test", "Ник");

            Assert.AreEqual(ParsedProfile.Place, "", "Местоположение");

            Assert.AreEqual(ParsedProfile.ProfilePhoto, "https://cdn.everypony.ru/static/local/user_photo_male.png", "Большая аватарка");

            Assert.AreEqual(ParsedProfile.Rating, 3.19f, "Рейтинг");

            Assert.AreEqual(ParsedProfile.Sex, "", "Пол");

            Assert.AreEqual(ParsedProfile.Votes, 1, "Количество голосов");
        }
    }
}
