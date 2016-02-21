using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public static class eCloningSettings
    {
        //set app hosting
        public static string AppHosting()
        {
            return "Cloud";
            //return "Local";
        }
        //set env
        public static string AppEnv()
        {
            return "Developement";
        }

        //set the institute name
        public static string Institute()
        {
            return "eCloning";
        }

        public static string SendgridLoginName()
        {
            return "azure_a0bed7402d312ae0c71db9d57a71c67c@azure.com";
        }
        public static string SendgridPsw()
        {
            return "boL5MRQUCtbM1K8";
        }
    }
}