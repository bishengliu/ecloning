using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class EnzymeActivity
    {
        public string Convert(int num)
        {
            var text = "<span></span>";
            if(num == 0)
            {
                text = "<span><10%</span>";
            } else if(num == 1)
            {
                text = "<span>10%</span>";
            }
            else if (num == 2)
            {
                text = "<span>10-25%</span>";
            }
            else if (num == 3)
            {
                text = "<span>25%</span>";
            }
            else if (num == 4)
            {
                text = "<span>25-50%</span>";
            }
            else if (num == 5)
            {
                text = "<span>50%</span>";
            }
            else if (num == 6)
            {
                text = "<span>50-75%</span>";
            }
            else if (num == 7)
            {
                text = "<span>75%</span>";
            }
            else if (num == 9)
            {
                text = "<span>100%</span>";
            }


            return text;
        }
    }
}