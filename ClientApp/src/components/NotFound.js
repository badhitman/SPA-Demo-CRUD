////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
import React, { Component } from 'react';

export class NotFound extends Component {
    static displayName = NotFound.name;
    render() {
        return (
            <div className="card text-center">
                <div className="card-header">Ошибка</div>
                <div className="card-body">
                    Не возможно отобразить результат запроса
                    {this.props.children}
                </div>
                <div className="card-footer text-muted">
                    Попробуйте <a href="/">начать сначала</a>
                </div>
            </div>
        );
    }
}
