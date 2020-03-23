////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React, { Component } from 'react';

/** Страница визуализации ошибки доступа к контроллеру */
export class AccessDenied extends Component {
    static displayName = AccessDenied.name;
    render() {
        const returnUrl = new URLSearchParams(this.props.location.search).get("ReturnUrl");
        return (
            <div className="card">
                <div className="card-header">Ошибка доступа</div>
                <div className="card-body">
                    Вы были перенаправлены сюда, потому что у вас нет прав доступа к запрошеной странице: <b>{returnUrl}</b>
                </div>
                <div className="card-footer text-muted">
                    Попробуйте перейти на <a href="/">главную страницу</a>
                </div>
            </div>
        );
    }
}
