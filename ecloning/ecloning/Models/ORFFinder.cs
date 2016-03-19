using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class ORFFinder
    {
        private ecloningEntities db = new ecloningEntities();
        //start nornal ATG is 0
        //alternative GTG TTG CTG is 1
        //all is 2
        public int startCodon { get; set; }
        //stop codons are:TAA TAG  TGA
        //TAA is 1
        //TAG is 2
        //TGA is 3
        //all is 0
        public int stopCodon { get; set; }
        //frame 1
        //frame 2
        //feame 3
        //frame 1, 2, 3 is 0
        public int frame { get; set; }
        // 0 is both directions
        //1 is forward
        //2 is reverse
        public int direction { get; set; }

        //minSzie = 30
        public int minSzie { get; set; }

        //constructor
        public ORFFinder(int startCodon, int stopCodon, int frame, int direction, int minSzie)
        {
            this.startCodon = startCodon;
            this.stopCodon = stopCodon;
            this.frame = frame;
            this.direction = direction;
            this.minSzie = minSzie;
        }

        
        
        public List<ORFObject> FindPlasmidORF(int frame, string sequence)
        {

            //dealing for now only forward seq
            var ORF = new List<ORFObject>();
            var ORF_FW = new List<ORFObject>();
            var ORF_RE = new List<ORFObject>();
            if (frame != 0)
            {
                ORF_FW = FindORF(frame, sequence, startCodon, stopCodon, minSzie);
            }
            else
            {
                var orf1= FindORF(1, sequence, startCodon, stopCodon, minSzie);
                var orf2 = FindORF(2, sequence, startCodon, stopCodon, minSzie);
                var orf3 = FindORF(3, sequence, startCodon, stopCodon, minSzie);
                ORF_FW = orf1.Concat(orf2).Concat(orf3).ToList();
            }
            //deal with reverse




            ORF = ORF_FW.Concat(ORF_RE).ToList();
            return ORF;
        }



        public static List<Tuple<int, int>> PromotorTerminatorPair(string sequence)
        {
            ecloningEntities db = new ecloningEntities();
            //find all promotor from common_features
            var promotors = db.common_feature.Where(f => f.plasmid_feature.feature != "promotor");
            bool resultPromotor = false;
            List<int> indexesPromotor = new List<int>();
            if (promotors.Count() > 0)
            {
                foreach (var item in promotors.ToList())
                {
                    //find all the indexes of features in both forward and reserver seq
                    //feature sequence
                    var subSeq = item.sequence;
                    indexesPromotor = FindSeq.NotRestriction(sequence, subSeq);
                    if (indexesPromotor.Count() > 0)
                    {
                        resultPromotor = true;
                        indexesPromotor.Sort();
                    }
                }
            }

            //find all polyA
            var terminators = db.common_feature.Where(f => f.plasmid_feature.feature != "terminator");
            bool resultTerminator = false;
            List<int> indexesTerminator = new List<int>();
            if (terminators.Count() > 0)
            {
                foreach (var item in terminators.ToList())
                {
                    //find all the indexes of features in both forward and reserver seq
                    //feature sequence
                    var subSeq = item.sequence;
                    indexesTerminator = FindSeq.NotRestriction(sequence, subSeq);
                    if (indexesTerminator.Count() > 0)
                    {
                        resultTerminator = true;
                        indexesTerminator.Sort();
                    }
                }
            }


            List<Tuple<int, int>> ProTerPair = new List<Tuple<int, int>>();
            if (resultPromotor == true && resultTerminator)
            {
                //zip promotor and terminator
                foreach (var promotor in indexesPromotor)
                {
                    if (indexesTerminator.Count() == 0)
                    {
                        break;
                    }
                    foreach (var terminator in indexesTerminator)
                    {
                        if (promotor < terminator)
                        {
                            ProTerPair.Add(new Tuple<int, int>(promotor, terminator));
                            indexesTerminator.RemoveRange(0, indexesTerminator.IndexOf(terminator));
                        }
                    }
                }
            }


            return ProTerPair;
        }

        public static List<Tuple<int, int>> StartStopPair(int frame, string sequence, int startCodon, int stopCodon, int minSzie)
        {

            //frame can only be 1, 2 and 3
            //prepare tempSeq according to reading frame
            string strFront = sequence.Substring(0, (frame - 1));
            string tempSeq = sequence.Remove(0, (frame - 1)) + strFront;

            //prepare empty list
            List<int> startPos = new List<int>();
            List<int> stopPos = new List<int>();
            //start codes
            //string[] start = new string[] { "ATG", "GTG", "TTG", "CTG" };
            //string[] stop = new string[] { "TAA", "TAG", "TGA" };
            if (startCodon == 0)
            {
                //ATG
                for (var i = 0; i < (tempSeq.Length - 2); i += 3)
                {
                    if (tempSeq.Substring(i, i + 2) == "ATG")
                    {
                        var index = i + (frame - 1);
                        startPos.Add(index);
                    }
                }

            }
            if (startCodon == 1)
            {
                // "GTG", "TTG", "CTG"
                for (var i = 0; i < (tempSeq.Length - 2); i += 3)
                {
                    if (tempSeq.Substring(i, i + 2) == "GTG" || tempSeq.Substring(i, i + 2) == "TTG" || tempSeq.Substring(i, i + 2) == "CTG")
                    {
                        var index = i + (frame - 1);
                        startPos.Add(index);
                    }
                }
            }
            if (startCodon == 2)
            {
                //all
                for (var i = 0; i < (tempSeq.Length - 2); i += 3)
                {
                    if (tempSeq.Substring(i, i + 2) == "ATG" || tempSeq.Substring(i, i + 2) == "GTG" || tempSeq.Substring(i, i + 2) == "TTG" || tempSeq.Substring(i, i + 2) == "CTG")
                    {
                        var index = i + (frame - 1);
                        startPos.Add(index);
                    }
                }
            }

            //stop codes
            if (stopCodon == 0)
            {
                //all
                for (var i = 0; i < (tempSeq.Length - 2); i += 3)
                {
                    if (tempSeq.Substring(i, i + 2) == "TAA" || tempSeq.Substring(i, i + 2) == "TAG" || tempSeq.Substring(i, i + 2) == "TGA")
                    {
                        var index = i + (frame - 1);
                        stopPos.Add(index);
                    }
                }
            }


            //zip start and stop
            //suppose all startcode will start and all stopcode stops absolutely
            List<Tuple<int, int>> StartStopPair = new List<Tuple<int, int>>();
            List<Tuple<int, int>> StartStopPair1 = new List<Tuple<int, int>>();
            List<Tuple<int, int>> StartStopPair2 = new List<Tuple<int, int>>();
            foreach (int stopItem in stopPos)
            {
                foreach (int startItem in startPos)
                {
                    if (startItem <= (stopItem - minSzie))
                    {
                        StartStopPair1.Add(new Tuple<int, int>(startItem, stopItem));
                    }
                }
            }
            foreach (int startItem in startPos)
            {
                foreach (int stopItem in stopPos)
                {
                    if (stopItem >= (stopItem + minSzie))
                    {
                        StartStopPair2.Add(new Tuple<int, int>(startItem, stopItem));
                    }
                }
            }
            StartStopPair = StartStopPair1.Concat(StartStopPair2).ToList();
            return StartStopPair;
        }


        public static List<Tuple<int, int>> FinalPair(List<Tuple<int, int>> proTerPair, List<Tuple<int, int>> startStopPair)
        {
            List<Tuple<int, int>> FinalPair = new List<Tuple<int, int>>();
            if (proTerPair.Count() > 0 && startStopPair.Count()>0)
            {
                //find all start and end pair that line in the promotor and terminator pair
                foreach (var itemPT in proTerPair)
                {
                    foreach (var itemSS in startStopPair)
                    {
                        if ((itemPT.Item1 < itemSS.Item1) && (itemPT.Item2 > itemSS.Item2))
                        {
                            FinalPair.Add(itemSS);
                        }
                    }
                }
            }
            return FinalPair;
        }


        public static List<ORFObject> FindORF(int frame, string sequence, int startCodon, int stopCodon, int minSzie)
        {
            var ORF = new List<ORFObject>();
            //frame can only be 1, 2 and 3
            List<Tuple<int, int>> startStopPair = new List<Tuple<int, int>>();
            List<Tuple<int, int>> proTerPair = new List<Tuple<int, int>>();
            List<Tuple<int, int>> finalPair = new List<Tuple<int, int>>();
            //find start and end pair
            startStopPair = StartStopPair(frame, sequence, startCodon, stopCodon, minSzie);

            if (startStopPair.Count() > 0)
            {
                //find promotor and terminator pairs
                proTerPair = PromotorTerminatorPair(sequence);
            }

            //find all start and end pair that line in the promotor and terminator pair
            if (proTerPair.Count() > 0)
            {
                finalPair = FinalPair(proTerPair, startStopPair);
            }

            if (finalPair.Count() > 0)
            {
                foreach (var o in finalPair)
                {
                    var orf = new ORFObject();
                    orf.Name = "ORF Frame " + frame.ToString();
                    orf.start = o.Item1;
                    orf.end = o.Item2;
                    orf.clockwise = 1;
                    ORF.Add(orf);
                }
            }
            return ORF;
        }
        

    }
}