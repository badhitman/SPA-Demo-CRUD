////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React, { Component } from 'react';
import { NavLink } from 'react-router-dom'
import App from '../App';

/** Страница визуализации ошибки доступа к контроллеру */
export class AccessDenied extends Component {
    static displayName = AccessDenied.name;
    render() {
        const returnUrl = new URLSearchParams(this.props.location.search).get("ReturnUrl");
        return (
            <div className="card">
                <div className="card-header">Доступ запрещён</div>
                <div className="card-body">
                    Вы были перенаправлены на эту страницу, потому что ваши полномочия/роль [{App.session.role}] не имеет прав доступа к запрошеному web-api: <b>{returnUrl}</b>
                </div>
                <div className="card-footer text-muted">
                    Попробуйте перейти на <NavLink to='/' title='перейти на домашнюю страницу'>главную страницу</NavLink>
                </div>
            </div>
        );
    }
}
