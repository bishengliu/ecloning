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


        //sendgrid
        public static string SendgridLoginName()
        {
            return "azure_a0bed7402d312ae0c71db9d57a71c67c@azure.com";
        }
        public static string SendgridPsw()
        {
            return "boL5MRQUCtbM1K8";
        }

        //institute Admin
        public static string iEmail()
        {
            return "bishengliu@gmail.com";
        }
        public static string iFirstName()
        {
            return "Bisheng";
        }
        public static string iLastName()
        {
            return "Liu";
        }
        public static string iCode()
        {
            return "3ZxaOlh6823UTz2pUP7ExHi9qy53uf65";
        }
    }
}