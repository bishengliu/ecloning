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
            try{
                string email = db.AspNetUsers.Where(u => u.Id == userId).FirstOrDefault().Email;
                this.userEmail = email;
                var person = db.people.Where(e => e.email == email).FirstOrDefault();
                this.userName = person.first_name + " " + person.last_name;
                this.PersonId = person.id;
            }
            catch (Exception)
            {

            }        
        }
    }

    public class PeopleInfo
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }

        private ecloningEntities db = new ecloningEntities();

        public PeopleInfo(int people_id)
        {
            try
            {
                var person = db.people.Find(people_id);
                this.Email = person.email;
                this.Name = person.first_name + " " + person.last_name;
                this.Id = people_id;
            }
            catch (Exception)
            {

            }
        }
    }

    public class PeopleIdName
    {
        public int id { get; set; }
        public string Name { get; set; }
    }
}