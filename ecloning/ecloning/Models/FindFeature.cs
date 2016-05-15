using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class FindFeature
    {
        private ecloningEntities db = new ecloningEntities();
        //this class find one feature in all plasmid, except the plasmid provided
        public void Find(dynamic feature, List<int>PeopleId, int plasmid_id, string type)
        {
            //find all plasmids
            var plasmidIds = db.plasmids.Where(p=>PeopleId.Contains(p.people_id)).Where(p => p.id != plasmid_id).Select(i => i.id);
            if (plasmidIds.Count() > 0)
            {
                bool result = false;
                foreach(int id in plasmidIds)
                {
                    var plasmid = db.plasmids.Find(id);
                    var pSeq = plasmid.sequence;
                    if(pSeq != null)
                    {
                        if (type != "primer")
                        {
                            feature = (common_feature)feature;
                        }
                        else
                        {
                            feature = (primer)feature;
                        }
                        var subSeq = feature.sequence;

                        //forward
                        List<int> indexesF = new List<int>();
                        indexesF = FindSeq.NotRestriction(pSeq, subSeq);
                        if (indexesF.Count() > 0)
                        {
                            //add to plasmd_feature
                            //start and end need to add 1, since seq starts from 1;
                            foreach (int index in indexesF)
                            {
                                var pfeature = new plasmid_map();
                                pfeature.plasmid_id = id;
                                pfeature.show_feature = 1;
                                if (type != "primer")
                                {
                                    pfeature.feature = feature.label;
                                    pfeature.feature_id = feature.feature_id;
                                }
                                else
                                {
                                    pfeature.feature = feature.name;
                                    pfeature.feature_id = 3;
                                }
                                pfeature.start = index + 1;
                                pfeature.end = index + subSeq.Length;
                                if (type != "primer")
                                {
                                    pfeature.common_id = feature.id;
                                }
                                pfeature.clockwise = 1;
                                db.plasmid_map.Add(pfeature);
                                result = true;
                            }
                        }

                        //deal with the right end
                        var endSeq = pSeq.Substring(pSeq.Length - subSeq.Length + 1, subSeq.Length - 1) + pSeq.Substring(0, subSeq.Length - 1);  // 2 * (subSeq.Length -1)
                        var endIdx = endSeq.IndexOf(subSeq);
                        if (endIdx != -1)
                        {
                            //find an extra feature
                            var f = new plasmid_map();
                            f.plasmid_id = id;
                            f.show_feature = 1;
                            if (type != "primer")
                            {
                                f.feature = feature.label;
                                f.feature_id = feature.feature_id;
                            }
                            else
                            {
                                f.feature = feature.name;
                                f.feature_id = 3;
                            }
                            f.start = pSeq.Length - subSeq.Length + endIdx + 1; //1 + 1
                            f.end = endIdx + 1;
                            if (type != "primer")
                            {
                                f.common_id = feature.id;
                            }
                            f.clockwise = 1;
                            db.plasmid_map.Add(f);
                            result = true;
                        }


                        //reverse the subseq (feature seq)                  
                        var reversesubSeq = FindSeq.ReverseSeq(subSeq);
                        //get completment DNA of plasmid seq, but not reverse
                        var cSequence = FindSeq.cDNA(pSeq);
                        List<int> indexesR = new List<int>();
                        indexesR = FindSeq.NotRestriction(cSequence, reversesubSeq);
                        if (indexesR.Count() > 0)
                        {
                            //add to plasmd_feature
                            //start and end need to add 1, since seq starts from 1;
                            foreach (int index in indexesR)
                            {
                                var pfeature = new plasmid_map();
                                pfeature.plasmid_id = id;
                                pfeature.show_feature = 1;
                                if (type != "primer")
                                {
                                    pfeature.feature = feature.label;
                                    pfeature.feature_id = feature.feature_id;
                                }
                                else
                                {
                                    pfeature.feature = feature.name;
                                    pfeature.feature_id = 3;
                                }
                                pfeature.start = index + 1;
                                pfeature.end = index + subSeq.Length;
                                if (type != "primer")
                                {
                                    pfeature.common_id = feature.id;
                                }
                                pfeature.clockwise = 0;
                                db.plasmid_map.Add(pfeature);
                                result = true;
                            }
                        }

                        //deal with the left end
                        var endcSeq = cSequence.Substring(cSequence.Length - reversesubSeq.Length + 1, subSeq.Length - 1) + cSequence.Substring(0, reversesubSeq.Length - 1);  // 2 * (subSeq.Length -1)
                        var endRIdx = endcSeq.IndexOf(reversesubSeq);
                        if (endRIdx != -1)
                        {
                            //find an extra feature
                            var rf = new plasmid_map();
                            rf.plasmid_id = id;
                            rf.show_feature = 1;
                            if (type != "primer")
                            {
                                rf.feature = feature.label;
                                rf.feature_id = feature.feature_id;
                            }
                            else
                            {
                                rf.feature = feature.name;
                                rf.feature_id = 3;
                            }
                            rf.start = cSequence.Length - subSeq.Length + endRIdx + 1;
                            rf.end = endRIdx + 1;
                            if (type != "primer")
                            {
                                rf.common_id = feature.id;
                            }
                            rf.clockwise = 0;
                            db.plasmid_map.Add(rf);
                            result = true;
                        }
                    }                    
                }
                if (result)
                {
                    db.SaveChanges();
                }
            }
        }
    }
}