using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class PlasmidInfo
    {
        private ecloningEntities db = new ecloningEntities();
        public string name { get; set; }
        public string sequence { get; set; }
        public int? seq_length { get; set; }

        //constructor
        public PlasmidInfo(int plasmid_id)
        {
            var plasmid = db.plasmids.Find(plasmid_id);
            this.name = plasmid.name;
            this.sequence = plasmid.sequence;
            this.seq_length = plasmid.seq_length;
        }

        //constructor
        public PlasmidInfo()
        {            
        }

        public string PlasmidName(int id)
        {
            var plasmid = db.plasmids.Find(id);
            return plasmid.name;
        }
    }

    public class pIdName
    {
        private ecloningEntities db = new ecloning.Models.ecloningEntities();
        public int id { get; set; }
        public string name { get; set; }

        public string getName(int? id)
        {
            return db.plasmids.Find(id).name;
        }
    }
}