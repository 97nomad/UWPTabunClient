using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWPTabunClient.Pages;
using Windows.UI.Xaml.Controls;

namespace UWPTabunClient.Parsers
{
    class UriParser
    {
        public enum PageType { Unknown, Post, Profile, Blog  };

        public static bool isInnerLink(string link)
        {
            if (link.First() == '/')
                return true;

            Uri tmp = new Uri(link);
            if (tmp.Host.Equals("tabun.everypony.ru"))
                return true;

            return false;

        }

        public static PageType getInnerLinkType(Uri link)
        {
            try
            {
                switch (link.Segments[1].TrimEnd('/'))
                {
                    case "profile":
                        return PageType.Profile;
                    case "blog":
                        if (link.Segments.Last().Contains(".html"))
                            return PageType.Post;
                        else
                            return PageType.Blog;
                    default:
                        return PageType.Unknown;
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("Exception in getInnetLinkType");
                return PageType.Unknown;
            }
        }

        public static PageType getInnerLinkType(string link)
        {
            Uri uri = new Uri(link);
            return getInnerLinkType(uri);
        }

        public static void GoToPage(Uri uri, Frame frame)
        {
            PageType pagetype = getInnerLinkType(uri);
            string lastPartOfUri = getLastPart(uri);
            switch (pagetype)
            {
                case PageType.Blog:
                    frame.Navigate(typeof(BlogPage), lastPartOfUri);
                    break;
                case PageType.Post:
                    frame.Navigate(typeof(PostPage), lastPartOfUri);
                    break;
                case PageType.Profile:
                    frame.Navigate(typeof(ProfilePage), lastPartOfUri);
                    break;
                case PageType.Unknown:
                    frame.Navigate(typeof(ErrorPage), "Не удалось распознать тип страницы");
                    break;
            }
        }

        public static void GoToPage(string uri, Frame frame)
        {
            Uri link = new Uri(uri);
            GoToPage(link, frame);
        }

        public static string getLastPart(Uri link)
        {
            return link.Segments.Last().Replace(".html", String.Empty).Replace("\\", String.Empty).TrimEnd('/');
        }

        public static string getLastPart(string link)
        {
            Uri uri = new Uri(link);
            return getLastPart(uri);
        }
    }
}
