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
    class TabunAPIManager : WebManager
    {
        public async Task<string> getStreamComments()
        {
            return await getAjaxAsync("https://tabun.everypony.ru/ajax/stream/comment/");
        }

        public async Task<string> getStreamTopics()
        {
            return await getAjaxAsync("https://tabun.everypony.ru/ajax/stream/topic/");
        }

        public async Task<string> addComment(int post_id, int reply, string text, bool isPost = true)
        {
            string uri = "https://tabun.everypony.ru/blog/ajaxaddcomment/";
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            list.Add(new KeyValuePair<string, string>("comment_text", text));
            list.Add(new KeyValuePair<string, string>("reply", reply.ToString()));
            list.Add(new KeyValuePair<string, string>("cmt_target_id", post_id.ToString()));

            return await getAjaxAsync(uri, list);
        }
    }
}
