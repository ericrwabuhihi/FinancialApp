using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialApp.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Date { get; set; }
        public decimal Amount { get; set; }
        public bool Type { get; set; }
        public int CategoryId { get; set; }
        public bool Reconciled { get; set; }
        public decimal ReconciledAmount { get; set; }
        public string EnteredById { get; set; }
        public bool IsVoid { get; set; }

        public virtual FinancialAccount Account { get; set; }
        public virtual Category Category { get; set; }
        public virtual ApplicationUser EnteredBy { get; set; }
    }
}