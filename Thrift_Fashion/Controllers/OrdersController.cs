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
    public class OrdersController : Controller
    {
        private ThriftFashionEntities db = new ThriftFashionEntities();

        // GET: Orders
        public ActionResult Index()
        {
            if (Session[Common.Constants.EMPLOYEE_SESSION] == null)
            {
                return Redirect("/Employees/EmployeeLogin");
            }
            else
            {
  var orders = db.Orders.Include(o => o.Address).Include(o => o.Customer);
            return View(orders.ToList());
            }
          
        }

        // GET: Orders/Details/5
        public ActionResult Details(int id)
        {
            if (Session[Common.Constants.EMPLOYEE_SESSION] == null)
            {
                return Redirect("/Employees/EmployeeLogin");
            }
            else
            {
  List<OrderDetail> listorderdetail = new List<OrderDetail>();
            foreach (OrderDetail orderDetail in db.OrderDetails)
            {
                if (orderDetail.OrderID == id)
                {
                    OrderDetail ordDe = new OrderDetail();
                    ordDe.ProductID = orderDetail.ProductID;
                    ordDe.Quantity = orderDetail.Quantity;
                    ordDe.Price = orderDetail.Price;
                    listorderdetail.Add(ordDe);
                }
            }
            ViewBag.listorderdetail = listorderdetail;

            return View(db.Orders.Find(id));
            }
          
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.AddressID = new SelectList(db.Addresses, "AddressID", "Province");
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Username");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,CustomerID,AddressID,OrderDate,ShipDate,Total,Note,Status")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AddressID = new SelectList(db.Addresses, "AddressID", "Province", order.AddressID);
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Username", order.CustomerID);
            return View(order);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session[Common.Constants.EMPLOYEE_SESSION] == null)
            {
                return Redirect("/Employees/EmployeeLogin");

            }
            else
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Order order = db.Orders.Find(id);
                if (order == null)
                {
                    return HttpNotFound();
                }
                ViewBag.AddressID = new SelectList(db.Addresses, "AddressID", "Province", order.AddressID);
                ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Username", order.CustomerID);
                return View(order);
            }
           
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderID,CustomerID,AddressID,OrderDate,ShipDate,Total,Note,Status")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                TempData["AlertMessage"] = "Order updated successfully!";
                return RedirectToAction("Index");
            }
            ViewBag.AddressID = new SelectList(db.Addresses, "AddressID", "Province", order.AddressID);
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Username", order.CustomerID);
            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
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
        //Duy Khanh
        public ActionResult ShowAllOrder()
        {
            if (Session[Common.Constants.CUSTOMER_SESSION] == null)
            {
                return Redirect("/Customers/CustomerLogin");
            }
            var cus = (UserSession)Session[Common.Constants.CUSTOMER_SESSION];
            List<Order> orders = db.Orders.Where(o => o.CustomerID == cus.ID).ToList();
            return View(orders.ToList());
        }
        //huy don hang
        public ActionResult RemoveOrder(int id)
        {
            var cus = (UserSession)Session[Common.Constants.CUSTOMER_SESSION];
            Order order = db.Orders.Find(id);
            order.Status = -1;
            List<OrderDetail> orderDetails = db.OrderDetails.Where(odd => odd.OrderID == id).ToList();
            foreach (OrderDetail odd in orderDetails)
            {
                Product product = db.Products.Find(odd.ProductID);
                product.Quantity += odd.Quantity;
            }
            db.SaveChanges();
            List<Order> orders = db.Orders.Where(o => o.CustomerID ==cus.ID).ToList();
            return View("ShowAllOrder", orders.ToList());
        }
    }
}
