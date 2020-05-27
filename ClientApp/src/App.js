////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React, { Component } from 'react';

import { Route, Switch, Redirect } from 'react-router';
import { NavLink } from 'react-router-dom';
import { Layout } from './components/Layout';
import { NotFound } from './components/NotFound';
import { Hub } from './Hub';

import Cookies from 'universal-cookie';

import './custom.css'

/** контекст SPA/UI приложения */
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
            /** просмотр журнала складских документов */
            'warehousedocuments',
            /** отчёты по номенклатуре и складам */
            'warehousesreports',
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
    /** Список доступных методов (aka CRUD) */
    static allowsMethods = [App.listNameMethod, App.viewNameMethod, App.createNameMethod, App.deleteNameMethod];

    /** Имя запрашиваемого контроллера (из react props/route) */
    static controller = '';
    /** Имя запрашиваемого метода (из react props/route) */
    static method = '';
    /** ID запрашиваемого объекта (из react props/route) */
    static id;

    /** полученные с сервера (ajax) контекстные данные */
    static data = null;

    /** user session context */
    static session = {
        /** Пользователь авторизован? */
        isAuthenticated: false,
        /** Имя (публичное) авторизованного пользователя */
        name: '',
        /** Роль, под которой авторизован пользователь в текущей сессии. При изменении роли пользователю на сторое сервера - необходимо заново авторизоваться в spa/gui.
         * Не смотря на то что в контексте сервера новые права/роль начинают дейтсвовать немедленно, пользовательский интерфейс имя текущей роли пользователя получает в момент входа */
        role: ''
    };
    /** Чтение состояния сессии пользователя */
    static readSession() {
        const cookies = new Cookies();
        const name = cookies.get('name');
        const role = cookies.get('role');

        if (name && name.length > 0) {
            App.session = {
                name: name,
                role: role,
                isAuthenticated: true
            };
        }
        else {
            App.session = {
                isAuthenticated: false
            };
        }

        if (cookies.get('debug').toLowerCase() === 'demo') {
            App.session.isDemo = true;
        }
    }

    /** текущие настройки WEB формы входа/регистрации */
    static webAuthSettings = {
        /** Разрешение авторизаии через Web форму */
        AllowedWebLogin: false,
        /** Разрешение регистрации через Web форму */
        AllowedWebRegistration: false
    }
    /** чтение настроек авторизации через Web форму */
    static webAuthSettingsRead() {
        if (App.session.isAuthenticated === true) {
            if (App.webAuthSettings) {
                delete App.webAuthSettings;
            }
            return;
        }

        const cookies = new Cookies();
        //
        const AllowedWebLogin = `${cookies.get('AllowedWebLogin')}`.toLowerCase() === 'true';
        const AllowedWebRegistration = `${cookies.get('AllowedWebRegistration')}`.toLowerCase() === 'true';

        App.webAuthSettings = {
            AllowedWebLogin,
            AllowedWebRegistration
        }
    }

    /** текущие настройки reCaptcha */
    static reCaptchaSettings = {
        /** Публичный ключ для работы reCaptcha v2 /Invisible sub-ver./ */
        reCaptchaV2InvisiblePublicKey: '',
        /** Публичный ключ для работы reCaptcha v2 /Widget sub-ver./ */
        reCaptchaV2PublicKey: ''
    }
    /** чтение настроек reCaptcha */
    static reCaptchaSettingsRead() {
        const cookies = new Cookies();

        const reCaptchaV2InvisiblePublicKey = cookies.get('reCaptchaV2InvisiblePublicKey');
        const reCaptchaV2PublicKey = cookies.get('reCaptchaV2PublicKey');

        App.reCaptchaSettings = {
            reCaptchaV2InvisiblePublicKey,
            reCaptchaV2PublicKey
        };
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
        if (App.session.isAuthenticated === true) {
            if (App.webAuthSettings) {
                delete App.webAuthSettings;
            }
        }
        else {
            App.webAuthSettingsRead();
        }
        App.reCaptchaSettingsRead();

        return (
            <Layout>
                <Switch>
                    <Redirect from={`/goods/${App.listNameMethod}`} to={`/groupsgoods/${App.listNameMethod}`} />
                    <Route path='/:controller?/:method?/:id?' component={Hub} />
                    <Route component={NotFound} />
                </Switch>
                <footer></footer>
            </Layout>
        );
    }

    static get emptyCardBody() {
        return (
            <>
                <p className="lead">Запрашиваемая страница находится в процессе разработки...</p>
                Перейти <NavLink to={`/${App.controller}/${App.listNameMethod}`}>к списку </NavLink> {App.method.toLowerCase() === App.deleteNameMethod.toLowerCase() && !isNaN(App.id) ? <> или <NavLink to={`/${App.controller}/${App.viewNameMethod}/${App.id}`}>открыть карточку объекта</NavLink></> : <></>}
            </>
        );
    }
}
