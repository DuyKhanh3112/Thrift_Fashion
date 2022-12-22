using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Thrift_Fashion.Models.VirtualModel
{
    public class MailHelperModel
    {
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Email { get; set; }
        [AllowHtml]
        public string Body { get; set; }
       
    }
}