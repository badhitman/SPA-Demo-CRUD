////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React, { Component } from 'react';

import { Route, Switch } from 'react-router';
import { Layout } from './components/Layout';
import { NotFound } from './components/NotFound';
import { Hub } from './Hub';

import Cookies from 'universal-cookie';

import './custom.css'

/** Контекст приложения */
export default class App extends Component {
    static displayName = App.name;

    /** доступные типы данных (список REST котроллеров) */
    static allowsControllers =
        [
            /** Пользователи */
            'users',
            /** Департаменты/отделы */
            'departments',
            /** Роли/Права */
            'roles',
            /** Сессия (logIn/logOut) */
            'signin',
            /** Файловое хранилище */
            'files',
            /** Пользовательские уведомления */
            'notifications',
            /** работа с собственным профилем */
            'profile',
            /** группы товаров */
            'groupsgoods',
            /** номенклатура */
            'goods',
            /** единицы измерения номенклатуры */
            'unitsgoods',
            /** документы регистров отгрузки номенклатуры в доставку/продажу */
            'turnovers',
            /** склады учёта количества номенклатуры */
            'warehouses',
            /** документы регистров поступления на склад */
            'receiptswarehousesdocuments',
            /** документы регистров внутреннего перемещения */
            'displacementsdocuments',
            /** доставка (настройки служб и методов) */
            'delivery',
            /** Telegram bot */
            'telegram',
            /** Electrum wallet */
            'electrum',
            /** Настройки/состояние сервера */
            'server'
        ];

    /** Имя метода для отображения списка объектов */
    static listNameMethod = 'list';
    /** Имя метода для отображения карточки объекта */
    static viewNameMethod = 'view';
    /** Имя метода для отображения формы создания объекта */
    static createNameMethod = 'create';
    /** Имя метода для отображения формы удаления объекта*/
    static deleteNameMethod = 'delete';
    /** Список доступных CRUD методов */
    static allowsMethods = [App.listNameMethod, App.viewNameMethod, App.createNameMethod, App.deleteNameMethod];

    /** Запрашиваемый контроллер (из react props) */
    static controller;
    /** Имя запрашиваемого метода (из react props) */
    static method;
    /** ID запрашиваемого объекта (из react props) */
    static id;

    /** полученные с сервера (ajax) контекстные данные */
    static data = null;

    /** context user session */
    static session =
        {
            /** Пользователь авторизован */
            isAuthenticated: false,
            /** Имя (публичное) авторизованного пользователя */
            name: '',
            /** Роль, под которой авторизован пользователь в текущей сессии. При изменении роли пользователю необходимо заново авторизоваться */
            role: '',

            /** Публичный ключ для работы reCaptcha v2 Invisible */
            reCaptchaV2InvisiblePublicKey: '',
            /** Публичный ключ для работы reCaptcha v2 Invisible */
            reCaptchaV2PublicKey: '',

            /** Разрешение авторизаии через Web форму */
            AllowedWebLogin: false,
            /** Разрешение регистрации через Web форму */
            AllowedWebRegistration: false
        };

    /** Чтение состояния сессии пользователя */
    static readSession() {
        const cookies = new Cookies();
        var name = cookies.get('name');
        var role = cookies.get('role');

        if (name && name.length > 0) {
            App.session =
            {
                name: name,
                role: role,
                isAuthenticated: true
            };
        }
        else {
            var AllowedWebLogin = cookies.get('AllowedWebLogin');
            if (AllowedWebLogin && AllowedWebLogin.length > 0) {
                AllowedWebLogin = (AllowedWebLogin.toLowerCase() === "true");
            }
            else {
                AllowedWebLogin = false;
            }

            var AllowedWebRegistration = cookies.get('AllowedWebRegistration');
            if (AllowedWebRegistration && AllowedWebRegistration.length > 0) {
                AllowedWebRegistration = (AllowedWebRegistration.toLowerCase() === "true");
            }
            else {
                AllowedWebRegistration = false;
            }

            App.session =
            {
                AllowedWebLogin: AllowedWebLogin,
                AllowedWebRegistration: AllowedWebRegistration,
                isAuthenticated: false
            };

            if (AllowedWebLogin === true || AllowedWebRegistration === true) {
                App.session.reCaptchaV2InvisiblePublicKey = cookies.get('reCaptchaV2InvisiblePublicKey');
                App.session.reCaptchaV2PublicKey = cookies.get('reCaptchaV2PublicKey');
            }
            else {
                App.session.reCaptchaV2InvisiblePublicKey = '';
                App.session.reCaptchaV2PublicKey = '';
            }
        }
        if (cookies.get('debug') === 'demo') {
            App.session.isDemo = true;
        }
    }

    /**
     * Конвертация объекта в массив
     * @param {object} obj
     */
    static mapObjectToArr(obj) {
        var errArr = Object.keys(obj).map((keyName, i) => { return `${keyName}: ${obj[keyName]}`; })
        return errArr;
    }

    /**
     * Получить расширение файла по имени (вместе с ведущей точкой)
     * @param {string} filename - имя файла
     */
    static getFileExtension(filename) {
        if (!filename) {
            return '';
        }
        if (filename.indexOf('.') < 0) {
            return '';
        }

        return '.' + filename.split('.').pop();
    }

    /**
     * Явялется ли файл (по имени файла) изображением
     * @param {string} filename
     */
    static fileNameIsImage(filename) {
        if (!filename) {
            return false;
        }

        const currentFileExtension = this.getFileExtension(filename).toLowerCase();
        return ['.svg', '.jpg', '.jpeg', '.png', '.gif', '.bmp'].includes(currentFileExtension);
    }

    /**
     * Получить размер файла в виде удобно читаемой строки
     * @param {number} SizeFile - размер в байтах
     */
    static SizeDataAsString(SizeFile) {
        if (isNaN(SizeFile)) {
            return '<NaN> - is Not a Number';
        }
        if (SizeFile < 1024)
            return `${SizeFile} bytes`;
        else if (SizeFile < 1024 * 1024)
            return `${(SizeFile / 1024).toFixed(2)} KB`;
        else
            return `${(SizeFile / 1024 / 1024).toFixed(2)} MB`;
    }

    render() {
        App.readSession();
        return (
            <Layout>
                <Switch>
                    <Route path='/:controller?/:method?/:id?' component={Hub} />
                    <Route component={NotFound} />
                </Switch>
                <footer></footer>
            </Layout>
        );
    }
}
