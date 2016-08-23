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
using TabunCsLibruary;
using TabunCsParser;
using UWPTabunClient.Managers;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPTabunClient.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MyProfilePage : Page
    {
        private LoginDialog loginDialog;
        private ExitDialog exitDialog;
        private Frame frame;
        public bool IsLoggedIn;

        public MyProfilePage()
        {
            this.InitializeComponent();
            loginDialog = new LoginDialog();
            exitDialog = new ExitDialog();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            frame = e.Parameter as Frame;

            string RawPage = await new TabunAPI().GetMainPage();
            MyProfile Profile = new MyProfileParser().Parse(RawPage);

            IsLoggedIn = Profile.IsLoggedIn;
            ProfileImage.Source = await WebManager.Instance.GetCachedImageSource(Profile.Avatar);
            LoginBlock.Text = Profile.Nickname;
            ForceBlock.Text = Profile.Force.ToString();
            VotesBlock.Text = Profile.Rating.ToString();

            ProfileButton.Click += async (s, ev) =>
            {
                if (!IsLoggedIn)
                {
                    await loginDialog.ShowAsync();
                    if (loginDialog.isLoginSuccessed)
                    {
                        loginDialog.isLoginSuccessed = false;
                        Frame.Navigate(typeof(MyProfilePage));
                    }
                }
                else
                {
                    frame.Navigate(typeof(ProfilePage), Profile.Nickname);
                }
            };
        }
    }
}
