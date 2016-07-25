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

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPTabunClient.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class ExitDialog : ContentDialog
    {
        public bool isSecondButtonWasClicked;

        public ExitDialog()
        {
            this.InitializeComponent();
            isSecondButtonWasClicked = false;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            isSecondButtonWasClicked = true;
        }

        public async Task<bool> logout()
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync("http://tabun.everypony.ru/login/exit/?security_ls_key=" + Windows.Storage.ApplicationData.Current.LocalSettings.Values["livestreet_security_key"] as string);
            }
            return true;
        }
    }
}
