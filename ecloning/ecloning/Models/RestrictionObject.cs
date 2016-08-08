using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class RestrictionObject
    {
        public string name { get; set; }
        public int Id { get; set; } //enzyme id
        public string prototype { get; set; }
        public string startActivity { get; set; }
        public string heatInactivation { get; set; }
        public string dam { get; set; }
        public string dcm { get; set; }
        public string cpg { get; set; }
        public int forward_cut { get; set; }
        public int reverse_cut { get; set; }
        public int? forward_cut2 { get; set; }
        public int? reverse_cut2 { get; set; }
    }

    public static class REnzyme
    {
        public static string FormatName(string name)
        {
            var eName = "";
            if(name.IndexOf('(') != -1 && name.IndexOf(')') != -1)
            {
                //get indexof of the first '('
                var idx = name.IndexOf('(');
                var front = name.Substring(0, idx - 1);
                var end = name.Substring(idx);
                eName =front + "<br/><p class=\"smallFont text-center\">" +  end + "</p>";
            }
            else
            {
                eName =  name;
            }


            return eName;
        }
    }
}