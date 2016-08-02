using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class lnbrConvert
    {
        public static string ln2br(string text)
        {
            string result = "";
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }
            result = text.Replace("\r\n", "<br />");
            return result;
        }

        public static string br2ln(string text)
        {
            string result = "";
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }
            result = text.Replace("<br />", "\r\n");
            return result;
        }

    }
}