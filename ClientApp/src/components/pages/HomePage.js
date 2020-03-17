////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
import React, { Component } from 'react';
import App from '../../App';

export class HomePage extends Component {
    static displayName = HomePage.name;

    render() {
        return (
            <div className="card">
                <div className="card-header">SPA lite app for demo</div>
                <div className="card-body">
                    <blockquote className="blockquote text-right mb-0">
                        <p className='lead mb-0'>Мания моя как у пиратов была на Карибах,</p>
                        <p className='lead'>Покорить мир, как его покорил Бах.</p>
                        <footer className="blockquote-footer"><a href='https://music.yandex.ru/album/63756/track/597196' className='text-secondary' target='_blank' rel="noopener noreferrer">Вокруг шум</a> [<cite title="Source Title"><a href='https://music.yandex.ru/artist/41126' className='text-muted' target='_blank' rel="noopener noreferrer">Каста</a></cite>]</footer>
                    </blockquote>
                </div>
                <div className="card-footer text-muted">
                    {App.session.isAuthenticated === true
                        ? `Приветсвую Вас, ${App.session.name}! Вы авторизованы в роли [${App.session.role}].`
                        : `для работы в приложении требуется авторизация`}
                </div>
            </div>
        );
    }
}