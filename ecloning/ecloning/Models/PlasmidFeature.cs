﻿using System;
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

                //backup current methylation

            }

            //remove current mehtylation info
            var previousMethylation = db.methylation_backup.Where(p => p.plasmid_id == PlasmidId);
            if (previousMethylation.Count() > 0)
            {
                foreach (var m in previousMethylation)
                {
                    db.methylation_backup.Remove(m);
                }

                //remove current methylation
                var currentMethylation = db.methylations.Where(p => p.plasmid_id == PlasmidId);
                foreach(var c in currentMethylation)
                {
                    var b = new methylation_backup();
                    b.plasmid_id = c.plasmid_id;
                    b.cut = c.cut;
                    b.clockwise = c.clockwise;
                    b.name = c.name;
                    b.dam_complete = c.dam_complete;
                    b.dam_impaired = c.dam_impaired;
                    b.dcm_complete = c.dcm_complete;
                    b.dcm_impaired = c.dcm_impaired;
                    db.methylation_backup.Add(b);
                    db.methylations.Remove(c);
                }
            }



            //========================================================================
            //========================================================================
            //===========================find common features (not primers)===========
            //find all features except primers
            //put all features in list except ORF/ restriction cut
            var features = db.common_feature.Where(f => f.plasmid_feature.feature != "orf" || f.plasmid_feature.feature != "enzyme");
            if(features.Count() == 0)
            {
                //result = false;
            }
            else
            {
                //find common features
                //except restriction cut and ORF
                foreach (var item in features.ToList())
                {
                    //find all the indexes of features in both forward and reserver seq
                    //feature sequence
                    var subSeq = item.sequence;

                    //forward
                    List<int> indexesF = new List<int>();
                    indexesF = FindSeq.NotRestriction(Sequence, subSeq);
                    if (indexesF.Count() > 0)
                    {
                        //add to plasmd_feature
                        //start and end need to add 1, since seq starts from 1;
                        foreach (int index in indexesF)
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

                    //deal with the right end
                    var endSeq = Sequence.Substring(Sequence.Length - subSeq.Length + 1, subSeq.Length - 1) + Sequence.Substring(0, subSeq.Length - 1);  // 2 * (subSeq.Length -1)
                    var endIdx = endSeq.IndexOf(subSeq);
                    if (endIdx != -1)
                    {
                        //find an extra feature
                        var f = new plasmid_map();
                        f.plasmid_id = PlasmidId;
                        f.show_feature = 1;
                        f.feature = item.label;
                        f.feature_id = item.feature_id;
                        f.start = Sequence.Length - subSeq.Length + endIdx + 1; //1 + 1
                        f.end = endIdx + 1;
                        f.common_id = item.id;
                        f.clockwise = 1;
                        db.plasmid_map.Add(f);
                        result = true;
                    }


                    //reverse the subseq (feature seq)                  
                    var reversesubSeq = FindSeq.ReverseSeq(item.sequence);
                    //get completment DNA of plasmid seq, but not reverse
                    var cSequence = FindSeq.cDNA(Sequence);
                    List<int> indexesR = new List<int>();
                    indexesR = FindSeq.NotRestriction(cSequence, reversesubSeq);
                    if (indexesR.Count() > 0)
                    {
                        //add to plasmd_feature
                        //start and end need to add 1, since seq starts from 1;
                        foreach (int index in indexesR)
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

                    //deal with the left end
                    var endcSeq = cSequence.Substring(cSequence.Length - reversesubSeq.Length + 1, subSeq.Length - 1) + cSequence.Substring(0, reversesubSeq.Length - 1);  // 2 * (subSeq.Length -1)
                    var endRIdx = endcSeq.IndexOf(reversesubSeq);
                    if (endRIdx != -1)
                    {
                        //find an extra feature
                        var rf = new plasmid_map();
                        rf.plasmid_id = PlasmidId;
                        rf.show_feature = 1;
                        rf.feature = item.label;
                        rf.feature_id = item.feature_id;
                        rf.start = Sequence.Length - subSeq.Length + endRIdx + 1; 
                        rf.end = endRIdx + 1;
                        rf.common_id = item.id;
                        rf.clockwise = 0;
                        db.plasmid_map.Add(rf);
                        result = true;
                    }
                }

            }


            //========================================================================
            //========================================================================
            //===========================find primers ================================
            var primers = db.primers;
            if (primers.Count() == 0)
            {
                //result = false;
            }
            else
            {
                //find common features
                //except restriction cut and ORF
                foreach (var item in primers.ToList())
                {
                    //find all the indexes of features in both forward and reserver seq
                    //feature sequence
                    var subSeq = item.sequence;

                    //forward
                    List<int> indexesF = new List<int>();
                    indexesF = FindSeq.NotRestriction(Sequence, subSeq);
                    if (indexesF.Count() > 0)
                    {
                        //add to plasmd_feature
                        //start and end need to add 1, since seq starts from 1;
                        foreach (int index in indexesF)
                        {
                            var feature = new plasmid_map();
                            feature.plasmid_id = PlasmidId;
                            feature.show_feature = 1;
                            feature.feature = item.name;
                            feature.feature_id = 3;
                            feature.start = index + 1;
                            feature.end = index + subSeq.Length;
                            //feature.common_id = item.id;
                            feature.clockwise = 1;
                            db.plasmid_map.Add(feature);
                        }
                        result = true;
                    }

                    //deal with the right end
                    var endSeq = Sequence.Substring(Sequence.Length - subSeq.Length + 1, subSeq.Length - 1) + Sequence.Substring(0, subSeq.Length - 1);  // 2 * (subSeq.Length -1)
                    var endIdx = endSeq.IndexOf(subSeq);
                    if (endIdx != -1)
                    {
                        //find an extra feature
                        var f = new plasmid_map();
                        f.plasmid_id = PlasmidId;
                        f.show_feature = 1;
                        f.feature = item.name;
                        f.feature_id = 3;
                        f.start = Sequence.Length - subSeq.Length + endIdx + 1; //1 + 1
                        f.end = endIdx + 1;
                        //f.common_id = item.id;
                        f.clockwise = 1;
                        db.plasmid_map.Add(f);
                        result = true;
                    }


                    //reverse the subseq (feature seq)                  
                    var reversesubSeq = FindSeq.ReverseSeq(item.sequence);
                    //get completment DNA of plasmid seq, but not reverse
                    var cSequence = FindSeq.cDNA(Sequence);
                    List<int> indexesR = new List<int>();
                    indexesR = FindSeq.NotRestriction(cSequence, reversesubSeq);
                    if (indexesR.Count() > 0)
                    {
                        //add to plasmd_feature
                        //start and end need to add 1, since seq starts from 1;
                        foreach (int index in indexesR)
                        {
                            var feature = new plasmid_map();
                            feature.plasmid_id = PlasmidId;
                            feature.show_feature = 1;
                            feature.feature = item.name;
                            feature.feature_id = 3;
                            feature.start = index + 1;
                            feature.end = index + subSeq.Length;
                            //feature.common_id = item.id;
                            feature.clockwise = 0;
                            db.plasmid_map.Add(feature);
                        }
                        result = true;
                    }

                    //deal with the left end
                    var endcSeq = cSequence.Substring(cSequence.Length - reversesubSeq.Length + 1, subSeq.Length - 1) + cSequence.Substring(0, reversesubSeq.Length - 1);  // 2 * (subSeq.Length -1)
                    var endRIdx = endcSeq.IndexOf(reversesubSeq);
                    if (endRIdx != -1)
                    {
                        //find an extra feature
                        var rf = new plasmid_map();
                        rf.plasmid_id = PlasmidId;
                        rf.show_feature = 1;
                        rf.feature = item.name;
                        rf.feature_id = 3;
                        rf.start = Sequence.Length - subSeq.Length + endRIdx + 1;
                        rf.end = endRIdx + 1;
                        //rf.common_id = item.id;
                        rf.clockwise = 0;
                        db.plasmid_map.Add(rf);
                        result = true;
                    }
                }

            }



            //========================================================================
            //========================================================================
            //=====================================================================
            //check restriciton cut

            //first check the group common restriction enzymes
            //if no common restriction, then look all the restriction enzymes

            //first the common the enzymes
            //if no common enzyme found, check all the enzymes available
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

            if (enzymeId.Count() > 0)
            {
                //check whether enzymeId.count >0 
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

                //add results to plasmid map table
                foreach( var rObject in restricitonObjects)
                {
                    var map = new plasmid_map();
                    var methylation = new methylation();

                    //add to map table
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
                    result = true;
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