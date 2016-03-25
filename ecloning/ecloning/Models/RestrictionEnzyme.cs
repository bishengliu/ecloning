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
                symbol = "<span><i class=\"fa fa-square-o fa-stack-2x\"></i><i class=\"fa fa-star fa-stack-1x text-danger\"></i></span>";
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
                symbol = "<button type=\"button\" class=\"btn btn-success btn-circle disabled\">NO</button>";
            }
            return symbol;
        }
    }
}