////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SPADemoCRUD.Models;

namespace SPADemoCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetMenu : ControllerBase
    {
        private readonly NestedMenu my = new NestedMenu()
        {
            Title = "me",
            Href = "#",
            Tooltip = "Личное пространство",
            Childs = new MenuItem[]
            {
                new MenuItem() { Title = "Сообщения", Href = "/notifications/list", Tooltip = "Отключено! Мои уведомления", IsDisabled = true },
                new MenuItem() { Title = "Профиль", Href = "/profile/view/", Tooltip = "Мои настройки" },
                null,
                new MenuItem() { Title = "Выход", Href = "/signin/", Tooltip = "Выйти из приложения" }
            }
        };
        private readonly NestedMenu users = new NestedMenu()
        {
            Title = "Пользователи",
            Href = "#",
            Tooltip = "Управление пользователями",
            Childs = new MenuItem[]
            {
                new MenuItem() { Title = "Пользователи", Href = "/users/list", Tooltip = "Акаунты зарегистрированных участников" },
                new MenuItem() { Title = "Группы", Href = "/departments/list", Tooltip = "Отделы и депаратменты" }
            }
        };
        private readonly NestedMenu catalogues = new NestedMenu()
        {
            Title = "Каталоги",
            Href = "#",
            Tooltip = "Справочники и наборы данных",
            Childs = new MenuItem[]
            {
                new MenuItem() { Title = "Номенклатура", Href = "/goods/list", Tooltip = "Отключено! Товары и всё такое", IsDisabled = true },
                new MenuItem() { Title = "Запасы", Href = "/turnover/list", Tooltip = "Отключено! Движения товаров", IsDisabled = true },
                new MenuItem() { Title = "Доставка", Href = "/delivery/list", Tooltip = "Отключено! Методы/Службы доставки", IsDisabled = true },
                null,
                new MenuItem() { Title = "Файлы", Href = "/files/list", Tooltip = "Файловое хранилище" }
            }
        };
        private readonly NestedMenu server = new NestedMenu()
        {
            Title = "Сервер",
            Href = "#",
            Tooltip = "Управление сервером",
            Childs = new MenuItem[]
            {
                new MenuItem() { Title = "Telegram", Href = "/telegram/list", Tooltip = "Отключено! Telegram Bot", IsDisabled = true },
                new MenuItem() { Title = "Electrum", Href = "/electrum/list", Tooltip = "Отключено! Electrum wallet", IsDisabled = true },
                new MenuItem() { Title = "Настройки", Href = "/server/view", Tooltip = "Настройки сервера" }
            }
        };

        // GET: api/getmenu
        [HttpGet]
        public ActionResult<IEnumerable<MenuItem>> GetUsers()
        {
            AccessLevelUserRolesEnum role = User.HasClaim(c => c.Type == ClaimTypes.Role)
                ? (AccessLevelUserRolesEnum)Enum.Parse(typeof(AccessLevelUserRolesEnum), User.FindFirst(c => c.Type == ClaimTypes.Role).Value)
                : AccessLevelUserRolesEnum.Guest;

            switch (role)
            {
                case AccessLevelUserRolesEnum.Auth:
                    return new NestedMenu[] { my };
                case AccessLevelUserRolesEnum.Verified:
                    return new NestedMenu[] { my };
                case AccessLevelUserRolesEnum.Privileged:
                    return new NestedMenu[] { my };
                case AccessLevelUserRolesEnum.Manager:
                    return new NestedMenu[] { users, my };
                case AccessLevelUserRolesEnum.Admin:
                    return new NestedMenu[] { users, catalogues, my };
                case AccessLevelUserRolesEnum.ROOT:
                    return new NestedMenu[] { users, catalogues, server, my };
                case AccessLevelUserRolesEnum.Guest:
                    return new MenuItem[]
                    {
                        new MenuItem()
                        {
                            Title = "Вход",
                            Href = "/signin/",
                            Tooltip = "Авторизация/Регистрация"
                        }
                    };
                default:
                    return new MenuItem[]
                    {
                        new MenuItem()
                        {
                            Title = "Ошибка",
                            Href = "#",
                            Tooltip = "Ошибка ассоциации роли с меню"
                        }
                    };
            }
        }
    }
}
