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
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        [RequireHousehold]
        public ActionResult Index()
        {
            var HHI = User.Identity.GetHouseholdId();

            var transactions = db.FinancialAccounts.Where(f=>f.HouseholdId==HHI).SelectMany(t=>t.Transactions);

            return View(transactions.ToList());
        }

        // get transactions/create

        public ActionResult Create(int id)
        {
            Transaction transactions = new Transaction();
            transactions.AccountId = id;
            transactions.EnteredById = User.Identity.GetUserId();

            ViewBag.categoryId = new SelectList(db.Categories, "Id", "Name");
            //ViewBag.EnteredById = new SelectList(db.Users, "Id", "UserName");
            //ViewBag.AccountId = id;
            // new SelectList(db.FinancialAccounts, "Id", "Name");


            return View(transactions);
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }

            return View(transaction);
        }


        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AccountId,Description,Date,Amount,Type,CategoryId,Reconciled,ReconciledAmount,EnteredById")] Transaction transaction)
       {

            if (ModelState.IsValid)

            {

                FinancialAccount acct = db.FinancialAccounts.FirstOrDefault(FA => FA.Id == transaction.AccountId);
    
                if (transaction.Type)
                {
                    // Income
                    acct.Balance += transaction.Amount;
                    acct.ReconciledBalance = (transaction.Reconciled)
                                             //either 
                                           ? acct.ReconciledBalance + transaction.Amount
                                           //or 
                                           : acct.ReconciledBalance;
                }
               else
                {
                    // Expenses
                    acct.Balance -= transaction.Amount;
                    acct.ReconciledBalance = (transaction.Reconciled)
                                           ? acct.ReconciledBalance - transaction.Amount
                                             :acct.ReconciledBalance;
                }

                transaction.Description = (transaction.Description == "" || transaction.Description== null)
                                       ? "no description." : transaction.Description;


                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Details", "FinancialAccounts", new { id = acct.Id });
            }

            ViewBag.AccountId = new SelectList(db.FinancialAccounts, "Id", "Name", transaction.AccountId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            ViewBag.EnteredById = new SelectList(db.Users, "Id", "FirstName", transaction.EnteredById);
            return View(transaction);
        }
    

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountId = new SelectList(db.FinancialAccounts, "Id", "Name", transaction.AccountId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            ViewBag.EnteredById = new SelectList(db.Users, "Id", "FirstName", transaction.EnteredById);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountId,Description,Date,Amount,Type,CategoryId,Reconciled,ReconciledAmount,EnteredById")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {

                transaction.Description = (transaction.Description == "" || transaction.Description == null)
                                         ? "no description"
                                         : transaction.Description;

                //FinancialAccount acct = db.FinancialAccounts.FirstOrDefault(FA => FA.Id == transaction.AccountId);
                FinancialAccount acct = db.FinancialAccounts.Find(transaction.AccountId);
                var originalTransaction = db.Transactions.AsNoTracking().Where(t => t.Id == transaction.Id).FirstOrDefault();

                if (transaction.Amount != originalTransaction.Amount)
                {
                    // update acct balance
                    acct.Balance = (transaction.Type)
                        ? acct.Balance - originalTransaction.Amount
                        : acct.Balance + originalTransaction.Amount;

                   acct.Balance = (transaction.Type) 
                        ? acct.Balance += transaction.Amount 
                        : acct.Balance -= transaction.Amount;

                }

                transaction.Reconciled = false;
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountId = new SelectList(db.FinancialAccounts, "Id", "Name", transaction.AccountId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            ViewBag.EnteredById = new SelectList(db.Users, "Id", "FirstName", transaction.EnteredById);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)

        {

            Transaction transaction = db.Transactions.Find(id);
            var acct = db.FinancialAccounts.FirstOrDefault(f => f.Id == transaction.AccountId);
            if (transaction.Type)
            { 
                acct.Balance -= transaction.Amount;
                acct.ReconciledBalance = (transaction.Reconciled)
               ? acct.ReconciledBalance - transaction.Amount
               : acct.ReconciledBalance;
           
        }

        else
        {
         acct.Balance += transaction.Amount;
                acct.ReconciledBalance = (transaction.Reconciled)
                                        ? acct.ReconciledBalance + transaction.Amount
                                        : acct.ReconciledBalance;        
        }
            db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Transactions/Void
        public ActionResult Void(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }



        //post transaction void
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Void(int Id)
        {
            Transaction transaction = db.Transactions.Find(Id);
            if (ModelState.IsValid)
            {
                FinancialAccount acct = db.FinancialAccounts.Find(transaction.AccountId);

                if (transaction.Type)
                {
                    acct.Balance -= transaction.Amount;
                    acct.ReconciledBalance -= transaction.Amount;

                }
                else
                {
                    acct.Balance += transaction.Amount;
                    acct.ReconciledBalance += transaction.Amount;

                }

                transaction.IsVoid = true;
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            ViewBag.EnteredById = new SelectList(db.Users, "Id", "FirstName", transaction.EnteredById);
            return View(transaction);
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
