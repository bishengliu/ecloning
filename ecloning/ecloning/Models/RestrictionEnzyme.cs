using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ecloning.Models
{
    public class RestrictionEnzyme
    {
        //return startActivity symbol
        public string StarActivitySymbol(bool starActivity)
        {
            string symbol = null;
            if (starActivity)
            {
                symbol = "<span class='fa-stack fa-lg'><i class='fa fa-circle-thin fa-stack-2x'></i><i class='fa fa-star fa-stack-1x text-danger'></i></span>";
            }
            return symbol;
        }

        public string DamSymbol(bool dam)
        {
            string symbol = null;
            if (dam)
            {
                symbol = "<button type='button' class='btn btn-danger btn-circle disabled'>Dam</button>";
            }
            return symbol;
        }

        public string DcmSymbol(bool dcm)
        {
            string symbol = null;
            if (dcm)
            {
                symbol = "<button type='button' class='btn btn-primary btn-circle disabled'>Dcm</button>";
            }
            return symbol;
        }

        public string CpGSymbol(bool cpg)
        {
            string symbol = null;
            if (cpg)
            {
                symbol = "<button type='button' class='btn btn-info btn-circle disabled'>CpG</button>";
            }
            return symbol;
        }

        public string InactivationSymbol(int inactivation)
        {
            string symbol = null;
            if (inactivation == 1)
            {
                //650C
                symbol = "<button type='button' class='btn btn-success btn-circle disabled'>65&deg;C</button>";
            }
            else if(inactivation == 2)
            {
                //850C
                symbol = "<button type='button' class='btn btn-success btn-circle disabled'>80&deg;C</button>";
            }
            else
            {
                symbol = "<span class='fa-stack fa-lg'><i class='fa fa-fire fa-stack-1x'></i><i class='fa fa-ban fa-stack-2x text-danger'></i></span>";
            }
            return symbol;
        }


        //process forward seq with one cut
        public string RestrictionForwardSeq(string seq, int forward_cut, int reverse_cut)
        {
            string newSeq = seq;
            int totalLengen = seq.Length;
            //precess cut
            if (forward_cut >= 1 && forward_cut > totalLengen)
            {
                //put N
                var Ns = new StringBuilder();
                for (int i = 1; i <= (forward_cut - totalLengen); i++)
                {
                    Ns.Append("N");
                }
                newSeq = newSeq + Ns;

                //CHECK THE REVERSE CUT
                if (reverse_cut >= 1 && reverse_cut > forward_cut)
                {
                    var N2 = new StringBuilder();
                    for (int i = 1; i <= (reverse_cut - forward_cut); i++)
                    {
                        N2.Append("N");
                    }
                    newSeq = newSeq + N2;
                }
                if (reverse_cut < 0)
                {
                    var N3 = new StringBuilder();
                    for (int i = -1; i >= reverse_cut; i--)
                    {
                        N3.Append("N");
                    }
                    newSeq = N3 + newSeq;
                }
            }
            if (forward_cut < 0)
            {
                //put N
                var Ns = new StringBuilder();
                for (int i = -1; i >= forward_cut; i--)
                {
                    Ns.Append("N");
                }
                newSeq = Ns + newSeq;
                //CHECK THE REVERSE CUT
                if (reverse_cut >= 1 && reverse_cut > totalLengen)
                {
                    var N2 = new StringBuilder();
                    for (int i = 1; i <= (reverse_cut - totalLengen); i++)
                    {
                        N2.Append("N");
                    }
                    newSeq = newSeq + N2;
                }
                if (reverse_cut < 0 && reverse_cut < forward_cut)
                {
                    var N3 = new StringBuilder();
                    for (int i = 1; i <= (Math.Abs(reverse_cut) - Math.Abs(forward_cut)); i++)
                    {
                        N3.Append("N");
                    }
                    newSeq = N3 + newSeq;
                }
            }

            return newSeq;
        }

        //find new cut position
        public int[] FindNewCut(string newSeq, string seq, int forward_cut, int reverse_cut)
        {
            

            //find index of ols seq in newseq
            int idx = newSeq.IndexOf(seq);
            int newFCut = forward_cut;
            int newRCut = reverse_cut;
            if (forward_cut > 0 && reverse_cut >0)
            {
                if(idx >= 0)
                {
                    //idx is 0
                    newFCut = forward_cut + idx;
                    newRCut = reverse_cut + idx;
                }
            } 
            if(forward_cut > 0 && reverse_cut < 0)
            {
                if (idx >= 0)
                {
                    //idx >0
                    newFCut = forward_cut + idx;
                    newRCut = 0;
                }
            }
            if (forward_cut < 0 && reverse_cut > 0)
            {
                if (idx >= 0)
                {
                    //idx >0
                    newFCut = 0;
                    newRCut = reverse_cut + idx;
                }
            }
            if (forward_cut < 0 && reverse_cut < 0)
            {
                if (idx >= 0)
                {
                    if (forward_cut <= reverse_cut)
                    {
                        //idx >0
                        newFCut = 0;
                        newRCut = Math.Abs(forward_cut) - Math.Abs(reverse_cut);
                    }
                    else
                    {
                        newFCut = idx + forward_cut;
                        newRCut = idx + reverse_cut;
                    }
                }
            }
            int[] cuts = new int[2] { newFCut, newRCut };
            return cuts;
        }

        //display prototype cut for restriction enzyme
        //if there is non-N letter code, show only single straind
        //if there is only N letter with ATGC show both strainds
        public string ShowPrototype(string seq, int forward_cut, int reverse_cut)
        {
            string prototype = "<span></span>";
            //total length of seq
            

            List<string> Letters = new List<string>() {"R", "K", "B", "Y", "S", "D", "W", "H", "M", "V"}; //doesn't contain "N"

            //process seq with one cut
            var processSeq = new RestrictionEnzyme();
            var newSeq = processSeq.RestrictionForwardSeq(seq, forward_cut, reverse_cut);

            //find new cut position on the newseq
            var cuts = processSeq.FindNewCut(newSeq, seq, forward_cut, reverse_cut);

            var newFCut = cuts[0];
            var newRCut = cuts[1];

            //find whether seq contains letters above
            bool hasLetter = false;
            foreach(var letter in Letters)
            {
                if (seq.Contains(letter))
                {
                    hasLetter = true;
                }
            }

            //build cut prototype
            int totalLengen = newSeq.Length;
            
            if (hasLetter)
            {
                //if there is non-N letter code, show only single straind
                string front = newSeq.Substring(0, newFCut);
                string end = newSeq.Substring(newFCut, (totalLengen - newFCut));
                prototype = "<pre><span class='seqFont'>5' - <strong>" + front + "</strong><span class='verticalLine'></span><strong>" + end + "</strong> - 3'</span></pre>";

            }
            else
            {
                //if there is only N letter with ATGC show both strainds

                if(newFCut >= newRCut)
                {
                    //forward seq
                    string front1 = newSeq.Substring(0, newRCut);
                    string front12 = newSeq.Substring(newRCut, (newFCut - newRCut));

                    string end1 = newSeq.Substring(newFCut, (totalLengen - newFCut));
                    string forward = "<pre><span class='seqFont'>5' - <strong>" + front1 + "</strong><span class='horizontalLine'><strong>" + front12 + "</strong></span><span class='verticalLine'></span><strong>" + end1 + "</strong> - 3'</span>";

                    //reverse seq
                    string cSeq = FindSeq.cDNA(newSeq);
                    string front2 = cSeq.Substring(0, newRCut);
                    string end2 = cSeq.Substring(newRCut, (totalLengen - newRCut));
                    prototype = forward + "<br/><span class='seqFont'>3' - <strong>" + front2 + "</strong><span class='verticalLine'></span><strong>" + end2 + "</strong> - 5'</span></pre>";
                }
                else
                {
                    //forward seq
                    string front1 = newSeq.Substring(0, newFCut);

                    string end1 = newSeq.Substring(newFCut, (newRCut - newFCut));
                    string end12 = newSeq.Substring(newRCut, (totalLengen - newRCut));

                    string forward = "<pre><span class='seqFont'>5' - <strong>" + front1 + "</strong><span class='verticalLine'></span><span class='horizontalLine'><strong>" + end1+ "</strong></span><strong>" + end12 + "</strong> - 3'</span>";

                    //reverse seq
                    string cSeq = FindSeq.cDNA(newSeq);
                    string front2 = cSeq.Substring(0, newRCut);
                    string end2 = cSeq.Substring(newRCut, (totalLengen - newRCut));
                    prototype = forward + "<br/><span class='seqFont'>3' - <strong>" + front2 + "</strong><span class='verticalLine'></span><strong>" + end2 + "</strong> - 5'</span></pre>";
                }
                
            }

            return prototype;
        }


        //process the most left and most right cuts
        public string RestrictionForwardSeq2(string seq, int forward_cut, int reverse_cut, int forward_cut2, int reverse_cut2)
        {

            var processSeq = new RestrictionEnzyme();
            //the most left cut
            var newSeq1 = processSeq.RestrictionForwardSeq(seq, forward_cut, reverse_cut);
            //find new most right cut
            var cuts = processSeq.FindNewCut(newSeq1, seq, forward_cut2, reverse_cut2);

            var newFCut2 = cuts[0];
            var newRCut2 = cuts[1];

            //the most right cut
            var newSeq2 = processSeq.RestrictionForwardSeq(newSeq1, newFCut2, reverse_cut2);
            return newSeq2;
        }

        public string ShowPrototype2(string seq, int forward_cut, int reverse_cut, int forward_cut2, int reverse_cut2)
        {
            string prototype = "<span></span>";
            //total length of seq


            List<string> Letters = new List<string>() { "R", "K", "B", "Y", "S", "D", "W", "H", "M", "V" }; //doesn't contain "N"

            //process seq with one cut
            var processSeq = new RestrictionEnzyme();
            var newSeq = processSeq.RestrictionForwardSeq2(seq, forward_cut, reverse_cut, forward_cut2, reverse_cut2);

            //find new the most left cut
            var leftCut = processSeq.FindNewCut(newSeq, seq, forward_cut, reverse_cut);
            var newLeftFCut = leftCut[0];
            var newLeftRCut = leftCut[1];

            //find the most right cut
            var rightCut = processSeq.FindNewCut(newSeq, seq, forward_cut2, reverse_cut2);
            var newRightFCut = rightCut[0];
            var newRightRCut = rightCut[1];

            //find whether seq contains letters above
            bool hasLetter = false;
            foreach (var letter in Letters)
            {
                if (seq.Contains(letter))
                {
                    hasLetter = true;
                }
            }

            //build the prototype
            int totalLengen = newSeq.Length;
            if (hasLetter)
            {
                //if there is non-N letter code, show only single straind
                string front = newSeq.Substring(0, newLeftFCut);
                string middle = newSeq.Substring(newLeftFCut, (newRightFCut - newLeftFCut));
                string end = newSeq.Substring(newRightFCut, (totalLengen - newRightFCut));
                prototype = "<pre><span class='seqFont'>5' - <strong>" + front + "</strong><span class='verticalLine'></span><strong>" + middle + "</strong><span class='verticalLine'></span><strong>" + end + "</strong> - 3'</span></pre>";

            }
            else
            {
                //if there is only N letter with ATGC show both strainds
                //the complementary seq
                string cSeq = FindSeq.cDNA(newSeq);
                //the most left cut
                if (newLeftFCut >= newLeftRCut)
                {
                    //the forward seq
                    string frontF = newSeq.Substring(0, newLeftRCut);
                    string middle1F = newSeq.Substring(newLeftRCut, (newLeftFCut - newLeftRCut));
                    string middle2F = "";
                    string middle3F = "";
                    string endF = "";

                                        
                    string frontR = cSeq.Substring(0, newLeftRCut);
                    string middle1R = cSeq.Substring(newLeftRCut, (newLeftFCut - newLeftRCut));
                    string middle2R = "";
                    string middle3R = "";
                    string endR = "";
                    //deal with the most right cut

                    if (newRightFCut >= newRightRCut)
                    {
                        //froward seq
                        middle2F = newSeq.Substring(newLeftFCut, (newRightRCut - newLeftFCut));
                        middle3F = newSeq.Substring(newRightRCut, (newRightFCut - newRightRCut));
                        endF = newSeq.Substring(newRightFCut, (totalLengen  - newRightFCut));

                        string forward = "<pre><span class='seqFont'>5' - <strong>" + frontF + "</strong><span class='horizontalLine'><strong>" + middle1F + "</strong></span><span class='verticalLine'></span><strong>" + middle2F + "</strong><span class='horizontalLine'><strong>" + middle3F + "</strong></span><span class='verticalLine'></span><strong>" + endF + "</strong> - 3'</span>";
                        
                        //reverse seq
                        middle2R = cSeq.Substring(newLeftFCut, (newRightRCut - newLeftFCut));
                        middle3R = cSeq.Substring(newRightRCut, (newRightFCut - newRightRCut));
                        endR = cSeq.Substring(newRightFCut, (totalLengen - newRightFCut));
                        prototype = forward + "<br/><span class='seqFont'>3' - <strong>" + frontR + "</strong><span class='verticalLine'></span><strong>" + middle1R + "</strong><strong>" + middle2R + "</strong><span class='verticalLine'></span><strong>" + middle3R + "</strong><strong>" + endF + "</strong> - 5'</span></pre>";

                    }
                    else
                    {
                        //forward seq
                        middle2F = newSeq.Substring(newLeftFCut, (newRightFCut - newLeftFCut));
                        middle3F = newSeq.Substring(newRightFCut, (newRightRCut - newRightFCut));
                        endF = newSeq.Substring(newRightRCut, (totalLengen - newRightRCut));

                        string forward = "<pre><span class='seqFont'>5' - <strong>" + frontF + "</strong><span class='horizontalLine'><strong>" + middle1F + "</strong></span><span class='verticalLine'></span><strong>" + middle2F + "</strong><span class='verticalLine'></span><span class='horizontalLine'><strong>" + middle3F + "</strong></span><strong>" + endF + "</strong> - 3'</span>";


                        //reverse_cut seq
                        middle2R = cSeq.Substring(newLeftFCut, (newRightFCut - newLeftFCut));
                        middle3R = cSeq.Substring(newRightFCut, (newRightRCut - newRightFCut));
                        endR = cSeq.Substring(newRightRCut, (totalLengen - newRightRCut));
                        prototype = forward + "<br/><span class='seqFont'>3' - <strong>" + frontR + "</strong><span class='verticalLine'></span><strong>" + middle1R + "</strong><strong>" + middle2R + "</strong><strong>" + middle3R + "</strong><span class='verticalLine'></span><strong>" + endF + "</strong> - 5'</span></pre>";

                    }

                }
                else
                {
                    //the forward seq
                    string frontF = newSeq.Substring(0, newLeftFCut);
                    string middle1F = newSeq.Substring(newLeftFCut, (newLeftRCut - newLeftFCut));
                    string middle2F = "";
                    string middle3F = "";
                    string endF = "";


                    //the complementary seq
                    string frontR = cSeq.Substring(0, newLeftFCut);
                    string middle1R = cSeq.Substring(newLeftFCut, (newLeftRCut - newLeftFCut));
                    string middle2R = "";
                    string middle3R = "";
                    string endR = "";
                    if (newRightFCut >= newRightRCut)
                    {
                        //froward seq
                        middle2F = newSeq.Substring(newLeftRCut, (newRightRCut - newLeftRCut));
                        middle3F = newSeq.Substring(newRightRCut, (newRightFCut - newRightRCut));
                        endF = newSeq.Substring(newRightFCut, (totalLengen - newRightFCut));

                        string forward = "<pre><span class='seqFont'>5' - <strong>" + frontF + "</strong><span class='verticalLine'></span><span class='horizontalLine'><strong>" + middle1F + "</strong></span><strong>" + middle2F + "</strong><span class='horizontalLine'><strong>" + middle3F + "</strong></span><span class='verticalLine'></span><strong>" + endF + "</strong> - 3'</span>";

                        //reverse seq
                        middle2R = cSeq.Substring(newLeftRCut, (newRightRCut - newLeftRCut));
                        middle3R = cSeq.Substring(newRightRCut, (newRightFCut - newRightRCut));
                        endR = cSeq.Substring(newRightFCut, (totalLengen - newRightFCut));
                        prototype = forward + "<br/><span class='seqFont'>3' - <strong>" + frontR + "</strong><strong>" + middle1R + "</strong><span class='verticalLine'></span><strong>" + middle2R + "</strong><span class='verticalLine'></span><strong>" + middle3R + "</strong><strong>" + endF + "</strong> - 5'</span></pre>";

                    }
                    else
                    {
                        //forward seq
                        middle2F = newSeq.Substring(newLeftRCut, (newRightFCut - newLeftRCut));
                        middle3F = newSeq.Substring(newRightFCut, (newRightRCut - newRightFCut));
                        endF = newSeq.Substring(newRightRCut, (totalLengen - newRightRCut));

                        string forward = "<pre><span class='seqFont'>5' - <strong>" + frontF + "</strong><span class='verticalLine'></span><span class='horizontalLine'><strong>" + middle1F + "</strong></span><strong>" + middle2F + "</strong><span class='verticalLine'></span><span class='horizontalLine'><strong>" + middle3F + "</strong></span><strong>" + endF + "</strong> - 3'</span>";


                        //reverse_cut seq
                        middle2R = cSeq.Substring(newLeftRCut, (newRightFCut - newLeftRCut));
                        middle3R = cSeq.Substring(newRightFCut, (newRightRCut - newRightFCut));
                        endR = cSeq.Substring(newRightRCut, (totalLengen - newRightRCut));
                        prototype = forward + "<br/><span class='seqFont'>3' - <strong>" + frontR + "</strong><strong>" + middle1R + "</strong><span class='verticalLine'></span><strong>" + middle2R + "</strong><strong>" + middle3R + "</strong><span class='verticalLine'></span><strong>" + endF + "</strong> - 5'</span></pre>";

                    }

                    //deal with the most right cut
                }
            }
            return prototype;
        }
    }
}