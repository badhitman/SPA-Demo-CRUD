////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using reCaptcha.Models.VerifyingUsersResponse;
using reCaptcha.stat;
using SPADemoCRUD.Models;
using SPADemoCRUD.Models.view;

namespace SPADemoCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : aSessionController
    {
        public RegistrationController(AppDataBaseContext db_context, IOptions<AppConfig> options) : base(db_context, options) { }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegisterModel regUser)
        {
            if (!AppOptions.AllowedWebRegistration)
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Регистрация отключена администратором",
                    Status = StylesMessageEnum.warning.ToString()
                });
            }

            if (ModelState.IsValid)
            {
                if (AppOptions.IsEnableReCaptchaV2 || AppOptions.IsEnableReCaptchaV2Invisible)
                {
                    reCaptcha2ResponseModel reCaptcha2Status = reCaptchaVerifier.reCaptcha2SiteVerify(AppOptions.reCaptchaV2PrivatKey, regUser.g_recaptcha_response, HttpContext.Connection.RemoteIpAddress.ToString());

                    if (reCaptcha2Status is null || !reCaptcha2Status.success || (reCaptcha2Status.ErrorСodes != null && reCaptcha2Status.ErrorСodes.Length > 0))
                    {
                        return new ObjectResult(new ServerActionResult()
                        {
                            Success = false,
                            Info = "Неудачная попытка регистрации. Ошибка валидации reCaptcha токена.",
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

                UserModel user = DbContext.Users.FirstOrDefault(u => u.Email == regUser.EmailRegister);
                if (user != null)
                {
                    return new ObjectResult(new ServerActionResult()
                    {
                        Success = false,
                        Info = "Неудачная попытка регистрации. Логин занят",
                        Status = StylesMessageEnum.warning.ToString()
                    });
                }

                DepartmentModel userDepartment = DbContext.Departments.FirstOrDefault(x => x.Name.ToLower() == "user");
                if (userDepartment is null)
                {
                    userDepartment = new DepartmentModel() { Name = "user" };
                    DbContext.Departments.Add(userDepartment);
                    DbContext.SaveChanges();
                }

                user = new UserModel { Email = regUser.EmailRegister, Name = regUser.PublicNameRegister, Password = regUser.PasswordRegister, Role = AccessLevelUserRolesEnum.Auth, DepartmentId = userDepartment.Id };
                DbContext.Users.Add(user);
                DbContext.SaveChanges();

                await Authenticate(user);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = true,
                    Info = "Регистрация успешно завершена",
                    Status = StylesMessageEnum.success.ToString()
                });
            }
            else
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Регистрация отклонена.",
                    Status = StylesMessageEnum.warning.ToString(),
                    Tag = ModelState
                });
            }
        }        
    }
}
