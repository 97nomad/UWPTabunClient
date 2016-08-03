using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabunCsParser
{
    public class Post
    {
        public int Id;
        public string Title;
        public string Author;
        public Uri AuthorImage;
        public string Blog;
        public string BlogId;
        public string Rating;
        public string VotesTotal;
        public string Text;
        public List<string> Tags;
        public string DateTime;
        public string CommentsCount;
        public int LastCommentId;
        public List<Comment> Comments;
    }
}
