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

    public class EnzymeIdName
    {
        private ecloningEntities db = new ecloningEntities();
        public int id { get; set; }
        public string Name { get; set; }

        public string getName(int id)
        {
            var enzyme = db.restri_enzyme.Find(id);
            return enzyme.name;
        }
    }
}