﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class SettingsPage : Page
    {
        private ExitDialog exitDialog;

        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private async void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            exitDialog = new ExitDialog();
            await exitDialog.ShowAsync();
            if (exitDialog.isSecondButtonWasClicked)
            {
                await exitDialog.logout();
                exitDialog.isSecondButtonWasClicked = false;
            }       
        }
    }
}