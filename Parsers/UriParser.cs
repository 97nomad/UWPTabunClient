using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static PageType getInnerLinkType(string link)
        {
            try {
                Uri tmp = new Uri(link);
                switch (tmp.Segments[1].TrimEnd('/'))
                {
                    case "profile":
                        return PageType.Profile;
                    case "blog":
                        if (tmp.Segments.Last().Contains(".html"))
                            return PageType.Post;
                        else
                            return PageType.Blog;
                    default:
                        return PageType.Unknown;
                }
            } catch (Exception)
            {
                Debug.WriteLine("Exception in getInnetLinkType");
                return PageType.Unknown;
            }
        }

        public static string getLastPart(string link)
        {
            Uri tmp = new Uri(link);
            return tmp.Segments.Last().Replace(".html", String.Empty).Replace("\\", String.Empty).TrimEnd('/');
        }
    }
}
