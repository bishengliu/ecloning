using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace ecloning.Models
{
    public class UserInfo
    {
        public string userEmail{ get; set; }
        public string userName { get; set; }
        public int PersonId { get; set; }

        private ecloningEntities db = new ecloningEntities();

        public UserInfo(string userId)
        {
            string email = db.AspNetUsers.Where(u => u.Id == userId).FirstOrDefault().Email;
            this.userEmail = email;
            var person = db.people.Where(e => e.email == email).FirstOrDefault();
            this.userName = person.first_name + " " + person.last_name;
            this.PersonId = person.id;
        }
    }
}