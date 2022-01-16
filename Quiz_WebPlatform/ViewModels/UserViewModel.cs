using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Quiz_WebPlatform.ViewModels
{
    public class UserViewModel
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        [DataType(DataType.Text)]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string UserPassword { get; set; }
    }
}