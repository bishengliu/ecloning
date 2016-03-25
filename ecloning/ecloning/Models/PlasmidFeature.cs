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
                    var b  = new plasmid_map_backup();
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
                    db.plasmid_map.Remove(c);
                }
            }
            

            //put all features in list except ORF/ restriction cut
            var features = db.common_feature.Where(f => f.plasmid_feature.feature != "orf" || f.plasmid_feature.feature != "enzyme");
            if(features.Count() == 0)
            {
                result = false;
            }
            else
            {

                //=======================================================
                //find common features
                //except restriction cut and ORF
                foreach(var item in features.ToList())
                {
                    //find all the indexes of features in both forward and reserver seq
                    //feature sequence
                    var subSeq = item.sequence;

                    //forward
                    List<int> indexes1 = new List<int>();
                    indexes1 = FindSeq.NotRestriction(Sequence, subSeq);
                    if (indexes1.Count() > 0)
                    {
                        //add to plasmd_feature
                        foreach(int index in indexes1)
                        {
                            var feature = new plasmid_map();
                            feature.plasmid_id = PlasmidId;
                            feature.show_feature = 1;
                            feature.feature = item.label;
                            feature.feature_id = item.feature_id;
                            feature.start = index;
                            feature.end = index + subSeq.Length;
                            feature.common_id = item.id;
                            feature.clockwise = 1;
                            db.plasmid_map.Add(feature);
                        }
                        result = true;
                    }


                    //reverse                    
                    var reversesubSeq = FindSeq.ReverseSeq(item.sequence);

                    //get completment DNA, but not reverse
                    var cSequence = FindSeq.cDNA(Sequence);
                    List<int> indexes2 = new List<int>();
                    indexes2 = FindSeq.NotRestriction(cSequence, reversesubSeq);
                    if (indexes2.Count() > 0)
                    {
                        //add to plasmd_feature
                        foreach (int index in indexes2)
                        {
                            var feature = new plasmid_map();
                            feature.plasmid_id = PlasmidId;
                            feature.show_feature = 1;
                            feature.feature = item.label;
                            feature.feature_id = item.feature_id;
                            feature.start = index;
                            feature.end = index + subSeq.Length;
                            feature.common_id = item.id;
                            feature.clockwise = 0;
                            db.plasmid_map.Add(feature);
                        }
                        result = true;
                    }
                }

            }

            //=====================================================================
            //check restriciton cut
            var restrictions = db.restri_enzyme;
            if (restrictions.Count() > 0)
            {
                //find all the indexes of features in both forward and reserver seq
                //forward

                //reverse must be checked for cut blockages


                //if indexes founded
                //result = true;
                //add table
                //don't add cut blockage
            }
            //========================================================================
            //========================================================================
            ////check ORF

            var orf = new List<ORFObject>();
            var orfFinder = new ORFFinder(0, 0, 0, 0, 300, Sequence);
            //ORFFinder(int startCodon, int stopCodon, int frame, int direction, int minSzie, string sequence)
            orf = orfFinder.FindPlasmidORF();
            if (orf.Count() > 0)
            {
                //save to plasmid_map table
                foreach (var orfItem in orf)
                {
                    var feature = new plasmid_map();
                    feature.plasmid_id = PlasmidId;
                    feature.show_feature = 1;
                    feature.feature = orfItem.Name;
                    feature.feature_id = 10;
                    feature.start = orfItem.start;
                    feature.end = orfItem.end;
                    feature.clockwise = orfItem.clockwise;
                    feature.common_id = null;
                    db.plasmid_map.Add(feature);
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