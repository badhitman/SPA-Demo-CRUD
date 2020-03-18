﻿namespace SPADemoCRUD.Models
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

        /// <summary>
        /// Разрешение Web регистрации. Если False то регистрация через веб интерфейс запрещён
        /// </summary>
        public bool AllowedWebRegistration { get; set; } = false;

        /// <summary>
        /// Флаг/Признак того, что разрешена Web авторизация вводом логина и пароля. Если False то вход возможен только по токену
        /// </summary>
        public bool AllowedWebLogin { get; set; } = false;

        /// <summary>
        /// Установка прав пользователю ROOT по его ID.
        /// Указав в этом параметре ID пользователя, система при запуске назначит ему (пользовтаелю) права ROOT
        /// </summary>
        public int SetUserRootById { get; set; }

        /// <summary>
        /// Флаг/Признак того что при первычной инициализации/создании базы данных требуется загрузить демо данные
        /// </summary>
        public bool HasDemoData { get; set; } = true;
    }
}