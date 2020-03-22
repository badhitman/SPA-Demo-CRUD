using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SPADemoCRUD.Models;

namespace SPADemoCRUD.Controllers
{
    public abstract class aSessionController : ControllerBase
    {
        protected readonly AppConfig AppOptions;
        protected readonly AppDataBaseContext DbContext;

        public aSessionController(AppDataBaseContext db_context, IOptions<AppConfig> options)
        {
            AppOptions = options.Value;
            DbContext = db_context;
        }

        protected async Task Authenticate(UserModel user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString())
            };
            //
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(id);
            HttpContext.User = claimsPrincipal;
            //
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
            UpdateSession(HttpContext, AppOptions);
        }

        public static void UpdateSession(HttpContext httpContext, AppConfig AppOptions)
        {
            CookieOptions cookieOptions = new CookieOptions()
            {
                Expires = DateTime.Now.AddSeconds(AppOptions.SessionCookieExpiresSeconds),
                HttpOnly = false,
                Secure = AppOptions.SessionCookieSslSecureOnly
            };

            if (httpContext.User.Identity.IsAuthenticated)
            {
                httpContext.Response.Cookies.Delete("AllowedWebLogin");
                httpContext.Response.Cookies.Delete("AllowedWebRegistration");
                httpContext.Response.Cookies.Delete("reCaptchaV2InvisiblePublicKey");
                httpContext.Response.Cookies.Delete("reCaptchaV2PublicKey");

                httpContext.Response.Cookies.Append("name", httpContext.User.Identity.Name, cookieOptions);
                string role = httpContext.User.HasClaim(c => c.Type == ClaimTypes.Role)
                ? httpContext.User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType)?.Value
                : "guest";
                httpContext.Response.Cookies.Append("role", role, cookieOptions);
            }
            else
            {
                httpContext.Response.Cookies.Delete("name");
                httpContext.Response.Cookies.Delete("role");

                if (AppOptions.AllowedWebLogin || AppOptions.AllowedWebRegistration)
                {
                    httpContext.Response.Cookies.Append("AllowedWebLogin", AppOptions.AllowedWebLogin.ToString(), cookieOptions);
                    httpContext.Response.Cookies.Append("AllowedWebRegistration", AppOptions.AllowedWebRegistration.ToString(), cookieOptions);
                    if (!string.IsNullOrWhiteSpace(AppOptions.reCaptchaV2InvisiblePublicKey))
                    {
                        httpContext.Response.Cookies.Append("reCaptchaV2InvisiblePublicKey", AppOptions.reCaptchaV2InvisiblePublicKey, cookieOptions);
                    }
                    else
                    {
                        httpContext.Response.Cookies.Delete("reCaptchaV2InvisiblePublicKey");
                    }
                    if (!string.IsNullOrWhiteSpace(AppOptions.reCaptchaV2PublicKey))
                    {
                        httpContext.Response.Cookies.Append("reCaptchaV2PublicKey", AppOptions.reCaptchaV2PublicKey ?? "", cookieOptions);
                    }
                    else
                    {
                        httpContext.Response.Cookies.Delete("reCaptchaV2PublicKey");
                    }
                }
            }
        }
    }
}