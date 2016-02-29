using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class InstituteAdmin
    {
        public string iEmail { get; set; }
        public string iFirstName { get; set; }
        public string iLastName { get; set; }
        public string iCode { get; set; }

        public InstituteAdmin(string env, string location)
        {

            if (env != null && env == "Developement")
            {
                //insitute admin
                var app_admin = new AppAdmin();
                this.iEmail = app_admin.email;
                this.iFirstName = app_admin.first_Name;
                this.iLastName = app_admin.last_name;
                this.iCode = app_admin.code;
            }
            else
            {
                //need to hard code an institute admin in eCloningSettings
                this.iEmail = eCloningSettings.iEmail;
                this.iFirstName = eCloningSettings.iFirstName;
                this.iLastName = eCloningSettings.iLastName;
            }
        }

    }
}