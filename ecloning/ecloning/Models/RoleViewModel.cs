using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class RoleViewModel
    {
        public string userName { get; set; }
        public string userId { get; set; }
        public string Email { get; set; }
        public bool isAssistant { get; set; }
        public bool? isActive { get; set; }
        public List<string> Roles { get; set; }
    }
}