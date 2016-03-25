using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace ecloning.Models
{
    public static class FindSeq
    {
        //reverse sequence
        public static string ReverseSeq (string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        //get complement DNA
        public static string cDNA(string s)
        {
            //s must be a DNA or RNA seq
            //A T G C for DNA
            //A U G C for RNA
            var cSeq = new StringBuilder();
            foreach(var c in s)
            {
                if(c.ToString() == "A")
                {
                    cSeq.Append("T");
                }
                if (c.ToString() == "T")
                {
                    cSeq.Append("A");
                }
                if (c.ToString() == "G")
                {
                    cSeq.Append("C");
                }
                if (c.ToString() == "C")
                {
                    cSeq.Append("G");
                }
                if(c.ToString() == "N")
                {
                    cSeq.Append("N");
                }
            }

            return cSeq.ToString();
        }

        //get complement DNA
        public static string cRNA(string s)
        {
            //s must be a DNA or RNA seq
            //A T G C for DNA
            //A U G C for RNA
            var cSeq = new StringBuilder();
            foreach (var c in s)
            {
                if (c.ToString() == "A")
                {
                    cSeq.Append("U");
                }
                if (c.ToString() == "U")
                {
                    cSeq.Append("A");
                }
                if (c.ToString() == "G")
                {
                    cSeq.Append("C");
                }
                if (c.ToString() == "C")
                {
                    cSeq.Append("G");
                }
            }

            return cSeq.ToString();
        }


        //for all features that are not restriction cut
        public static List<int> NotRestriction(string fullSeq, string subSeq)
        {
            //full seq can be forward and reverse
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += subSeq.Length)
            {
                index = fullSeq.IndexOf(subSeq, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        //for all restriciton cut for both forward and reverse
        public static IDictionary<int, bool> Restriction(string fullSeq, string subSeq)
        {
            //full seq can be forward and reverse

            IDictionary<int, bool> dict = new Dictionary<int, bool>();
            bool blockage = false;
            for (int index = 0; ; index += subSeq.Length)
            {               
                index = fullSeq.IndexOf(subSeq, index);
                if(index != -1)
                {
                    //check possible cut blockage: CpG methylation, Dam and Dcm
                    //need to understand how blockage happens
                    //need to check both forward and reverse sequecen for cut blockage
                    blockage = true;
                    dict.Add(index, blockage);
                }
                else
                {
                    break;
                }                 
            }
            return dict;
        }

    }
}