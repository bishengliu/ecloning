using System;
using System.Collections.Generic;
using System.Linq;
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
                symbol = "<span class=\" fa-stack fa-lg\"><i class=\"fa fa-circle-thin fa-stack-2x\"></i><i class=\"fa fa-star fa-stack-1x text-danger\"></i></span>";
            }
            return symbol;
        }

        public string DamSymbol(bool dam)
        {
            string symbol = null;
            if (dam)
            {
                symbol = "<button type=\"button\" class=\"btn btn-danger btn-circle disabled\">Dam</button>";
            }
            return symbol;
        }

        public string DcmSymbol(bool dcm)
        {
            string symbol = null;
            if (dcm)
            {
                symbol = "<button type=\"button\" class=\"btn btn-primary btn-circle disabled\">Dcm</button>";
            }
            return symbol;
        }

        public string CpGSymbol(bool cpg)
        {
            string symbol = null;
            if (cpg)
            {
                symbol = "<button type=\"button\" class=\"btn btn-info btn-circle disabled\">CpG</button>";
            }
            return symbol;
        }
        public string InactivationSymbol(int inactivation)
        {
            string symbol = null;
            if (inactivation == 1)
            {
                //650C
                symbol = "<button type=\"button\" class=\"btn btn-success btn-circle disabled\">65&deg;C</button>";
            }
            else if(inactivation == 2)
            {
                //850C
                symbol = "<button type=\"button\" class=\"btn btn-success btn-circle disabled\">85&deg;C</button>";
            }
            else
            {
                symbol = "<span class=\" fa-stack fa-lg\"><i class=\"fa fa-fire fa-stack-1x\"></i><i class=\"fa fa-ban fa-stack-2x text-danger\"></i></span>";
            }
            return symbol;
        }

        //display prototype cut for restriction enzyme
        //if there is non-N letter code, show only single straind
        //if there is only N letter with ATGC show both strainds
        public string ShowPrototype(string seq, int forward_cut, int reverse_cut)
        {
            string prototype = "<span></span>";
            //total length of seq
            int totalLengen = seq.Length;

            List<string> Letters = new List<string>() {"R", "K", "B", "Y", "S", "D", "W", "H", "M", "V"}; //doesn't contain "N"

            //find whether seq contains letters above
            bool hasLetter = false;
            foreach(var letter in Letters)
            {
                if (seq.Contains(letter))
                {
                    hasLetter = true;
                }
            }
            if (hasLetter)
            {
                //if there is non-N letter code, show only single straind
                string front = seq.Substring(0, forward_cut);
                string end = seq.Substring(forward_cut, (totalLengen - forward_cut));
                prototype = "<span class=\"seqFont\">5' - <strong>" + front + "</strong><span class=\"verticalLine\"><strong></span><strong>" + end + "</strong> - 3'</span>";

            }
            else
            {
                //if there is only N letter with ATGC show both strainds

                if(forward_cut >= reverse_cut)
                {
                    //forward seq
                    string front1 = seq.Substring(0, reverse_cut);
                    string front12 = seq.Substring(reverse_cut, (forward_cut - reverse_cut));

                    string end1 = seq.Substring(forward_cut, (totalLengen - forward_cut));
                    prototype = "<span class=\"seqFont\">5' - <strong>" + front1 + "</strong><span class=\"horizontalLine\"><strong>" + front12 + "</strong></span><span class=\"verticalLine\"></span><strong>" + end1 + "</strong> - 3'</span>";

                    //reverse seq
                    string cSeq = FindSeq.cDNA(seq);
                    string front2 = cSeq.Substring(0, reverse_cut);
                    string end2 = cSeq.Substring(reverse_cut, (totalLengen - reverse_cut));
                    prototype = prototype + "<br/>";
                    prototype = prototype + "<span class=\"seqFont\">3' - <strong>" + front2 + "</strong><span class=\"verticalLine\"></span><strong>" + end2 + "</strong> - 5'</span>";
                }
                else
                {
                    //forward seq
                    string front1 = seq.Substring(0, forward_cut);

                    string end1 = seq.Substring(forward_cut, (reverse_cut - forward_cut));
                    string end12 = seq.Substring(reverse_cut, (totalLengen - reverse_cut));

                    prototype = "<span class=\"seqFont\">5' - <strong>" + front1 + "</strong><span class=\"verticalLine\"></span><span class=\"horizontalLine\"><strong>" + end1+ "</strong></span><strong>" + end12 + "</strong> - 3'</span>";

                    //reverse seq
                    string cSeq = FindSeq.cDNA(seq);
                    string front2 = cSeq.Substring(0, reverse_cut);
                    string end2 = cSeq.Substring(reverse_cut, (totalLengen - reverse_cut));
                    prototype = prototype + "<br/>";
                    prototype = prototype + "<span class=\"seqFont\">3' - <strong>" + front2 + "</strong><span class=\"verticalLine\"></span><strong>" + end2 + "</strong> - 5'</span>";
                }
                
            }

            return prototype;
        }
    }
}