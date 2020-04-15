////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MultiTool;
using reCaptcha.Models.VerifyingUsersResponse;
using reCaptcha.stat;
using SPADemoCRUD.Models;

namespace SPADemoCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        protected readonly AppConfig AppOptions;
        protected readonly AppDataBaseContext DbContext;

        public AuthorizationController(AppDataBaseContext db_context, IOptions<AppConfig> options)
        {
            AppOptions = options.Value;
            DbContext = db_context;
        }

        /// <summary>
        /// Авторизация
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] LoginModel loginUser)
        {
            if (!AppOptions.AllowedWebLogin)
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Возможность авторизации через web интерфейс отключена администратором.",
                    Status = StylesMessageEnum.warning.ToString()
                });
            }

            if (!ModelState.IsValid)
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка контроля валидации модели. Контроллер авторизации отклонил запрос.",
                    Status = StylesMessageEnum.danger.ToString(),
                    Tag = ModelState
                });
            }

            if (AppOptions.IsEnableReCaptchaV2 || AppOptions.IsEnableReCaptchaV2Invisible)
            {
                string privatKey = AppOptions.IsEnableReCaptchaV2Invisible
                    ? AppOptions.reCaptchaV2InvisiblePrivatKey
                    : AppOptions.reCaptchaV2PrivatKey;

                reCaptcha2ResponseModel reCaptcha2Status = reCaptchaVerifier.reCaptcha2SiteVerify(privatKey, loginUser.g_recaptcha_response, HttpContext.Connection.RemoteIpAddress.ToString());

                if (reCaptcha2Status is null || !reCaptcha2Status.success || (reCaptcha2Status.ErrorСodes != null && reCaptcha2Status.ErrorСodes.Length > 0))
                {
                    return new ObjectResult(new ServerActionResult()
                    {
                        Success = false,
                        Info = "Неудачная попытка входа. Ошибка валидации reCaptcha токена.",
                        Status = StylesMessageEnum.danger.ToString(),
                        Tag = reCaptcha2Status
                    });
                }
            }

            UserObjectModel user = DbContext.Users.Include(x => x.Department).FirstOrDefault(u => u.Email == loginUser.EmailLogin && u.Password == glob_tools.GetHashString(loginUser.PasswordLogin));//glob_tools.GetHashString
            if (user != null)
            {
                if (user.isDisabled)
                {
                    return new ObjectResult(new ServerActionResult()
                    {
                        Success = false,
                        Info = "Вход временно невозможен. Пользователь отключён от системы",
                        Status = StylesMessageEnum.danger.ToString(),
                        Tag = new { user.Name, user.Role, Department = user.Department.Name }
                    });
                }
                await Authenticate(user);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = true,
                    Info = "Вход успешно выполнен.",
                    Status = StylesMessageEnum.success.ToString(),
                    Tag = new { user.Name, user.Role, Department = user.Department.Name }
                });
            }
            else
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Неверный логин и/или пароль.",
                    Status = StylesMessageEnum.warning.ToString()
                });
            }
        }

        /// <summary>
        /// Регистрация
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegisterModel regUser)
        {
            if (!AppOptions.AllowedWebRegistration)
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Регистрация через web интерфейс отключена администратором",
                    Status = StylesMessageEnum.warning.ToString()
                });
            }

            if (!ModelState.IsValid)
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка контроля валидации модели. Регистрация отклонена.",
                    Status = StylesMessageEnum.warning.ToString(),
                    Tag = ModelState
                });
            }

            if (AppOptions.IsEnableReCaptchaV2 || AppOptions.IsEnableReCaptchaV2Invisible)
            {
                reCaptcha2ResponseModel reCaptcha2Status = reCaptchaVerifier.reCaptcha2SiteVerify(AppOptions.reCaptchaV2PrivatKey, regUser.g_recaptcha_response, HttpContext.Connection.RemoteIpAddress.ToString());

                if (reCaptcha2Status is null || !reCaptcha2Status.success || (reCaptcha2Status.ErrorСodes != null && reCaptcha2Status.ErrorСodes.Length > 0))
                {
                    return new ObjectResult(new ServerActionResult()
                    {
                        Success = false,
                        Info = "Неудачная попытка регистрации. Ошибка верификации reCaptcha.",
                        Status = StylesMessageEnum.danger.ToString(),
                        Tag = reCaptcha2Status
                    });
                }
            }

            bool checkPublicName = await DbContext.Users.AnyAsync(u => u.Name == regUser.PublicNameRegister);
            if (checkPublicName)
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Неудачная попытка регистрации. Публичное имя используется другим пользователем. Введите другое.",
                    Status = StylesMessageEnum.warning.ToString()
                });
            }

            UserObjectModel user = DbContext.Users.FirstOrDefault(u => u.Email == regUser.EmailRegister);
            if (user != null)
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Неудачная попытка регистрации. Логин занят. Придумайте другой лгин для входа или воспользуйтесь формой входа.",
                    Status = StylesMessageEnum.warning.ToString()
                });
            }

            DepartmentObjectModel userDepartment = DbContext.Departments.FirstOrDefault(x => x.Name.ToLower() == "user");
            if (userDepartment is null)
            {
                userDepartment = new DepartmentObjectModel() { Name = "user", Readonly = true };
                DbContext.Departments.Add(userDepartment);
                DbContext.SaveChanges();
            }

            user = new UserObjectModel { Email = regUser.EmailRegister, Name = regUser.PublicNameRegister, Password = glob_tools.GetHashString(regUser.PasswordRegister), Role = AccessLevelUserRolesEnum.Auth, DepartmentId = userDepartment.Id };
            DbContext.Users.Add(user);
            DbContext.SaveChanges();

            await Authenticate(user);
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Регистрация успешно завершена",
                Status = StylesMessageEnum.success.ToString(),
                Tag = new { user.Name, user.Role, Department = user.Department.Name }
            });
        }

        [HttpDelete]
        [Authorize(Policy = "AccessMinLevelAuth")]
        public async void Delete()
        {
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            UpdateSessionCookies(HttpContext, AppOptions);
        }

        private async Task Authenticate(UserObjectModel user)
        {
            var claims = new List<Claim>
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString())
            };
            //
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(id);
            HttpContext.User = claimsPrincipal;
            //           
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
            UpdateSessionCookies(HttpContext, AppOptions);
        }

        public static void UpdateSessionCookies(HttpContext httpContext, AppConfig AppOptions)
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
                ? httpContext.User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value
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

#if DEBUG
            if (AppOptions.HasDemoData)
            {
                httpContext.Response.Cookies.Append("debug", "demo", cookieOptions);
            }
#endif
        }
    }
}
