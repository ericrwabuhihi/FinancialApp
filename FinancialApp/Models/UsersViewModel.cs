using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialApp.Models
{
    public class UsersViewModel
    {
        // these are empty containers and allow us to create and add items of type roles or users
        public ApplicationUser User { get; set; }
        public List<string> Roles { get; set; }

    }
}