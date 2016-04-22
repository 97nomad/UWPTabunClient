﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Diagnostics;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
using Windows.UI.ViewManagement;
using Windows.UI.Text;

namespace UWPTabunClient.Parsers
{
    class AbstractParser
    {
#if (LOCALMIRROR)
        protected const string siteDomain = "127.0.0.1";
        protected const string profileword = "/profile";
#else
        protected const string siteDomain = "tabun.everypony.ru";
        protected const string profileword = "";
#endif
        protected WebManager webManager;

        public AbstractParser()
        {
            webManager = new WebManager();
        }

        public async Task<HtmlNode> getRootNodeOfPage(string url)
        {
            try {
                Debug.WriteLine("Загрузка страницы " + url);
                string page = await webManager.getPageAsync(url);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(page);
                return doc.DocumentNode;
            } catch (Exception)
            {
                Debug.WriteLine("Ошибка при загрузке страницы " + url);
                return null;
            }
        }

        protected bool isAttributeValueEquals(HtmlAttributeCollection attributes, string attr, string str)
        {
            try
            {
                if (attributes.Contains(attr))
                    if (attributes[attr].Value == str)
                        return true;
            } catch (Exception) { }

            return false;
        }
        protected bool isAttributeValueContains(HtmlAttributeCollection attributes, string attr, string str)
        {
            try
            {
                if (attributes.Contains(attr))
                    if (attributes[attr].Value.Contains(str))
                        return true;
            }
            catch (Exception) { }

            return false;
        }

        public static string normalizeImageUriDebug(string uri)
        {
#if (LOCALMIRROR)
            return "http://" + siteDomain + uri.TrimStart('.');
#else
            if (uri.Contains("https://") || uri.Contains("http://"))
                return uri;
            else
            {
                //Debug.WriteLine("Кривая ссылка normalizeImageUriDebug " + uri);
                return "http://" + uri.TrimStart('/');
            }
#endif
        }

        public static string normalizeUri(string uri)
        {
            if (uri.First() == '/')
            {
                Debug.WriteLine("Кривая ссылка normalizeUri " + uri);
                return "http://" + siteDomain + uri;
            }
            return uri;
        }



        public static string getLivestreetSecurityKey(HtmlNode page)
        {
            try
            {
                var scripts = page.Descendants("script").ToArray();

                foreach (HtmlNode node in scripts)
                {
                    foreach (string line in node.InnerHtml.Split('\n'))
                    {
                        if (line.Contains("LIVESTREET_SECURITY_KEY"))
                            return line.Split('\'')[1]; // Индусотаааааааа! Но работает.
                    }
                }
            }
            catch (Exception) { }

            return null;
        }

        public static string getSessionId(HtmlNode page) // TODO дважды индусота и вообще не труЪ
        {
            try
            {
                var scripts = page.Descendants("script").ToArray();

                foreach (HtmlNode node in scripts)
                {
                    foreach (string line in node.InnerHtml.Split('\n'))
                    {
                        if (line.Contains("SESSION_ID"))
                            return line.Split('\'')[1]; // Индусотаааааааа! Но работает.
                    }
                }
            }
            catch (Exception) { }

            return null;
        }

        protected HtmlNode[] getArrayDescendants(HtmlNode node, string descendantName)
        {
            try {
                return node.Descendants(descendantName).ToArray();
            } catch (NullReferenceException)
            {
                return null;
            }
        }

        protected HtmlNode[] getArrayDescendantsWithAttribute
            (HtmlNode node, string descendantName, string attributeName, string attributeValue)
        {
            try {
                return node
                    .Descendants(descendantName)
                    .Where(x => isAttributeValueEquals(x.Attributes, attributeName, attributeValue))
                    .ToArray();
            } catch (NullReferenceException)
            {
                return null;
            }
        }

        protected HtmlNode getFirstDescendant(HtmlNode node, string descendantName)
        {
            try {
                return node
                    .Descendants(descendantName).First();
            } catch (NullReferenceException)
            {
                return null;
            }
        }

        protected HtmlNode getFirstDescendantWithAttribute
            (HtmlNode node, string descendantName, string attributeName, string attributeValue)
        {
            try {
                return node
                    .Descendants(descendantName)
                    .Where(x => isAttributeValueEquals(x.Attributes, attributeName, attributeValue))
                    .First();
            } catch (NullReferenceException)
            {
                return null;
            }
        }

        // Получить текст из первого дочернего элемента
        protected string getInnerTextFromFirstDescendant(HtmlNode node, string descendant)
        {
            try
            {
                return node.Descendants(descendant).First().InnerText.Trim();
            } catch (NullReferenceException)
            {
                return "";
            }
        }

        // Получить текст из первого дочернего элемента с заданными атрибутами
        protected string getInnerTextFromFirstDescendantWithAttribute
            (HtmlNode node, string descendant, string attributeName, string attributeValue)
        {
            try
            {
                return node
                    .Descendants(descendant)
                    .Where(x => isAttributeValueEquals(x.Attributes, attributeName, attributeValue))
                    .First()
                    .InnerText
                    .Trim();
            } catch (NullReferenceException)
            {
                return "";
            }

        }
    }
}
