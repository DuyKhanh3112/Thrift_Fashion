using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Thrift_Fashion.Models;
using Thrift_Fashion.Models.VirtualModel;

namespace Thrift_Fashion.Controllers
{
    public class WishlistsController : Controller
    {
        private ThriftFashionEntities db = new ThriftFashionEntities();
        //Duy Khanh
        // GET: Wishlists
        public ActionResult Index()
        {
            var cus = (UserSession)Session[Common.Constants.CUSTOMER_SESSION];
            if (Session[Common.Constants.CUSTOMER_SESSION] == null)
            {
                return Redirect("/Customers/CustomerLogin");
            }
            List<Wishlist> wishlists = db.Wishlists.Where(wl => wl.CustomerID == cus.ID && wl.Product.Status==true).ToList();
            return View(wishlists.ToList());
        }
        public ActionResult RemoveWishList(int id)
        {
            var cus = (UserSession)Session[Common.Constants.CUSTOMER_SESSION];
            if (Session[Common.Constants.CUSTOMER_SESSION] == null)
            {
                return Redirect("/Customers/CustomerLogin");
            }
            Wishlist wishlist = db.Wishlists.Find(id);
            db.Wishlists.Remove(wishlist);
            db.SaveChanges();
            List<Wishlist> wishlists = db.Wishlists.Where(wl => wl.CustomerID == cus.ID && wl.Product.Status==true).ToList();
            return View("Index", wishlists.ToList());
        }
    }
}
