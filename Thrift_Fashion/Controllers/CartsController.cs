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
    public class CartsController : Controller
    {
        private ThriftFashionEntities db = new ThriftFashionEntities();

        // GET: Carts
        // GET: Carts
        public ActionResult Index()
        {
            if (Session[Common.Constants.CUSTOMER_SESSION] == null)
            {
                return Redirect("/Customers/CustomerLogin");
            }
            var cus= (UserSession)Session[Common.Constants.CUSTOMER_SESSION];
            List<Cart> carts = db.Carts.Where(odd => odd.CustomerID == cus.ID && odd.Product.Status==true).ToList();
            return View(carts.ToList());
        }
        public ActionResult Checkout()
        {
            if (Session[Common.Constants.CUSTOMER_SESSION] == null)
            {
                return Redirect("/Customers/CustomerLogin");
            }
            AddressModel addressModel = new AddressModel();
            List<AddressModel> listAdddress = new List<AddressModel>();
            var cus = (UserSession)Session[Common.Constants.CUSTOMER_SESSION];
            List<Cart> carts = db.Carts.Where(odd => odd.CustomerID == cus.ID && odd.Product.Status==true).ToList();
            var address = db.Addresses.Where(ad => ad.CustomerID == cus.ID && ad.Status==true).SingleOrDefault();

            ViewBag.Address = address.AddressDetail + "--" + address.District + "--" + address.Province;
                
               
            
           
            return View(carts.ToList());
        }
        [HttpPost]
      
        public ActionResult Checkout([Bind(Include = "CartID,CustomerID,ProductID,Quantity,CreatedDate")] Cart cart)
        {
            var cus = (UserSession)Session[Common.Constants.CUSTOMER_SESSION];
            var countAdd = db.Addresses.Where(a => a.CustomerID == cus.ID).Count();
            if(countAdd== 0)
            {
                return Redirect("/Addresses/CreateAddress");
            }
            if (Session[Common.Constants.CUSTOMER_SESSION] == null)
            {
                return Redirect("/Customers/CustomerLogin");
            }
  
            Order order = new Order();
            decimal amount = 0;
            foreach (Cart c in db.Carts)
            {
                if (c.CustomerID == cus.ID)
                {
                    amount += c.Quantity * c.Product.Price * (100 - c.Product.Discount) / 100;
                }
            }
            foreach (Address a in db.Addresses.ToList())
            {
                if (a.CustomerID == cus.ID && a.Status == true)
                {
                    order.AddressID = a.AddressID;
                }
            }
            //save order
            order.CustomerID = cus.ID;
            order.OrderDate = DateTime.Now;
            order.ShipDate = DateTime.Now.AddDays(15);
            order.Total = amount;
            order.Status = 0;
            db.Orders.Add(order);
            // db.SaveChanges();
            //save orderdetails
            foreach (Cart c in db.Carts)
            {
                if (c.CustomerID == cus.ID)
                {
                    OrderDetail orderDetail = new OrderDetail();
                    orderDetail.OrderID = order.OrderID;
                    orderDetail.ProductID = c.ProductID;
                    orderDetail.Quantity = c.Quantity;
                    orderDetail.Price = c.Product.Price * (100 - c.Product.Discount) / 100;
                    db.OrderDetails.Add(orderDetail);

                }
            }
            foreach (Cart c in db.Carts)
            {
                if (c.CustomerID == cus.ID)
                {
                    Product product = db.Products.Find(c.ProductID);
                    product.Quantity -= c.Quantity;

                }
            }
            foreach (Cart c in db.Carts)
            {
                if (c.CustomerID == cus.ID)
                {
                    Cart ca = db.Carts.Find(c.CartID);
                    db.Carts.Remove(ca);
                }
            }
            db.SaveChanges();
            List<Cart> carts = db.Carts.Where(odd => odd.CustomerID == cus.ID).ToList();
            return View("Index", carts.ToList());
        }
        public ActionResult DeleteCart(int id)
        {
            var cus = (UserSession)Session[Common.Constants.CUSTOMER_SESSION];
            if (Session[Common.Constants.CUSTOMER_SESSION] == null)
            {
                return Redirect("/Customers/CustomerLogin");
            }
            Cart cart = db.Carts.Find(id);
            db.Carts.Remove(cart);
            db.SaveChanges();
            List<Cart> carts = db.Carts.Where(odd => odd.CustomerID == cus.ID && odd.Product.Status == true).ToList();
            return View("Index",carts.ToList());

        }
        public ActionResult plusCart(int id)
        { 
            if (Session[Common.Constants.CUSTOMER_SESSION] == null)
            {
                return Redirect("/Customers/CustomerLogin");
            }
            int count = 0; 
            Cart c = db.Carts.Find(id);
            List<OrderDetail> orderDetail = db.OrderDetails.ToList();
            Product p = db.Products.Find(c.ProductID);
            foreach (OrderDetail o in orderDetail) {
               if(c.ProductID== o.ProductID)
                {
                    count += o.Quantity;
                }
            }
            if ((p.Quantity - count) > c.Quantity)
            {
                c.Quantity++;
            }
            var cus = (UserSession)Session[Common.Constants.CUSTOMER_SESSION];

            db.SaveChanges();
            List<Cart> carts = db.Carts.Where(odd => odd.CustomerID == cus.ID && odd.Product.Status == true).ToList();
            return View("Index",carts.ToList());
        }
        public ActionResult reduceCart(int id)
        {
            if (Session[Common.Constants.CUSTOMER_SESSION] == null)
            {
                return Redirect("/Customers/CustomerLogin");
            }
            Cart c = db.Carts.Find(id);
            if (c.Quantity>1)
            {
                c.Quantity--;
            }
            else
            {
                db.Carts.Remove(c);
            }
            var cus = (UserSession)Session[Common.Constants.CUSTOMER_SESSION];
            db.SaveChanges();
            List<Cart> carts = db.Carts.Where(odd => odd.CustomerID == cus.ID && odd.Product.Status == true).ToList();
            return View("Index", carts.ToList());
        }
        // GET: Carts/Details/5
        public ActionResult Details(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        // GET: Carts/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Username");
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "Name");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CartID,CustomerID,ProductID,Quantity,CreatedDate")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                db.Carts.Add(cart);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Username", cart.CustomerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "Name", cart.ProductID);
            return View(cart);
        }

        // GET: Carts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Username", cart.CustomerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "Name", cart.ProductID);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CartID,CustomerID,ProductID,Quantity,CreatedDate")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cart).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Username", cart.CustomerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "Name", cart.ProductID);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cart cart = db.Carts.Find(id);
            db.Carts.Remove(cart);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [ChildActionOnly]
        public PartialViewResult P_Cart()
        {
            if (Session[Common.Constants.CUSTOMER_SESSION] == null)
            {
                List<Cart> carts = null;
                ViewBag.TotalPrice = 0;
                ViewBag.Qty = 0;
                return PartialView(carts);
            }
            else
            {
                var cus = (UserSession)Session[Common.Constants.CUSTOMER_SESSION];
                List<Cart> carts = db.Carts.Where(odd => odd.CustomerID == cus.ID && odd.Product.Status == true).ToList();
                decimal totalprice = 0;
                int qty = 0;
                foreach (var item in carts)
                {
                    qty += item.Quantity;
                    totalprice += item.Product.Price*item.Quantity;
                }
                ViewBag.Qty = qty;
                ViewBag.TotalPrice = totalprice.ToString("N0");
                return PartialView(carts);
            }
           
        }
    }
}
