////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
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
using reCaptcha.Models.VerifyingUsersResponse;
using reCaptcha.stat;
using SPADemoCRUD.Models;

namespace SPADemoCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly AppDataBaseContext _context;
        private readonly AppConfig AppOptions;

        public SessionController(AppDataBaseContext context, IOptions<AppConfig> options)
        {
            AppOptions = options.Value;
            _context = context;
        }

        // GET: api/Account
        [HttpGet]
        public ActionResult<object> Get()
        {
            if (User.Identity.IsAuthenticated)
            {
                string role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType)?.Value;
                return new
                {
                    User.Identity.IsAuthenticated,
                    User.Identity.Name,
                    role
                };
            }

            if (AppOptions.AllowedWebLogin || AppOptions.AllowedWebRegistration)
            {
                return new
                {
                    isAuthenticated = false,

                    AppOptions.reCaptchaV2InvisiblePublicKey,
                    AppOptions.reCaptchaV2PublicKey,

                    AppOptions.AllowedWebLogin,
                    AppOptions.AllowedWebRegistration
                };
            }

            return new
            {
                isAuthenticated = false,
                message = "Web авторизация/регистрация отключена администратором"
            };
        }

        [HttpPost]
        public async void Post([FromBody] RegisterModel regUser)
        {
            if (!AppOptions.AllowedWebRegistration)
            {
                return;
            }

            if (ModelState.IsValid)
            {
                if (AppOptions.IsEnableReCaptchaV2 || AppOptions.IsEnableReCaptchaV2Invisible)
                {
                    reCaptcha2ResponseModel reCaptcha2Status = reCaptchaVerifier.reCaptcha2SiteVerify(AppOptions.reCaptchaV2PrivatKey, regUser.g_recaptcha_response, HttpContext.Connection.RemoteIpAddress.ToString());

                    if (reCaptcha2Status is null || !reCaptcha2Status.success || (reCaptcha2Status.ErrorСodes != null && reCaptcha2Status.ErrorСodes.Length > 0))
                    {
                        ModelState.AddModelError("", "Вы не прошли проверку reCaptcha. Повторите попытку ещё раз.");
                    }
                }

                UserModel user = _context.Users.FirstOrDefault(u => u.Email == regUser.EmailRegister);
                if (user == null)
                {
                    RoleModel userRole = _context.Roles.FirstOrDefault(x => x.Name.ToLower() == "user");
                    if (userRole is null)
                    {
                        userRole = new RoleModel() { Name = "user" };
                        _context.Roles.Add(userRole);
                        _context.SaveChanges();
                    }

                    DepartmentModel userDepartment = _context.Departments.FirstOrDefault(x => x.Name.ToLower() == "user");
                    if (userDepartment is null)
                    {
                        userDepartment = new DepartmentModel() { Name = "user" };
                        _context.Departments.Add(userDepartment);
                        _context.SaveChanges();
                    }

                    user = new UserModel { Email = regUser.EmailRegister, Name = regUser.UsernameRegister, Password = regUser.PasswordRegister, RoleId = userRole.Id, DepartmentId = userDepartment.Id };
                    _context.Users.Add(user);
                    _context.SaveChanges();

                    await Authenticate(user);
                }
                else
                {
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
                }
            }
        }

        [HttpPut]
        public void Put([FromBody] LoginModel loginUser)
        {
            if (!AppOptions.AllowedWebLogin)
            {
                return;
            }

            if (ModelState.IsValid)
            {
                if (AppOptions.IsEnableReCaptchaV2 || AppOptions.IsEnableReCaptchaV2Invisible)
                {
                    reCaptcha2ResponseModel reCaptcha2Status = reCaptchaVerifier.reCaptcha2SiteVerify(AppOptions.reCaptchaV2PrivatKey, loginUser.g_recaptcha_response, HttpContext.Connection.RemoteIpAddress.ToString());

                    if (reCaptcha2Status is null || !reCaptcha2Status.success || (reCaptcha2Status.ErrorСodes != null && reCaptcha2Status.ErrorСodes.Length > 0))
                    {
                        ModelState.AddModelError("", "Вы не прошли проверку reCaptcha. Повторите попытку ещё раз.");
                    }
                }

                UserModel user = _context.Users.Include(x => x.Role).FirstOrDefault(u => u.Email == loginUser.EmailLogin && u.Password == loginUser.PasswordLogin);
                if (user != null)
                {
                    _ = Authenticate(user);
                }
                else
                {
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
                }
            }
        }

        private async Task Authenticate(UserModel user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
            };
            //
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            //
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        // GET: api/Account/xyz-xyz-xyz-xyz
        //[HttpGet("{id}")]
        //public async Task<ActionResult<object>> LoginByToken(string id)
        //{
        //    return null;
        //}

        [HttpDelete]
        [Authorize]
        public async void Delete()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
