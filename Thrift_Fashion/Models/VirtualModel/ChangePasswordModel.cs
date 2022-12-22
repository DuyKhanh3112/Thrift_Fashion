using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Thrift_Fashion.Models.VirtualModel
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "This field is required")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string NewPassword { get; set; }

        [Compare(nameof(NewPassword), ErrorMessage = "Wrong confirm password...")]
        [Required(ErrorMessage = "This field is required")]
        public string ConfirmPassword { get; set; }
    }
}