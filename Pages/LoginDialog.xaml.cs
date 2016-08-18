using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using UWPTabunClient.Models;
using UWPTabunClient.Parsers;
using UWPTabunClient.Managers;
using TabunCsLibruary;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPTabunClient.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class LoginDialog : ContentDialog
    {
        private HtmlNode rootNode;
        public bool isLoginSuccessed;
        private AbstractParser parser;

        public LoginDialog()
        {
            this.InitializeComponent();
            isLoginSuccessed = false;
            parser = new AbstractParser();
        }

        private async void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var response = await new TabunAPI().Login(Login.Text, Password.Password);
            JsonResponse json = JsonConvert.DeserializeObject<JsonResponse>(response);
            if (json.bStateError == false)
                isLoginSuccessed = true;
        }
    }
}
