using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPTabunClient.Managers
{
    class GlobalVariables
    {
#if (LOCALMIRROR)
        public static string domain = "192.168.1.106";
#else
        public static string domain = "tabun.everypony.ru";
#endif
        public static string protocol = "https://";

        public static string linkRoot = protocol + domain + "/";
        public static string linkBlog = protocol + domain + "/blog/";
        public static string linkMainpage = protocol + domain + "/index/";
        public static string linkProfile = protocol + domain + "/profile/";

        public static string linkAjaxStreamComments = protocol + domain + "/ajax/stream/comment/";
        public static string linkAjaxStreamTopics = protocol + domain + "/ajax/stream/topic/";
        public static string linkAjaxAddComment = protocol + domain + "/blog/ajaxaddcomment/";
        public static string linkAjaxResponseComment = protocol + domain + "/blog/ajaxresponsecomment/";
        public static string linkAjaxLogin = protocol + domain + "/login/ajax-login?login=";

        public static string linkLogin = protocol + domain + "/login/";
    }
}
