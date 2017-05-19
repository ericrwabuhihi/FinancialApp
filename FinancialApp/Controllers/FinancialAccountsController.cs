using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinancialApp.Models;
using Microsoft.AspNet.Identity;

namespace FinancialApp.Controllers
{
    public class FinancialAccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: FinancialAccounts
        public ActionResult Index()

        {
            var financialAccounts = db.FinancialAccounts.Include(f => f.Household);
            return View(financialAccounts.ToList());
        }



        // GET: FinancialAccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FinancialAccount financialAccount = db.FinancialAccounts.Find(id);
            if (financialAccount == null)
            {
                return HttpNotFound();
            }
            return View(financialAccount);
        }

        // GET: FinancialAccounts/Create
        public ActionResult Create(int id)
        {
            FinancialAccount acct = new FinancialAccount();
            acct.Household = db.Households.Find(id);
            acct.HouseholdId = id;

            //ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name");
            return View(acct);
        }

        // POST: FinancialAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,HouseholdId,Name,Balance,ReconciledBalance")] FinancialAccount financialAccount)
        {
            if (ModelState.IsValid)
            {
                var householdId = User.Identity.GetHouseholdId();
                var household = db.Households.Find(householdId);
                //     hh.id       = FINACCT.HouseholdId
                // where is ffacct here?
                household.FinancialAccounts.Add(financialAccount);
                db.SaveChanges();
                return RedirectToAction("Details","Households", new { id = household.Id } );               
            }

            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", financialAccount.HouseholdId);
            return View(financialAccount);
        }

        // GET: FinancialAccounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FinancialAccount financialAccount = db.FinancialAccounts.Find(id);
            if (financialAccount == null)
            {
                return HttpNotFound();
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", financialAccount.HouseholdId);
            return View(financialAccount);
        }

        // POST: FinancialAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HouseholdId,Name,Balance,ReconciledBalance")] FinancialAccount financialAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(financialAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", financialAccount.HouseholdId);
            return View(financialAccount);
        }

        // GET: FinancialAccounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FinancialAccount financialAccount = db.FinancialAccounts.Find(id);
            if (financialAccount == null)
            {
                return HttpNotFound();
            }
            return View(financialAccount);
        }

        // POST: FinancialAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FinancialAccount financialAccount = db.FinancialAccounts.Find(id);
            db.FinancialAccounts.Remove(financialAccount);
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
    }
}
