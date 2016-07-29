using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TabunCsLibruary
{
    public sealed class TabunWebClient
    {
        //private static readonly Lazy<WebClient> InstanceField = new Lazy<WebClient>(() => new WebClient());

        //public static WebClient Instance { get { return InstanceField.Value; } }

        public static async Task<string> GetPageAsync(string Url, Dictionary<string, string> Parameters = null)
        {
            HttpClient client = new HttpClient();
            if (Parameters == null) Parameters = new Dictionary<string, string>();

            FormUrlEncodedContent Content = new FormUrlEncodedContent(Parameters);
            HttpResponseMessage Response = await client.PostAsync(Url, Content);
            Response.EnsureSuccessStatusCode();

            return await Response.Content.ReadAsStringAsync();
        }
    }
}
