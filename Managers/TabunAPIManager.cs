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

        public async Task<JsonResponse> getStreamComments()
        {
            var response = await webManager.getPostAsync(GlobalVariables.linkAjaxStreamComments);
            var json = JsonConvert.DeserializeObject<JsonResponse>(response);
            return json;
        }

        public async Task<JsonResponse> getStreamTopics()
        {
            var response = await webManager.getPostAsync(GlobalVariables.linkAjaxStreamTopics);
            var json = JsonConvert.DeserializeObject<JsonResponse>(response);
            return json;
        }

        public async Task<bool> addComment(int post_id, int reply, string text, bool isPost = true)
        {
            string uri = GlobalVariables.linkAjaxAddComment;
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "comment_text", text },
                { "reply", reply.ToString() },
                { "cmt_target_id", post_id.ToString() },
            };

            var json = await webManager.getPostAsync(uri, parameters);
            var parsedjson = JsonConvert.DeserializeObject<JsonResponse>(json);
            return !parsedjson.bStateError;
        }
    }
}