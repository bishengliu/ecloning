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

        //sequence of plasmid
        public string sequence { get; set; }

        //constructor
        public ORFFinder(int startCodon, int stopCodon, int frame, int direction, int minSzie, string sequence)
        {
            this.startCodon = startCodon;
            this.stopCodon = stopCodon;
            this.frame = frame;
            this.direction = direction;
            this.minSzie = minSzie;
            this.sequence = sequence;
        }
        
        
        public List<ORFObject> FindPlasmidORF()
        {

            
            var ORF = new List<ORFObject>();
            var ORF_FW = new List<ORFObject>();
            var ORF_RE = new List<ORFObject>();
            if (direction == 0)
            {
                //both direction
                //dealing for now only forward seq
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
                if (frame != 0)
                {
                    ORF_RE = FindReverseORF(frame, sequence, startCodon, stopCodon, minSzie);
                }
                else
                {
                    var orf1 = FindReverseORF(1, sequence, startCodon, stopCodon, minSzie);
                    var orf2 = FindReverseORF(2, sequence, startCodon, stopCodon, minSzie);
                    var orf3 = FindReverseORF(3, sequence, startCodon, stopCodon, minSzie);
                    ORF_RE = orf1.Concat(orf2).Concat(orf3).ToList();
                }

            }
            else if (direction == 1)
            {
                //forward only
                if (frame != 0)
                {
                    ORF_FW = FindORF(frame, sequence, startCodon, stopCodon, minSzie);
                }
                else
                {
                    var orf1 = FindORF(1, sequence, startCodon, stopCodon, minSzie);
                    var orf2 = FindORF(2, sequence, startCodon, stopCodon, minSzie);
                    var orf3 = FindORF(3, sequence, startCodon, stopCodon, minSzie);
                    ORF_FW = orf1.Concat(orf2).Concat(orf3).ToList();
                }
            }
            else
            {
                //reverse only
                if (frame != 0)
                {
                    ORF_RE = FindReverseORF(frame, sequence, startCodon, stopCodon, minSzie);
                }
                else
                {
                    var orf1 = FindReverseORF(1, sequence, startCodon, stopCodon, minSzie);
                    var orf2 = FindReverseORF(2, sequence, startCodon, stopCodon, minSzie);
                    var orf3 = FindReverseORF(3, sequence, startCodon, stopCodon, minSzie);
                    ORF_RE = orf1.Concat(orf2).Concat(orf3).ToList();
                }
            }
            
            ORF = ORF_FW.Concat(ORF_RE).ToList();
            return ORF;
        }


        //forward methods
        //return List<Tuple<int, int>>(promotpr, terminator)
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
            var tempTerminator = 0; //for keeping the last terminator
            if (resultPromotor == true && resultTerminator ==true)
            {
                //zip promotor and terminator
                foreach (var promotor in indexesPromotor)
                {
                    if (indexesTerminator.Count() == 0)
                    {
                        break;
                    }
                    if(promotor < tempTerminator)
                    {
                        continue;
                    }
                    foreach (var terminator in indexesTerminator)
                    {
                        if (promotor < terminator)
                        {
                            ProTerPair.Add(new Tuple<int, int>(promotor, terminator));
                            tempTerminator = terminator;
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
            startPos.Sort();
            stopPos.Sort();
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

        //return ORF
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


        //reverse methods
        //return List<Tuple<int, int>>(terminatorIndex, promotprIndex)
        public static List<Tuple<int, int>> PTReversePair(string sequence)
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
                    //feature sequence
                    var subSeq = FindSeq.ReverseSeq(item.sequence);
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

                    //feature sequence
                    var subSeq =FindSeq.ReverseSeq(item.sequence);
                    indexesTerminator = FindSeq.NotRestriction(sequence, subSeq);
                    if (indexesTerminator.Count() > 0)
                    {
                        resultTerminator = true;
                        indexesTerminator.Sort();
                    }
                }
            }

            List<Tuple<int, int>> ProTerPair = new List<Tuple<int, int>>();
            var tempPromotor = 0;
            if (resultPromotor == true && resultTerminator ==true)
            {
                //zip promotor and terminator
                foreach (var terminator in indexesTerminator)
                {
                    if (indexesPromotor.Count() == 0)
                    {
                        break;
                    }
                    if(terminator < tempPromotor)
                    {
                        continue;
                    }
                    foreach (var promotor in indexesPromotor)
                    {
                        if (terminator < promotor)
                        {
                            ProTerPair.Add(new Tuple<int, int>(terminator, promotor));
                            tempPromotor = promotor;
                            indexesPromotor.RemoveRange(0, indexesPromotor.IndexOf(promotor));
                        }
                    }
                }
            }


            return ProTerPair;
        }


        //return List<Tuple<int, int>>(stopCodonIndex, startCodonIndex)
        public static List<Tuple<int, int>> SSReversePair(int frame, string sequence, int startCodon, int stopCodon, int minSzie)
        {

            //frame can only be 1, 2 and 3
            //prepare tempSeq according to reading frame
            string strFront = sequence.Substring((sequence.Length -(frame - 1)), sequence.Length);
            string tempSeq = strFront + sequence.Remove((sequence.Length - (frame - 1)), sequence.Length);

            //prepare empty list
            List<int> startPos = new List<int>();
            List<int> stopPos = new List<int>();
            //start codes
            //string[] start = new string[] { "ATG", "GTG", "TTG", "CTG" };
            //string[] stop = new string[] { "TAA", "TAG", "TGA" };
            if (startCodon == 0)
            {
                //ATG revser to GTA
                for (var i = 0; i < (tempSeq.Length - 2); i += 3)
                {
                    if (tempSeq.Substring(i, i + 2) == "GTA")
                    {
                        int index = 0;
                        if (i>= 2)
                        {
                            index = i - ((frame - 1));
                        }
                        else
                        {
                            index = tempSeq.Length -1 - i;
                        }
                        startPos.Add(index);
                    }
                }

            }

            if (startCodon == 1)
            {
                // "GTG", "TTG", "CTG" revser is GTG GTT and GTC
                for (var i = 0; i < (tempSeq.Length - 2); i += 3)
                {
                    if (tempSeq.Substring(i, i + 2) == "GTG" || tempSeq.Substring(i, i + 2) == "GTT" || tempSeq.Substring(i, i + 2) == "GTC")
                    {
                        int index = 0;
                        if (i >= 2)
                        {
                            index = i - ((frame - 1));
                        }
                        else
                        {
                            index = tempSeq.Length - 1 - i;
                        }
                        startPos.Add(index);
                    }
                }
            }
            if (startCodon == 2)
            {
                //all
                for (var i = 0; i < (tempSeq.Length - 2); i += 3)
                {
                    if (tempSeq.Substring(i, i + 2) == "GTA" || tempSeq.Substring(i, i + 2) == "GTG" || tempSeq.Substring(i, i + 2) == "GTT" || tempSeq.Substring(i, i + 2) == "GTC")
                    {
                        int index = 0;
                        if (i >= 2)
                        {
                            index = i - ((frame - 1));
                        }
                        else
                        {
                            index = tempSeq.Length - 1 - i;
                        }
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
                    if (tempSeq.Substring(i, i + 2) == "AAT" || tempSeq.Substring(i, i + 2) == "GAT" || tempSeq.Substring(i, i + 2) == "AGT")
                    {
                        int index = 0;
                        if (i >= 2)
                        {
                            index = i - ((frame - 1));
                        }
                        else
                        {
                            index = tempSeq.Length - 1 - i;
                        }
                        stopPos.Add(index);
                    }
                }
            }

            //zip start and stop
            //suppose all startcode will start and all stopcode stops absolutely
            List<Tuple<int, int>> StartStopPair = new List<Tuple<int, int>>();
            List<Tuple<int, int>> StartStopPair1 = new List<Tuple<int, int>>();
            List<Tuple<int, int>> StartStopPair2 = new List<Tuple<int, int>>();
            stopPos.Sort();
            startPos.Sort();
            foreach (int stopItem in stopPos)
            {
                foreach (int startItem in startPos)
                {
                    if (startItem >= (stopItem + minSzie))
                    {
                        StartStopPair1.Add(new Tuple<int, int>(stopItem, startItem));
                    }
                }
            }
            foreach (int startItem in startPos)
            {
                foreach (int stopItem in stopPos)
                {
                    if (stopItem <= (startItem - minSzie))
                    {
                        StartStopPair2.Add(new Tuple<int, int>(stopItem, startItem));
                    }
                }
            }
            StartStopPair = StartStopPair1.Concat(StartStopPair2).ToList();
            return StartStopPair;
        }

        //return List<Tuple<int, int>>(stopCodonIndex, startCodonIndex)
        public static List<Tuple<int, int>> FinalReversePair(List<Tuple<int, int>> pTReversePair, List<Tuple<int, int>> sSReversePair)
        {
            List<Tuple<int, int>> finalReversePair = new List<Tuple<int, int>>();
            if (pTReversePair.Count() > 0 && sSReversePair.Count() > 0)
            {
                //find all start and end pair that line in the promotor and terminator pair
                foreach (var itemPT in pTReversePair)
                {
                    foreach (var itemSS in sSReversePair)
                    {
                        if ((itemPT.Item1 < itemSS.Item1) && (itemPT.Item2 > itemSS.Item2))
                        {
                            finalReversePair.Add(itemSS);
                        }
                    }
                }
            }
            return finalReversePair;
        }

        //return ReverseORF
        public static List<ORFObject> FindReverseORF(int frame, string sequence, int startCodon, int stopCodon, int minSzie)
        {
            var ReverseORF = new List<ORFObject>();
            //frame can only be 1, 2 and 3
            List<Tuple<int, int>> sSReversePair = new List<Tuple<int, int>>();
            List<Tuple<int, int>> pTReversePair = new List<Tuple<int, int>>();
            List<Tuple<int, int>> finalReversePair = new List<Tuple<int, int>>();
            //find start and end pair
            sSReversePair = SSReversePair(frame, sequence, startCodon, stopCodon, minSzie);

            if (sSReversePair.Count() > 0)
            {
                //find promotor and terminator pairs
                pTReversePair = PTReversePair(sequence);
            }

            //find all start and end pair that line in the promotor and terminator pair
            if (pTReversePair.Count() > 0)
            {
                finalReversePair = FinalPair(pTReversePair, sSReversePair);
            }

            if (finalReversePair.Count() > 0)
            {
                foreach (var o in finalReversePair)
                {
                    var orf = new ORFObject();
                    orf.Name = "ORF Frame " + frame.ToString();
                    orf.start = o.Item1;
                    orf.end = o.Item2;
                    orf.clockwise = 0;
                    ReverseORF.Add(orf);
                }
            }
            return ReverseORF;
        }
    }
}