using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialApp.Models
{
    public class InviteHouseholdUsersViewModel
    {

        public string HouseholdName { get; set; }
        public int FinancialAccountId { get; set; }
        public List<ApplicationUser> UsersList { get; set; }
        public  string AddUserId { get; set; }
        public string RemoveUserId { get; set; }
    }
}