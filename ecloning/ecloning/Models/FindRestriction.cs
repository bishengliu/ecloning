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
            List<RestriFeatureObject> Objects = new List<RestriFeatureObject>();

            foreach (var e in enzymeId)
            {
                var enzyme = db.restri_enzyme.Find(e);


                //find forward
                var restriSeq = enzyme.forward_seq;
                //deel the letter codes in the restriction sites, generate the possible restriction sites
                var decodes = new DecodeRestrictionSeq();
                var forwardSeqList = decodes.Decode(restriSeq);


                if (enzyme.forward_cut2 == null || enzyme.reverse_cut2 == null)
                {
                
                    foreach(var rs in forwardSeqList)
                    {
                        List<RestriFeatureObject> FRrObjects = new List<RestriFeatureObject>();
                        var findObjects = new FindRestriction();
                        FRrObjects = findObjects.FRrObject(rs, fullSeq, enzyme);
                        Objects = Objects.Concat(FRrObjects).ToList();
                    }
                }
                else
                {
                    //more than one cuts each time

                }


            }


            return Objects;
        }



        //this funciton returen forward and completement restrictionObject
        public List<RestriFeatureObject> FRrObject(string rs, string fullSeq, ecloning.Models.restri_enzyme enzyme)
        {
            List<RestriFeatureObject> FRrObjects = new List<RestriFeatureObject>();
            List<RestriFeatureObject> FrObjects = new List<RestriFeatureObject>();
            List<RestriFeatureObject> RrObjects = new List<RestriFeatureObject>();


            //find forward
            var findObject = new FindRestriction();
            FrObjects = findObject.FrObject(rs, fullSeq, enzyme);
            //find the completment
            var crs = FindSeq.cDNA(rs);
            if (FrObjects.Count() > 0)
            {
                if (object.Equals(rs, crs))
                {
                    //find left side for dam or dcm
                    FrObjects = findObject.newFrObject(rs, fullSeq, enzyme, FrObjects);
                }
            }        
            if(!object.Equals(rs, crs))
            {
                RrObjects = findObject.FrObject(crs, fullSeq, enzyme);
            }
            FRrObjects = FrObjects.Concat(RrObjects).ToList();

            return FRrObjects;
        }
        //single cut one enzyme
        //forward
        public List<RestriFeatureObject> FrObject(string rs, string fullSeq, ecloning.Models.restri_enzyme enzyme)
        {
            List<RestriFeatureObject> rObjects = new List<RestriFeatureObject>();

            //get dam and dcm info

            //dam
            //completely overlapping and completely blocked
            var CBDam = db.Dams.Where(a => a.COverlapping == true && a.CBlocked == true).Select(n => n.name).ToList();
            //completely overlapping and partially impaired
            var CIDam = db.Dams.Where(a => a.COverlapping == true && a.CBlocked == false).Select(n => n.name).ToList();
            //partiallyly overlapping and completely blocked
            var PBDam = db.Dams.Where(a => a.COverlapping == false && a.CBlocked == true).Select(n => n.name).ToList();
            //partiallyly overlapping and partially impaired
            var PIDam = db.Dams.Where(a => a.COverlapping == false && a.CBlocked == true).Select(n => n.name).ToList();

            //dcm
            //completely overlapping and completely blocked
            var CBDcm = db.Dcms.Where(a => a.COverlapping == true && a.CBlocked == true).Select(n => n.name).ToList();
            //completely overlapping and partially impaired
            var CIDcm = db.Dcms.Where(a => a.COverlapping == true && a.CBlocked == false).Select(n => n.name).ToList();
            //partiallyly overlapping and completely blocked
            var PBDcm = db.Dcms.Where(a => a.COverlapping == false && a.CBlocked == true).Select(n => n.name).ToList();
            //partiallyly overlapping and partially impaired
            var PIDcm = db.Dcms.Where(a => a.COverlapping == false && a.CBlocked == true).Select(n => n.name).ToList();


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

                    //deal with end 
                    if(index == fullSeq.LastIndexOf(rs))
                    {
                        string tempFulSeq = fullSeq + fullSeq.Substring(0, rs.Length-1);
                        index = tempFulSeq.IndexOf(rs, index);
                        if (index == -1)
                        {
                            break;
                            //don't generate the object
                        }
                        ///do find a extra one that span the begining and end of the seq
                        ///
                        FObject.clockwise = 1;
                        FObject.start = index;
                        FObject.end = rs.Length -(fullSeq.Length - index) -1; //switch to 0 index mode
                        if(enzyme.forward_cut >= 1)
                        {
                            FObject.cut = (enzyme.forward_cut + index < fullSeq.Length)?(enzyme.forward_cut + index - 1):(enzyme.forward_cut - (fullSeq.Length - index) - 1); //switch to 0 index mode
                        }
                        else
                        {
                            FObject.cut = enzyme.forward_cut + index - 1; //switch to 0 index mode
                        }
                    }
                    else
                    {
                        //find
                        FObject.clockwise = 1;
                        FObject.start = index;
                        FObject.end = rs.Length + index - 1; //switch to 0 index mode
                        FObject.cut = enzyme.forward_cut + index - 1; //switch to 0 index mode
                    }


                    //*******************************************************************************************************check dam*****************************************************************************************************//

                    //completely overlapping
                    if(CBDam.Count()>0 && CBDam.Contains(enzyme.name))
                    {
                        //completely overlapping and completely blocked
                        FObject.dam_complete = true;
                        FObject.dam_impaired = false;
                    }
                    else if (CIDam.Count() > 0 && CIDam.Contains(enzyme.name))
                    {
                        //completely overlapping and partially impaired
                        FObject.dam_complete = false;
                        FObject.dam_impaired = true;
                    }

                    //partially overlapping 
                    //need to check the seq GATC
                    else if(PBDam.Count()>0 && PBDam.Contains(enzyme.name))
                    {
                        //completely blocked
                        //find the appending letters
                        var damEnzyme = db.Dams.Where(n => n.name == enzyme.name).FirstOrDefault();
                        //appending  letters cannot be both empty, can only be "T" or "TC"
                        if(damEnzyme.appending != null)
                        {
                            string letters = null;
                            if(damEnzyme.appending == "C")
                            {
                                letters = "GAT";
                            }
                            if (damEnzyme.appending == "TC")
                            {
                                letters = "GA";
                            }

                            if(string.IsNullOrWhiteSpace(letters))
                            {
                                //if letters are empty, assume it is not blocked
                                FObject.dam_complete = false;
                                FObject.dam_impaired = false;
                            }
                            else
                            {

                                //find the index of the letters in rs
                                int letterIndex = rs.IndexOf(letters);
                                if(letterIndex != -1)
                                {
                                    if(letters == "GAT")
                                    {
                                        if(index + letterIndex + 3 > fullSeq.Length)
                                        {
                                            //out of range
                                            string newFullSeq = fullSeq + fullSeq.Substring(0, 20);
                                            if (newFullSeq[index + letterIndex + 3].ToString() == "C".ToString())
                                            {
                                                FObject.dam_complete = true;
                                                FObject.dam_impaired = false;
                                            }
                                        }
                                        else
                                        {
                                            if(fullSeq[index+ letterIndex + 3].ToString() == "C".ToString())
                                            {
                                                FObject.dam_complete = true;
                                                FObject.dam_impaired = false;
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
                                                FObject.dam_complete = true;
                                                FObject.dam_impaired = false;
                                            }
                                        }
                                        else
                                        {
                                            if ((fullSeq[index + letterIndex + 2].ToString() == "T".ToString()) && (fullSeq[index+ letterIndex + 3].ToString() == "C".ToString()))
                                            {
                                                FObject.dam_complete = true;
                                                FObject.dam_impaired = false;
                                            }
                                        }

                                    }
                                }
                                else
                                {
                                    FObject.dam_complete = false;
                                    FObject.dam_impaired = false;
                                }

                            }
                        }
                        else
                        {
                            //letters are not found on rs, assume it is not blocked
                            FObject.dam_complete = false;
                            FObject.dam_impaired = false;
                        }
                    }
                    else if(PIDam.Count() > 0 && PIDam.Contains(enzyme.name))
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
                                FObject.dam_complete = false;
                                FObject.dam_impaired = false;
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
                                                FObject.dam_complete = false;
                                                FObject.dam_impaired = true;
                                            }
                                        }
                                        else
                                        {
                                            if (fullSeq[index + letterIndex + 3].ToString() == "C".ToString())
                                            {
                                                FObject.dam_complete = false;
                                                FObject.dam_impaired = true;
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
                                                FObject.dam_complete = false;
                                                FObject.dam_impaired = true;
                                            }
                                        }
                                        else
                                        {
                                            if ((fullSeq[index + letterIndex + 2].ToString() == "T".ToString()) && (fullSeq[index + letterIndex + 3].ToString() == "C".ToString()))
                                            {
                                                FObject.dam_complete = false;
                                                FObject.dam_impaired = true;
                                            }
                                        }

                                    }
                                }
                                else
                                {
                                    //letters are not found on rs, assume it is not blocked
                                    FObject.dam_complete = false;
                                    FObject.dam_impaired = false;
                                }
                            }
                        }
                        else
                        {
                            //if the appending letter is empty, assume it is not blocked
                            FObject.dam_complete = false;
                            FObject.dam_impaired = false;
                        }

                    }
                    else
                    {
                        //enzyme not in the dam list, set both to false
                        FObject.dam_complete = false;
                        FObject.dam_impaired = false;
                    }


                    //**********************************************************************************************************check dcm**********************************************************************************************************//


                    //completely overlapping
                    if (CBDcm.Count() > 0 && CBDcm.Contains(enzyme.name))
                    {
                        //completely overlapping and completely blocked
                        FObject.dcm_complete = true;
                        FObject.dcm_impaired = false;
                    }
                    else if (CIDcm.Count() > 0 && CIDcm.Contains(enzyme.name))
                    {
                        //completely overlapping and partially impaired
                        FObject.dcm_complete = false;
                        FObject.dcm_impaired = true;
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

                            if (rs.IndexOf("CCAGG") !=-1 || rs.IndexOf("CCTGG") != -1)
                            {
                                FObject.dcm_complete = true;
                                FObject.dcm_impaired = false;
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(dcmEnzyme.prefixing) && string.IsNullOrWhiteSpace(dcmEnzyme.appending))
                        {
                            //prefixing letters are not empty but appeing letters are empty, 
                            int prefixCount = dcmEnzyme.prefixing.Length;
                            if(prefixCount<5 && ("CCAGG".IndexOf(dcmEnzyme.prefixing) != -1 || "CCTGG".IndexOf(dcmEnzyme.prefixing) != -1))
                            {
                                letters1 = "CCAGG".Substring(0, prefixCount);
                                letters2 = "CCTGG".Substring(0, prefixCount);
                                if(index < prefixCount)
                                {

                                    //need to take letter from the end of the fullSeq
                                    string front = fullSeq.Substring(0, index);
                                    string end = fullSeq.Substring(Math.Max(0, fullSeq.Length - (prefixCount - index)));
                                    if(end+front == letters1 || end + front == letters2)
                                    {
                                        FObject.dcm_complete = true;
                                        FObject.dcm_impaired = false;
                                    }
                                }
                                else
                                {
                                    if(fullSeq.Substring((index - prefixCount), prefixCount)== letters1 || fullSeq.Substring((index - prefixCount), prefixCount) == letters2)
                                    {
                                        FObject.dcm_complete = true;
                                        FObject.dcm_impaired = false;
                                    }
                                }

                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(dcmEnzyme.prefixing) && !string.IsNullOrWhiteSpace(dcmEnzyme.appending))
                        {
                            //prefixing letters are empty but appeing letters are not empty
                            int appendCount = dcmEnzyme.appending.Length;
                            if (appendCount < 5 && ("CCAGG".IndexOf(dcmEnzyme.appending) != -1 || "CCTGG".IndexOf(dcmEnzyme.appending) != -1))
                            {
                                letters1 = "CCAGG".Substring((5 - appendCount), appendCount);
                                letters2 = "CCTGG".Substring((5 - appendCount), appendCount);
                                if(index + rs.Length + appendCount > fullSeq.Length)
                                {
                                    string front = fullSeq.Substring(index + rs.Length);
                                    string end = fullSeq.Substring(0, appendCount - (fullSeq.Length - index - rs.Length));
                                    if (front + end == letters1 && front + end == letters2)
                                    {
                                        FObject.dcm_complete = true;
                                        FObject.dcm_impaired = false;
                                    }
                                }
                                else
                                {
                                    if (fullSeq.Substring((index + rs.Length), appendCount) == letters1 && fullSeq.Substring((index + rs.Length), appendCount) == letters2)
                                    {
                                        FObject.dcm_complete = true;
                                        FObject.dcm_impaired = false;
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


                            if ((prefixCount+appendCount) < 5 && (("CCAGG".IndexOf(dcmEnzyme.appending) != -1 || "CCTGG".IndexOf(dcmEnzyme.appending) != -1)) && ("CCAGG".IndexOf(dcmEnzyme.prefixing) != -1 || "CCTGG".IndexOf(dcmEnzyme.prefixing) != -1))
                            {
                                string front = (index<prefixCount)? (fullSeq.Substring(Math.Max(0, fullSeq.Length - (prefixCount - index))) + fullSeq.Substring(0, index)) :fullSeq.Substring(index - prefixCount, prefixCount);
                                string mid = fullSeq.Substring(index, rs.Length);
                                string end = (index+rs.Length+appendCount > fullSeq.Length)?(fullSeq.Substring(index + rs.Length) + fullSeq.Substring(0, appendCount - (fullSeq.Length - index - rs.Length))) :fullSeq.Substring(index + appendCount, appendCount);

                                if(front + mid +end == "CCAGG" || front + mid + end == "CCTGG")
                                {
                                    FObject.dcm_complete = true;
                                    FObject.dcm_impaired = false;
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
                                FObject.dcm_complete = false;
                                FObject.dcm_impaired = true;
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(dcmEnzyme.prefixing) && string.IsNullOrWhiteSpace(dcmEnzyme.appending))
                        {
                            //prefixing letters are not empty but appeing letters are empty, 
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
                                        FObject.dcm_complete = false;
                                        FObject.dcm_impaired = true;
                                    }
                                }
                                else
                                {
                                    if (fullSeq.Substring((index - prefixCount), prefixCount) == letters1 || fullSeq.Substring((index - prefixCount), prefixCount) == letters2)
                                    {
                                        FObject.dcm_complete = false;
                                        FObject.dcm_impaired = true;
                                    }
                                }

                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(dcmEnzyme.prefixing) && !string.IsNullOrWhiteSpace(dcmEnzyme.appending))
                        {
                            //prefixing letters are empty but appeing letters are not empty
                            int appendCount = dcmEnzyme.appending.Length;
                            if (appendCount < 5 && ("CCAGG".IndexOf(dcmEnzyme.appending) != -1 || "CCTGG".IndexOf(dcmEnzyme.appending) != -1))
                            {
                                letters1 = "CCAGG".Substring((5 - appendCount), appendCount);
                                letters2 = "CCTGG".Substring((5 - appendCount), appendCount);
                                if (index + rs.Length + appendCount > fullSeq.Length)
                                {
                                    string front = fullSeq.Substring(index + rs.Length);
                                    string end = fullSeq.Substring(0, appendCount - (fullSeq.Length - index - rs.Length));
                                    if (front + end == letters1 && front + end == letters2)
                                    {
                                        FObject.dcm_complete = false;
                                        FObject.dcm_impaired = true;
                                    }
                                }
                                else
                                {
                                    if (fullSeq.Substring((index + rs.Length), appendCount) == letters1 && fullSeq.Substring((index + rs.Length), appendCount) == letters2)
                                    {
                                        FObject.dcm_complete = false;
                                        FObject.dcm_impaired = true;
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
                                    FObject.dcm_complete = false;
                                    FObject.dcm_impaired = true;
                                }
                            }

                        }

                    }
                    else
                    {
                        //enzyme not in the dam list, set both to false
                        FObject.dcm_complete = false;
                        FObject.dcm_impaired = false;
                    }



                    //set the name of the feature
                    var featureName = enzyme.name;
                    if((FObject.dam_complete || FObject.dam_impaired) && (FObject.dcm_complete || FObject.dcm_impaired))
                    {
                        featureName = featureName + " (affected by Dam/Dcm Methylation)";
                    }
                    if((FObject.dam_complete || FObject.dam_impaired) && (FObject.dcm_complete == false && FObject.dcm_impaired == false))
                    {
                        featureName = featureName + " (affected by Dam Methylation)";
                    }
                    if ((FObject.dcm_complete || FObject.dcm_impaired) && (FObject.dam_complete == false && FObject.dam_impaired == false))
                    {
                        featureName = featureName + " (affected by Dcm Methylation)";
                    }

                }

                rObjects.Add(FObject);
            }
            return rObjects;
        }

        //if rs complementaty is same same as rs, then no need to find new restriciton site, but need to check the dam and dcm
        public List<RestriFeatureObject> newFrObject(string rs, string fullSeq, ecloning.Models.restri_enzyme enzyme, List<RestriFeatureObject> FrObjects)
        {
            //get dam and dcm info

            //dam
            
            //partiallyly overlapping and completely blocked
            var PBDam = db.Dams.Where(a => a.COverlapping == false && a.CBlocked == true).Select(n => n.name).ToList();
            //partiallyly overlapping and partially impaired
            var PIDam = db.Dams.Where(a => a.COverlapping == false && a.CBlocked == true).Select(n => n.name).ToList();

            //dcm
            
            //partiallyly overlapping and completely blocked
            var PBDcm = db.Dcms.Where(a => a.COverlapping == false && a.CBlocked == true).Select(n => n.name).ToList();
            //partiallyly overlapping and partially impaired
            var PIDcm = db.Dcms.Where(a => a.COverlapping == false && a.CBlocked == true).Select(n => n.name).ToList();


            foreach (var o in FrObjects)
            {
                int index = o.start;
                if(index < 3)
                {
                    break;
                }
                //check dam
                if (o.dam_complete == false && o.dam_impaired == false)
                {
                    if (PBDam.Count() > 0 && PBDam.Contains(enzyme.name))
                    {
                        //completely blocked
                        //find the appending letters
                        var damEnzyme = db.Dams.Where(n => n.name == enzyme.name).FirstOrDefault();
                        //appending  letters cannot be both empty
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
                                //if the appending letter is empty, assume it is blocked
                                o.dam_complete = true;
                                o.dam_impaired = false;
                            }
                            else
                            {
                                if (letters == "GAT")
                                {
                                    if (fullSeq[index - 3].ToString() == "C".ToString())
                                    {
                                        o.dam_complete = true;
                                        o.dam_impaired = false;
                                    }
                                }

                                if (letters == "GA")
                                {
                                    if ((fullSeq[index - 2].ToString() == "T".ToString()) && (fullSeq[index - 3].ToString() == "C".ToString()))
                                    {
                                        o.dam_complete = true;
                                        o.dam_impaired = false;
                                    }
                                }
                            }
                        }
                    }
                    if (PIDam.Count() > 0 && PIDam.Contains(enzyme.name))
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
                                //if the appending letter is empty, assume it is blocked
                                o.dam_complete = false;
                                o.dam_impaired = true;
                            }
                            else
                            {
                                if (letters == "GAT")
                                {
                                    if (fullSeq[index - 3].ToString() == "C".ToString())
                                    {
                                        o.dam_complete = false;
                                        o.dam_impaired = true;
                                    }
                                }

                                if (letters == "GA")
                                {
                                    if ((fullSeq[index - 2].ToString() == "T".ToString()) && (fullSeq[index - 3].ToString() == "C".ToString()))
                                    {
                                        o.dam_complete = false;
                                        o.dam_impaired = true;
                                    }
                                }
                            }
                        }
                    }
                }
                //check dcm
                if(o.dcm_complete == false && o.dcm_impaired == false)
                {
                    if (PBDcm.Count() > 0 && PBDcm.Contains(enzyme.name))
                    {
                        //completely blocked
                        //find the appending letters
                        var dcmEnzyme = db.Dcms.Where(n => n.name == enzyme.name).FirstOrDefault();
                        //appending  letters cannot be both empty
                        if (dcmEnzyme.appending != null)
                        {
                            string letters = null;
                            if (dcmEnzyme.appending == "C")
                            {
                                letters = "GAT";
                            }
                            if (dcmEnzyme.appending == "TC")
                            {
                                letters = "GA";
                            }

                            if (string.IsNullOrWhiteSpace(letters))
                            {
                                //if the appending letter is empty, assume it is blocked
                                o.dcm_complete = true;
                                o.dcm_impaired = false;
                            }
                            else
                            {
                                if (letters == "GAT")
                                {
                                    if (fullSeq[index - 3].ToString() == "C".ToString())
                                    {
                                        o.dcm_complete = true;
                                        o.dcm_impaired = false;
                                    }
                                }

                                if (letters == "GA")
                                {
                                    if ((fullSeq[index - 2].ToString() == "T".ToString()) && (fullSeq[index - 3].ToString() == "C".ToString()))
                                    {
                                        o.dcm_complete = true;
                                        o.dcm_impaired = false;
                                    }
                                }
                            }
                        }
                    }
                    if (PIDcm.Count() > 0 && PIDcm.Contains(enzyme.name))
                    {
                        //partially impaired
                        //find the appending letters
                        var dcmEnzyme = db.Dcms.Where(n => n.name == enzyme.name).FirstOrDefault();
                        if (dcmEnzyme.appending != null)
                        {
                            string letters = null;
                            if (dcmEnzyme.appending == "C")
                            {
                                letters = "GAT";
                            }
                            if (dcmEnzyme.appending == "TC")
                            {
                                letters = "GA";
                            }

                            if (string.IsNullOrWhiteSpace(letters))
                            {
                                //if the appending letter is empty, assume it is blocked
                                o.dcm_complete = false;
                                o.dcm_impaired = true;
                            }
                            else
                            {
                                if (letters == "GAT")
                                {
                                    if (fullSeq[index - 3].ToString() == "C".ToString())
                                    {
                                        o.dcm_complete = false;
                                        o.dcm_impaired = true;
                                    }
                                }

                                if (letters == "GA")
                                {
                                    if ((fullSeq[index - 2].ToString() == "T".ToString()) && (fullSeq[index - 3].ToString() == "C".ToString()))
                                    {
                                        o.dcm_complete = false;
                                        o.dcm_impaired = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return FrObjects;
        }

        //complementary
        //for rs != crs
    }
}