using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FinancialApp.Models;

namespace FinancialApp.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [RequireHousehold]
        public ActionResult Index()
        {
            var Id = User.Identity.GetHouseholdId();
            Household household = db.Households.Find(Id);
            if (household== null)
            {

                return HttpNotFound();
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