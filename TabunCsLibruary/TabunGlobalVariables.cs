using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabunCsLibruary
{
    public struct TabunGlobalVariables
    {
        public static string Domain = "tabun.everypony.ru";
        public static string Protocol = "https://";

        public static string LinkRoot = Protocol + Domain + "/";
        //public static string LinkRootWithoutSlash = Protocol + Domain;
        public static string LinkBlog = Protocol + Domain + "/blog/";
        public static string LinkMainpage = Protocol + Domain + "/index/";
        public static string LinkProfile = Protocol + Domain + "/profile/";

        public static string LinkAjaxStreamComments = Protocol + Domain + "/ajax/stream/comment/";
        public static string LinkAjaxStreamTopics = Protocol + Domain + "/ajax/stream/topic/";
        //public static string LinkAjaxAddComment = Protocol + Domain + "/blog/ajaxaddcomment/";
        //public static string LinkAjaxResponseComment = Protocol + Domain + "/blog/ajaxresponsecomment/";
        //public static string LinkAjaxLogin = Protocol + Domain + "/login/ajax-login?login=";

        public static string LinkLogin = Protocol + Domain + "/login/";
    }
}
