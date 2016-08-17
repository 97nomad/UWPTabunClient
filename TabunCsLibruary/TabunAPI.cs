using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace TabunCsLibruary
{
    public class TabunAPI
    {
        TabunWebClient TWebClient;

        public TabunAPI()
        {
            TWebClient = new TabunWebClient();
        }

        private async Task<bool> CheckLSKAndID()
        {
            ApplicationDataContainer Storage = ApplicationData.Current.LocalSettings;
            if (Storage.Values["livestreet_security_key"] != null || Storage.Values["sessionId"] != null)
                return true;

            string Page = await TWebClient.GetPageAsync(TabunGlobalVariables.LinkLogin);

            foreach (string Line in Page.Split('\n'))   // Не совсем уверен в таком способе поиска
            {
                if (Line.Contains("LIVESTREET_SECURITY_KEY"))
                    Storage.Values["livestreet_security_key"] = Line.Split('\'')[1];
                if (Line.Contains("SESSION_ID"))
                    Storage.Values["sessionId"] = Line.Split('\'')[1];
            }

            return false;
        }

        public async Task<string> GetPost(int PostId)
        {
            await CheckLSKAndID();
            return await TWebClient.GetPageAsync(TabunGlobalVariables.LinkRoot + PostId + ".html");
        }

        public async Task<string> GetProfile(string Nickname)
        {
            await CheckLSKAndID();
            return await TWebClient.GetPageAsync(TabunGlobalVariables.LinkProfile + Nickname + "/");
        }

        public async Task<string> GetBlog(string BlogName)
        {
            await CheckLSKAndID();
            return await TWebClient.GetPageAsync(TabunGlobalVariables.LinkBlog + BlogName + "/");
        }

        public async Task<string> GetMainPage(int PageNumber = 1)
        {
            await CheckLSKAndID();
            return await TWebClient.GetPageAsync(TabunGlobalVariables.LinkMainpage + "page" + PageNumber);
        }

        public async Task<string> GetStreamNewComments()
        {
            await CheckLSKAndID();
            Dictionary<string, string> Parameters = new Dictionary<string, string>();
            ApplicationDataContainer Storage = ApplicationData.Current.LocalSettings;
            Parameters.Add("security_ls_key", Storage.Values["livestreet_security_key"] as string);

            string Result = await TWebClient.GetPostAsync(TabunGlobalVariables.LinkAjaxStreamComments, Parameters);
            return JsonConvert.DeserializeObject<JsonResponse>(Result).sText;
        }

        public async Task<string> GetStreamNewTopics()
        {
            await CheckLSKAndID();
            Dictionary<string, string> Parameters = new Dictionary<string, string>();
            ApplicationDataContainer Storage = ApplicationData.Current.LocalSettings;
            Parameters.Add("security_ls_key", Storage.Values["livestreet_security_key"] as string);

            string Result = await TWebClient.GetPostAsync(TabunGlobalVariables.LinkAjaxStreamTopics, Parameters);
            return JsonConvert.DeserializeObject<JsonResponse>(Result).sText;
        }

        public async Task<bool> AddComment(int PostId, int ReplyId, string Text)
        {
            await CheckLSKAndID();
            ApplicationDataContainer Storage = ApplicationData.Current.LocalSettings;
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { "comment_text", Text },
                { "reply", ReplyId.ToString() },
                { "cmt_target_id", PostId.ToString() },
                { "security_ls_key", Storage.Values["livestreet_security_key"] as string },
            };

            var Result = await TWebClient.GetPostAsync(TabunGlobalVariables.LinkAjaxAddComment, Parameters);
            var ParsedJson = JsonConvert.DeserializeObject<JsonResponse>(Result);
            return !ParsedJson.bStateError;
        }

        public async Task<string> GetNewTopicComments(int PostId, int LastCommentId)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { "idCommentLast", LastCommentId.ToString()},
                { "idTarget", PostId.ToString() },
                { "typeTarget", "topic" }
            };

            return await TWebClient.GetPostAsync(TabunGlobalVariables.LinkAjaxResponseComment, Parameters);
        }
    }
}
