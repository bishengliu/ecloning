using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class FindRestriction
    {
        private ecloningEntities db = new ecloningEntities();


        //find the restriction enzyme feature
        public List<RestriFeatureObject> RestricitonObject(string fullSeq, List<int> enzymeId, int cutNum=0)
        {
            List<RestriFeatureObject> FObjects = new List<RestriFeatureObject>();

            foreach (var e in enzymeId)
            {
                List<RestriFeatureObject> Objects = new List<RestriFeatureObject>();
                var enzyme = db.restri_enzyme.Find(e);
                if(enzyme != null)
                {
                    //find forward
                    var restriSeq = enzyme.forward_seq;
                    var decodes = new DecodeRestrictionSeq();
                    if (enzyme.forward_cut2 == null || enzyme.reverse_cut2 == null)
                    {                    
                        //deel the letter codes in the restriction sites, generate the possible restriction sites, decode also N
                        var forwardSeqList = decodes.Decode(restriSeq);
                        foreach(var rs in forwardSeqList)
                        {
                            List<RestriFeatureObject> FRrObjects = new List<RestriFeatureObject>();
                            var findObjects = new FindRestriction();
                            FRrObjects = findObjects.FRrObject(cutNum, rs, fullSeq, enzyme, true);
                            if (FRrObjects.Count() > 0)
                            {
                                Objects = Objects.Concat(FRrObjects).ToList();
                            }                          
                        }
                    }
                    else
                    {
                        //more than one cuts each time
                        //deel the letter codes in the restriction sites, generate the possible restriction sites, don't decode "N"
                        var forwardSeqList = decodes.DecodeNonN(restriSeq);
                        foreach (var rs in forwardSeqList)
                        {
                            List<RestriFeatureObject> FRr2Objects = new List<RestriFeatureObject>();

                            //mutiple cut following these rules
                            //ATCGNNNNNNATGC
                            var findObjects = new FindRestriction();
                            //don't look for dam and dcm
                            //generate 2 object for each index
                            FRr2Objects = FRr2Object(cutNum, rs, fullSeq, enzyme, true);
                            if (FRr2Objects.Count() > 0)
                            {
                                Objects = Objects.Concat(FRr2Objects).ToList();
                            }
                        }
                    }
                }
                Objects = Objects.GroupBy(c => c.cut).Select(f => f.First()).ToList();
                if (cutNum != 0 && Objects.Count() <= cutNum)
                {
                    FObjects = FObjects.Concat(Objects).ToList();
                }
                if(cutNum == 0)
                {
                    FObjects = FObjects.Concat(Objects).ToList();
                }
            }
            return FObjects;
        }

        //====================================deel with one cut each enzyme=======================================================//
        //this funciton returen forward and completement restrictionObject
        public List<RestriFeatureObject> FRrObject(int cutNum, string rs, string fullSeq, ecloning.Models.restri_enzyme enzyme, bool isCircular)
        {
            List<RestriFeatureObject> FRrObjects = new List<RestriFeatureObject>();
            List<RestriFeatureObject> FrObjects = new List<RestriFeatureObject>();
            List<RestriFeatureObject> RrObjects = new List<RestriFeatureObject>();


            //find forward
            var findObject = new FindRestriction();
            FrObjects = findObject.FrObject(rs, fullSeq, enzyme, isCircular);
            if (cutNum == 0 || (cutNum != 0 && FrObjects.Count() < cutNum))
            {
                //find the completment
                var crs = FindSeq.cDNA(rs);
                //find reverse
                var rcrs = FindSeq.ReverseSeq(crs);

                if (FrObjects.Count() > 0)
                {
                    if (object.Equals(rs, rcrs))
                    {
                        //find left side for dam or dcm
                        FrObjects = findObject.newFrObject(rcrs, fullSeq, enzyme, FrObjects, isCircular);
                    }
                }
                if (!object.Equals(rs, rcrs))
                {
                    RrObjects = findObject.FrObject(rcrs, fullSeq, enzyme, isCircular);
                }
            }
            FRrObjects = FrObjects.Concat(RrObjects).ToList();     
            return FRrObjects;
        }

        //single cut one enzyme
        //forward
        public List<RestriFeatureObject> FrObject(string rs, string fullSeq, ecloning.Models.restri_enzyme enzyme, bool isCircular)
        {
            List<RestriFeatureObject> rObjects = new List<RestriFeatureObject>();
            for (int index = 0; ; index += rs.Length)
            {
                //object
                var FObject = new RestriFeatureObject();
                index = fullSeq.IndexOf(rs, index);
                if (index == -1)
                {
                    break;
                    //don't generate the object
                }
                else
                {
                    FObject.clockwise = 1;
                    FObject.start = index;
                    FObject.end = rs.Length + index - 1; //switch to 0 index mode

                    if (enzyme.forward_cut <0 && index < Math.Abs(enzyme.forward_cut))
                    {
                        //enzyme cut is left to the restriciton site
                        var lenth1 = Math.Abs(enzyme.forward_cut) - index;
                        FObject.cut = fullSeq.Length - lenth1 - 1;                        
                    }
                    if (enzyme.forward_cut > enzyme.forward_seq.Length && (index + Math.Abs(enzyme.forward_cut) > fullSeq.Length))
                    {
                        //enzyme cut is to the right if the restriciotn site
                        var length1 = enzyme.forward_cut - (fullSeq.Length - index);
                        FObject.cut = FObject.start = length1 - 1;                       
                    }
                    else
                    {                        
                        FObject.cut = enzyme.forward_cut + index - 1; //switch to 0 index mode 
                    }
                    
                    //when the cut is outside the resriciton site                   
                    //dam and dcm is not possible

                    //check dam and dcm
                    var findRestrction = new FindRestriction();
                    //chack dam
                    var dam = findRestrction.CheckDam(rs, index, fullSeq, enzyme);
                    FObject.dam_complete = dam.Keys.FirstOrDefault();
                    FObject.dam_impaired = dam.Values.FirstOrDefault();

                    //check dcm 
                    var dcm = findRestrction.CheckDcm(rs, index, fullSeq, enzyme);

                    FObject.dcm_complete = dcm.Keys.FirstOrDefault();
                    FObject.dcm_impaired = dcm.Values.FirstOrDefault();

                    //set the name of the feature
                    var featureName = enzyme.name;
                    FObject.name = featureName;
                }

                rObjects.Add(FObject);
            }


            //deal with the end
            if (isCircular)
            {
                //if it is plasmid, default
                var findRestrction = new FindRestriction();
                var EndObject = findRestrction.FindEndRestriction(rs, fullSeq, enzyme);
                if(EndObject.name != null)
                {
                    rObjects.Add(EndObject);
                }
            }
            return rObjects;
        }


        public RestriFeatureObject FindEndRestriction(string rs, string fullSeq, ecloning.Models.restri_enzyme enzyme)
        {
            var EndObject = new RestriFeatureObject();

            //right end
            string rightEndSeq = fullSeq.Substring(fullSeq.Length - rs.Length) + fullSeq.Substring(0, rs.Length - 1);
            int index = rightEndSeq.IndexOf(rs);
            if (index != -1)
            {
                EndObject.clockwise = 1;
                EndObject.start = fullSeq.Length - index;

                EndObject.end = rs.Length + index - ( rs.Length - 1) -1; //switch to 0 index mode
                
                //cut after index
                if (enzyme.forward_cut >= 1 && enzyme.forward_cut + index < rs.Length)
                {
                    EndObject.cut = fullSeq.Length - 1 - rs.Length - 1  + index + Math.Abs(enzyme.forward_cut);
                }
                else if (enzyme.forward_cut < 0)
                {
                    EndObject.cut = fullSeq.Length -1 - rs.Length -1 - Math.Abs(enzyme.forward_cut) + index; //switch to 0 index mode
                }
                else
                {
                    EndObject.cut = index + enzyme.forward_cut -(rs.Length - 1) - 1;
                }

                //check dam and dcm
                var findRestrction = new FindRestriction();
                //chack dam
                var dam = findRestrction.CheckDam(rs, index, fullSeq, enzyme);
                EndObject.dam_complete = dam.Keys.FirstOrDefault();
                EndObject.dam_impaired = dam.Values.FirstOrDefault();

                //check dcm 
                var dcm = findRestrction.CheckDcm(rs, index, fullSeq, enzyme);

                EndObject.dcm_complete = dcm.Keys.FirstOrDefault();
                EndObject.dcm_impaired = dcm.Values.FirstOrDefault();

                //set the name of the feature
                var featureName = enzyme.name;
                //if ((EndObject.dam_complete || EndObject.dam_impaired) && (EndObject.dcm_complete || EndObject.dcm_impaired))
                //{
                //    featureName = featureName + " (affected by Dam/Dcm Methylation)";
                //}
                //if ((EndObject.dam_complete || EndObject.dam_impaired) && (EndObject.dcm_complete == false && EndObject.dcm_impaired == false))
                //{
                //    featureName = featureName + " (affected by Dam Methylation)";
                //}
                //if ((EndObject.dcm_complete || EndObject.dcm_impaired) && (EndObject.dam_complete == false && EndObject.dam_impaired == false))
                //{
                //    featureName = featureName + " (affected by Dcm Methylation)";
                //}
                EndObject.name = featureName;
            }           
            return EndObject;
        }


        public Dictionary<bool, bool> CheckDam(string rs, int index, string fullSeq, ecloning.Models.restri_enzyme enzyme)
        {
            var dict = new Dictionary<bool, bool>();

            //dam : GATC
            //completely overlapping and completely blocked
            var CBDam = db.Dams.Where(a => a.COverlapping == true && a.CBlocked == true).Select(n => n.name).ToList();
            //completely overlapping and partially impaired
            var CIDam = db.Dams.Where(a => a.COverlapping == true && a.CBlocked == false).Select(n => n.name).ToList();
            //partiallyly overlapping and completely blocked
            var PBDam = db.Dams.Where(a => a.COverlapping == false && a.CBlocked == true).Select(n => n.name).ToList();
            //partiallyly overlapping and partially impaired
            var PIDam = db.Dams.Where(a => a.COverlapping == false && a.CBlocked == true).Select(n => n.name).ToList();


            //completely overlapping
            if (CBDam.Count() > 0 && CBDam.Contains(enzyme.name))
            {
                //completely overlapping and completely blocked
                dict.Add(true, false);
            }
            else if (CIDam.Count() > 0 && CIDam.Contains(enzyme.name))
            {
                //completely overlapping and partially impaired
                dict.Add(false, true);
            }

            //partially overlapping 
            //need to check the seq GATC
            else if (PBDam.Count() > 0 && PBDam.Contains(enzyme.name))
            {
                //completely blocked
                //find the appending letters
                var damEnzyme = db.Dams.Where(n => n.name == enzyme.name).FirstOrDefault();
                //appending  letters cannot be both empty, can only be "T" or "TC"
                if (damEnzyme.appending != null)
                {
                    string letters = null;
                    if (damEnzyme.appending == "C")
                    {
                        letters = "GAT";
                    }
                    if (damEnzyme.appending == "TC")
                    {
                        letters = "GA";
                    }

                    if (string.IsNullOrWhiteSpace(letters))
                    {
                        //if letters are empty, assume it is not blocked
                        dict.Add(false, false);
                    }
                    else
                    {

                        //find the index of the letters in rs
                        int letterIndex = rs.IndexOf(letters);
                        if (letterIndex != -1)
                        {
                            if (letters == "GAT")
                            {
                                if (index + letterIndex + 3 > fullSeq.Length)
                                {
                                    //out of range
                                    string newFullSeq = fullSeq + fullSeq.Substring(0, 20);
                                    if (newFullSeq[index + letterIndex + 3].ToString() == "C".ToString())
                                    {
                                        dict.Add(true, false);
                                    }
                                }
                                else
                                {
                                    if (fullSeq[index + letterIndex + 3].ToString() == "C".ToString())
                                    {
                                        dict.Add(true, false);
                                    }
                                }
                            }

                            if (letters == "GA")
                            {
                                if (index + letterIndex + 3 > fullSeq.Length)
                                {
                                    //out of range
                                    string newFullSeq = fullSeq + fullSeq.Substring(0, 20);
                                    if ((newFullSeq[index + letterIndex + 2].ToString() == "T".ToString()) && (newFullSeq[index + letterIndex + 3].ToString() == "C".ToString()))
                                    {
                                        dict.Add(true, false);
                                    }
                                }
                                else
                                {
                                    if ((fullSeq[index + letterIndex + 2].ToString() == "T".ToString()) && (fullSeq[index + letterIndex + 3].ToString() == "C".ToString()))
                                    {
                                        dict.Add(true, false);
                                    }
                                }

                            }
                        }
                        else
                        {
                            dict.Add(false, false);
                        }

                    }
                }
                else
                {
                    //letters are not found on rs, assume it is not blocked
                    dict.Add(false, false);
                }
            }
            else if (PIDam.Count() > 0 && PIDam.Contains(enzyme.name))
            {
                //partially impaired
                //find the appending letters
                var damEnzyme = db.Dams.Where(n => n.name == enzyme.name).FirstOrDefault();
                if (damEnzyme.appending != null)
                {
                    string letters = null;
                    if (damEnzyme.appending == "C")
                    {
                        letters = "GAT";
                    }
                    if (damEnzyme.appending == "TC")
                    {
                        letters = "GA";
                    }

                    if (string.IsNullOrWhiteSpace(letters))
                    {
                        //if letters are empty, assume it is not blocked
                        dict.Add(false, false);
                    }
                    else
                    {
                        //find the index of the letters in rs
                        int letterIndex = rs.IndexOf(letters);
                        if (letterIndex != -1)
                        {

                            if (letters == "GAT")
                            {
                                if (index + letterIndex + 3 > fullSeq.Length)
                                {
                                    //out of range
                                    string newFullSeq = fullSeq + fullSeq.Substring(0, 20);
                                    if (newFullSeq[index + letterIndex + 3].ToString() == "C".ToString())
                                    {
                                        dict.Add(false, true);
                                    }
                                }
                                else
                                {
                                    if (fullSeq[index + letterIndex + 3].ToString() == "C".ToString())
                                    {
                                        dict.Add(false, true);
                                    }
                                }
                            }
                            if (letters == "GA")
                            {
                                if (index + letterIndex + 3 > fullSeq.Length)
                                {
                                    //out of range
                                    string newFullSeq = fullSeq + fullSeq.Substring(0, 20);
                                    if ((newFullSeq[index + letterIndex + 2].ToString() == "T".ToString()) && (newFullSeq[index + letterIndex + 3].ToString() == "C".ToString()))
                                    {
                                        dict.Add(false, true);
                                    }
                                }
                                else
                                {
                                    if ((fullSeq[index + letterIndex + 2].ToString() == "T".ToString()) && (fullSeq[index + letterIndex + 3].ToString() == "C".ToString()))
                                    {
                                        dict.Add(false, true);
                                    }
                                }

                            }
                        }
                        else
                        {
                            //letters are not found on rs, assume it is not blocked
                            dict.Add(false, false);
                        }
                    }
                }
                else
                {
                    //if the appending letter is empty, assume it is not blocked
                    dict.Add(false, false);
                }

            }
            else
            {
                //enzyme not in the dam list, set both to false
                dict.Add(false, false);
            }

            return dict;
        }

        public Dictionary<bool, bool> CheckDcm(string rs, int index, string fullSeq, ecloning.Models.restri_enzyme enzyme)
        {
            var dict = new Dictionary<bool, bool>();
            //complately blocked, impaired
            //dcm
            //completely overlapping and completely blocked
            var CBDcm = db.Dcms.Where(a => a.COverlapping == true && a.CBlocked == true).Select(n => n.name).ToList();
            //completely overlapping and partially impaired
            var CIDcm = db.Dcms.Where(a => a.COverlapping == true && a.CBlocked == false).Select(n => n.name).ToList();
            //partiallyly overlapping and completely blocked
            var PBDcm = db.Dcms.Where(a => a.COverlapping == false && a.CBlocked == true).Select(n => n.name).ToList();
            //partiallyly overlapping and partially impaired
            var PIDcm = db.Dcms.Where(a => a.COverlapping == false && a.CBlocked == true).Select(n => n.name).ToList();


            //**********************************************************************************************************check dcm**********************************************************************************************************//


            //completely overlapping
            if (CBDcm.Count() > 0 && CBDcm.Contains(enzyme.name))
            {
                //completely overlapping and completely blocked
                dict.Add(true, false);
            }
            else if (CIDcm.Count() > 0 && CIDcm.Contains(enzyme.name))
            {
                //completely overlapping and partially impaired
                dict.Add(false, true);
            }

            //partially overlapping 
            //need to check the seq CCAGG OR CCTGG
            else if (PBDcm.Count() > 0 && PBDcm.Contains(enzyme.name))
            {
                string letters1 = null;
                string letters2 = null;
                //completely blocked
                //find the appending letters
                var dcmEnzyme = db.Dcms.Where(n => n.name == enzyme.name).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(dcmEnzyme.prefixing) && string.IsNullOrWhiteSpace(dcmEnzyme.appending))
                {
                    //appending and prefxing letters are both empty, then it is internal dcm

                    if (rs.IndexOf("CCAGG") != -1 || rs.IndexOf("CCTGG") != -1)
                    {
                        dict.Add(true, false);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(dcmEnzyme.prefixing) && string.IsNullOrWhiteSpace(dcmEnzyme.appending))
                {
                    //prefixing letters are not empty but appeding letters are empty, 
                    int prefixCount = dcmEnzyme.prefixing.Length;
                    if (prefixCount < 5 && ("CCAGG".IndexOf(dcmEnzyme.prefixing) != -1 || "CCTGG".IndexOf(dcmEnzyme.prefixing) != -1))
                    {
                        letters1 = "CCAGG".Substring(0, prefixCount);
                        letters2 = "CCTGG".Substring(0, prefixCount);
                        //if cut in at the very beginning of the fullseq
                        if (index < prefixCount)
                        {                        
                            //need to take letter from the end of the fullSeq
                            string front = fullSeq.Substring(0, index); //take the letters before index, should be very short
                            string end = fullSeq.Substring(Math.Max(0, fullSeq.Length - (prefixCount - index))); //take the last few letters from fullseq
                            if (end + front == letters1 || end + front == letters2)
                            {
                                dict.Add(true, false);
                            }
                        }
                        else
                        {
                            if (fullSeq.Substring((index - prefixCount), prefixCount) == letters1 || fullSeq.Substring((index - prefixCount), prefixCount) == letters2)
                            {
                                dict.Add(true, false);
                            }
                        }

                    }
                }
                else if (string.IsNullOrWhiteSpace(dcmEnzyme.prefixing) && !string.IsNullOrWhiteSpace(dcmEnzyme.appending))
                {
                    //prefixing letters are empty but appeing letters are not empty
                    int appendCount = dcmEnzyme.appending.Length;
                    if (appendCount < 5 && ("CCAGG".IndexOf(dcmEnzyme.appending) != -1 || "CCTGG".IndexOf(dcmEnzyme.appending) != -1))
                    {
                        letters1 = "CCAGG".Substring((4 - appendCount), appendCount);
                        letters2 = "CCTGG".Substring((4 - appendCount), appendCount);
                        if (index + rs.Length + appendCount > fullSeq.Length)
                        {
                            string front = fullSeq.Substring(index + rs.Length);
                            string end = fullSeq.Substring(0, appendCount - (fullSeq.Length - (index + 1) - (rs.Length -1)));
                            if (front + end == letters1 || front + end == letters2)
                            {
                                dict.Add(true, false);
                            }
                        }
                        else
                        {
                            if (fullSeq.Substring((index + rs.Length), appendCount) == letters1 && fullSeq.Substring((index + rs.Length), appendCount) == letters2)
                            {
                                dict.Add(true, false);
                            }
                        }

                    }
                }
                else
                {
                    //appending and prefxing letters are both not empty                         
                    //not likely, too short
                    int prefixCount = dcmEnzyme.prefixing.Length;
                    int appendCount = dcmEnzyme.appending.Length;


                    if ((prefixCount + appendCount) < 5 && (("CCAGG".IndexOf(dcmEnzyme.appending) != -1 || "CCTGG".IndexOf(dcmEnzyme.appending) != -1)) && ("CCAGG".IndexOf(dcmEnzyme.prefixing) != -1 || "CCTGG".IndexOf(dcmEnzyme.prefixing) != -1))
                    {
                        string front = (index < prefixCount) ? (fullSeq.Substring(Math.Max(0, fullSeq.Length - (prefixCount - index))) + fullSeq.Substring(0, index)) : fullSeq.Substring(index - prefixCount, prefixCount);
                        string mid = fullSeq.Substring(index, rs.Length);
                        string end = (index + rs.Length + appendCount > fullSeq.Length) ? (fullSeq.Substring(index + rs.Length) + fullSeq.Substring(0, appendCount - (fullSeq.Length - index - rs.Length))) : fullSeq.Substring(index + appendCount, appendCount);

                        if (front + mid + end == "CCAGG" || front + mid + end == "CCTGG")
                        {
                            dict.Add(true, false);
                        }
                    }

                }

            }
            else if (PIDcm.Count() > 0 && PIDcm.Contains(enzyme.name))
            {
                //partially impaired
                //find the appending letters
                var dcmEnzyme = db.Dcms.Where(n => n.name == enzyme.name).FirstOrDefault();
                string letters1 = null;
                string letters2 = null;
                //completely blocked
                //find the appending letters
                if (string.IsNullOrWhiteSpace(dcmEnzyme.prefixing) && string.IsNullOrWhiteSpace(dcmEnzyme.appending))
                {
                    //appending and prefxing letters are both empty, then it is internal dcm

                    if (rs.IndexOf("CCAGG") != -1 || rs.IndexOf("CCTGG") != -1)
                    {
                        dict.Add(false, true);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(dcmEnzyme.prefixing) && string.IsNullOrWhiteSpace(dcmEnzyme.appending))
                {
                    //prefixing letters are not empty but appeding letters are empty, 
                    int prefixCount = dcmEnzyme.prefixing.Length;
                    if (prefixCount < 5 && ("CCAGG".IndexOf(dcmEnzyme.prefixing) != -1 || "CCTGG".IndexOf(dcmEnzyme.prefixing) != -1))
                    {
                        letters1 = "CCAGG".Substring(0, prefixCount);
                        letters2 = "CCTGG".Substring(0, prefixCount);
                        if (index < prefixCount)
                        {
                            //need to take letter from the end of the fullSeq
                            string front = fullSeq.Substring(0, index);
                            string end = fullSeq.Substring(Math.Max(0, fullSeq.Length - (prefixCount - index)));
                            if (end + front == letters1 || end + front == letters2)
                            {
                                dict.Add(false, true);
                            }
                        }
                        else
                        {
                            if (fullSeq.Substring((index - prefixCount), prefixCount) == letters1 || fullSeq.Substring((index - prefixCount), prefixCount) == letters2)
                            {
                                dict.Add(false, true);
                            }
                        }

                    }


                }
                else if (string.IsNullOrWhiteSpace(dcmEnzyme.prefixing) && !string.IsNullOrWhiteSpace(dcmEnzyme.appending))
                {
                    //prefixing letters are empty but appeing letters are not empty
                    int appendCount = dcmEnzyme.appending.Length;
                    if (appendCount < 5 && ("CCAGG".IndexOf(dcmEnzyme.appending) != -1 || "CCTGG".IndexOf(dcmEnzyme.appending) != -1))
                    {
                        letters1 = "CCAGG".Substring((4 - appendCount), appendCount);
                        letters2 = "CCTGG".Substring((4 - appendCount), appendCount);
                        if (index + rs.Length + appendCount > fullSeq.Length)
                        {
                            string front = fullSeq.Substring(index + rs.Length);
                            string end = fullSeq.Substring(0, appendCount - (fullSeq.Length - (index + 1) - (rs.Length - 1)));
                            if (front + end == letters1 || front + end == letters2)
                            {
                                dict.Add(false, true);
                            }
                        }
                        else
                        {
                            if (fullSeq.Substring((index + rs.Length), appendCount) == letters1 && fullSeq.Substring((index + rs.Length), appendCount) == letters2)
                            {
                                dict.Add(false, true);
                            }
                        }

                    }
                }
                else
                {
                    //appending and prefxing letters are both not empty                         
                    //not likely, too short

                    int prefixCount = dcmEnzyme.prefixing.Length;
                    int appendCount = dcmEnzyme.appending.Length;


                    if ((prefixCount + appendCount) < 5 && (("CCAGG".IndexOf(dcmEnzyme.appending) != -1 || "CCTGG".IndexOf(dcmEnzyme.appending) != -1)) && ("CCAGG".IndexOf(dcmEnzyme.prefixing) != -1 || "CCTGG".IndexOf(dcmEnzyme.prefixing) != -1))
                    {
                        string front = (index < prefixCount) ? (fullSeq.Substring(Math.Max(0, fullSeq.Length - (prefixCount - index))) + fullSeq.Substring(0, index)) : fullSeq.Substring(index - prefixCount, prefixCount);
                        string mid = fullSeq.Substring(index, rs.Length);
                        string end = (index + rs.Length + appendCount > fullSeq.Length) ? (fullSeq.Substring(index + rs.Length) + fullSeq.Substring(0, appendCount - (fullSeq.Length - index - rs.Length))) : fullSeq.Substring(index + appendCount, appendCount);

                        if (front + mid + end == "CCAGG" || front + mid + end == "CCTGG")
                        {
                            dict.Add(false, true);
                        }
                    }
                }

            }
            else
            {
                //enzyme not in the dam list, set both to false
                dict.Add(false, false);
            }
            return dict;
        }

        //reverse
        public List<RestriFeatureObject> RrObject(string crs, string fullSeq, ecloning.Models.restri_enzyme enzyme, bool isCircular)
        {
            List<RestriFeatureObject> rObjects = new List<RestriFeatureObject>();

            for (int index = 0; ; index += crs.Length)
            {
                //object
                var FObject = new RestriFeatureObject();
                index = fullSeq.IndexOf(crs, index);
                if (index == -1)
                {
                    break;
                    //don't generate the object
                }
                else
                {
                    FObject.clockwise = 0;
                    FObject.start = index;
                    FObject.end = crs.Length + index - 1; //switch to 0 index mode

                    //deel with end pour of range problem
                    if (enzyme.reverse_cut< 0 && (index + crs.Length + Math.Abs(enzyme.reverse_cut) -1 > fullSeq.Length))
                    {
                        //FObject.clockwise = 0;
                        //FObject.start = index;
                        var length2 = Math.Abs(enzyme.reverse_cut) - (fullSeq.Length - index);
                        FObject.cut = length2 - 1;
                    }
                    else if ((enzyme.reverse_cut > crs.Length) && (index < (enzyme.reverse_cut + 1 -crs.Length)))
                    {
                        //FObject.clockwise = 0;
                        //FObject.start = index;
                        var length2 = enzyme.reverse_cut - enzyme.forward_seq.Length - index;                        
                    }
                    else
                    {                       
                        FObject.cut =(crs.Length - enzyme.reverse_cut)  + index - 1; //switch to 0 index mode 
                    }
                                       
                    //check dam and dcm
                    var findRestrction = new FindRestriction();
                    //chack dam
                    var dam = findRestrction.CheckRDam(crs, index, fullSeq, enzyme);
                    FObject.dam_complete = dam.Keys.FirstOrDefault();
                    FObject.dam_impaired = dam.Values.FirstOrDefault();

                    //check dcm 
                    var dcm = findRestrction.CheckRDcm(crs, index, fullSeq, enzyme);

                    FObject.dcm_complete = dcm.Keys.FirstOrDefault();
                    FObject.dcm_impaired = dcm.Values.FirstOrDefault();

                    //set the name of the feature
                    var featureName = enzyme.name;
                    FObject.name = featureName;
                }

                rObjects.Add(FObject);
            }


            //deal with the end
            if (isCircular)
            {
                //if it is plasmid, default
                var findRestrction = new FindRestriction();
                var EndObject = findRestrction.FindREndRestriction(crs, fullSeq, enzyme);
                if(EndObject.name != null)
                {
                    rObjects.Add(EndObject);
                }
            }
            return rObjects;
        }

        public RestriFeatureObject FindREndRestriction(string crs, string fullSeq, ecloning.Models.restri_enzyme enzyme)
        {
            var EndObject = new RestriFeatureObject();

            var index = fullSeq.LastIndexOf(crs);
            if (index != -1)
            {
                string tempFulSeq = fullSeq + fullSeq.Substring(0, crs.Length - 1);
                index = tempFulSeq.IndexOf(crs, index);// start from the last index, and look for an extra cut
                if (index != -1)
                {
                    ///do find an extra one that span the begining and end of the seq
                    ///
                    EndObject.clockwise = 0;
                    EndObject.start = index;
                    EndObject.end = crs.Length - (fullSeq.Length - 1 - index) - 1; //switch to 0 index mode


                    //cut after index
                    if (enzyme.reverse_cut >= 1)
                    {
                        EndObject.cut = (enzyme.reverse_cut + index <= fullSeq.Length) ? (enzyme.reverse_cut - 1 + index) : (enzyme.reverse_cut - 1 - (fullSeq.Length - 1 - index)); //switch to 0 index mode
                    }
                    else
                    {
                        EndObject.cut = enzyme.reverse_cut + index; //switch to 0 index mode
                    }


                    //check dam and dcm
                    var findRestrction = new FindRestriction();
                    //chack dam
                    var dam = findRestrction.CheckRDam(crs, index, fullSeq, enzyme);
                    EndObject.dam_complete = dam.Keys.FirstOrDefault();
                    EndObject.dam_impaired = dam.Values.FirstOrDefault();

                    //check dcm 
                    var dcm = findRestrction.CheckRDcm(crs, index, fullSeq, enzyme);

                    EndObject.dcm_complete = dcm.Keys.FirstOrDefault();
                    EndObject.dcm_impaired = dcm.Values.FirstOrDefault();

                    //set the name of the feature
                    var featureName = enzyme.name;
                    //if ((EndObject.dam_complete || EndObject.dam_impaired) && (EndObject.dcm_complete || EndObject.dcm_impaired))
                    //{
                    //    featureName = featureName + " (affected by Dam/Dcm Methylation)";
                    //}
                    //if ((EndObject.dam_complete || EndObject.dam_impaired) && (EndObject.dcm_complete == false && EndObject.dcm_impaired == false))
                    //{
                    //    featureName = featureName + " (affected by Dam Methylation)";
                    //}
                    //if ((EndObject.dcm_complete || EndObject.dcm_impaired) && (EndObject.dam_complete == false && EndObject.dam_impaired == false))
                    //{
                    //    featureName = featureName + " (affected by Dcm Methylation)";
                    //}
                    EndObject.name = featureName;
                }
            }

            return EndObject;
        }

        public Dictionary<bool, bool> CheckRDam(string crs, int index, string fullSeq, ecloning.Models.restri_enzyme enzyme)
        {
            var dict = new Dictionary<bool, bool>();

            //dam: GATC rc
            //completely overlapping and completely blocked
            var CBDam = db.Dams.Where(a => a.COverlapping == true && a.CBlocked == true).Select(n => n.name).ToList();
            //completely overlapping and partially impaired
            var CIDam = db.Dams.Where(a => a.COverlapping == true && a.CBlocked == false).Select(n => n.name).ToList();
            //partiallyly overlapping and completely blocked
            var PBDam = db.Dams.Where(a => a.COverlapping == false && a.CBlocked == true).Select(n => n.name).ToList();
            //partiallyly overlapping and partially impaired
            var PIDam = db.Dams.Where(a => a.COverlapping == false && a.CBlocked == true).Select(n => n.name).ToList();


            //completely overlapping
            if (CBDam.Count() > 0 && CBDam.Contains(enzyme.name))
            {
                //completely overlapping and completely blocked
                dict.Add(true, false);
            }
            else if (CIDam.Count() > 0 && CIDam.Contains(enzyme.name))
            {
                //completely overlapping and partially impaired
                dict.Add(false, true);
            }

            //partially overlapping 
            //need to check the seq GATC
            else if (PBDam.Count() > 0 && PBDam.Contains(enzyme.name))
            {
                //completely blocked
                //find the appending letters
                var damEnzyme = db.Dams.Where(n => n.name == enzyme.name).FirstOrDefault();
                //appending  letters cannot be both empty, can only be "T" or "TC"
                if (damEnzyme.appending != null)
                {
                    string letters = null;
                    if (damEnzyme.appending == "C")
                    {
                        letters = "ATC";
                    }
                    if (damEnzyme.appending == "TC")
                    {
                        letters = "TC";
                    }

                    if (string.IsNullOrWhiteSpace(letters))
                    {
                        //if letters are empty, assume it is not blocked
                        dict.Add(false, false);
                    }
                    else
                    {

                        //find the index of the letters in crs
                        int letterIndex = crs.IndexOf(letters);
                        if (letterIndex != -1)
                        {
                            if (letters == "ATC")
                            {
                                if (index ==0)
                                {
                                    //out of range
                                    if (fullSeq[fullSeq.Length -1].ToString() == "G".ToString())
                                    {
                                        dict.Add(true, false);
                                    }
                                }
                                else
                                {
                                    if (fullSeq[index -1].ToString() == "G".ToString())
                                    {
                                        dict.Add(true, false);
                                    }
                                }
                            }

                            if (letters == "TC")
                            {
                                if (index <=1)
                                {
                                    //out of range
                                    string newFullSeq = fullSeq.Substring(0, 20) + fullSeq;
                                    if ((newFullSeq[index -1].ToString() == "A".ToString()) && (newFullSeq[index -2].ToString() == "G".ToString()))
                                    {
                                        dict.Add(true, false);
                                    }
                                }
                                else
                                {
                                    if ((fullSeq[index - 1].ToString() == "A".ToString()) && (fullSeq[index - 2].ToString() == "G".ToString()))
                                    {
                                        dict.Add(true, false);
                                    }
                                }

                            }
                        }
                        else
                        {
                            dict.Add(false, false);
                        }

                    }
                }
                else
                {
                    //letters are not found on rs, assume it is not blocked
                    dict.Add(false, false);
                }
            }
            else if (PIDam.Count() > 0 && PIDam.Contains(enzyme.name))
            {
                //partially impaired
                //find the appending letters
                var damEnzyme = db.Dams.Where(n => n.name == enzyme.name).FirstOrDefault();
                if (damEnzyme.appending != null)
                {
                    string letters = null;
                    if (damEnzyme.appending == "C")
                    {
                        letters = "ATC";
                    }
                    if (damEnzyme.appending == "TC")
                    {
                        letters = "TC";
                    }

                    if (string.IsNullOrWhiteSpace(letters))
                    {
                        //if letters are empty, assume it is not blocked
                        dict.Add(false, false);
                    }
                    else
                    {
                        //find the index of the letters in crs
                        int letterIndex = crs.IndexOf(letters);
                        if (letterIndex != -1)
                        {
                            if (letters == "ATC")
                            {
                                if (index == 0)
                                {
                                    //out of range
                                    if (fullSeq[fullSeq.Length - 1].ToString() == "G".ToString())
                                    {
                                        dict.Add(false, true);
                                    }
                                }
                                else
                                {
                                    if (fullSeq[index - 1].ToString() == "G".ToString())
                                    {
                                        dict.Add(false, true);
                                    }
                                }
                            }

                            if (letters == "TC")
                            {
                                if (index <= 1)
                                {
                                    //out of range
                                    string newFullSeq = fullSeq.Substring(0, 20) + fullSeq;
                                    if ((newFullSeq[index - 1].ToString() == "A".ToString()) && (newFullSeq[index - 2].ToString() == "G".ToString()))
                                    {
                                        dict.Add(false, true);
                                    }
                                }
                                else
                                {
                                    if ((fullSeq[index - 1].ToString() == "A".ToString()) && (fullSeq[index - 2].ToString() == "G".ToString()))
                                    {
                                        dict.Add(false, true);
                                    }
                                }

                            }

                        }
                        else
                        {
                            //letters are not found on rs, assume it is not blocked
                            dict.Add(false, false);
                        }
                    }
                }
                else
                {
                    //if the appending letter is empty, assume it is not blocked
                    dict.Add(false, false);
                }

            }
            else
            {
                //enzyme not in the dam list, set both to false
                dict.Add(false, false);
            }

            return dict;
        }


        public Dictionary<bool, bool> CheckRDcm(string crs, int index, string fullSeq, ecloning.Models.restri_enzyme enzyme)
        {
            var dict = new Dictionary<bool, bool>();
            //complately blocked, impaired
            //dcm
            //completely overlapping and completely blocked
            var CBDcm = db.Dcms.Where(a => a.COverlapping == true && a.CBlocked == true).Select(n => n.name).ToList();
            //completely overlapping and partially impaired
            var CIDcm = db.Dcms.Where(a => a.COverlapping == true && a.CBlocked == false).Select(n => n.name).ToList();
            //partiallyly overlapping and completely blocked
            var PBDcm = db.Dcms.Where(a => a.COverlapping == false && a.CBlocked == true).Select(n => n.name).ToList();
            //partiallyly overlapping and partially impaired
            var PIDcm = db.Dcms.Where(a => a.COverlapping == false && a.CBlocked == true).Select(n => n.name).ToList();


            //**********************************************************************************************************check dcm**********************************************************************************************************//


            //completely overlapping
            if (CBDcm.Count() > 0 && CBDcm.Contains(enzyme.name))
            {
                //completely overlapping and completely blocked
                dict.Add(true, false);
            }
            else if (CIDcm.Count() > 0 && CIDcm.Contains(enzyme.name))
            {
                //completely overlapping and partially impaired
                dict.Add(false, true);
            }

            //partially overlapping 
            //need to check the seq CCTGG OR CCAGG (reversed com)
            else if (PBDcm.Count() > 0 && PBDcm.Contains(enzyme.name))
            {
                string letters1 = null;
                string letters2 = null;
                //completely blocked
                //find the appending letters
                var dcmEnzyme = db.Dcms.Where(n => n.name == enzyme.name).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(dcmEnzyme.prefixing) && string.IsNullOrWhiteSpace(dcmEnzyme.appending))
                {
                    //appending and prefxing letters are both empty, then it is internal dcm

                    if (crs.IndexOf("CCTGG") != -1 || crs.IndexOf("CCAGG") != -1)
                    {
                        dict.Add(true, false);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(dcmEnzyme.prefixing) && string.IsNullOrWhiteSpace(dcmEnzyme.appending))
                {
                    //prefixing letters are not empty but appeding letters are empty, 

                    //switch prefix and appending

                    int appendCount = dcmEnzyme.prefixing.Length;
                    if (appendCount < 5 && ("CCAGG".IndexOf(dcmEnzyme.prefixing) != -1 || "CCTGG".IndexOf(dcmEnzyme.prefixing) != -1))
                    {
                        letters1 = "CCTGG".Substring((4 - appendCount), appendCount);
                        letters2 = "CCAGG".Substring((4 - appendCount), appendCount);
                        if (index + crs.Length + appendCount > fullSeq.Length)
                        {
                            string front = fullSeq.Substring(index + crs.Length);
                            string end = fullSeq.Substring(0, appendCount - (fullSeq.Length - (index + 1) - (crs.Length - 1)));
                            if (front + end == letters1 || front + end == letters2)
                            {
                                dict.Add(true, false);
                            }
                        }
                        else
                        {
                            if (fullSeq.Substring((index + crs.Length), appendCount) == letters1 && fullSeq.Substring((index + crs.Length), appendCount) == letters2)
                            {
                                dict.Add(true, false);
                            }
                        }

                    }

                }
                else if (string.IsNullOrWhiteSpace(dcmEnzyme.prefixing) && !string.IsNullOrWhiteSpace(dcmEnzyme.appending))
                {
                    //prefixing letters are empty but appeing letters are not empty
                    int prefixCount = dcmEnzyme.appending.Length;
                    if (prefixCount < 5 && ("CCAGG".IndexOf(dcmEnzyme.appending) != -1 || "CCTGG".IndexOf(dcmEnzyme.appending) != -1))
                    {
                        letters1 = "CCAGG".Substring(0, prefixCount);
                        letters2 = "CCTGG".Substring(0, prefixCount);
                        //if cut in at the very beginning of the fullseq
                        if (index < prefixCount)
                        {
                            //need to take letter from the end of the fullSeq
                            string front = fullSeq.Substring(0, index); //take the letters before index, should be very short
                            string end = fullSeq.Substring(Math.Max(0, fullSeq.Length - (prefixCount - index))); //take the last few letters from fullseq
                            if (end + front == letters1 || end + front == letters2)
                            {
                                dict.Add(true, false);
                            }
                        }
                        else
                        {
                            if (fullSeq.Substring((index - prefixCount), prefixCount) == letters1 || fullSeq.Substring((index - prefixCount), prefixCount) == letters2)
                            {
                                dict.Add(true, false);
                            }
                        }

                    }
                }
                else
                {
                    //appending and prefxing letters are both not empty                         
                    //not likely, too short

                    //switch both
                    int prefixCount = dcmEnzyme.appending.Length;
                    int appendCount = dcmEnzyme.prefixing.Length;


                    if ((prefixCount + appendCount) < 5 && (("CCAGG".IndexOf(dcmEnzyme.appending) != -1 || "CCTGG".IndexOf(dcmEnzyme.appending) != -1)) && ("CCAGG".IndexOf(dcmEnzyme.prefixing) != -1 || "CCTGG".IndexOf(dcmEnzyme.prefixing) != -1))
                    {
                        string front = (index < prefixCount) ? (fullSeq.Substring(Math.Max(0, fullSeq.Length - (prefixCount - index))) + fullSeq.Substring(0, index)) : fullSeq.Substring(index - prefixCount, prefixCount);
                        string mid = fullSeq.Substring(index, crs.Length);
                        string end = (index + crs.Length + appendCount > fullSeq.Length) ? (fullSeq.Substring(index + crs.Length) + fullSeq.Substring(0, appendCount - (fullSeq.Length - index - crs.Length))) : fullSeq.Substring(index + appendCount, appendCount);

                        if (front + mid + end == "CCAGG" || front + mid + end == "CCTGG")
                        {
                            dict.Add(true, false);
                        }
                    }
                }

            }
            else if (PIDcm.Count() > 0 && PIDcm.Contains(enzyme.name))
            {
                //partially impaired
                //find the appending letters
                var dcmEnzyme = db.Dcms.Where(n => n.name == enzyme.name).FirstOrDefault();
                string letters1 = null;
                string letters2 = null;
                //completely blocked
                //find the appending letters
                if (string.IsNullOrWhiteSpace(dcmEnzyme.prefixing) && string.IsNullOrWhiteSpace(dcmEnzyme.appending))
                {
                    //appending and prefxing letters are both empty, then it is internal dcm

                    if (crs.IndexOf("CCAGG") != -1 || crs.IndexOf("CCTGG") != -1)
                    {
                        dict.Add(false, true);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(dcmEnzyme.prefixing) && string.IsNullOrWhiteSpace(dcmEnzyme.appending))
                {
                    //prefixing letters are not empty but appeding letters are empty, 
                    //switch
                    int appendCount = dcmEnzyme.prefixing.Length;
                    if (appendCount < 5 && ("CCAGG".IndexOf(dcmEnzyme.appending) != -1 || "CCTGG".IndexOf(dcmEnzyme.appending) != -1))
                    {
                        letters1 = "CCAGG".Substring((4 - appendCount), appendCount);
                        letters2 = "CCTGG".Substring((4 - appendCount), appendCount);
                        if (index + crs.Length + appendCount > fullSeq.Length)
                        {
                            string front = fullSeq.Substring(index + crs.Length);
                            string end = fullSeq.Substring(0, appendCount - (fullSeq.Length - (index + 1) - (crs.Length - 1)));
                            if (front + end == letters1 || front + end == letters2)
                            {
                                dict.Add(false, true);
                            }
                        }
                        else
                        {
                            if (fullSeq.Substring((index + crs.Length), appendCount) == letters1 && fullSeq.Substring((index + crs.Length), appendCount) == letters2)
                            {
                                dict.Add(false, true);
                            }
                        }

                    }
                }
                else if (string.IsNullOrWhiteSpace(dcmEnzyme.prefixing) && !string.IsNullOrWhiteSpace(dcmEnzyme.appending))
                {
                    //prefixing letters are empty but appeing letters are not empty
                    int prefixCount = dcmEnzyme.appending.Length;
                    if (prefixCount < 5 && ("CCAGG".IndexOf(dcmEnzyme.prefixing) != -1 || "CCTGG".IndexOf(dcmEnzyme.prefixing) != -1))
                    {
                        letters1 = "CCAGG".Substring(0, prefixCount);
                        letters2 = "CCTGG".Substring(0, prefixCount);
                        if (index < prefixCount)
                        {
                            //need to take letter from the end of the fullSeq
                            string front = fullSeq.Substring(0, index);
                            string end = fullSeq.Substring(Math.Max(0, fullSeq.Length - (prefixCount - index)));
                            if (end + front == letters1 || end + front == letters2)
                            {
                                dict.Add(false, true);
                            }
                        }
                        else
                        {
                            if (fullSeq.Substring((index - prefixCount), prefixCount) == letters1 || fullSeq.Substring((index - prefixCount), prefixCount) == letters2)
                            {
                                dict.Add(false, true);
                            }
                        }

                    }
                }
                else
                {
                    //appending and prefxing letters are both not empty                         
                    //not likely, too short
                    //switch both
                    int prefixCount = dcmEnzyme.appending.Length;
                    int appendCount = dcmEnzyme.prefixing.Length;


                    if ((prefixCount + appendCount) < 5 && (("CCAGG".IndexOf(dcmEnzyme.appending) != -1 || "CCTGG".IndexOf(dcmEnzyme.appending) != -1)) && ("CCAGG".IndexOf(dcmEnzyme.prefixing) != -1 || "CCTGG".IndexOf(dcmEnzyme.prefixing) != -1))
                    {
                        string front = (index < prefixCount) ? (fullSeq.Substring(Math.Max(0, fullSeq.Length - (prefixCount - index))) + fullSeq.Substring(0, index)) : fullSeq.Substring(index - prefixCount, prefixCount);
                        string mid = fullSeq.Substring(index, crs.Length);
                        string end = (index + crs.Length + appendCount > fullSeq.Length) ? (fullSeq.Substring(index + crs.Length) + fullSeq.Substring(0, appendCount - (fullSeq.Length - index - crs.Length))) : fullSeq.Substring(index + appendCount, appendCount);

                        if (front + mid + end == "CCAGG" || front + mid + end == "CCTGG")
                        {
                            dict.Add(false, true);
                        }
                    }
                }

            }
            else
            {
                //enzyme not in the dam list, set both to false
                dict.Add(false, false);
            }
            return dict;
        }


        //if rs complementaty is same same as rs, then no need to find new restriciton site, but need to check the dam and dcm
        public List<RestriFeatureObject> newFrObject(string rs, string fullSeq, restri_enzyme enzyme, List<RestriFeatureObject> FrObjects, bool isCircular)
        {
            var findObject = new FindRestriction();
            foreach (var o in FrObjects)
            {
                //chack dam
                var dam = findObject.CheckRDam(rs, o.start, fullSeq, enzyme);
                if (!o.dam_complete)
                {
                    o.dam_complete = dam.Keys.FirstOrDefault();
                }
                if (!o.dam_impaired)
                {
                    o.dam_impaired = dam.Values.FirstOrDefault();
                }
                //check dcm
                var dcm = findObject.CheckRDcm(rs, o.start, fullSeq, enzyme);
                if (!o.dcm_complete)
                {
                    o.dcm_complete = dcm.Keys.FirstOrDefault();
                }
                if (!o.dcm_impaired)
                {
                    o.dcm_impaired = dcm.Values.FirstOrDefault();
                }

                //update name
                var featureName = enzyme.name;
                o.name = featureName;
            }
            return FrObjects;
        }


        //=====================deel with more than one cut each enzyme ======================================================//

        //find the non N letters in the restriciton sites
        
        public List<RestriFeatureObject> FRr2Object (int cutNum, string rs, string fullSeq, ecloning.Models.restri_enzyme enzyme, bool isCircular)
        {
            List<RestriFeatureObject> FRr2Objects = new List<RestriFeatureObject>();
            List<RestriFeatureObject> Fr2Objects = new List<RestriFeatureObject>();
            List<RestriFeatureObject> Rr2Objects = new List<RestriFeatureObject>();

            var findObject = new FindRestriction();
            Fr2Objects = findObject.Fr2Object(rs, fullSeq, enzyme, isCircular);
            if (cutNum == 0 || (cutNum != 0 && Fr2Objects.Count() < cutNum))
            {
                Rr2Objects = findObject.Rr2Object(rs, fullSeq, enzyme, isCircular);
            }

            FRr2Objects = Fr2Objects.Concat(Rr2Objects).ToList();

            //remove duplicates
            FRr2Objects = FRr2Objects.GroupBy(c => c.cut).Select(f => f.First()).ToList();

            return FRr2Objects;
        }

        public List<RestriFeatureObject> Fr2Object(string rs, string fullSeq, restri_enzyme enzyme, bool isCircular)
        {
            List<RestriFeatureObject> r2Objects = new List<RestriFeatureObject>();
            //process rs
            var findRestrciton = new FindRestriction();
            var r2SeqObject = new TwocutRestriObject();
            r2SeqObject = findRestrciton.TwocutForardSeq(rs);
            //find the leftSeq
            for (int index = 0; ; index += r2SeqObject.leftSeq.Length)
            {
                //need to add two objects for each matched index
                var FObject1 = new RestriFeatureObject();
                var FObject2 = new RestriFeatureObject();
                index = fullSeq.IndexOf(r2SeqObject.leftSeq, index);
                if (index == -1)
                {
                    break;
                    //don't generate the object
                }
                else
                {
                    //check rightSeq
                    int rightIndex = index + r2SeqObject.leftSeq.Length + r2SeqObject.innerLength;
                    if(rightIndex > fullSeq.Length - 1)
                    {
                        //out of the range at the end, deal this when it is circular
                        continue;
                    }
                    if (fullSeq.Substring(rightIndex, r2SeqObject.rightSeq.Length) == r2SeqObject.rightSeq)
                    {
                        FObject1.start = index;
                        FObject1.end = index + rs.Length - 1;
                        FObject2.start = index;
                        FObject2.end = index + rs.Length - 1;

                        //add to object
                        //index found but the cut is out of the range
                        if (index < Math.Abs(enzyme.forward_cut))
                        {
                            //left end
                            var lenth1 = Math.Abs(enzyme.forward_cut) - index;
                            FObject1.cut = fullSeq.Length - lenth1 - 1;
                            FObject2.cut = index + (int)enzyme.forward_cut2;
                        }
                        else if ((index + Math.Abs((int)enzyme.forward_cut2)) > fullSeq.Length)
                        {
                            //right end
                            FObject1.cut = index + enzyme.forward_cut;
                            var length = index + (int)enzyme.forward_cut2 - fullSeq.Length;
                            FObject2.cut = length - 1;
                        }
                        else
                        {
                            FObject1.cut = index + enzyme.forward_cut;
                            FObject2.cut = index + (int)enzyme.forward_cut2;
                        }

                        FObject1.clockwise = 1;
                        FObject2.clockwise = 1;

                        //for now don't check dam and dcm

                        FObject1.name = enzyme.name;
                        FObject2.name = enzyme.name;

                        FObject1.dam_complete = false;
                        FObject1.dam_impaired = false;
                        FObject1.dcm_complete = false;
                        FObject1.dcm_impaired = false;


                        FObject2.dam_complete = false;
                        FObject2.dam_impaired = false;
                        FObject2.dcm_complete = false;
                        FObject2.dcm_impaired = false;

                        r2Objects.Add(FObject1);
                        r2Objects.Add(FObject2);
                    }
                }
            }
            if (isCircular)
            {
                //end
                var endObjects = findRestrciton.FindEnd2Restriction(r2SeqObject, fullSeq, enzyme);
                foreach (var o in endObjects)
                {
                    if (o.name != null)
                    {
                        r2Objects.Add(o);
                    }
                }
            }
            return r2Objects;
    }

        public List<RestriFeatureObject> FindEnd2Restriction(TwocutRestriObject r2SeqObject, string fullSeq, restri_enzyme enzyme)
        {
            List<RestriFeatureObject> r2EndObjects = new List<RestriFeatureObject>();
            RestriFeatureObject r2EndObject = new RestriFeatureObject();
            RestriFeatureObject r2EndObject2 = new RestriFeatureObject();


            //restriction site, the length
            var rsLength = r2SeqObject.leftSeq.Length + r2SeqObject.innerLength + r2SeqObject.rightSeq.Length;


            string rightEndSeq = fullSeq.Substring(fullSeq.Length - rsLength +1) + fullSeq.Substring(0, rsLength - 1);
            int index = rightEndSeq.IndexOf(r2SeqObject.leftSeq);

            if(index != -1)
            {
                int rightIndex = index + r2SeqObject.leftSeq.Length + r2SeqObject.innerLength - 1;
                if (rightIndex <= rsLength -2 && rightEndSeq.Substring(rightIndex, r2SeqObject.rightSeq.Length) == r2SeqObject.rightSeq)
                if (rsLength >=2 && rightIndex <= rsLength -2 && rightEndSeq.Substring(rightIndex, r2SeqObject.rightSeq.Length) == r2SeqObject.rightSeq)
                {
                    //left cut 
                    r2EndObject.cut = fullSeq.Length - 1 - rsLength - 1 - Math.Abs(enzyme.forward_cut) + index;
                    r2EndObject2.cut = rsLength - 1 - (2 * (rsLength - 1) - (index + rsLength)) + (int)enzyme.forward_cut2 - 1;

                    r2EndObject.clockwise = 1;
                    r2EndObject.start = fullSeq.Length - index;
                    r2EndObject.end = rsLength + index - (rsLength - 1) - 1; //switch to 0 index mode
                    r2EndObject2.clockwise = 1;
                    r2EndObject2.start = fullSeq.Length - index;
                    r2EndObject2.end = rsLength + index - (rsLength - 1) - 1; //switch to 0 index mode

                    r2EndObject.name = enzyme.name;
                    r2EndObject2.name = enzyme.name;

                    r2EndObject.dam_complete = false;
                    r2EndObject.dam_impaired = false;
                    r2EndObject.dcm_complete = false;
                    r2EndObject.dcm_impaired = false;

                    r2EndObject2.dam_complete = false;
                    r2EndObject2.dam_impaired = false;
                    r2EndObject2.dcm_complete = false;
                    r2EndObject2.dcm_impaired = false;
                    r2EndObjects.Add(r2EndObject);
                    r2EndObjects.Add(r2EndObject2);
                }
            }

            return r2EndObjects;
        }

        public List<RestriFeatureObject> Rr2Object(string rs, string fullSeq, restri_enzyme enzyme, bool isCircular)
        {
            List<RestriFeatureObject> r2Objects = new List<RestriFeatureObject>();

            //get the complementary fullseq
            var cfullSeq = FindSeq.cDNA(fullSeq);
            //get the reverse rs
            var rrs = FindSeq.ReverseSeq(rs);

            var findRestrciton = new FindRestriction();
            var r2SeqObject = new TwocutRestriObject();
            r2SeqObject = findRestrciton.TwocutForardSeq(rrs);
            for (int index = 0; ; index += r2SeqObject.leftSeq.Length)
            {
                //need to add two objects for each matched index
                var FObject1 = new RestriFeatureObject();
                var FObject2 = new RestriFeatureObject();
                index = cfullSeq.IndexOf(r2SeqObject.leftSeq, index);
                if (index == -1)
                {
                    break;
                    //don't generate the object
                }
                else
                {
                    //check rightSeq
                    int rightIndex = index + r2SeqObject.leftSeq.Length + r2SeqObject.innerLength;
                    if (rightIndex > cfullSeq.Length - 1)
                    {
                        //out of the range at the end, deal this when it is circular
                        continue;
                    }
                    if (cfullSeq.Substring(rightIndex, r2SeqObject.rightSeq.Length) == r2SeqObject.rightSeq)
                    {

                        FObject1.start = index;
                        FObject1.end = index + rrs.Length - 1;
                        FObject2.start = index;
                        FObject2.end = index + rrs.Length - 1;

                        //add to object
                        //index found but the cut is out of the range
                        if (index - 1 < Math.Abs((int)enzyme.forward_cut2) - rrs.Length)
                        {
                            //left end
                            var lenth1 = Math.Abs((int)enzyme.forward_cut2) - rrs.Length + 1 - index;
                            FObject1.cut = fullSeq.Length - lenth1 - 1;
                            FObject2.cut = index + Math.Abs(enzyme.forward_cut) - 1;
                        }
                        else if ((index + Math.Abs((int)enzyme.forward_cut) - 1) > fullSeq.Length)
                        {
                            //right end
                            FObject1.cut = index + Math.Abs((int)enzyme.forward_cut2) - rrs.Length - 1;
                            var length = index + Math.Abs((int)enzyme.forward_cut) - 1 - fullSeq.Length;
                            FObject2.cut = length - 1;
                        }
                        else
                        {
                            //in the middle
                            FObject1.cut = index + Math.Abs((int)enzyme.forward_cut2) - rrs.Length - 1;
                            FObject2.cut = index + Math.Abs(enzyme.forward_cut) - 1;
                        }

                        FObject1.clockwise = 0;
                        FObject2.clockwise = 0;

                        //for now don't check dam and dcm

                        FObject1.name = enzyme.name;
                        FObject2.name = enzyme.name;

                        FObject1.dam_complete = false;
                        FObject1.dam_impaired = false;
                        FObject1.dcm_complete = false;
                        FObject1.dcm_impaired = false;


                        FObject2.dam_complete = false;
                        FObject2.dam_impaired = false;
                        FObject2.dcm_complete = false;
                        FObject2.dcm_impaired = false;

                        r2Objects.Add(FObject1);
                        r2Objects.Add(FObject2);
                    }
                }
            }
            //check the circular
            if (isCircular)
            {
                //end
                var endObjects = findRestrciton.FindEnd2Restriction(r2SeqObject, cfullSeq, enzyme);
                foreach (var o in endObjects)
                {
                    if (o.name != null)
                    {
                        r2Objects.Add(o);
                    }
                }
            }
            return r2Objects;
        }

        public List<RestriFeatureObject> FindREnd2Restriction(TwocutRestriObject r2SeqObject, string cfullSeq, restri_enzyme enzyme)
        {
            List<RestriFeatureObject> r2EndObjects = new List<RestriFeatureObject>();
            RestriFeatureObject r2EndObject = new RestriFeatureObject();
            RestriFeatureObject r2EndObject2 = new RestriFeatureObject();
            var rsLength = r2SeqObject.leftSeq.Length + r2SeqObject.innerLength + r2SeqObject.rightSeq.Length;
            string rightEndSeq = cfullSeq.Substring(cfullSeq.Length - rsLength + 1) + cfullSeq.Substring(0, rsLength - 1);
            int index = rightEndSeq.IndexOf(r2SeqObject.leftSeq);
            if (index != -1)
            {
                int rightIndex = index + r2SeqObject.leftSeq.Length + r2SeqObject.innerLength - 1 ;
                if (rsLength >= 2 && rightIndex <= rsLength - 2 && rightEndSeq.Substring(rightIndex, r2SeqObject.rightSeq.Length) == r2SeqObject.rightSeq)
                {

                    r2EndObject.cut = cfullSeq.Length - 1 - rsLength - 1 - Math.Abs((int)enzyme.forward_cut2 - rsLength + 1) + index;
                    r2EndObject2.cut = rsLength - 1 - (2 * (rsLength - 1) - (index + rsLength)) + Math.Abs(enzyme.forward_cut) -1 - 1;

                    r2EndObject.clockwise = 0;
                    r2EndObject.start = cfullSeq.Length - index;
                    r2EndObject.end = rsLength + index - (rsLength - 1) - 1; //switch to 0 index mode
                    r2EndObject2.clockwise = 0;
                    r2EndObject2.start = cfullSeq.Length - index;
                    r2EndObject2.end = rsLength + index - (rsLength - 1) - 1; //switch to 0 index mode


                    r2EndObject.name = enzyme.name;
                    r2EndObject2.name = enzyme.name;

                    r2EndObject.dam_complete = false;
                    r2EndObject.dam_impaired = false;
                    r2EndObject.dcm_complete = false;
                    r2EndObject.dcm_impaired = false;

                    r2EndObject2.dam_complete = false;
                    r2EndObject2.dam_impaired = false;
                    r2EndObject2.dcm_complete = false;
                    r2EndObject2.dcm_impaired = false;
                    r2EndObjects.Add(r2EndObject);
                    r2EndObjects.Add(r2EndObject2);
                }
            }
           return r2EndObjects;
        }

        public TwocutRestriObject TwocutForardSeq(string rs)
        {
            var seqObject = new TwocutRestriObject();

            string leftSeq = null;
            string rightSeq = null;
            int innerLength = 0;

            rs = rs.TrimEnd('N').TrimStart('N');
            if (rs.Contains('N'))
            {
                int firstN = rs.IndexOf('N');
                int lastN = rs.LastIndexOf('N');
                leftSeq = rs.Substring(0, firstN);
                rightSeq = rs.Substring(lastN+1);
                innerLength = lastN - firstN + 1;
            }
            else
            {
                leftSeq = rs;
            }

            seqObject.leftSeq = leftSeq;
            seqObject.rightSeq = rightSeq;
            seqObject.innerLength = innerLength;

            return seqObject;
        }

    }
}