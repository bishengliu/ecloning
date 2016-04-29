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
        
        //method to find ORFs
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
                    //frame 1, 2 or 3
                    ORF_FW = FindORF(frame, sequence, startCodon, stopCodon, minSzie);
                }
                else
                {
                    //frame 1, 2 and 3
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
        //find the promotor and terminator pairs in a plasmid
        //return List<Tuple<int, int>>(promotpr, terminator)
        public static List<Tuple<int, int>> PromotorTerminatorPair(string sequence)
        {
            ecloningEntities db = new ecloningEntities();
            //find all promotor from common_features
            var promotors = db.common_feature.Where(f => f.plasmid_feature.id == 2);
            bool resultPromotor = false;
            List<int> PromotorIndexes = new List<int>(); //keep all the indexes
            List<int> indexesPerPromotor = new List<int>();
            if (promotors.Count() > 0)
            {
                foreach (var item in promotors.ToList())
                {
                    //feature sequence
                    var subSeq = item.sequence;
                    indexesPerPromotor = FindSeq.NotRestriction(sequence, subSeq);
                    if (indexesPerPromotor.Count() > 0)
                    {
                        PromotorIndexes = PromotorIndexes.Concat(indexesPerPromotor).ToList();
                        resultPromotor = true;                        
                    }
                }
            }

            //find all polyA
            var terminators = db.common_feature.Where(f => f.plasmid_feature.id == 8);
            bool resultTerminator = false;
            List<int> TerminatorIndexes = new List<int>(); //keep all the indexes
            List<int> indexesPerTerminator = new List<int>();
            if (terminators.Count() > 0)
            {
                foreach (var item in terminators.ToList())
                {
                    //feature sequence
                    var subSeq = item.sequence;
                    indexesPerTerminator = FindSeq.NotRestriction(sequence, subSeq);
                    if (indexesPerTerminator.Count() > 0)
                    {
                        TerminatorIndexes = TerminatorIndexes.Concat(indexesPerTerminator).ToList();
                        resultTerminator = true;                        
                    }
                }
            }

            //sort the indexes
            PromotorIndexes.Sort();
            TerminatorIndexes.Sort();

            //generate the promotor and terminator pairs
            List<Tuple<int, int>> ProTerPair = new List<Tuple<int, int>>();
            var tempTerminator = 0; //for keeping the last terminator
            if (resultPromotor == true && resultTerminator ==true && PromotorIndexes.Count()>0 && TerminatorIndexes.Count()>0)
            {
                //zip promotor and terminator
                foreach (var promotor in PromotorIndexes)
                {
                    if (TerminatorIndexes.Count() == 0)
                    {
                        break;
                    }
                    if(promotor < tempTerminator)
                    {
                        continue;
                    }
                    foreach (var terminator in TerminatorIndexes)
                    {
                        if (promotor < terminator)
                        {
                            ProTerPair.Add(new Tuple<int, int>(promotor, terminator));
                            tempTerminator = terminator;
                            TerminatorIndexes.Remove(terminator);
                            break;
                        }
                    }
                }
            }
            return ProTerPair;
        }

        //find start codon and stop codon in firward sequence
        //An ORF is a continuous stretch of codons that do not contain a stop codon (usually UAA, UAG or UGA)
        //use stop codon to determine how many ORF
        public static List<Tuple<int, int>> StartStopPair(int frame, string sequence, int startCodon, int stopCodon, int minSzie)
        {

            //frame can only be 1, 2 and 3
            //prepare tempSeq according to reading frame
            string strFront = null;
            string tempSeq = null;
            if(frame == 1)
            {
                strFront = null;
                tempSeq = sequence;
            }
            else
            {
                strFront = sequence.Substring(0, (frame - 1));
                tempSeq = sequence.Remove(0, (frame - 1)) + strFront;
            }
            

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
                    if (tempSeq.Substring(i, 3) == "ATG")
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
                    if (tempSeq.Substring(i, 3) == "GTG" || tempSeq.Substring(i, 3) == "TTG" || tempSeq.Substring(i, 3) == "CTG")
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
                    if (tempSeq.Substring(i, 3) == "ATG" || tempSeq.Substring(i, 3) == "GTG" || tempSeq.Substring(i, 3) == "TTG" || tempSeq.Substring(i, 3) == "CTG")
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
                    if (tempSeq.Substring(i, 3) == "TAA" || tempSeq.Substring(i, 3) == "TAG" || tempSeq.Substring(i, 3) == "TGA")
                    {
                        var index = i + (frame - 1);
                        stopPos.Add(index);
                    }
                }
            }
            if (stopCodon == 1)
            {
                //all
                for (var i = 0; i < (tempSeq.Length - 2); i += 3)
                {
                    if (tempSeq.Substring(i, 3) == "TAA")
                    {
                        var index = i + (frame - 1);
                        stopPos.Add(index);
                    }
                }
            }
            if (stopCodon == 2)
            {
                //all
                for (var i = 0; i < (tempSeq.Length - 2); i += 3)
                {
                    if (tempSeq.Substring(i, 3) == "TAG")
                    {
                        var index = i + (frame - 1);
                        stopPos.Add(index);
                    }
                }
            }
            if (stopCodon == 3)
            {
                //all
                for (var i = 0; i < (tempSeq.Length - 2); i += 3)
                {
                    if (tempSeq.Substring(i, 3) == "TGA")
                    {
                        var index = i + (frame - 1);
                        stopPos.Add(index);
                    }
                }
            }
            //zip start and stop
            //suppose all startcode will start and all stopcode stops absolutely
            List<Tuple<int, int>> StartStopPair = new List<Tuple<int, int>>();
            startPos.Sort();
            stopPos.Sort();


            if(startPos.Count()>0 && stopPos.Count() > 0)
            {
                var tempStart = -1;
                foreach (int stopItem in stopPos)
                {
                    foreach (int startItem in startPos)
                    {
                        if(startItem < tempStart)
                        {
                            continue;
                        }

                        if (startItem <= (stopItem - minSzie))
                        {
                            StartStopPair.Add(new Tuple<int, int>(startItem, stopItem));
                            tempStart = stopItem;
                            break;
                        }
                        else
                        {
                            tempStart = stopItem > startItem ? stopItem: startItem;
                            break;
                        }
                    }
                }
            }

            return StartStopPair;
        }

        //restrict the start codon and stop codon pair in the promotor/terminator pairs
        //so that the start/stop codon, or ORF found is determined by user common_features, whcih are likely to be GOI in a plasmid.
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
        //dump the start/stop codon into ORF project, so that it could be connected with table common_features
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
                    orf.Name = "ORF frame " + frame.ToString();
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
            var cSequence = FindSeq.cDNA(sequence);
            //find all promotor from common_features
            var promotors = db.common_feature.Where(f => f.plasmid_feature.id == 2);
            bool resultPromotor = false;
            List<int> PromotorIndexes = new List<int>(); //keep all the indexes
            List<int> indexesPerPromotor = new List<int>();
            if (promotors.Count() > 0)
            {
                foreach (var item in promotors.ToList())
                {
                    //feature sequence
                    var subSeq = FindSeq.ReverseSeq(item.sequence);
                    indexesPerPromotor = FindSeq.NotRestriction(cSequence, subSeq);
                    if (indexesPerPromotor.Count() > 0)
                    {
                        PromotorIndexes = PromotorIndexes.Concat(indexesPerPromotor).ToList();
                        resultPromotor = true;
                    }
                }
            }

            //find all polyA
            var terminators = db.common_feature.Where(f => f.plasmid_feature.id == 8);
            bool resultTerminator = false;
            List<int> TerminatorIndexes = new List<int>(); //keep all the indexes
            List<int> indexesPerTerminator = new List<int>();
            if (terminators.Count() > 0)
            {
                foreach (var item in terminators.ToList())
                {

                    //feature sequence
                    var subSeq =FindSeq.ReverseSeq(item.sequence);
                    indexesPerTerminator = FindSeq.NotRestriction(cSequence, subSeq);
                    if (indexesPerTerminator.Count() > 0)
                    {
                        TerminatorIndexes = TerminatorIndexes.Concat(indexesPerTerminator).ToList();
                        resultTerminator = true;
                    }
                }
            }
            //sort
            PromotorIndexes.Sort();
            TerminatorIndexes.Sort();

            //find pairs
            List<Tuple<int, int>> ProTerPair = new List<Tuple<int, int>>();
            var tempPromotor = 0;
            if (resultPromotor == true && resultTerminator ==true && PromotorIndexes.Count() > 0 && TerminatorIndexes.Count() > 0)
            {
                //zip promotor and terminator
                foreach (var terminator in TerminatorIndexes)
                {
                    if (PromotorIndexes.Count() == 0)
                    {
                        break;
                    }
                    if(terminator < tempPromotor)
                    {
                        continue;
                    }
                    foreach (var promotor in PromotorIndexes)
                    {
                        if (terminator < promotor)
                        {
                            ProTerPair.Add(new Tuple<int, int>(terminator, promotor));
                            tempPromotor = promotor;
                            PromotorIndexes.Remove(promotor);
                            break;
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
            string strFront = null;
            string tempSeq = null;
            if (frame == 1)
            {
                strFront = null;
                tempSeq = FindSeq.cDNA(sequence);
            }
            else
            {
                strFront = sequence.Substring(((sequence.Length-1) - (frame - 1)), (frame - 1));
                tempSeq = strFront + sequence.Remove(((sequence.Length - 1) - (frame - 1)), (frame - 1));
            }

            //prepare empty list
            List<int> startPos = new List<int>();
            List<int> stopPos = new List<int>();
            //start codes
            //string[] start = new string[] { "ATG", "GTG", "TTG", "CTG" };
            //string[] stop = new string[] { "TAA", "TAG", "TGA" };
            if (startCodon == 0)
            {
                //ATG reverse to GTA
                for (var i = 0; i < (tempSeq.Length - 2); i += 3)
                {
                    if (tempSeq.Substring(i, 3) == "GTA")
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
                // "GTG", "TTG", "CTG" reverse is GTG GTT and GTC
                for (var i = 0; i < (tempSeq.Length - 2); i += 3)
                {
                    if (tempSeq.Substring(i, 3) == "GTG" || tempSeq.Substring(i, 3) == "GTT" || tempSeq.Substring(i, 3) == "GTC")
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
                    if (tempSeq.Substring(i, 3) == "GTA" || tempSeq.Substring(i, 3) == "GTG" || tempSeq.Substring(i, 3) == "GTT" || tempSeq.Substring(i, 3) == "GTC")
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
                    if (tempSeq.Substring(i, 3) == "AAT" || tempSeq.Substring(i, 3) == "GAT" || tempSeq.Substring(i, 3) == "AGT")
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
            if (stopCodon == 1)
            {
                //all
                for (var i = 0; i < (tempSeq.Length - 2); i += 3)
                {
                    if (tempSeq.Substring(i, 3) == "AAT")
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
            if (stopCodon == 2)
            {
                //all
                for (var i = 0; i < (tempSeq.Length - 2); i += 3)
                {
                    if (tempSeq.Substring(i, 3) == "GAT")
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
            if (stopCodon == 3)
            {
                //all
                for (var i = 0; i < (tempSeq.Length - 2); i += 3)
                {
                    if (tempSeq.Substring(i, 3) == "AGT")
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

            stopPos.Sort();
            startPos.Sort();


            if (startPos.Count() > 0 && stopPos.Count() > 0)
            {
                var tempStop = -1;
                foreach (int startItem in startPos)
                {
                    foreach (int stopItem in stopPos)
                    {
                        if (stopItem < tempStop)
                        {
                            continue;
                        }

                        if (stopItem <= (startItem - minSzie))
                        {
                            StartStopPair.Add(new Tuple<int, int>(stopItem, startItem));
                            tempStop = startItem;
                            break;
                        }
                        else
                        {
                            tempStop = startItem > stopItem ? startItem : stopItem;
                            break;
                        }
                    }
                }
            }

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
                    orf.Name = "ORF frame " + frame.ToString();
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