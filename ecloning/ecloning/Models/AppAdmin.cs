using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class AppAdmin
    {
        public string first_Name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string code { get; set; }
        public string appName { get; set; }
       

        public AppAdmin(string env)
        {
            if (env == "Developement")
            {
                this.first_Name = "Admin";
                this.last_name = "ePlasmid";
                this.email = "bishengliu@gmail.com";
                this.code = eCloningSettings.iCode;
                this.appName = eCloningSettings.appName;
            }
            else
            {
                this.first_Name = eCloningSettings.iFirstName;
                this.last_name = eCloningSettings.iLastName;
                this.email = eCloningSettings.iEmail;
                this.code = eCloningSettings.iCode;
                this.appName = eCloningSettings.appName;
            }
        }

    }
}