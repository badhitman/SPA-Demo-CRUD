////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SPADemoCRUD.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Не указан Email")]
        [DataType(DataType.EmailAddress)]
        public string EmailRegister { get; set; }

        [Required(ErrorMessage = "Не указано публичное имя")]
        [DataType(DataType.Text)]
        public string PublicNameRegister { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string PasswordRegister { get; set; }

        [DataType(DataType.Password)]
        [Compare("PasswordRegister", ErrorMessage = "Пароль введен неверно")]
        public string ConfirmPasswordRegister { get; set; }

        public string g_recaptcha_response { get; set; }
    }
}
