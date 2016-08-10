using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabunCsLibruary
{
    public class TabunAPI
    {
        TabunWebClient TWebClient;

        public TabunAPI()
        {
            TWebClient = new TabunWebClient();
        }

        public async Task<string> GetPost(int PostId)
        {
            return await TWebClient.GetPageAsync(TabunGlobalVariables.LinkRoot + PostId + ".html");
        }

        public async Task<string> GetProfile(string Nickname)
        {
            return await TWebClient.GetPageAsync(TabunGlobalVariables.LinkProfile + Nickname + "/");
        }
    }
}
