using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Thrift_Fashion.Models;
using Thrift_Fashion.Models.VirtualModel;

namespace Thrift_Fashion.Controllers
{
    public class ProductsController : Controller
    {
        private ThriftFashionEntities db = new ThriftFashionEntities();

        // GET: Products
        public ActionResult Index()
        {
            if (Session[Common.Constants.EMPLOYEE_SESSION] == null)
            {
                return Redirect("/Employees/EmployeeLogin");
            }
            else
            {
                var products = db.Products.Include(p => p.Brand).Include(p => p.Category);
                return View(products.ToList());
            }

        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
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
                Product product = db.Products.Find(id);
                if (product == null)
                {
                    return HttpNotFound();
                }
                return View(product);
            }

        }

        // GET: Products/Create
        public ActionResult Create()
        {
            if (Session[Common.Constants.EMPLOYEE_SESSION] == null)
            {
                return Redirect("/Employees/EmployeeLogin");
            }
            else
            {
                ViewBag.BrandID = new SelectList(db.Brands, "BrandID", "Name");
                ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name");
                return View();
            }

        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,BrandID,CategoryID,Name,Description,Price,Quantity,Discount,Image")] Product product, IEnumerable<HttpPostedFileBase> myFiles)
        {
            string _FileName = "";
            string _path = "";
            if (ModelState.IsValid)
            {
                HttpPostedFileBase logo = Request.Files["Logo"];
                if (logo.FileName != "")
                {

                    string name = logo.FileName;
                    string tail = name.Substring(name.IndexOf("."));
                    _FileName = "Image" + product.ProductID + tail;
                    _path = Path.Combine(Server.MapPath("~/Image/Upload/Product"), _FileName);
                    logo.SaveAs(_path);
                    product.Image = _FileName;
                }
                else
                {
                    product.Image = "defaultimg.png";
                }
                product.Status = true;
                product.CreatedDate = DateTime.Now;
                db.Products.Add(product);
                db.SaveChanges();
                int dem = 0;
                foreach (var file in myFiles)
                {

                    if (file != null && file.ContentLength > 0)
                    {
                        dem++;
                        string name = file.FileName;
                        string tail = name.Substring(name.IndexOf("."));
                        _FileName = "Image" + product.ProductID + "_" + dem + tail;
                        string _pathimg = Path.Combine(Server.MapPath("~/Image/Upload/Product"), _FileName);
                        file.SaveAs(_pathimg);
                        Image image = new Image();
                        image.ProductID = product.ProductID;
                        image.Name = _FileName;
                        db.Images.Add(image);
                        db.SaveChanges();
                        TempData["AlertMessage"] = "Product created successfully!";
                    }
                }
                return RedirectToAction("Index");
            }

            ViewBag.BrandID = new SelectList(db.Brands, "BrandID", "Name", product.BrandID);
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name", product.CategoryID);
            return View(product);
        }

        // GET: Products/Edit/5
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
                Product product = db.Products.Find(id);
                if (product == null)
                {
                    return HttpNotFound();
                }
                ViewBag.BrandID = new SelectList(db.Brands, "BrandID", "Name", product.BrandID);
                ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name", product.CategoryID);
                return View(product);
            }

        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,BrandID,CategoryID,Name,Description,Price,Quantity,Discount,Image,CreatedDate,Status")] Product product)
        {
            string _FileName = "";
            string _path = "";
            if (ModelState.IsValid)
            {
                HttpPostedFileBase logo = Request.Files["Logo"];
                if (logo.FileName != "")
                {

                    string name = logo.FileName;
                    string tail = name.Substring(name.IndexOf("."));
                    _FileName = "Image" + product.ProductID + tail;
                    _path = Path.Combine(Server.MapPath("~/Image/Upload/Product"), _FileName);
                    logo.SaveAs(_path);
                    product.Image = _FileName;
                }
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                TempData["AlertMessage"] = "Product updated successfully!";
                return RedirectToAction("Index");
            }
            ViewBag.BrandID = new SelectList(db.Brands, "BrandID", "Name", product.BrandID);
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name", product.CategoryID);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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

        //Show all product
        //public ActionResult ShowAllProduct()
        //{
        //    var products = db.Products.Where(p=>p.Status==true).ToList();
        //    return View(products.ToList());
        //}
        //add wish list
        public ActionResult AddWishList(int id)
        {
            if (Session[Common.Constants.CUSTOMER_SESSION] == null)
            {
                return Redirect("/Customers/CustomerLogin");
            }
            var cus = (UserSession)Session[Common.Constants.CUSTOMER_SESSION];
            List<Wishlist> wishlist = db.Wishlists.Where(wl => wl.ProductID == id && wl.CustomerID == cus.ID && wl.Product.Status == true).ToList();
            if (wishlist.Count == 0)
            {
                Wishlist w = new Wishlist();
                w.CustomerID = cus.ID;
                w.ProductID = id;
                w.CreatedDate = DateTime.Now;
                db.Wishlists.Add(w);
                db.SaveChanges();
                TempData["AlertMessage"] = "Add wish list successfully!";
            }
            else
            {
                TempData["AlertMessage"] = "Add wish list not successfully!";
            }
            List<Wishlist> wishlists = db.Wishlists.Where(wl => wl.CustomerID == cus.ID && wl.Product.Status == true).ToList();
            return Redirect("/Wishlists/Index");
        }
        //public ActionResult AddWishListDetail(int id)
        //{
        //    if (Session[Common.Constants.CUSTOMER_SESSION] == null)
        //    {
        //        return Redirect("/Customers/CustomerLogin");
        //    }
        //    var cus = (UserSession)Session[Common.Constants.CUSTOMER_SESSION];
        //    List<Wishlist> wishlist = db.Wishlists.Where(wl => wl.ProductID == id && wl.CustomerID == cus.ID && wl.Product.Status == true).ToList();
        //    if (wishlist.Count == 0)
        //    {
        //        Wishlist w = new Wishlist();
        //        w.CustomerID = cus.ID;
        //        w.ProductID = id;
        //        w.CreatedDate = DateTime.Now;
        //        db.Wishlists.Add(w);
        //        db.SaveChanges();
        //        TempData["AlertMessage"] = "Add wish list successfully!";
        //    }
        //    else
        //    {
        //        TempData["AlertMessage"] = "Add wish list not successfully!";
        //    }
        //    List<Wishlist> wishlists = db.Wishlists.Where(wl => wl.CustomerID == cus.ID && wl.Product.Status == true).ToList();
        //    return View("/Wishlists/Index", wishlists.ToList());
        //}
        //Create cart
        public ActionResult CreateCart(int id)
        {
            if (Session[Common.Constants.CUSTOMER_SESSION] == null)
            {
                return Redirect("/Customers/CustomerLogin");
            }
            var cus = (UserSession)Session[Common.Constants.CUSTOMER_SESSION];
            List<Cart> carts = db.Carts.Where(cr => cr.ProductID == id && cr.CustomerID == cus.ID && cr.Product.Status == true).ToList();
            if (carts.Count == 0)
            {
                Cart c = new Cart();
                c.CustomerID = cus.ID;
                c.ProductID = id;
                c.Quantity = 1;
                c.CreatedDate = DateTime.Now;
                db.Carts.Add(c);
                db.SaveChanges();
            }
            else
            {
                foreach (Cart c in carts)
                {
                    c.Quantity++;
                    db.SaveChanges();
                }
            }

            return Redirect("/Carts/Index");
        }
        //public ActionResult CreateCartDetails(int id)
        //{
        //    if (Session[Common.Constants.CUSTOMER_SESSION] == null)
        //    {
        //        return Redirect("/Customers/CustomerLogin");
        //    }
        //    var cus = (UserSession)Session[Common.Constants.CUSTOMER_SESSION];
        //    List<Cart> carts = db.Carts.Where(cr => cr.ProductID == id && cr.CustomerID == cus.ID && cr.Product.Status == true).ToList();
        //    if (carts.Count == 0)
        //    {
        //        Cart c = new Cart();
        //        c.CustomerID = cus.ID;
        //        c.ProductID = id;
        //        c.Quantity = 1;
        //        c.CreatedDate = DateTime.Now;
        //        db.Carts.Add(c);
        //        db.SaveChanges();
        //    }
        //    else
        //    {
        //        foreach (Cart c in carts)
        //        {
        //            c.Quantity++;
        //            db.SaveChanges();
        //        }
        //    }
        //    var products = db.Products.Where(p => p.Status == true).ToList();
        //    return RedirectToAction("Product_detail/" + id);
        //}

        public ActionResult ShowbyCategory(int id)
        {
            var products = db.Products.Where(p => p.CategoryID == id).ToList();
            return View("ShowAllProduct", products.ToList());
        }
        //show by brand
        public ActionResult ShowbyBrand(int id)
        {
            var products = db.Products.Where(p => p.BrandID == id).ToList();
            return View("ShowAllProduct", products.ToList());
        }
        public ActionResult Product_detail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.image = db.Images.Where(img => img.ProductID == id).ToList();
            Product product = db.Products.Find(id);
            ViewBag.Feadback = db.Feedbacks.Where(fe => fe.ProductID == id).OrderByDescending(x => x.CreatedDate).ToList();
            ViewBag.category = db.Products.Where(p => p.CategoryID == product.CategoryID).ToList();
            ViewBag.Band = db.Products.Where(p => p.Brand.BrandID == product.BrandID).ToList();
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Product_detail(string des, int id)
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
                    feedback.ProductID = id;
                    db.Feedbacks.Add(feedback);
                    db.SaveChanges();
                    return RedirectToAction("Product_detail/" + feedback.ProductID);

                }
                return View();
            }

        }
        public string showname(int? id)
        {
            string nameuser = "";
            Customer customer = new Customer();
            customer = db.Customers.Find(id);
            nameuser = customer.Username;
            return nameuser;
        }

        public JsonResult ListName(string q)
        {
            var data = db.Products.Where(x => x.Name.Contains(q)).Select(x => x.Name).ToList();
            return Json(new
            {
                data = data,
                status = true
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                var model = db.Products.ToList();
                return View(model);

            }
            else
            {
                var model = db.Products.Where(x => x.Name.Contains(keyword)).ToList();
                return View(model);
            }

        }
        public ActionResult ShowAllProduct()
        {
            string price = Request["filterby"];
            TempData["key"] = price;
            List<Product> product = new List<Product>();
            product = db.Products.ToList();
            if (price == "1")
            {
                product = db.Products.Where(p => p.Price < 100).ToList();
            }
            else if (price == "2")
            {
                product = db.Products.Where(p => p.Price > 99 && p.Price < 200).ToList();
            }
            else if (price == "3")
            {
                product = db.Products.Where(p => p.Price > 199 && p.Price < 299).ToList();
            }
            else if (price == "4")
            {
                product = db.Products.Where(p => p.Price > 299 && p.Price < 400).ToList();
            }
            else if (price == "5")
            {
                product = db.Products.Where(p => p.Price > 400).ToList();
            }
            else if (price == "7")
            {
                product = (from p in db.Products
                           orderby (p.Price * (100 - p.Discount) / 100) ascending
                           select p).ToList();
            }
            else if (price == "8")
            {
                product = (from p in db.Products
                           orderby p.Price*(100-p.Discount)/100 descending
                           select p).ToList();
            }
            else if (price == "9")
            {                
                product = (from p in db.Products
                           orderby p.CreatedDate ascending
                           select p).ToList();
            }
            else if (price == "10")
            {
                product = (from p in db.Products
                           orderby p.CreatedDate descending
                           select p).ToList();
            }
            return View(product);
        }



    }
}
