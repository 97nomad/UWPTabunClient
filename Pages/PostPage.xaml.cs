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
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using UWPTabunClient.Models;
using UWPTabunClient.Pages;
using UWPTabunClient.Parsers;
using System.Diagnostics;
using System.Threading.Tasks;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPTabunClient.Pages
{
    public sealed partial class PostPage : Page
    {
        Post post;
        List<Comment> comments;
        PostParser parser;

        public PostPage()
        {
            this.InitializeComponent();
            parser = new PostParser();
        }

        private void LoadPost()
        {
            TitleBlock.Text = post.title;
            RatingBlock.Text = post.rating;
            AuthorImage.Source = post.author_image;
            AuthorBlock.Text = post.author;
            BlogBlock.Text = post.blog;
            BlogButton.Tag = post.blog_id;

            BodyBlock.Blocks.Add(post.text);

            TagsBlock.Text = post.tags;
            DateTimeBlock.Text = post.datatime;
            CommentsCountBlock.Text = post.commentsCount;
        }

        private void LoadComments()
        {
            foreach (Comment c in comments)
            {
                CommentsBlock.Items.Add(c);
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            parser.htmlParser.frame = Frame;

            LoadingBar.IsEnabled = true;

            if (await parser.loadPage(e.Parameter.ToString()))
            {
                try {
                    post = await parser.getPost();
                    comments = (await parser.getComments()).Descendants();
                    LoadPost();
                    LoadComments();
                } catch (Exception exp)
                {
                    Frame.Navigate(typeof(ErrorPage), exp.Message);
                }
            } else
            {
                Frame.Navigate(typeof(ErrorPage), "Ошибка при загрузке страницы");
            }

            LoadingBar.IsEnabled = false;
            LoadingBar.IsIndeterminate = false;
            MainGrid.Visibility = Visibility.Visible;
        }

        private void AuthorButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProfilePage), AuthorBlock.Text);
        }

        private void CommentAuthorButton_Click(object sender, RoutedEventArgs e)
        {
            var authorfield = (sender as Button).Tag.ToString();
            Frame.Navigate(typeof(ProfilePage), authorfield);
        }

        // Костыль, потому что RichTextBlock не умеет в DataBindings
        private void RichTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            var rtb = sender as RichTextBlock;
            rtb.Blocks.Add(rtb.DataContext as Paragraph);
        }

        private void BlogButton_Click(object sender, RoutedEventArgs e)
        {
            string blogId = (sender as Button).Tag.ToString();
            Frame.Navigate(typeof(BlogPage), blogId);
        }

        private async void LeaveCommentButton_Click(object sender, RoutedEventArgs e)
        {
            LeaveCommentDialog dialog = new LeaveCommentDialog(parser.postId, int.Parse((sender as Button).Tag.ToString()));
            await dialog.ShowAsync();
            await refreshComments();
        }

        public async Task<bool> refreshComments()
        {
            var newComments = await parser.refreshComments();
            if (newComments != null)
            {
                foreach (KeyValuePair<int, Comment> comment in newComments)
                {
                    var parentComment = comments.FindLast(x => x.id == comment.Key);
                    comment.Value.parentNode = parentComment;
                    parentComment.childNodes.Add(comment.Value);
                    if (comment.Key != 0)
                    {
                        int insertId;

                        if (comment.Value.parentNode.childNodes.Count != 0)
                            insertId = comments.FindLastIndex(x => x.id == comment.Value.parentNode.childNodes.Last().id) + 1;
                        else
                            insertId = comments.FindLastIndex(x => x.id == comment.Key) + 1;

                        comments.Insert(insertId, comment.Value);
                        CommentsBlock.Items.Insert(insertId, comment.Value);
                    }
                    else
                    {
                        comments.Add(comment.Value);
                        CommentsBlock.Items.Add(comment.Value);
                    }
                }
            }
            return true;
        }
    }
}