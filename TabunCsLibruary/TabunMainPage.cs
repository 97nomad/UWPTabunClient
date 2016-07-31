using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabunCsLibruary
{
    public class TabunMainPage
    {
        TabunWebClient TWebClient;

        public TabunMainPage()
        {
            TWebClient = new TabunWebClient();
        }

        public async Task<string> GetPage(int PageNumber = 1)
        {
            return await TWebClient.GetPageAsync(TabunGlobalVariables.LinkMainpage + "page" + PageNumber);
        }
    }
}
