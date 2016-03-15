using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class RestoreMap
    {
        private ecloningEntities db = new ecloningEntities();
        public int PlasmidId { get; set; }
        public bool result { get; set; }

        public RestoreMap(int id)
        {
            PlasmidId = id;

            var previousBackup = db.plasmid_map_backup.Where(p => p.plasmid_id == PlasmidId);            
            if (previousBackup.Count() > 0)
            {
                //find all current map features
                var currentPlasmidMap = db.plasmid_map.Where(p => p.plasmid_id == PlasmidId);
                //remove all current features
                if (currentPlasmidMap.Count() > 0)
                {
                    foreach (var c in currentPlasmidMap.ToList())
                    {
                        db.plasmid_map.Remove(c);
                    }
                }

                //restore backuped features
                foreach (var r in previousBackup.ToList())
                {
                    var b = new plasmid_map();
                    b.plasmid_id = PlasmidId;
                    b.show_feature = r.show_feature;
                    b.feature = r.feature;
                    b.feature_id = r.feature_id;
                    b.start = r.start;
                    b.end = r.end;
                    b.cut = r.cut;
                    b.common_id = r.common_id;
                    b.clockwise = r.clockwise;
                    b.des = r.des;
                    db.plasmid_map.Add(b);
                    db.plasmid_map_backup.Remove(r);
                }

                //copy to backup table
                if (currentPlasmidMap.Count() > 0)
                {
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