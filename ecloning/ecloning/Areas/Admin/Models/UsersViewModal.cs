using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Areas.Admin.Models
{
    public class UsersViewModal
    {
        public List<Administrator> Administrators { get; set; }
        public List<Department> Departments { get; set; }
    }

    public class Administrator
    {
        public int peopleId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Des { get; set; }
        public List<Group> Groups { get; set; }
    }
    public class Group
    {
        public int groupId { get; set; }
        public int departId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Des { get; set; }
        public List<People> People { get; set; }
    }
    public class People
    {
        public int peopleId { get; set; }
        public int departId { get; set; }
        public int groupId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool? Active { get; set; }
    }
}