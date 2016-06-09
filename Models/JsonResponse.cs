using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPTabunClient.Models
{
    class JsonResponse
    {
        public string sText;
        public string sMsgTitle;
        public string sMsg;
        public bool bStateError;

        public JsonResponse()
        {
            sText = "";
            sMsgTitle = "";
            sMsg = "";
            bStateError = false;
        }
    }
}
