using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPTabunClient.Models
{
    class Comment
    {
        public int id;
        public Paragraph text;
        public string author;
        public SoftwareBitmapSource author_image;
        public string datetime;
        public int rating;

        public Comment parentNode;
        public List<Comment> childNodes;

        public Windows.UI.Xaml.Thickness deep { get
            {
                return new Windows.UI.Xaml.Thickness(getDeep() * 10, 1, 1, 1);
            } }

        public string ratingColor { get
            {
                if (rating == 0)
                    return "Black";
                if (rating > 0)
                    return "Green";
                if (rating < 0)
                    return "Red";
                return "Black";
            } }

        private List<Comment> resultComments;

        public Comment()
        {
            id = 0;
            text = new Paragraph();
            author = "";
            author_image = new SoftwareBitmapSource();
            datetime = "";
            rating = 0;

            parentNode = null;
            childNodes = new List<Comment>();
        }

        public Comment findChildById(int id)
        {
            return childNodes.Where(x => x.id == id).First();
        }

        public List<Comment> Descendants()
        {
            resultComments = new List<Comment>();
            recursive(this);
            return resultComments;
        }

        private void recursive(Comment comm)
        {
            foreach (Comment c in comm.childNodes)
            {
                if (c != null && comm.childNodes.Count != 0)
                {
                    resultComments.Add(c);
                    recursive(c);
                }
            }
        }

        private int getDeep()
        {
            int i = 0;
            Comment c = parentNode;
            while (c != null)
            {
                i++;
                c = c.parentNode;
            }
            return i;
        }
    }

}
