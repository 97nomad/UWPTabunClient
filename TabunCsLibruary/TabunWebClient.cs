using System;
using System.Collections.Generic;
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
            HttpClient Client = new HttpClient();
            if (Parameters == null) Parameters = new Dictionary<string, string>();

            FormUrlEncodedContent Content = new FormUrlEncodedContent(Parameters);
            HttpResponseMessage Response = await Client.PostAsync(Url, Content);
            Response.EnsureSuccessStatusCode();

            return await Response.Content.ReadAsStringAsync();
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
    }
}
