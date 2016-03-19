using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class ORFObject
    {
        public string Name { get; set; }
        public int start { get; set; }
        public int end { get; set; }
        public int clockwise { get; set; }
    }
}