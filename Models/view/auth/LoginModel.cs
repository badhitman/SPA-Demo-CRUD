////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SPADemoCRUD.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Не указан Email авторизации")]
        [DataType(DataType.EmailAddress)]
        public string EmailLogin { get; set; }

        [Required(ErrorMessage = "Не указан пароль авторизации")]
        [DataType(DataType.Password)]
        public string PasswordLogin { get; set; }

        public string g_recaptcha_response { get; set; }
    }
}
