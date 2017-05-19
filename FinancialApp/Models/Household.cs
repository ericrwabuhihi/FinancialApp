using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialApp.Models
{
    public class Household
    {
        public Household()
        {
            FinancialAccounts = new HashSet<FinancialAccount>();
            Budgets = new HashSet<Budget>();
            Users = new HashSet<ApplicationUser>();
            Categories = new HashSet<Category>();
        }
        //NUEmail new user email

        public int Id { get; set; }
        public string Name { get; set; }
    
   
        public virtual ICollection<FinancialAccount> FinancialAccounts { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<Budget> Budgets { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
    }
}