using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Thrift_Fashion.Models.VirtualModel
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "This field is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string Password { get; set; }

        [Compare(nameof(Password),ErrorMessage ="Wrong confirm password...")]
        [Required(ErrorMessage = "This field is required")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Email is not valid.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"(0[1-9])+([0-9]{8})\b", ErrorMessage = "Phone number is not valid. The phone number required 10 character")]
        public string Phone { get; set; }
    }
}