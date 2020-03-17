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
using SPADemoCRUD.Models;

namespace SPADemoCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly AppDataBaseContext _context;

        public SessionController(AppDataBaseContext context)
        {
            _context = context;
        }

        // GET: api/Account
        [HttpGet]
        public ActionResult<object> Get()
        {
            string role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType)?.Value;
            return new { User.Identity.IsAuthenticated, User.Identity.Name, role };
        }

        [HttpPost]
        public async void Post([FromBody] RegisterModel value)
        {
            if (ModelState.IsValid)
            {
                UserModel user = _context.Users.FirstOrDefault(u => u.Email == value.EmailRegister);
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

                    user = new UserModel { Email = value.EmailRegister, Name = value.UsernameRegister, Password = value.PasswordRegister, RoleId = userRole.Id, DepartmentId = userDepartment.Id };
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
        public void Put([FromBody] LoginModel senderUser)
        {
            if (ModelState.IsValid)
            {
                UserModel user = _context.Users.Include(x => x.Role).FirstOrDefault(u => u.Email == senderUser.EmailLogin && u.Password == senderUser.PasswordLogin);
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
