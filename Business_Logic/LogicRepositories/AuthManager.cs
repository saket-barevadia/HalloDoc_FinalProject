
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.AspNetCore.Routing;

using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace HelloDocMvc.Repository.Repositories
{
    public class AuthManager
    {
        [AttributeUsage(AttributeTargets.All)]
        public class CustomAuthorize : Attribute, IAuthorizationFilter
        {
            private readonly string _role;
            private readonly string _menu;
            public CustomAuthorize(string role = "", string menu = null)
            {
                _role = role;
                this._menu = menu;
            }






            public void OnAuthorization(AuthorizationFilterContext filterContext)
            {
                var jwtService = filterContext.HttpContext.RequestServices.GetService<IjwtService>();
              
                var _admin = filterContext.HttpContext.RequestServices.GetService<IAdminDashboard>();


                var user = filterContext.HttpContext.Session.GetString("user");

                var roleMain = filterContext.HttpContext.Session.GetInt32("roleId");



                if (jwtService == null)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index" }));
                }


                if (!string.IsNullOrEmpty(_role))
                {
                    if (!(user == _role))
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "AccessDenied" }));
                    }
                }


                if (_menu != null)
                {
                    List<string> roleMenu = _admin.GetListOfRoleMenu((int)roleMain);
                    bool f = false;
                    if (roleMenu.Any(r => r == _menu))
                    {
                        f = true;
                    }
                    if (!f)
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "AccessDenied" }));
                        return;
                    }
                }

            }
        }
    }
}

