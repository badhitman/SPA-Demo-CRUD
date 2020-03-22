////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
import React, { Component } from 'react';

export class AccessDenied extends Component {
    static displayName = AccessDenied.name;
    render() {
        return (
            <div className="card">
                <div className="card-header">Ошибка доступа</div>
                <div className="card-body">
                    Вы были перенаправлены сюда, потому что у вас нет прав доступа к запрошеной странице
                    {this.props.children}
                </div>
                <div className="card-footer text-muted">
                    Попробуйте <a href="/">начать сначала</a>
                </div>
            </div>
        );
    }
}
