using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

        //for all features that are not restriction cut
        public static IEnumerable<int> NotRestriction(this string fullSeq, string subSeq)
        {
            //full seq can be forward and reverse
            for (int index = 0; ; index += subSeq.Length)
            {
                index = fullSeq.IndexOf(subSeq, index);
                if (index == -1)
                    break;
                yield return index;
            }
        }

        //for all restriciton cut for both forward and reverse
        public static IDictionary<int, bool> Restriction(this string fullSeq, string subSeq)
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