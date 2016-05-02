using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UWPTabunClient.Models;

namespace UWPTabunClient.Managers
{
    class TabunAPIManager
    {
        private WebManager webManager;

        public TabunAPIManager()
        {
            webManager = WebManager.Instance;
        }

        public async Task<string> getStreamComments()
        {
            return await webManager.getAjaxAsync(GlobalVariables.linkAjaxStreamComments);
        }

        public async Task<string> getStreamTopics()
        {
            return await webManager.getAjaxAsync(GlobalVariables.linkAjaxStreamTopics);
        }

        public async Task<string> addComment(int post_id, int reply, string text, bool isPost = true)
        {
            string uri = GlobalVariables.linkAjaxAddComment;
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            list.Add(new KeyValuePair<string, string>("comment_text", text));
            list.Add(new KeyValuePair<string, string>("reply", reply.ToString()));
            list.Add(new KeyValuePair<string, string>("cmt_target_id", post_id.ToString()));

            return await webManager.getAjaxAsync(uri, list);
        }
    }
}
