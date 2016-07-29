using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabunCsLibruary
{
    public class TabunStream
    {
        public TabunStream()
        {

        }

        public async Task<string> GetNewComments(Dictionary<string, string> Parameters)
        {
            return await TabunWebClient.GetPageAsync(TabunGlobalVariables.LinkAjaxStreamComments, Parameters);
        }

        public async Task<string> GetNewTopics(Dictionary<string, string> Parameters)
        {
            return await TabunWebClient.GetPageAsync(TabunGlobalVariables.LinkAjaxStreamTopics, Parameters);
        }
    }
}
