using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class DeployConfig
    {
        //initilize enviroment
        public string env { get; set; }

        //initilize app location: local or cloud (azure)
        public string app_location { get; set; }

        //initilize institue Admin

        public string iEmail { get; set; }
        public string iFirstName { get; set; }
        public string iLastName { get; set; }


        public DeployConfig(string env, string location)
        {
            this.env = env;
            this.app_location = location; //only for ms azure cloud

            if (this.env != null && this.env == "Developement")
            {
                //insitute admin
                var app_admin = new AppAdmin();
                this.iEmail = app_admin.email;
                this.iFirstName = app_admin.first_Name;
                this.iLastName = app_admin.last_name;
            }
            else
            {
                //need to hard code an institute admin
                var app_admin = new AppAdmin();
                this.iEmail = app_admin.email;
                this.iFirstName = app_admin.first_Name;
                this.iLastName = app_admin.last_name;
            }
        }

    }
}