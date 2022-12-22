using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Thrift_Fashion.Models.VirtualModel
{
    public class LoginModel
    {
        [Required(ErrorMessage = "This field is required")]
        public string username { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string password { get; set; }


        public bool remember { get; set; }
    }
}