using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabunCsParser
{
    public struct Blog
    {
        public List<PostPreview> Posts;
        public string Name;
        public float Rating;
        public int Votes;
        public string Description;
    }
}
