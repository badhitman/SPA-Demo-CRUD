////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React, { Component } from 'react';
import { NavLink } from 'react-router-dom'

export class NotFound extends Component {
    static displayName = NotFound.name;
    render() {
        return (
            <div className="card text-center">
                <div className="card-header">Объект не найден</div>
                <div className="card-body">
                    Не возможно отобразить результат запроса. Возможно компонент ещё не реализован или не внедрён в маршрутизацию. Обратитесь к разработчикам.
                    {this.props.children}
                </div>
                <div className="card-footer text-muted">
                    Попробуйте <NavLink to='/' title='перейти на домашнюю страницу'>начать сначала</NavLink>
                </div>
            </div>
        );
    }
}
