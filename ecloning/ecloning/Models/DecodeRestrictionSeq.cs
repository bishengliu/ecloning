using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class DecodeRestrictionSeq
    {
        public string seq { get; set; }
        public static List<string> letters = new List<string>() { "R", "K", "B" , "Y", "S", "D", "W", "H" , "N" , "M", "V" };

        public List<string> Decode(string restricitonSeq)
        {
            List<string> decodedSeq = new List<string>();

            //first convert seq string into a list and then evaluate each letter and convert each letter to a sub list
            List<List<string>> finalList = new List<List<string>>();
            List<string> seqList = restricitonSeq.Select(s => s.ToString()).ToList();
            foreach(var c in seqList)
            {
                List<string> codes = ToCodeLetter(c);               
                finalList.Add(codes);
            }
           
            //convert the finalLIst to seq list
            foreach (var o in finalList)
            {
                if (decodedSeq.Count() > 0)
                {
                    //tempLIst
                    List<string> L1 = new List<string>();
                    foreach (var i in o)
                    {
                        foreach(var f in decodedSeq)
                        {
                            L1.Add(f + i);
                        }
                    }
                    //empty decodedSeq and copy L1 to decodedSeq
                    decodedSeq.Clear();
                    foreach(var n  in L1)
                    {
                        decodedSeq.Add(n);
                    }                                        
                }
                else
                {
                    foreach (var i in o)
                    {
                        decodedSeq.Add(i);
                    }
                }
            }            
            return decodedSeq;
        }

        public  static List<string> ToCodeLetter (string l)
        {
            List<string> codes = new List<string>();
            if (letters.Contains(l.ToUpper()))
            {
                switch (l)
                {
                    case "R":
                        codes.Add("G"); 
                        codes.Add("A");
                        break;
                    case "K":
                        codes.Add("G");
                        codes.Add("T");
                        break;
                    case "B":
                        codes.Add("C");
                        codes.Add("G");
                        codes.Add("T");
                        break;
                    case "Y":
                        codes.Add("C");
                        codes.Add("T");
                        break;
                    case "S":
                        codes.Add("C");
                        codes.Add("G");
                        break;
                    case "D":
                        codes.Add("A");
                        codes.Add("G");
                        codes.Add("T");
                        break;
                    case "W":
                        codes.Add("A");
                        codes.Add("T");
                        break;
                    case "H":
                        codes.Add("A");
                        codes.Add("C");
                        codes.Add("T");
                        break;
                    case "M":
                        codes.Add("A");
                        codes.Add("C");
                        break;
                    case "V":
                        codes.Add("A");
                        codes.Add("C");
                        codes.Add("G");
                        break;
                }
            }
            else
            {
                codes.Add(l.ToUpper());
            }
            return codes;
        }
    }
}