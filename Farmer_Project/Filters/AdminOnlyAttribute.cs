using Farmer_Project.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Farmer_Project.Filters
{
    // 限管理者存取，未登入或非管理者導回登入頁
    public class AdminOnlyAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var session = context.HttpContext.Session;
            var isLoggedIn = session.GetString(SessionKeys.IsLoggedIn);
            var userRole = session.GetString(SessionKeys.UserRole);

            if (isLoggedIn != "true" || userRole != UserRoles.Admin)
            {
                context.Result = new RedirectToActionResult("Login", "Home", null);
            }
        }
    }
}
