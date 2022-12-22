using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Thrift_Fashion.Models.VirtualModel
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Email is not valid.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string Code { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string NewPassword { get; set; }

        [Compare(nameof(NewPassword),ErrorMessage ="Wrong confirm password...")]
        [Required(ErrorMessage = "This field is required")]
        public string ConfirmPassword { get; set; }
    }
}