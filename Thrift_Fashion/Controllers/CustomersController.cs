using BotDetect.Web.Mvc;
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
    public class CustomersController : Controller
    {
        private ThriftFashionEntities db = new ThriftFashionEntities();

        // GET: Customers
        public ActionResult Index()
        {
            if (Session[Common.Constants.EMPLOYEE_SESSION] == null)
            {
                return Redirect("/Employees/EmployeeLogin");
            }
            else
            {
                return View(db.Customers.ToList());
            }

        }

        // GET: Customers/Details/5
        public ActionResult Details(int id)
        {
            if (Session[Common.Constants.EMPLOYEE_SESSION] == null)
            {
                return Redirect("/Employees/EmployeeLogin");
            }
            else
            {
                List<Address> listaddress = new List<Address>();
                foreach (Address address in db.Addresses)
                {
                    if (address.CustomerID == id)
                    {
                        Address addr = new Address();
                        addr.Province = address.Province;
                        addr.District = address.District;
                        addr.AddressDetail = address.AddressDetail;
                        addr.Status = address.Status;
                        listaddress.Add(addr);
                    }
                }
                ViewBag.listaddress = listaddress;

                List<Order> listorder = new List<Order>();
                foreach (Order order in db.Orders)
                {
                    if (order.CustomerID == id)
                    {
                        Order ord = new Order();
                        ord.OrderID = order.OrderID;
                        ord.OrderDate = order.OrderDate;
                        ord.ShipDate = order.ShipDate;
                        ord.Total = order.Total;
                        ord.Note = order.Note;
                        ord.Status = order.Status;
                        listorder.Add(ord);
                    }
                }
                ViewBag.listorder = listorder;

                return View(db.Customers.Find(id));
            }

        }





        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CustomerID,Username,Password,Fullname,Email,Phone,Status")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }







        public ActionResult CustomerLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CustomerLogin(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var customer = db.Customers.SingleOrDefault(x => x.Username == loginModel.username);
                if (customer == null)
                {
                    ModelState.AddModelError("", "No Exist Account...");
                    return View();
                }
                if (customer != null)
                {
                    if (customer.Password != Encryptor.MD5Hash(loginModel.password))
                    {
                        ModelState.AddModelError("", "Wrong Password...");
                        return View();
                    }
                    else if (customer.Password == Encryptor.MD5Hash(loginModel.password))
                    {
                        Session[Constants.CUSTOMER_SESSION] = null;

                        var userSession = new UserSession();
                        userSession.Username = customer.Username;
                        userSession.ID = customer.CustomerID;
                        userSession.Name = customer.Fullname;
                        Session.Add(Constants.CUSTOMER_SESSION, userSession);

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

                        return Redirect("/");
                    }
                }
            }
            return View();

        }

        public ActionResult CustomerLogout()
        {
            Session[Constants.CUSTOMER_SESSION] = null;
            return Redirect("/");
        }

        public ActionResult CustomerRegister()
        {
            return View();
        }

        [HttpPost]
        [CaptchaValidationActionFilter("CaptchaCode", "registerCaptcha", "Wrong Confirmation Code !")]
        public ActionResult CustomerRegister(RegisterModel customer)
        {
            if (ModelState.IsValid)
            {
                if (db.Customers.Count(x => x.Username == customer.Username) > 0)
                {
                    ModelState.AddModelError("", "Existed UserName...");
                    return View();
                }

                if (db.Customers.Count(x => x.Email == customer.Email) > 0)
                {
                    ModelState.AddModelError("", "Existed Email...");
                    return View();
                }

                var model = new Customer();
                model.Username = customer.Username;
                model.Password = Encryptor.MD5Hash(customer.Password);
                model.Fullname = customer.FullName;
                model.Email = customer.Email;
                model.Phone = customer.Phone;
                model.Status = true;
                db.Customers.Add(model);
                var result = db.SaveChanges();
                if (result > 0)
                {

                    string content = System.IO.File.ReadAllText(Server.MapPath("~/Assets/Welcome.html"));
                    content = content.Replace("{{FullName}}", customer.FullName);
                    content = content.Replace("{{UserName}}", customer.Username);
                    new MailHelper().SendMail(customer.Email, "Hi. Welcome to Thrift Fashion", content);

                    Session[Constants.CUSTOMER_SESSION] = null;
                    var entity = db.Customers.Find(model.CustomerID);
                    var userSession = new UserSession();
                    userSession.Username = entity.Username;
                    userSession.ID = entity.CustomerID;
                    userSession.Name = entity.Fullname;
                    Session.Add(Constants.CUSTOMER_SESSION, userSession);
                }
                return Redirect("/");
            }
            return View();
        }

        public JsonResult FillPassword(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {

                HttpCookie fillPassword = Request.Cookies[userName];
                if (fillPassword != null)
                {
                    var data = fillPassword["Password"].ToString();
                    return Json(new
                    {
                        Status = true,
                        Data = data
                    }, JsonRequestBehavior.AllowGet);

                }
            }
            return Json(new
            {
                Status = false
            });

        }

        public JsonResult ForgotPassword(string email)
        {

            if (string.IsNullOrEmpty(email))
            {
                return Json(new
                {
                    Status = false
                });
            }
            if (db.Customers.Where(x => x.Email == email).Count() == 0)
            {
                TempData["msg"] = "<script>alert('Succesfully');</script>";
                return Json(new
                {
                    Status = false
                });
            }
            var customer = db.Customers.Where(x => x.Email == email).SingleOrDefault();
            var randomEntity = new Random();
            var codeForgotPassword = randomEntity.Next(1000000);

            if (Request.Cookies[email] != null)
            {
                Response.Cookies[email].Expires = DateTime.Now.AddDays(-1);
            }

            HttpCookie ckCode = new HttpCookie(email);
            ckCode["Email"] = email;
            ckCode["Code"] = codeForgotPassword.ToString();
            ckCode.Expires = DateTime.Now.AddMinutes(30);
            Response.Cookies.Add(ckCode);

            string content = System.IO.File.ReadAllText(Server.MapPath("~/Assets/ForgotPassword.html"));
            content = content.Replace("{{Name}}", customer.Fullname);
            content = content.Replace("{{Email}}", email);
            content = content.Replace("{{Code}}", codeForgotPassword.ToString());
            new MailHelper().SendMail(email, "Forgot Password", content);
            return Json(new
            {
                Status = true
            });
        }

        public ActionResult UpdatePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpdatePassword(ForgotPasswordModel entity)
        {
            if (ModelState.IsValid)
            {
                var customer = db.Customers.Where(x => x.Email == entity.Email).SingleOrDefault();
                var ckCode = Request.Cookies[entity.Email];
                if (customer.Email == ckCode["Email"])
                {
                    if (entity.Code == ckCode["Code"].ToString())
                    {
                        customer.Password = Encryptor.MD5Hash(entity.NewPassword);
                        db.SaveChanges();
                        Session[Constants.CUSTOMER_SESSION] = null;

                        var userSession = new UserSession();
                        userSession.Username = customer.Username;
                        userSession.ID = customer.CustomerID;
                        userSession.Name = customer.Fullname;
                        Session.Add(Constants.CUSTOMER_SESSION, userSession);

                        return Redirect("/");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Wrong code...");
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Wrong email...");
                    return View();
                }


            }
            return View();
        }
        public ActionResult ChangePassword()
        {
            var session = (UserSession)Session[Constants.CUSTOMER_SESSION];
            if (session == null)
            {

                return RedirectToAction("CustomerLogin");
            }
            else
            {
                return View();
            }

        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel entity)
        {
            if (ModelState.IsValid)
            {
                var customerSession = (Models.VirtualModel.UserSession)Session[Common.Constants.CUSTOMER_SESSION];
                var customer = db.Customers.Find(customerSession.ID);
                if (customer.Password == Encryptor.MD5Hash(entity.OldPassword))
                {
                    customer.Password = Encryptor.MD5Hash(entity.NewPassword);
                    db.SaveChanges();
                    return View();
                }
                else
                {
                    ModelState.AddModelError("", "Wrong password...");
                    return View();
                }
            }

            return View();
        }

        new public ActionResult Profile()
        {
            var session = (UserSession)Session[Constants.CUSTOMER_SESSION];
            if (session == null)
            {

                return RedirectToAction("CustomerLogin");
            }
            else
            {
                var customerSession = (Models.VirtualModel.UserSession)Session[Common.Constants.CUSTOMER_SESSION];
                var customer = db.Customers.Find(customerSession.ID);
                return View(customer);
            }
        }

        [HttpPost]
        new public ActionResult Profile(Customer entity)
        {
            if (ModelState.IsValid)
            {

                var customerSession = (Models.VirtualModel.UserSession)Session[Common.Constants.CUSTOMER_SESSION];
                var customer = db.Customers.Find(customerSession.ID);
                if (customer.Username != entity.Username)
                {
                    if (db.Customers.Count(x => x.Username == entity.Username) > 0)
                    {
                        ModelState.AddModelError("", "UserName cannot be duplicated...");
                        return View();
                    }
                }
                if (customer.Email != entity.Email)
                {
                    if (db.Customers.Count(x => x.Email == entity.Email) > 0)
                    {
                        ModelState.AddModelError("", "Email cannot be duplicated...");
                        return View();
                    }
                }
                customer.CustomerID = entity.CustomerID;
                customer.Username = entity.Username;
                customer.Fullname = entity.Fullname;
                customer.Email = entity.Email;
                customer.Phone = entity.Phone;
                customer.Status = entity.Status;
                db.SaveChanges();
                return View();
            }
            return View();
        }

        [HttpPost]
        public JsonResult ChangeStatus(int id)
        {

            var customer = db.Customers.Find(id);
            customer.Status = !customer.Status;
            db.SaveChanges();
            return Json(new
            {
                Status = customer.Status
            });


        }


    }
}
