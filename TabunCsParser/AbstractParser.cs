using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabunCsParser
{
    public class AbstractParser
    {
        public AbstractParser()
        {

        }

        public HtmlNode GetRootNodeOfPage(string page)
        {
            HtmlDocument doc = new HtmlDocument();
            try
            {
                doc.LoadHtml(page);
            }
            catch (Exception)
            {
                throw new System.ArgumentException("Не удалось загрузить страницу.");
            }
            return doc.DocumentNode;
        }
    }
}
