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
            List<RestriFeatureObject> restricFeatures = new List<RestriFeatureObject>();
            foreach (var e in enzymeId)
            {
                var enzyme = db.restri_enzyme.Find(e);


                //find forward
                var restriSeq = enzyme.forward_seq;
                //deel the letter codes in the restriction sites, generate the possible restriction sites
                var decodes = new DecodeRestrictionSeq();
                var forwardSeqList = decodes.Decode(restriSeq);



                if(enzyme.forward_cut2 == null || enzyme.reverse_cut2 == null)
                {
                    //only one cut each time
                    foreach(var rs in forwardSeqList)
                    {
                        List<int> indexes = new List<int>();
                        for (int index = 0; ; index += rs.Length)
                        {
                            index = fullSeq.IndexOf(rs, index);
                            if (index == -1)
                                break;
                            indexes.Add(index);
                        }
                        if(cutNum != 0)
                        {
                            //user set the cut number
                            if (indexes.Count() > cutNum)
                            {
                                //too many cuts, skip
                                break;
                            }
                            else
                            {
                                //need to add these to the map    
                                foreach(var i in indexes)
                                {
                                    //check for dam and dcm

                                }                                                            
                            }
                        }
                        else
                        {
                            //find all the cuts and show on the map
                            //check for dam and dcm

                            

                        }

                    }
                }
                else
                {
                    //more than one cuts each time

                }

            }


            return restricFeatures;

        }
    }
}