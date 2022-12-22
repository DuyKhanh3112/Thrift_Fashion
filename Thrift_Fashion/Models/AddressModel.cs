using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Thrift_Fashion.Models
{
    public class AddressModel
    {
        public int AddressID { get; set; }
        public string AddressDetails { get; set; }
        public AddressModel()
        {

        }
        public AddressModel(int AddressID, string AddressDetails)
        {
            this.AddressID = AddressID;
            this.AddressDetails = AddressDetails;
        }
    }
}