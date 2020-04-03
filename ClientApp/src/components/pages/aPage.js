////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React, { Component } from 'react';
import jQuery from 'jquery';
import App from '../../App';

/** Базовый (типа абстрактный) компонент */
export class aPage extends Component {
    static displayName = aPage.name;

    /** Префикс/приставка url тела запроса к api */
    apiPrefix = '/api';
    /** имя сущьности api (тип данных) */
    apiName = '';
    /** Постфикс/окончание url тела запроса к api  */
    apiPostfix = '';

    constructor(props) {
        super(props);

        /** Создание метода Array.isArray(), если он ещё не реализован в браузере. */
        if (!Array.isArray) {
            Array.isArray = function (arg) {
                return Object.prototype.toString.call(arg) === '[object Array]';
            };
        }

        this.state =
        {
            cardTitle: 'Loading...',
            loading: true
        };
    }

    componentDidMount() {
        this.load();
    }

    /**
     * Отправка уведомления клиенту в виде ALERT
     * @param {string} message - текст сообщения
     * @param {string} status - bootstrap статус (цветовое оформление: primary, secondary, success, danger, warning)
     * @param {number} fadeIn - скорость вывода сообщения
     * @param {number} fadeOut - скорость увядания сообщения
     * @param {string} fadeBehavior - jquery флаг поведения анимации
     */
    clientAlert(message, status, fadeIn = 1000, fadeOut = 1000, fadeBehavior = 'swing') {
        var domElement = jQuery(`<div class="mt-2 alert alert-${status}" role="alert">${message}</div>`);
        jQuery('.card:first').after(domElement.hide().fadeIn(fadeIn, fadeBehavior, function () { domElement.fadeOut(fadeOut); }));
    }

    /** Рендер функциональной панели, размещённого в заголовочной части карточки (прижата к правой части) */
    cardHeaderPanel() {
        return <>Заглушка. Требуется переопределения в потомке</>;
    }

    /** Рендер тела карточки страницы */
    cardBody() {
        return <>Заглушка. Требуется переопределения в потомке</>;
    }

    render() {
        if (App.data === null) {
            this.load();
            return <>reload ...</>;
        }
        return (
            <>
                <div className="card">
                    <div className="card-header">
                        <div className="d-flex">
                            <div className="mr-auto p-2">{this.state.cardTitle}</div>
                            <div className="p-2">{this.cardHeaderPanel()}</div>
                        </div>
                    </div>
                    <div className="card-body pb-1">
                        {this.state.loading === true ? <p><em>Загрузка данных...</em></p> : this.cardBody()}
                    </div>
                </div>
            </>
        );
    }
}