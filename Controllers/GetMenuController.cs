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
        private readonly NestedMenuModel my = new NestedMenuModel()
        {
            Title = "me",
            Href = "#",
            Tooltip = "Личное пространство",
            Childs = new MenuItemModel[]
            {
                new MenuItemModel() { Title = "Сообщения", Href = "/notifications/list", Tooltip = "Отключено! Мои уведомления", IsDisabled = true },
                new MenuItemModel() { Title = "Профиль", Href = "/profile/view/", Tooltip = "Мои настройки" },
                null,
                new MenuItemModel() { Title = "Выход", Href = "/signin/", Tooltip = "Выйти из приложения" }
            }
        };
        private readonly NestedMenuModel users = new NestedMenuModel()
        {
            Title = "Пользователи",
            Href = "#",
            Tooltip = "Управление пользователями",
            Childs = new MenuItemModel[]
            {
                new MenuItemModel() { Title = "Пользователи", Href = "/users/list", Tooltip = "Акаунты зарегистрированных участников" },
                new MenuItemModel() { Title = "Группы", Href = "/departments/list", Tooltip = "Отделы и депаратменты" }
            }
        };
        private readonly NestedMenuModel catalogues = new NestedMenuModel()
        {
            Title = "Каталоги",
            Href = "#",
            Tooltip = "Справочники и наборы данных",
            Childs = new MenuItemModel[]
            {
                new MenuItemModel() { Title = "Номенклатура", Href = "/groupsgoods/list", Tooltip = "Товары и группы" },
                new MenuItemModel() { Title = "Склад.учёт", Href = "/warehouses/list", Tooltip = "Склады, остатки и документы движения товаров", IsDisabled = true },
                new MenuItemModel() { Title = "Доставка", Href = "/delivery/list", Tooltip = "Отключено! Методы/Службы доставки", IsDisabled = true },
                null,
                new MenuItemModel() { Title = "Файлы", Href = "/files/list", Tooltip = "Файловое хранилище" }
            }
        };
        private readonly NestedMenuModel server = new NestedMenuModel()
        {
            Title = "Сервер",
            Href = "#",
            Tooltip = "Управление сервером",
            Childs = new MenuItemModel[]
            {
                new MenuItemModel() { Title = "Telegram", Href = "/telegram/list", Tooltip = "Отключено! Telegram Bot", IsDisabled = true },
                new MenuItemModel() { Title = "Electrum", Href = "/electrum/list", Tooltip = "Отключено! Electrum wallet", IsDisabled = true },
                new MenuItemModel() { Title = "Настройки", Href = "/server/view", Tooltip = "Настройки сервера" }
            }
        };

        // GET: api/getmenu
        [HttpGet]
        public ActionResult<IEnumerable<MenuItemModel>> GetUsers()
        {
            AccessLevelUserRolesEnum role = User.HasClaim(c => c.Type == ClaimTypes.Role)
                ? (AccessLevelUserRolesEnum)Enum.Parse(typeof(AccessLevelUserRolesEnum), User.FindFirst(c => c.Type == ClaimTypes.Role).Value)
                : AccessLevelUserRolesEnum.Guest;

            switch (role)
            {
                case AccessLevelUserRolesEnum.Auth:
                    return new NestedMenuModel[] { my };
                case AccessLevelUserRolesEnum.Verified:
                    return new NestedMenuModel[] { my };
                case AccessLevelUserRolesEnum.Privileged:
                    return new NestedMenuModel[] { my };
                case AccessLevelUserRolesEnum.Manager:
                    return new NestedMenuModel[] { users, my };
                case AccessLevelUserRolesEnum.Admin:
                    return new NestedMenuModel[] { users, catalogues, my };
                case AccessLevelUserRolesEnum.ROOT:
                    return new NestedMenuModel[] { users, catalogues, server, my };
                case AccessLevelUserRolesEnum.Guest:
                    return new MenuItemModel[]
                    {
                        new MenuItemModel()
                        {
                            Title = "Вход",
                            Href = "/signin/",
                            Tooltip = "Авторизация/Регистрация"
                        }
                    };
                default:
                    return new MenuItemModel[]
                    {
                        new MenuItemModel()
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
