﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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

// Документацию по шаблону элемента диалогового окна содержимого см. в разделе http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPTabunClient.Pages
{
    public sealed partial class LeaveCommentDialog : ContentDialog
    {
        public LeaveCommentDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {            
            string text = "";
            TextBoxComment.Document.GetText(Windows.UI.Text.TextGetOptions.None, out text);
            Debug.WriteLine(text);
        }

        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            var text = TextBoxComment.Document.Selection.Text;
            TextBoxComment.Document.Selection.Text = "<strong>" + text + "</strong>";
        }

        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            var text = TextBoxComment.Document.Selection.Text;
            TextBoxComment.Document.Selection.Text = "<em>" + text + "</em>";
        }

        private void StrikeoutButton_Click(object sender, RoutedEventArgs e)
        {
            var text = TextBoxComment.Document.Selection.Text;
            TextBoxComment.Document.Selection.Text = "<s>" + text + "</s>";
        }

        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            var text = TextBoxComment.Document.Selection.Text;
            TextBoxComment.Document.Selection.Text = "<u>" + text + "</u>";
        }
    }
}
