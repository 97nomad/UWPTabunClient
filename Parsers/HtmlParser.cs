using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media.Imaging;
using UWPTabunClient.Pages;

namespace UWPTabunClient.Parsers
{
    class HtmlParser : AbstractParser
    {
        public Frame frame;

        public async Task<Paragraph> convertNodeToParagraph(HtmlNode node)
        {
            Paragraph paragraph = new Paragraph();
            foreach (HtmlNode n in node.ChildNodes.ToArray())
            {
                try
                {
                    var tmp = await parseNode(n);
                    if (tmp != null)
                        paragraph.Inlines.Add(tmp);
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                }
            }

            return paragraph;
        }

        public async Task<List<Paragraph>> convertNodeToListOfParagraph(HtmlNode node)
        {
            List<Paragraph> resultList = new List<Paragraph>();
            foreach (HtmlNode n in node.ChildNodes.ToArray())
            {
                try
                {
                    Paragraph par = new Paragraph();
                    var tmp = await parseNode(n);
                    if (tmp != null)
                        par.Inlines.Add(tmp);
                    resultList.Add(par);
                } catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                }
            }
            return resultList;
        }

        public async Task<List<Inline>> convertNodeToListOfInline(HtmlNode node)
        {
            List<Inline> resultList = new List<Inline>();
            foreach (HtmlNode n in node.ChildNodes.ToArray())
            {
                try
                {
                    Inline tmp = await parseNode(n);
                    if (tmp != null)
                        resultList.Add(tmp);
                } catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                }
            }
            return resultList;
        }

        private async Task<Inline> parseNode(HtmlNode node)
        {
            switch (node.Name)
            {
                case "#text":
                case "strong":
                case "em":
                case "s":
                case "u":
                case "pre":
                    return pText(node);
                case "br":
                    return pBr(node);
                case "h4":
                    return pH4(node);
                case "a":
                    return await pA(node);
                case "img":
                    return await pImg(node);
                case "span":
                    return await pSpan(node);
                case "blockquote":
                    return pBlockquote(node);
                case "ol":
                    return await pOl(node);
            }
            return null;
        }

        private async Task<Inline> pSpan(HtmlNode node)
        {
            if (!node.Attributes.Contains("class"))
            {
                return pText(node);
            }

            switch (node.Attributes["class"].Value)
            {
                case "spoiler":
                    return await pSpoiler(node);
                case "spoiler-gray":
                    return pLightSpoiler(node);
            }
            return null;
        }

        private Inline pText(HtmlNode node)
        {
            Run run = new Run();
            run.Text = HtmlEntity.DeEntitize(node.InnerText).Replace("\t", String.Empty).Replace("\n", String.Empty);//.Trim();
            return run;
        }

        private Inline pBr(HtmlNode node)
        {
            Run run = new Run();
            run.Text = "\n";
            return run;
        }

        private Inline pH4(HtmlNode node)
        {
            Run run = new Run();
            run.Text = HtmlEntity.DeEntitize(node.InnerText).Trim() + "\n";
            run.FontSize = 18;
            run.FontWeight = FontWeights.SemiBold;
            return run;
        }

        private Inline pBlockquote(HtmlNode node)
        {
            InlineUIContainer container = new InlineUIContainer();
            StackPanel panel = new StackPanel();
            TextBlock text = new TextBlock();

            text.Text = HtmlEntity.DeEntitize(node.InnerText).Trim();
            text.Margin = new Thickness(10, 0, 0, 0);
            text.FontWeight = FontWeights.SemiLight;

            panel.Orientation = Orientation.Horizontal;
            panel.Children.Add(text);
            container.Child = panel;
            return container;
        }

        private async Task<Inline> pA(HtmlNode node)
        {
            bool isContainsStrangeElemets = false; // Специальный флаг, который определяет, использовать ли Hyperlink или Button для отрисовки ссылки
            foreach (HtmlNode n in node.ChildNodes.ToArray())
            {
                if (n.Name != "#text")
                {
                    isContainsStrangeElemets = true;
                    break;
                }
            }

            if (isContainsStrangeElemets)
            {
                InlineUIContainer container = new InlineUIContainer();
                Button hpButton = new Button();
                hpButton.Style = App.Current.Resources["HyperLinkInvisibleButton"] as Style;
                hpButton.Margin = new Thickness(2, 0, 2, 0);

                RichTextBlock contentBlock = new RichTextBlock();
                contentBlock.IsTextSelectionEnabled = false;
                contentBlock.Blocks.Add(await convertNodeToParagraph(node));
                hpButton.Content = contentBlock;

                string link = normalizeUri(node.Attributes["href"].Value);
                UriParser.isInnerLink(node.Attributes["href"].Value);

                hpButton.Click += (s, e) =>
                {
                    UriParser.GoToPage(link, frame);
                };

                container.Child = hpButton;
                return container;
            }
            else
            {
                Hyperlink hyperlink = new Hyperlink();
                List<Inline> parseResult = await convertNodeToListOfInline(node);

                foreach (Inline i in parseResult)
                    hyperlink.Inlines.Add(i);

                string link = normalizeUri(node.Attributes["href"].Value);
                UriParser.isInnerLink(node.Attributes["href"].Value);

                hyperlink.Click += (s, e) =>
                {
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
                return hyperlink;
            }
        }

        private async Task<Inline> pImg(HtmlNode node)
        {
            double ImgMaxWidth = ApplicationView.GetForCurrentView().VisibleBounds.Width;
            InlineUIContainer container = new InlineUIContainer();
            Image image = new Image();
            SoftwareBitmapSource source = new SoftwareBitmapSource();

            var bitmap = await webManager.getCachedImageAsync(
                normalizeImageUriDebug(node.Attributes["src"].Value));
            await source.SetBitmapAsync(bitmap);
            image.Source = source;
            image.Stretch = Windows.UI.Xaml.Media.Stretch.None;

            if (bitmap.PixelWidth >= ImgMaxWidth)
            {
                image.Stretch = Windows.UI.Xaml.Media.Stretch.Uniform;
                image.MaxWidth = ImgMaxWidth;
            }

            if (node.Attributes.Contains("width"))
                image.MaxWidth = int.Parse(node.Attributes["width"].Value);
            if (node.Attributes.Contains("height"))
                image.MaxHeight = int.Parse(node.Attributes["height"].Value);
            if (node.Attributes.Contains("width") | node.Attributes.Contains("height"))
                image.Stretch = Windows.UI.Xaml.Media.Stretch.Uniform;

            container.Child = image;
            return container;
        }

        private async Task<Inline> pSpoiler(HtmlNode node)
        {
            HtmlNode title = node.SelectSingleNode(".//span[@class='spoiler-title']");
            HtmlNode body = node.SelectSingleNode(".//span[@class='spoiler-body']");

            InlineUIContainer container = new InlineUIContainer();
            StackPanel containerPanel = new StackPanel();
            containerPanel.Orientation = Orientation.Vertical;

            RichTextBlock titleBlock = new RichTextBlock();
            RichTextBlock bodyBlock = new RichTextBlock();
            bodyBlock.Visibility = Visibility.Collapsed;

            titleBlock.IsTextSelectionEnabled = false;
            bodyBlock.IsTextSelectionEnabled = false;
            bodyBlock.Margin = new Thickness(10, 0, 0, 0);

            titleBlock.Blocks.Add(await convertNodeToParagraph(title));
            foreach (Paragraph p in await convertNodeToListOfParagraph(body))
            {
                bodyBlock.Blocks.Add(p);
            }

            Button spoilerButton = new Button();
            spoilerButton.Content = titleBlock;

            container.Child = containerPanel;
            containerPanel.Children.Add(spoilerButton);
            containerPanel.Children.Add(bodyBlock);

            spoilerButton.Click += (s, e) =>
            {
                if (bodyBlock.Visibility == Visibility.Collapsed)
                    bodyBlock.Visibility = Visibility.Visible;
                else
                    bodyBlock.Visibility = Visibility.Collapsed;
            };

            return container;
        }
        private Inline pLightSpoiler(HtmlNode node)
        {
            InlineUIContainer container = new InlineUIContainer();
            Border border = new Border();
            TextBlock text = new TextBlock();

            border.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Colors.DarkGray);
            text.FontWeight = FontWeights.Light;

            text.Text = HtmlEntity.DeEntitize(node.InnerText);

            border.Child = text;
            container.Child = border;
            return container;
        }

        private async Task<Inline> pOl(HtmlNode node)
        {
            InlineUIContainer container = new InlineUIContainer();

            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Vertical;
            container.Child = panel;

            int i = 1;
            foreach (HtmlNode n in node.SelectNodes(".//li"))
            {
                RichTextBlock block = new RichTextBlock();
                block.IsTextSelectionEnabled = false;

                Run number = new Run();
                number.Text = i + ". ";
                i++;

                Paragraph par = await convertNodeToParagraph(n);
                par.Inlines.Insert(0, number);

                block.Blocks.Add(par);
                panel.Children.Add(block);
            }

            return container;
        }
    }
}