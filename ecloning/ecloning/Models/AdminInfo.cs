using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{    
    public class AdminInfo
    {
        private ecloningEntities db = new ecloningEntities();
        public List<int> AdminId(List<string> adminNameList)
        {
            List<int> adminId = new List<int>();
            foreach(var n in adminNameList)
            {
                var admin = db.groups.Where(a => a.name == n);
                if (admin.Count() > 0)
                {
                    adminId.Add(admin.FirstOrDefault().id);
                }
            }
            return adminId;
        }
    }
}