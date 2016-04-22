using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPTabunClient.Models
{
    class Post
    {
        public int id;
        public string title;
        public string author;
        public SoftwareBitmapSource author_image;
        public string blog;
        public string blog_id;
        public string rating;
        public Paragraph text;
        public string tags;
        public string datatime;

        public Post()
        {
            id = 1;
            title = "";
            author = "";
            author_image = new SoftwareBitmapSource();
            blog = "";
            blog_id = "";
            rating = "";
            text = new Paragraph();
            tags = "";
            datatime = "";

        }

        

    }

}
