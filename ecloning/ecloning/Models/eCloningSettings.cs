using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    //add "AppAdmin", "Institute Admin" for both department and group
    //set the app_license


    public static class eCloningSettings
    {
        //set app hosting
        //public readonly static string AppHosting = "Cloud";
        //public readonly static string AppHosting = "Hybrid"; //store upload in local root path, but use send grid as smtp
        public readonly static string AppHosting = "Local";
        //set env
        public readonly static string AppEnv = "Developement";

        public readonly static string AppURI = "http://localhost:2974/";
        //set the institute name
        public readonly static string Institute = "eCloning";

        //sendgrid
        public readonly static string SendgridLoginName = "azure_a0bed7402d312ae0c71db9d57a71c67c@azure.com";
        public readonly static string SendgridPsw = "boL5MRQUCtbM1K8";

        //institute Admin
        public readonly static string iEmail = "bishengliu@gmail.com";
        public readonly static string iFirstName = "Bisheng";
        public readonly static string iLastName = "Liu";
        public readonly static string iCode = "3ZxaOlh6823UTz2pUP7ExHi9qy53uf65";


        //upload plasmid to azure directoryName
        public readonly static string plasmidDir = "plasmid";

        //upload bundle to azure dirctoryName
        public readonly static string bundleDir = "pbundle";

        //upload protocol to azure directoryName
        public readonly static string protocolDir = "protocol";

        //local data path
        public readonly static string filePath = "~/App_Data/";

        //set the min length of cut band
        //public readonly static int bLength = 100;

        //set the max num of restriction cuts to find
        public readonly static int cutNum = 10;

        //set how the restriction is found
        //true ==> find all restriction enzymes
        //false ==> find first saved faviroute enzymes in the group, if not then find all
        public readonly static bool enzymeScope = true;
    }
}