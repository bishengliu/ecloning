using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class BackupMap
    {
        private ecloningEntities db = new ecloningEntities();
        public int PlasmidId { get; set; }
        public bool result { get; set; }

        public BackupMap(int id)
        {
            PlasmidId = id;

            var currentPlasmidMap = db.plasmid_map.Where(p => p.plasmid_id == PlasmidId);
            if (currentPlasmidMap.Count() > 0)
            {
                //remove all the previous backuped features
                var previousBackup = db.plasmid_map_backup.Where(p => p.plasmid_id == PlasmidId);
                if (previousBackup.Count() > 0)
                {
                    foreach (var b in previousBackup.ToList())
                    {
                        db.plasmid_map_backup.Remove(b);
                    }
                }

                //backup current features
                foreach (var c in currentPlasmidMap.ToList())
                {
                    var b = new plasmid_map_backup();
                    b.plasmid_id = PlasmidId;
                    b.show_feature = c.show_feature;
                    b.feature = c.feature;
                    b.feature_id = c.feature_id;
                    b.start = c.start;
                    b.end = c.end;
                    b.cut = c.cut;
                    b.common_id = c.common_id;
                    b.clockwise = c.clockwise;
                    b.des = c.des;
                    db.plasmid_map_backup.Add(b);
                }
                result = true;
            }
            if (result == true)
            {
                db.SaveChanges();
            }
        }
    }
}