using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Thrift_Fashion.Models;

namespace Thrift_Fashion.Controllers
{
    public class CategoriesController : Controller
    {
        private ThriftFashionEntities db = new ThriftFashionEntities();

        // GET: Categories
        public ActionResult Index()
        {
            if (Session[Common.Constants.EMPLOYEE_SESSION] == null)
            {
                return Redirect("/Employees/EmployeeLogin");
            }
            else
            {
                return View(db.Categories.ToList());
            }

        }



        // GET: Categories/Create
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

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryID,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Categories/Edit/5
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
                Category category = db.Categories.Find(id);
                if (category == null)
                {
                    return HttpNotFound();
                }
                return View(category);
            }

        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryID,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                TempData["AlertMessage"] = "Category updated successfully!";
                return RedirectToAction("Index");
            }
            return View(category);
        }






        [ChildActionOnly]
        public PartialViewResult P_ListCate()
        {
            var listCate = db.Categories.ToList();
            return PartialView(listCate);
        }
    }
}
