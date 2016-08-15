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

        public async Task<string> GetBlog(string BlogName)
        {
            return await TWebClient.GetPageAsync(TabunGlobalVariables.LinkBlog + BlogName + "/");
        }

        public async Task<string> GetMainPage(int PageNumber = 1)
        {
            return await TWebClient.GetPageAsync(TabunGlobalVariables.LinkMainpage + "page" + PageNumber);
        }

        public async Task<string> GetStreamNewComments(Dictionary<string, string> Parameters)
        {
            return await TWebClient.GetPostAsync(TabunGlobalVariables.LinkAjaxStreamComments, Parameters);
        }

        public async Task<string> GetStreamNewTopics(Dictionary<string, string> Parameters)
        {
            return await TWebClient.GetPostAsync(TabunGlobalVariables.LinkAjaxStreamTopics, Parameters);
        }
    }
}
