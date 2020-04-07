////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SPADemoCRUD.Models
{
    public class AppConfig
    {
        #region reCaptcha

        /// <summary>
        /// (ПУБЛИЧНЫЙ. reCaptcha V2)
        /// Используйте этот ключ в HTML-коде, который ваш сайт передает на устройства пользователей.
        /// </summary>
        public string reCaptchaV2PublicKey { get; set; }
        /// <summary>
        /// (ПРИВАТНЫЙ!. reCaptcha V2)
        /// Используйте этот секретный ключ для обмена данными между сайтом и сервисом reCAPTCHA.
        /// </summary>
        public string reCaptchaV2PrivatKey { get; set; }
        public bool IsEnableReCaptchaV2 => !string.IsNullOrEmpty(reCaptchaV2PublicKey) && !string.IsNullOrEmpty(reCaptchaV2PrivatKey);

        /// <summary>
        /// (ПУБЛИЧНЫЙ. reCaptcha V2)
        /// Используйте этот ключ в HTML-коде, который ваш сайт передает на устройства пользователей.
        /// </summary>
        public string reCaptchaV2InvisiblePublicKey { get; set; }
        /// <summary>
        /// (ПРИВАТНЫЙ!. reCaptcha V2)
        /// Используйте этот секретный ключ для обмена данными между сайтом и сервисом reCAPTCHA.
        /// </summary>
        public string reCaptchaV2InvisiblePrivatKey { get; set; }
        public bool IsEnableReCaptchaV2Invisible => !string.IsNullOrEmpty(reCaptchaV2InvisiblePublicKey) && !string.IsNullOrEmpty(reCaptchaV2InvisiblePrivatKey);

        #endregion

        #region session

        /// <summary>
        /// Разрешение Web регистрации. Если False то регистрация через веб интерфейс запрещён
        /// </summary>
        public bool AllowedWebRegistration { get; set; } = false;

        /// <summary>
        /// Флаг/Признак того, что разрешена Web авторизация вводом логина и пароля. Если False то вход возможен только по токену
        /// </summary>
        public bool AllowedWebLogin { get; set; } = false;

        /// <summary>
        /// Срок жизни сессии и связаных с ней кукисов
        /// </summary>
        public int SessionCookieExpiresSeconds { get; set; } = 60 * 60;

        /// <summary>
        /// Передавать cookie через Secure Sockets Layer (SSL) - то есть только по протоколу HTTPS
        /// </summary>
        public bool SessionCookieSslSecureOnly { get; set; } = false;

        #endregion

        /// <summary>
        /// Флаг/Признак того что при первычной инициализации/создании базы данных требуется загрузить демо данные
        /// </summary>
        public bool HasDemoData { get; set; } = true;

        /// <summary>
        /// Установка прав пользователю ROOT по его ID.
        /// Указав в этом параметре ID пользователя, система при запуске назначит ему (пользовтаелю) права ROOT
        /// </summary>
        public int SetUserRootById { get; set; }

        /// <summary>
        /// Папка загружаемых файлов (new DirectoryInfo(AppOptions.UploadsPatch))
        /// </summary>
        public string UploadsPath { get; set; } = "uploads";

        /// <summary>
        /// Высота thumb картинки
        /// </summary>
        public int ThumbMaxHeight { get; set; } = 150;

        /// <summary>
        /// Ширина thumb картинки
        /// </summary>
        public int ThumbMaxWidth { get; set; } = 150;

        /// <summary>
        /// Системная учётная запись. От её имени совершаются системные события.
        /// </summary>
        public int SystemUser { get; set; } = 1;
    }
}
