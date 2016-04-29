using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class pBundle
    {
        public string Name { get; set; }
        public string Des { get; set; }
        public string Upload { get; set; }

        public int parentalBundle { get; set; } //this will be created from parant bundle
        public List<BundleItem> Plasmids { get; set; }

    }

    public class BundleItem
    {
        public int plasmidId { get; set; }
        public string plasmidRole { get; set; }
    }
}