using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FinancialApp.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using System.Net;

namespace FinancialApp.Controllers
{
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Households. If you have HouseholdId, you will get to household..

        [RequireHousehold]
        public ActionResult Index()
        {
            var hhId = User.Identity.GetHouseholdId();
            Household household = new Household();

            //jya muri database muri household table, urebe muri users , members(uwaloginze in)ufite iyi id.
            household = db.Households.Find(hhId);
            ViewBag.Message = TempData["Message"];
            ViewBag.Error = TempData["Error"];
            //Household household = db.Households.Find(householdId);

            return View(household);



            //return View(db.Households.ToList()); because there is only one household, it isnt a list/not IEnumerable!
        }

        // GET: Households/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Households/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Household household)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                user.HouseholdId = household.Id;
                db.Households.Add(household);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(household);
        }


        // gET invite household user

        public ActionResult Invite()
        {


            return View();
        }

        // POST Invite Household User
        [HttpPost]

        public async Task<ActionResult> Invite(string email)
        {
            var code = Guid.NewGuid();
            var callbackUrl = Url.Action("JoinHousehold", "Households",
                new { code = code }, protocol: Request.Url.Scheme);

            EmailService ems = new EmailService();
            IdentityMessage msg = new IdentityMessage();

            msg.Body = "Hey! Please joing my household... Click <a href=\"" + callbackUrl + "\">here</a>";
            msg.Destination = email;
            msg.Subject = "Invited to my Household";

            await ems.SendAsync(msg);


            //create a record in the invites table

            Invite model = new Invite();
            model.Email = email;
            model.HHToken = code;
            model.HouseholdId = User.Identity.GetHouseholdId().Value;
            model.InviteDate = DateTimeOffset.Now;
            model.InvitedById = User.Identity.GetUserId();

            db.Invites.Add(model);
            db.SaveChanges();


            return RedirectToAction("Index", "Households");
        }



        [Authorize]
        public ActionResult JoinHousehold(Guid? code)
        {
            if (User.Identity.IsInHousehold())
            {
                return RedirectToAction("Index", "Households");
            }


            HouseholdViewModel vm = new HouseholdViewModel();

            // determine whether the user has been sent an invite and set property

            if (code != null)
            {
                Invite result = db.Invites.FirstOrDefault(i => i.HHToken == code);
                vm.IsJoinHouse = true;
                vm.HHId = result.HouseholdId;
                vm.HHName = result.Household.Name;

                // set used flag to true for this invite
                result.HasBeenUsed = true;
                ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
                user.InviteEmail = result.Email;
                db.SaveChanges();
            }

            return View(vm);
            //return RedirectToAction("Index", "Households");
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> JoinHousehold( HouseholdViewModel vm)
        {
            Household hh = db.Households.Find(vm.HHId);
            var user = db.Users.Find(User.Identity. GetUserId());
            hh.Users.Add(user);
            db.SaveChanges();

            await ControllerContext.HttpContext.RefreshAuthentication(user);
            return RedirectToAction("Index", "Households");
            
        }


        [Authorize]
        [HttpPost]
        public ActionResult Create(HouseholdViewModel vm)
        { // antonio added async task and await which i commented out
            //Create new Household and save to DB
            Household hh = new Household();
            hh.Name = vm.HHName;
            db.Households.Add(hh);
            db.SaveChanges();

            //Add the current user as the first member of the new household
            vm.Member = db.Users.Find(User.Identity.GetUserId());
            hh.Users.Add(vm.Member);
            db.SaveChanges();
            //await controllercontext.httpcontext.Refreshauthentification(User);

            return RedirectToAction("Index", "Households");
        }



        // GET: Households/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }


        // GET: Households/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Household household)
        {
            if (ModelState.IsValid)
            {
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(household);
        }

        // GET: Households/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Household household = db.Households.Find(id);
            db.Households.Remove(household);
            db.SaveChanges();
            return RedirectToAction("Index");
        }




      //  Get leave household

        //public ActionResult LeaveHousehold(int Id)
        //{
        //    var userId = User.Identity.GetUserId();
        //    var user = db.Users.Find(userId);
        //    var householdId = User.Identity.GetHouseholdId();
        //    var household = db.Households.Find(householdId);
        //    return View();

        //}

        //Post Leave Household

      [HttpPost]
      [ValidateAntiForgeryToken]
        public async Task<ActionResult> LeaveHousehold(bool? confirmLeaveHousehold)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var householdId = User.Identity.GetHouseholdId();
            var household = db.Households.Find(householdId);

            if (confirmLeaveHousehold != null && household.Users.Contains(user))
            {
                household.Users.Remove(user);
                db.SaveChanges();
                await ControllerContext.HttpContext.RefreshAuthentication(user);
                return RedirectToAction("JoinHousehold");
            }

            TempData["Error"] = "Please confirm you want to leave this household.";
            return RedirectToAction("Index");
        }


        // get  Recent transactions

        public ActionResult RecentTransactions()
            {

                var RecentTransactions = db.FinancialAccounts
                                            .Where(h=>h.HouseholdId == User.Identity.GetHouseholdId())
                                            .SelectMany(t=>t.Transactions)
                                            .OrderByDescending(t => t.Date).Take(5);


                return View();
            }

        





        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
