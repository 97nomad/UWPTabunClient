using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.UI;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPTabunClient.Models
{
    class Profile
    {
        public SoftwareBitmapSource avatar_100x100;
        public SoftwareBitmapSource profile_photo;

        public string nickname;
        public string name;
        public string force;
        public string rating;
        public string votes;

        public Color ratingColor { get
            {
                if (float.Parse(rating, CultureInfo.InvariantCulture) < 0)
                    return Colors.Red;
                if (float.Parse(rating, CultureInfo.InvariantCulture) > 0)
                    return Colors.Green;
                return Colors.Black;
            } }

        //public string about;
        public Paragraph about;

        public string sex;
        public string dateOfBirdth;
        public string place;

        public string consist;
        public string dateOfRegistration;
        public string dateOfLastVisite;

        public List<Friend> friends;
        public List<string> contacts;
        public Paragraph blogsConsistIn;

        public Profile()
        {
            avatar_100x100 = new SoftwareBitmapSource();
            profile_photo = new SoftwareBitmapSource();

            nickname = "";
            name = "";
            force = "";
            rating = "";
            votes = "";

            about = new Paragraph();

            sex = "";
            dateOfBirdth = "";
            place = "";
            consist = "";
            dateOfLastVisite = "";
            dateOfRegistration = "";

            friends = new List<Friend>();
            contacts = new List<string>();
            blogsConsistIn = new Paragraph();
        }
    }
}
