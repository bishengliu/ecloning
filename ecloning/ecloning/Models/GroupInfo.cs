using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class GroupInfo
    {
        private ecloningEntities db = new ecloningEntities();

        public List<int> groupId = new List<int>();
        public Dictionary<int, string> groupIdName = new Dictionary<int, string>();

        public GroupInfo(int peopleId)
        {
            //find groupId
            var group_people = db.group_people.Where(p => p.people_id == peopleId);
            if (group_people.Count() > 0)
            {
                foreach (int i in group_people.Select(g => g.group_id).ToList())
                {
                    groupId.Add(i);
                }                
            }
            if (groupId.Count() > 0)
            {
                foreach(int gp in groupId)
                {
                    var groupName = db.groups.Find(gp).name;
                    groupIdName.Add(gp, groupName);
                }
            }

        }
    }
}