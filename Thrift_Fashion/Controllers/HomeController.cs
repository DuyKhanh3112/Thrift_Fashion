using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Thrift_Fashion.Models;
using Thrift_Fashion.Models.VirtualModel;

namespace Thrift_Fashion.Controllers
{
    public class HomeController : Controller
    {
        private ThriftFashionEntities db = new ThriftFashionEntities();

        public ActionResult Index()
        {
            List<Product> products = db.Products.ToList();
            List<OrderDetail> orderDetails = db.OrderDetails.ToList();
            List<Product> productHot = new List<Product>();
            foreach (Product p in products)
            {
                if ((db.OrderDetails.Where(o => o.ProductID == p.ProductID && o.Order.Status == 2).Count()) > 0)
                {
                    productHot.Add(p);
                }
            }

            ViewBag.Hot= productHot.ToList();
            ViewBag.sale = db.Products.Where(p => p.Discount > 0).ToList();
            List<Product> productnew = new List<Product>();
           foreach(Product p in products)
            {
                if (p.CreatedDate >= DateTime.Now.AddDays(-7))
                {
                    productnew.Add(p);
                                        }
}
            return View(productnew);
        }
        public ActionResult Feedback()
        {
            return View();
        }
       [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Feedback(string des)
        {
            if (ModelState.IsValid)
            {
                if (Session[Common.Constants.CUSTOMER_SESSION] == null)
                {
                    return View();
                }
                else
                {
                    var cus = (UserSession)Session[Common.Constants.CUSTOMER_SESSION];
                    if (ModelState.IsValid)
                    {
                        Feedback feedback = new Feedback();
                        feedback.CreatedDate = DateTime.Now;
                        feedback.Description = des;
                        feedback.CustomerID = cus.ID;
                        feedback.ProductID = null;
                        db.Feedbacks.Add(feedback);
                        db.SaveChanges();
                        return RedirectToAction("/Index");

                    }
                    return RedirectToAction("/Feedback");
                }
            }
            return View();
           
          
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}