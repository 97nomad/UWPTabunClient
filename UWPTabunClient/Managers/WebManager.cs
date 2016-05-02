using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using System.IO;
using System.Diagnostics;
using Windows.Graphics.Imaging;
using Newtonsoft.Json;
using UWPTabunClient.Models;

namespace UWPTabunClient.Managers
{
    sealed class WebManager
    {
        private static readonly Lazy<WebManager> InstanceField = new Lazy<WebManager>(() => new WebManager());

        public static WebManager Instance { get { return InstanceField.Value; } }

        private List<KeyValuePair<string, SoftwareBitmap>> imagePool;   // Кажется, тут что-то течёт
        private CacheManager cache;

        private WebManager()
        {
            imagePool = new List<KeyValuePair<string, SoftwareBitmap>>();
            cache = new CacheManager();
        }

        public async Task<string> getAjaxAsync(string uri, List<KeyValuePair<string, string>> list = null)
        {
            var storage = Windows.Storage.ApplicationData.Current.LocalSettings;
            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
            
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.Accept = "application/json, text/javascript, */*; q=0.01";
            request.Method = "POST";
            request.Headers["X-Requested-With"] = "XMLHttpRequest";

            using (StreamWriter writer = new StreamWriter(await request.GetRequestStreamAsync()))
            {
                writer.WriteLine("security_ls_key=" + storage.Values["livestreet_security_key"] as string);
                if (list != null)
                    foreach (KeyValuePair<string, string> pair in list)
                        writer.WriteLine(pair.Key + "=" + pair.Value);
                writer.Flush();
            }

            var response = await request.GetResponseAsync();

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                string responseString = reader.ReadToEnd();
                var json = JsonConvert.DeserializeObject<JsonResponse>(responseString).sText;
                return json;
            }
        }

        public async Task<string> getPageAsync(string uri)
        {
            Debug.WriteLine("getPageAsync: " + uri);

            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler();
            handler.CookieContainer = cookieContainer;
            handler.UseCookies = true;

            var storage = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (storage.Values["sessionId"] != null)
                cookieContainer.Add(new Uri(GlobalVariables.linkRoot),
                    new Cookie("TABUNSESSIONID", (storage.Values["sessionId"] as string)));

            using (HttpClient client = new HttpClient(handler))
            {
                var response = await client.GetAsync(uri);

                response.EnsureSuccessStatusCode();
                string resultString = await response.Content.ReadAsStringAsync();

                return resultString;
            }
        }

        public async Task<SoftwareBitmap> getCachedImageAsync(string url)
        {
            // Провека, есть ли изображение в пуле
            foreach (KeyValuePair<string, SoftwareBitmap> kvp in imagePool)
            {
                if (kvp.Key == url)
                    return kvp.Value;
            }

            // Преобразуем строку в Uri
            KeyValuePair<string, string> uri = convertPathFilenameFromUri(url);

            // Проверка на существование файла на диске
            if (await cache.isFileActual(url))
            {
                SoftwareBitmap bitmap = await cache.readImageFile(url);

                return bitmap;
            } else
            {
                // Загрузка файла на диск
                SoftwareBitmap bitmap = null;

                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var response = await client.GetAsync(url);
                        response.EnsureSuccessStatusCode();

                        Stream inputStream = await response.Content.ReadAsStreamAsync();
                        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(inputStream.AsRandomAccessStream());
                        bitmap = await decoder.GetSoftwareBitmapAsync();
                        bitmap = SoftwareBitmap.Convert(bitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore);
                    }
                }
                catch (Exception)
                {
                    Debug.WriteLine("Ошибка при загрузке изображения: " + uri);
                }

                if (bitmap != null) // Если картинка загрузилась
                {
                    await cache.createImageFile(url, bitmap); // Запись загруженного файла на диск
                    imagePool.Add(new KeyValuePair<string, SoftwareBitmap>(url, bitmap));
                    return bitmap;
                }
            }

            return null;

        }

        public static KeyValuePair<string, string> convertPathFilenameFromUri(string url)
        {
            Uri uri = new Uri(url);
            string path = uri.Host;

            foreach (string str in uri.Segments.Take(uri.Segments.Length - 1))
                path += str.Replace(".", String.Empty);
            path = path.Replace('/', '\\');
            string filename = uri.Segments.Last();

            return new KeyValuePair<string, string>(path, filename);
        }
    }
}
