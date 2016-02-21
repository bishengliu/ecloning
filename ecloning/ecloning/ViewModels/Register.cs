using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ecloning.ViewModels
{
    public class Register
    {
        [Display(Name = "Department")]
        [Required(ErrorMessage = "Required")]
        public string department { get; set; }

        [Display(Name = "Group")]
        [Required(ErrorMessage = "Required")]
        public string group { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Required")]
        public string first_name { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Required")]
        public string last_name { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string email { get; set; }

        [Display(Name = "Invitation Code")]
        [Required(ErrorMessage = "Required")]
        public string code { get; set; }
    }
}