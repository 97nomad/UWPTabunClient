using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabunCsParser
{
    public class Profile
    {
        public Uri Avatar100x100;
        public Uri ProfilePhoto;

        public string Nickname;
        public string Name;
        public float Force;
        public float Rating;
        public int Votes;

        public string About;

        public string Sex;
        public string DateOfBirdth;
        public string Place;

        public string DateOfRegistration;
        public string DateOfLastVisite;

        public Dictionary<string, Uri> Friends;
        public Dictionary<Uri, string> Blogs;
    }
}
