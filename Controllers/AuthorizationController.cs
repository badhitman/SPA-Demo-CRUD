////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using reCaptcha.Models.VerifyingUsersResponse;
using reCaptcha.stat;
using SPADemoCRUD.Models;
using SPADemoCRUD.Models.view;

namespace SPADemoCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : aSessionController
    {

        public AuthorizationController(AppDataBaseContext db_context, IOptions<AppConfig> options) : base(db_context, options) { }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LoginModel loginUser)
        {
            if (!AppOptions.AllowedWebLogin)
            {
                return new BadRequestObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Возможность авторизации отключена администратором",
                    Status = StylesMessageEnum.warning.ToString()
                });
            }

            if (ModelState.IsValid)
            {
                if (AppOptions.IsEnableReCaptchaV2 || AppOptions.IsEnableReCaptchaV2Invisible)
                {
                    string privatKey = AppOptions.IsEnableReCaptchaV2Invisible
                        ? AppOptions.reCaptchaV2InvisiblePrivatKey
                        : AppOptions.reCaptchaV2PrivatKey;

                    reCaptcha2ResponseModel reCaptcha2Status = reCaptchaVerifier.reCaptcha2SiteVerify(privatKey, loginUser.g_recaptcha_response, HttpContext.Connection.RemoteIpAddress.ToString());

                    if (reCaptcha2Status is null || !reCaptcha2Status.success || (reCaptcha2Status.ErrorСodes != null && reCaptcha2Status.ErrorСodes.Length > 0))
                    {
                        return new BadRequestObjectResult(new ServerActionResult()
                        {
                            Success = false,
                            Info = "Неудачная попытка входа. Ошибка валидации reCaptcha токена.",
                            Status = StylesMessageEnum.danger.ToString(),
                            Tag = reCaptcha2Status
                        });
                    }
                }

                UserModel user = DbContext.Users.FirstOrDefault(u => u.Email == loginUser.EmailLogin && u.Password == loginUser.PasswordLogin);
                if (user != null)
                {
                    await Authenticate(user);
                    return new ObjectResult(new ServerActionResult()
                    {
                        Success = true,
                        Info = "Вход успешно выполнен.",
                        Status = StylesMessageEnum.success.ToString()
                    });
                }
                else
                {
                    return new ObjectResult(new ServerActionResult()
                    {
                        Success = false,
                        Info = "Неверный логин и/или пароль.",
                        Status = StylesMessageEnum.warning.ToString(),
                        Tag = ModelState
                    });
                }
            }
            else
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Авторизация отклонена.",
                    Status = StylesMessageEnum.warning.ToString(),
                    Tag = ModelState
                });
            }
        }

        // GET: api/Account/xyz-xyz-xyz-xyz
        //[HttpGet("{id}")]
        //public async Task<ActionResult<object>> LoginByToken(string id)
        //{
        //    return null;
        //}

        [HttpDelete]
        [Authorize(Policy = "AccessMinLevelAuth")]
        public async void Delete()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            ClaimsIdentity id = new ClaimsIdentity();
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(id);
            HttpContext.User = claimsPrincipal;

            UpdateSession(HttpContext, AppOptions);
        }
    }
}
