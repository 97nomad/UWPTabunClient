using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabunCsLibruary
{
    public class TabunStream
    {
        TabunWebClient TWebClient;
        public TabunStream()
        {
            TWebClient = new TabunWebClient();
        }

        public async Task<string> GetNewComments(Dictionary<string, string> Parameters)
        {
            return await TWebClient.GetPostAsync(TabunGlobalVariables.LinkAjaxStreamComments, Parameters);
        }

        public async Task<string> GetNewTopics(Dictionary<string, string> Parameters)
        {
            return await TWebClient.GetPostAsync(TabunGlobalVariables.LinkAjaxStreamTopics, Parameters);
        }
    }
}
