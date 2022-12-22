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

namespace Thrift_Fashion.Controllers
{
    public class BrandsController : Controller
    {
        private ThriftFashionEntities db = new ThriftFashionEntities();

        // GET: Brands
        public ActionResult Index()
        {
            if (Session[Common.Constants.EMPLOYEE_SESSION] == null)
            {
                return Redirect("/Employees/EmployeeLogin");
            }
            else
            {
                return View(db.Brands.ToList());
            }


        }




        // GET: Brands/Create
        public ActionResult Create()
        {
            if (Session[Common.Constants.EMPLOYEE_SESSION] == null)
            {
                return Redirect("/Employees/EmployeeLogin");
            }
            else
            {
                return View();
            }

        }

        // POST: Brands/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BrandID,Name,Logo")] Brand brand)
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
                    _FileName = "logo" + brand.BrandID + tail;
                    _path = Path.Combine(Server.MapPath("~/Image/Upload/Logo"), _FileName);
                    logo.SaveAs(_path);
                    brand.Logo = _FileName;
                }
                else
                {
                    brand.Logo = "defaultimg.png";
                }
                db.Brands.Add(brand);
                db.SaveChanges();
                TempData["AlertMessage"] = "Brand created successfully!";
                return RedirectToAction("Index");
            }

            return View(brand);
        }

        // GET: Brands/Edit/5
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
                Brand brand = db.Brands.Find(id);
                if (brand == null)
                {
                    return HttpNotFound();
                }
                return View(brand);
            }

        }

        // POST: Brands/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BrandID,Name,Logo")] Brand brand)
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
                    _FileName = "logo" + brand.BrandID + tail;
                    _path = Path.Combine(Server.MapPath("~/Image/Upload/Logo"), _FileName);
                    logo.SaveAs(_path);
                    brand.Logo = _FileName;
                }
                db.Entry(brand).State = EntityState.Modified;
                db.SaveChanges();
                TempData["AlertMessage"] = "Brand updated successfully!";
                return RedirectToAction("Index");
            }
            return View(brand);
        }

        [ChildActionOnly]
        public PartialViewResult P_ListBrand()
        {
            var listBrand = db.Brands.ToList();
            return PartialView(listBrand);
        }
    }
}
