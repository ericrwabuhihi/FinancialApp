using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System.Security.Claims;
using System.Security.Principal;
using FinancialApp.Models;
using System.Threading.Tasks;

namespace FinancialApp
{
    public static class AuthExtensions
    {

        public static ApplicationDbContext db = new ApplicationDbContext();

        public static string GetFirstName(this IIdentity user)
        {
            var ClaimsUser = (ClaimsIdentity)user;
            var claim = ClaimsUser.Claims.FirstOrDefault(c => c.Type == "FirstName");
            return claim.Value ?? null;
        }
        public static string GetLastName(this IIdentity user)
        {
            var ClaimsUser = (ClaimsIdentity)user;
            var claim = ClaimsUser.Claims.FirstOrDefault(c => c.Type == "LastName");
            return claim.Value ?? null;
        }
        public static string GetDisplayName(this IIdentity user)
        {
            var ClaimsUser = (ClaimsIdentity)user;
            var claim = ClaimsUser.Claims.FirstOrDefault(c => c.Type == "DisplayName");
            return claim.Value ?? null;
        }


        public static int? GetHouseholdId(this IIdentity user)
        {
            var claimsIdentity = (ClaimsIdentity)user;
            var HouseholdClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "HouseholdId");
            if (HouseholdClaim != null)
                return int.Parse(HouseholdClaim.Value);
            else return null;
        }

        public static bool IsInHousehold(this IIdentity user)
        {
            var householdClaim = ((ClaimsIdentity)user).Claims.FirstOrDefault(c => c.Type == "HouseholdId");
            return householdClaim != null && !string.IsNullOrWhiteSpace(householdClaim.Value);
        }

        public static async Task RefreshAuthentication(this HttpContextBase context, ApplicationUser user)
        {
            context.GetOwinContext().Authentication.SignOut();
            await context.GetOwinContext().Get<ApplicationSignInManager>().SignInAsync(user, isPersistent: false, rememberBrowser: false);
        }

    }
}