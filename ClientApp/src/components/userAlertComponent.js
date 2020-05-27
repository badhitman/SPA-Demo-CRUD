////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import { Component } from 'react';
import jQuery from 'jquery';

/** Базовый (типа абстрактный) компонент с поддержкой вывода пользовательского уведомления */
export class userAlertComponent extends Component {
    static displayName = userAlertComponent.name;

    /**
     * Отправка уведомления клиенту в виде ALERT
     * @param {string} message - текст сообщения
     * @param {string} anchor - анкер/селектор привязки dom объекта сообщения (например div или footer)
     * @param {string} status - bootstrap статус (цветовое оформление: primary, secondary, success, danger, warning)
     * @param {number} fadeIn - скорость вывода/появления/отрисовки dom объекта сообщения
     * @param {number} fadeOut - скорость увядания сообщения
     * @param {string} fadeBehavior - jquery название поведения анимации
     */
    clientAlert(message, status = 'primary', anchor = 'div', fadeIn = 1000, fadeOut = 5000, fadeBehavior = 'swing') {
        var domElement = jQuery(`<div class="mt-2 alert alert-${status}" role="alert">${message}</div>`);
        jQuery(anchor).last().after(domElement.fadeIn(fadeIn, fadeBehavior, function () { domElement.fadeOut(fadeOut); }));
    }
}