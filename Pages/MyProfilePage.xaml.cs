using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
using UWPTabunClient.Parsers;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPTabunClient.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MyProfilePage : Page
    {
        private MyProfileParser parser;
        private bool isLoggedIn;

        private LoginDialog loginDialog;
        private ExitDialog exitDialog;

        public MyProfilePage()
        {
            this.InitializeComponent();
            parser = new MyProfileParser();
            loginDialog = new LoginDialog();
            exitDialog = new ExitDialog();
            isLoggedIn = false;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (await parser.loadPage())
            {
                if (parser.isUserLoggedIn())
                {
                    isLoggedIn = true;
                    ProfileImage.Source = await parser.getProfileImage();
                    LoginBlock.Text = parser.getLogin();
                    ForceBlock.Text = parser.getStrength();
                    VotesBlock.Text = parser.getRating();
                }
            }

            if (isLoggedIn)
                    LoginPanel.Visibility = Visibility.Visible;
                else
                    SinginPanel.Visibility = Visibility.Visible;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!isLoggedIn)
            {
                await loginDialog.ShowAsync();
                if (loginDialog.isLoginSuccessed)
                {
                    loginDialog.isLoginSuccessed = false;
                    isLoggedIn = true;
                    Frame.Navigate(typeof(MyProfilePage));
                }
            } else
            {
                await exitDialog.ShowAsync();
                if (exitDialog.isSecondButtonWasClicked)
                {
                    await loginDialog.logout();
                    exitDialog.isSecondButtonWasClicked = false;
                    isLoggedIn = false;
                    Frame.Navigate(typeof(MyProfilePage));
                }
            }

        }
    }
}
