using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPTabunClient.Models
{
    class JsonResponseComment : JsonResponse
    {
        public int iMaxIdComment;
        public List<Dictionary<string, string>> aComments;
    }
}
