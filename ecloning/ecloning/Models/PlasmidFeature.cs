using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class PlasmidFeature
    {
        private ecloningEntities db = new ecloningEntities();
        public string Sequence { get; set; }
        public int PlasmidId { get; set; }
        public bool result { get; set; }

        public PlasmidFeature(int id, string seq)
        {
            PlasmidId = id;
            Sequence = seq;

            //remove exsited features to backup table

            //copy existed features into backup table

            //put all features in list except ORF
            var features = db.common_feature.Where(f => f.plasmid_feature.feature != "orf");
            if(features.Count() == 0)
            {
                result = false;
            }
            else
            {

                //find common features
                foreach(var item in features.ToList())
                {
                    //find all the indexes of features in both forward and reserver seq
                    //forward

                    //reverse

                    //if indexes founded
                    result = true;
                //add table
                }

            }


            //check restriciton cut
            var restrictions = db.restrictions;
            if (restrictions.Count() > 0)
            {
                //find all the indexes of features in both forward and reserver seq
                //forward

                //reverse must be checked for cut blockages


                //if indexes founded
                result = true;
                //add table
                //don't add cut blockage
            }

            if(result == true)
            {
                db.SaveChanges();
            }
        }
    }
}