using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;


namespace ecloning.Models
{
    public class PlasmidFeature
    {
        private ecloningEntities db = new ecloningEntities();
        public string Sequence { get; set; }
        public int PlasmidId { get; set; }
        public bool result { get; set; }

        //to get group id for group used enzymes
        public List<int> GroupId { get; set; }


        public PlasmidFeature(int id, string seq, List<int> groupId)
        {
            //**************all the classes doesn't add 1 to indexes, therefore, must add 1 to all feature starts, ends and cuts***************//

            PlasmidId = id;
            Sequence = seq;
            GroupId = groupId;

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
                //!!!!!!!!!!NEED TO DEEL WITH CIRCULAIR END!!!!!!!REMOVE THIS IF DONE!!!!!!!!!!
                //=======================================================
                //find common features
                //except restriction cut and ORF
                foreach (var item in features.ToList())
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
                        //start and end need to add 1, since seq starts from 1;
                        foreach (int index in indexes1)
                        {
                            var feature = new plasmid_map();
                            feature.plasmid_id = PlasmidId;
                            feature.show_feature = 1;
                            feature.feature = item.label;
                            feature.feature_id = item.feature_id;
                            feature.start = index +1;
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
                        //start and end need to add 1, since seq starts from 1;
                        foreach (int index in indexes2)
                        {
                            var feature = new plasmid_map();
                            feature.plasmid_id = PlasmidId;
                            feature.show_feature = 1;
                            feature.feature = item.label;
                            feature.feature_id = item.feature_id;
                            feature.start = index + 1;
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

            //first check the group common restriction enzymes
            //if no common restriction, then look all the restriction enzymes

            //first the common the enzymes
            List<int> enzymeId = new List<int>();
            var common_restriction = db.common_restriction.Where(g => GroupId.Contains(g.group_id));
            if (common_restriction.Count() > 0)
            {
                enzymeId = common_restriction.OrderBy(e => e.enzyme_id).Select(e => e.enzyme_id).Distinct().ToList();
            }
            else
            {
                var restrictions = db.restri_enzyme;
                if (restrictions.Count() > 0)
                {
                    enzymeId = restrictions.OrderBy(e => e.id).Select(e => e.id).Distinct().ToList();
                }
            }

            //generate enzyme restriction features

            var restriciton = new FindRestriction();
            var restricitonObjects = restriciton.RestricitonObject(Sequence, enzymeId); //find all the restrictions cutNum default is 0 == all.

            //remove all the old methylation info
            var methy = db.methylations.Where(p => p.plasmid_id == PlasmidId);
            if (methy.Count() > 0)
            {
                foreach (var m in methy)
                {
                    db.methylations.Remove(m);
                }
            }
            foreach( var rObject in restricitonObjects)
            {
                var map = new plasmid_map();
                var methylation = new methylation();

                //add tp map table
                map.plasmid_id = PlasmidId;
                map.show_feature = 1;
                map.feature = rObject.name;
                map.feature_id = 4;
                map.start = rObject.start + 1;
                map.end = rObject.end + 1;
                map.cut = rObject.cut +1;
                map.clockwise = rObject.clockwise;
                db.plasmid_map.Add(map);


                //add to methylation table
                methylation.plasmid_id = PlasmidId;
                methylation.cut = rObject.cut +1;
                methylation.clockwise = rObject.clockwise;
                methylation.name = rObject.name;
                methylation.dam_complete = rObject.dam_complete;
                methylation.dam_impaired = rObject.dam_impaired;
                methylation.dcm_complete = rObject.dcm_complete;
                methylation.dcm_impaired = rObject.dcm_impaired;
                if(methylation.dam_complete || methylation.dam_impaired || methylation.dcm_complete || methylation.dcm_impaired)
                {
                    db.methylations.Add(methylation);
                }
            }


            //========================================================================
            //========================================================================
            ////check ORF
            //=======================================================
            //!!!!!!!!!!NEED TO DEEL WITH CIRCULAIR END!!!!!!! REMOVE THIS IF DONE!!!!!!!!!!
            //=======================================================
            var orf = new List<ORFObject>();
            var orfFinder = new ORFFinder(0, 0, 0, 0, 300, Sequence);
            //ORFFinder(int startCodon, int stopCodon, int frame, int direction, int minSzie, string sequence)
            orf = orfFinder.FindPlasmidORF();
            if (orf.Count() > 0)
            {
                //save to plasmid_map table
                //start and end need to add 1, since seq starts from 1;
                foreach (var orfItem in orf)
                {
                    var feature = new plasmid_map();
                    feature.plasmid_id = PlasmidId;
                    feature.show_feature = 1;
                    feature.feature = orfItem.Name;
                    feature.feature_id = 10;
                    feature.start = orfItem.start+1;
                    feature.end = orfItem.end+1;
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