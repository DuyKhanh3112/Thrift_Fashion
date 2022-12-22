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
    public class FeedbacksController : Controller
    {
        private ThriftFashionEntities db = new ThriftFashionEntities();

        // GET: Feedbacks
        public ActionResult Index()
        {
            if (Session[Common.Constants.EMPLOYEE_SESSION] == null)
            {
                return Redirect("/Employees/EmployeeLogin");
            }
            else
            {
                var feedbacks = db.Feedbacks.Include(f => f.Customer).Include(f => f.Product);
                return View(feedbacks.ToList());
            }

        }

        // GET: Feedbacks/Details/5
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
                Feedback feedback = db.Feedbacks.Find(id);
                if (feedback == null)
                {
                    return HttpNotFound();
                }
                return View(feedback);
            }

        }

       

       

     

       

       
    }
}
