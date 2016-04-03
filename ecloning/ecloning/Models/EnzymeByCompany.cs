using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class EnzymeByCompany
    {
        public int company_id { get; set; }
        public List<int> enzymeId { get; set; }
    }
}