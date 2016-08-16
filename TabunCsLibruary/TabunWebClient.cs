using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;

namespace TabunCsLibruary
{
    public sealed class TabunWebClient
    {
        //private static readonly Lazy<WebClient> InstanceField = new Lazy<WebClient>(() => new WebClient());

        //public static WebClient Instance { get { return InstanceField.Value; } }
        private async Task<bool> CheckLSKAndSID()
        {
            ApplicationDataContainer Storage = ApplicationData.Current.LocalSettings;
            if (Storage.Values["livestreet_security_key"] != null || Storage.Values["session_id"] != null)
            {
                return false;
            }

            string Page = "";
            using (HttpClient Client = new HttpClient())
            {
                HttpResponseMessage response = await Client.GetAsync(TabunGlobalVariables.LinkLogin);
                response.EnsureSuccessStatusCode();

                Page = await response.Content.ReadAsStringAsync();
            }

            foreach (string Line in Page.Split('\n'))
            {
                if (Line.Contains("LIVESTREET_SECURITY_KEY"))
                    Storage.Values["livestreet_security_key"] = Line.Split('\'')[1];
                if (Line.Contains("SESSION_ID"))
                    Storage.Values["session_id"] = Line.Split('\'')[1];
            }

            return true;
        }

        public async Task<string> GetPostAsync(string Url, Dictionary<string, string> Parameters = null)
        {
            CookieContainer Cookies = new CookieContainer();
            HttpClientHandler Handler = new HttpClientHandler()
            {
                CookieContainer = Cookies,
                UseCookies = true,
            };

            await CheckLSKAndSID();
            ApplicationDataContainer Storage = ApplicationData.Current.LocalSettings;
            Cookies.Add(new Uri(TabunGlobalVariables.LinkRoot),
                new Cookie("TABUNSESSIONID", (Storage.Values["sessionId"] as string)));

            using (HttpClient Client = new HttpClient(Handler))
            {
                if (Parameters == null) Parameters = new Dictionary<string, string>();

                FormUrlEncodedContent Content = new FormUrlEncodedContent(Parameters);
                HttpResponseMessage Response = await Client.PostAsync(Url, Content);
                Response.EnsureSuccessStatusCode();

                return await Response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> GetPageAsync(string Url)
        {
            CookieContainer Cookies = new CookieContainer();
            HttpClientHandler Handler = new HttpClientHandler();
            Handler.CookieContainer = Cookies;
            Handler.UseCookies = true;

            ApplicationDataContainer Storage = ApplicationData.Current.LocalSettings;
            await CheckLSKAndSID();
            Cookies.Add(new Uri(TabunGlobalVariables.LinkRoot),
                new Cookie("TABUNSESSIONID", (Storage.Values["sessionId"] as string)));

            using (HttpClient client = new HttpClient(Handler))
            {
                var response = await client.GetAsync(Url);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }


        // Чёртова магия, без которой нормально не работает стрим
        public async Task<string> GetAjaxAsync(string uri, Dictionary<string, string> Parameters)
        {
            ApplicationDataContainer Storage = ApplicationData.Current.LocalSettings;
            HttpWebRequest Request = WebRequest.Create(uri) as HttpWebRequest;

            Request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            Request.Accept = "application/json, text/javascript, */*; q=0.01";
            Request.Method = "POST";
            Request.Headers["X-Requested-With"] = "XMLHttpRequest";

            using (StreamWriter Writer = new StreamWriter(await Request.GetRequestStreamAsync()))
            {
                foreach (KeyValuePair<string, string> Pair in Parameters)
                    Writer.WriteLine(Pair.Key + "=" + Pair.Value);
                Writer.Flush();
            }

            var Response = await Request.GetResponseAsync();

            using (var reader = new StreamReader(Response.GetResponseStream()))
            {
                string responseString = reader.ReadToEnd();
                var json = JsonConvert.DeserializeObject<JsonResponse>(responseString).sText;
                return json;
            }
        }
    }
}
