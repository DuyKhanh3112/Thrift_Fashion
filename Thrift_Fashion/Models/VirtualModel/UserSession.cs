using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Thrift_Fashion.Models.VirtualModel
{
    public class UserSession
    {
        public string Username { get; set; }

        public int ID { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }
    }
}