using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Thrift_Fashion.Common;
using Thrift_Fashion.Models;
using Thrift_Fashion.Models.VirtualModel;

namespace Thrift_Fashion.Controllers
{
    public class EmployeesController : Controller
    {
        private ThriftFashionEntities db = new ThriftFashionEntities();

        // GET: Employees
        public ActionResult Index()
        {
            return View(db.Employees.ToList());
        }

        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeID,Username,Password,Fullname,Email,Phone")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Employees.Add(employee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeID,Username,Password,Fullname,Email,Phone")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
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


        public ActionResult EmployeeLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EmployeeLogin(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var employee = db.Employees.SingleOrDefault(x => x.Username == loginModel.username);
                if (employee == null)
                {
                    ModelState.AddModelError("", "No Exist Account...");
                    return View();
                }
                if (employee != null)
                {
                    if (employee.Password != Encryptor.MD5Hash(loginModel.password))
                    {
                        ModelState.AddModelError("", "Wrong Password...");
                        return View();
                    }
                    else if (employee.Password == Encryptor.MD5Hash(loginModel.password))
                    {
                        Session[Constants.CUSTOMER_SESSION] = null;

                        var userSession = new UserSession();
                        userSession.Username = employee.Username;
                        userSession.ID = employee.EmployeeID;
                        userSession.Name = employee.Fullname;
                        Session.Add(Constants.EMPLOYEE_SESSION, userSession);

                        if (loginModel.remember == true)
                        {
                            if (Request.Cookies[loginModel.username] != null)
                            {
                                Response.Cookies[loginModel.username].Expires = DateTime.Now.AddDays(-1);
                            }

                            HttpCookie rememberAccount = new HttpCookie(loginModel.username);
                            rememberAccount["UserName"] = loginModel.username;
                            rememberAccount["Password"] = loginModel.password;
                            rememberAccount.Expires = DateTime.Now.AddDays(30);
                            Response.Cookies.Add(rememberAccount);
                        }

                        return Redirect("/Brands/Index");
                    }
                }
            }
            return View();

        }

        public ActionResult Logout()
        {
            Session[Common.Constants.EMPLOYEE_SESSION] = null;
            return RedirectToAction("EmployeeLogin");
        }
    }
}
