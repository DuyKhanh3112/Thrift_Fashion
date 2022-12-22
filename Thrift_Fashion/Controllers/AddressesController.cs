using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Thrift_Fashion.Common;
using Thrift_Fashion.Models;
using Thrift_Fashion.Models.VirtualModel;

namespace Thrift_Fashion.Controllers
{
    public class AddressesController : Controller
    {
        private ThriftFashionEntities db = new ThriftFashionEntities();

        // GET: Addresses
        public ActionResult Index()
        {
            var addresses = db.Addresses.Include(a => a.Customer);
            return View(addresses.ToList());
        }                                  

        public ActionResult Address()
        {
            var session = (UserSession)Session[Constants.CUSTOMER_SESSION];
            if (session == null)
            {

                return Redirect("/Customers/CustomerLogin");
            }
            else
            {
                var customerSession = (Models.VirtualModel.UserSession)Session[Common.Constants.CUSTOMER_SESSION];
                var address = db.Addresses.Where(x => x.CustomerID == customerSession.ID).ToList();
                return View(address);
            }
        }

        public ActionResult CreateAddress()
        {
            var session = (UserSession)Session[Constants.CUSTOMER_SESSION];
            if (session == null)
            {

                return Redirect("/Customers/CustomerLogin");
            }
            else
            {

                return View();
            }
        }

        [HttpPost]
        public ActionResult CreateAddress(Address entity)
        {
            if (ModelState.IsValid)
            {
                var session = (UserSession)Session[Constants.CUSTOMER_SESSION];
                var address = new Address();
                address.CustomerID = session.ID;
                address.Province = entity.Province;
                address.District = entity.District;
                address.AddressDetail = entity.AddressDetail;
                if (db.Addresses.Where(x => x.CustomerID == session.ID).ToList().Count() == 0)
                {
                    address.Status = true;
                }
                else
                {
                    address.Status = false;
                }
                db.Addresses.Add(address);
                db.SaveChanges();
                return RedirectToAction("Address");
            }
            return View();
        }

        public JsonResult ChangeStatus(int id)
        {

            var address = db.Addresses.Find(id);
            address.Status = !address.Status;
            var oldAddress = db.Addresses.Where(x => x.CustomerID == address.CustomerID && x.Status == true).SingleOrDefault();
            oldAddress.Status = false;
            db.SaveChanges();
            return Json(new
            {
                Status = address.Status
            });
        }


        public JsonResult Delete(int id)
        {
            var address = db.Addresses.Find(id);
            db.Addresses.Remove(address);
            db.SaveChanges();
            return Json(new
            {
                status = true
            });

        }







    }
}
