using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabunCsParser
{
    public class Comment
    {
        public int Id;
        public int ParentId;
        public string Text;
        public string Author;
        public Uri AuthorImage;
        public string DateTime;
        public int Rating;
        public bool IsRead;
    }
}
