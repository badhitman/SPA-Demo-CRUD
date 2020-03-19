////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
import React, { Component } from 'react';
import { Route, Switch } from 'react-router';
import { Layout } from './components/Layout';
import { NotFound } from './components/NotFound';
import { Hub } from './components/Hub';

import './custom.css'

/** Контекст приложения */
export default class App extends Component {
    static displayName = App.name;

    /** доступные типы данных (список REST котроллеров) */
    static allowsControllers = ['users', 'departments', 'signin'];

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

    /** Имя запрашиваемого метода */
    static method;
    /** ID запрашиваемого объекта */
    static id;

    /** context data */
    static data = { id: '', name: '' };

    /** context session */
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
            AllowedWebRegistration: false,

            /** Сообщение/пояснение ответа сервера */
            message: ''
        };

    render() {
        return (
            <Layout>
                <Switch>
                    <Route path='/:controller?/:method?/:id?' component={Hub} />
                    <Route component={NotFound} />
                </Switch>
            </Layout>
        );
    }
}
