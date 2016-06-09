using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using UWPTabunClient.Pages;
using UWPTabunClient.Parsers;

namespace UWPTabunClient.Managers
{
    class CrutchManager
    {
        public static void crutchForButtonsInRichTextBlock(Frame frame, object sender)
        {
            try
            {   // Костыли должны быть безопасными
                var rtb = sender as RichTextBlock;
                foreach (Inline inline in (rtb.DataContext as Paragraph).Inlines)
                {
                    // Сложно описать, как это работает, но оно работает
                    InlineUIContainer i = null;
                    try
                    {
                        i = inline as InlineUIContainer;
                    }
                    catch (Exception) { }

                    if (i != null && i.Child.GetType() == typeof(Button))
                    {
                        Button b = i.Child as Button;
                        b.Click += (s, ea) =>
                        {
                            string link = b.Tag as string;
                            switch (UriParser.getInnerLinkType(link))
                            {
                                case UriParser.PageType.Post:
                                    frame.Navigate(typeof(PostPage), UriParser.getLastPart(link));
                                    break;
                                case UriParser.PageType.Profile:
                                    frame.Navigate(typeof(ProfilePage), UriParser.getLastPart(link));
                                    break;
                                case UriParser.PageType.Blog:
                                    frame.Navigate(typeof(BlogPage), UriParser.getLastPart(link));
                                    break;
                            }
                        };
                    }
                }
            }
            catch (Exception) { }
        }
    }
}
