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
    public class ImagesController : Controller
    {
        private ThriftFashionEntities db = new ThriftFashionEntities();

        // GET: Images
        public ActionResult Index()
        {
            if (Session[Common.Constants.EMPLOYEE_SESSION] == null)
            {
                return Redirect("/Employees/EmployeeLogin");
            }
            else
            {
                var images = db.Images.Include(i => i.Product);
                return View(images.ToList());
            }

        }







        // GET: Images/Edit/5
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
                Image image = db.Images.Find(id);
                if (image == null)
                {
                    return HttpNotFound();
                }
                ViewBag.ProductID = new SelectList(db.Products, "ProductID", "Name", image.ProductID);
                return View(image);
            }

        }

        // POST: Images/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ImageID,ProductID,Name")] Image image)
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
                    string namefile = name.Substring(0, name.IndexOf("."));
                    _FileName = namefile + tail;
                    _path = Path.Combine(Server.MapPath("~/Image/Upload/Product"), _FileName);
                    logo.SaveAs(_path);
                    image.Name = _FileName;
                }
                db.Entry(image).State = EntityState.Modified;
                db.SaveChanges();
                TempData["AlertMessage"] = "Image updated successfully!";
                return Redirect("/Images/Index/" + image.ProductID);
            }
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "Name", image.ProductID);
            return View(image);
        }

        // GET: Images/Delete/5
        public ActionResult Delete(int? id)
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
                Image image = db.Images.Find(id);
                if (image == null)
                {
                    return HttpNotFound();
                }
                return View(image);
            }

        }

        // POST: Images/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Image image = db.Images.Find(id);
            db.Images.Remove(image);
            db.SaveChanges();
            TempData["AlertMessage"] = "Image Deleted successfully!";
            return Redirect("/Images/Index/" + image.ProductID);
        }


    }
}
