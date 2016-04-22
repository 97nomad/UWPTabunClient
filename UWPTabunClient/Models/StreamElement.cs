using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPTabunClient.Models
{
    class StreamElement
    {
        public string author;
        public string blog;
        public string topic;
        public string link;
        public int comments_count;

        public StreamElement()
        {
            author = "";
            blog = "";
            topic = "";
            link = "";
            comments_count = 0;
        }
    }
}
