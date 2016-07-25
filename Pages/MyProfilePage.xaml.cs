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
        public bool isLoggedIn;
        public string username;

        private LoginDialog loginDialog;
        private ExitDialog exitDialog;
        private Frame frame;

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
            frame = e.Parameter as Frame;

            if (await parser.loadPage())
            {
                if (parser.isUserLoggedIn())
                {
                    isLoggedIn = true;
                    username = parser.getLogin();
                    ProfileImage.Source = await parser.getProfileImage();
                    LoginBlock.Text = username;
                    ForceBlock.Text = parser.getStrength();
                    VotesBlock.Text = parser.getRating();
                }
                else
                {
                    isLoggedIn = false;
                    LoginBlock.Text = "Guest";
                    ForceBlock.Text = "null";
                    VotesBlock.Text = "null";
                }
            }
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
                frame.Navigate(typeof(ProfilePage), username);
                /*await exitDialog.ShowAsync();
                if (exitDialog.isSecondButtonWasClicked)
                {
                    await loginDialog.logout();
                    exitDialog.isSecondButtonWasClicked = false;
                    isLoggedIn = false;
                    Frame.Navigate(typeof(MyProfilePage));
                }*/
            }

        }
    }
}
