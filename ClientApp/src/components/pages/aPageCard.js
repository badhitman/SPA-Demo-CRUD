////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import App from '../../App';
import jQuery from 'jquery';
import { NavLink } from 'react-router-dom';
import { aPage } from './aPage';

/** Карточка объекта. Базовый (типа абстрактный) компонент */
export class aPageCard extends aPage {
    static displayName = aPageCard.name;

    /** имя кнопки отправки данных на сервер (с последующим переходом к списку) */
    okButtonName = 'okButton';
    /** имя кнопки только отправки данных на сервер (без последующего перехода)*/
    saveButtonName = 'saveButton';

    constructor(props) {
        super(props);

        /** Обработчик нажатия кнопки "Ок" */
        this.handleClickButton = this.handleClickButton.bind(this);
        /** Вкл/Выкл объект */
        this.handleClickButtonDisable = this.handleClickButtonDisable.bind(this);
    }

    async ajax() {
        if (App.id) {
            this.apiPostfix = `${this.apiPostfix}/${App.id}`;
        }

        await super.ajax();
    }

    /**
     * Обработчик нажатия кнопки Ok/Записать
     * @param {any} e - context handle button
     */
    async handleClickButton(e) {
        var nameButton = e.target.name;
        var form = e.target.form;

        var response;
        const apiName = App.controller;
        var urlBody = `${this.apiPrefix}/${apiName}${this.apiPostfix}`;

        var sendedFormData = jQuery(form).serializeArray().reduce(function (obj, item) {
            if (item.name.toLowerCase() === 'id' || form[item.name].type.toLowerCase() === 'number' || form[item.name].tagName.toLowerCase() === 'select') {
                obj[item.name] = parseInt(item.value, 10);
            }
            else {
                obj[item.name] = item.value;
            }
            return obj;
        }, {});

        sendedFormData['isDisabled'] = App.data.isDisabled === true;

        try {
            switch (App.method) {
                case App.viewNameMethod:
                    response = await fetch(urlBody, {
                        method: 'PUT',
                        body: JSON.stringify(sendedFormData),
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    });
                    break;
                case App.createNameMethod:
                    response = await fetch(urlBody, {
                        method: 'POST',
                        body: JSON.stringify(sendedFormData),
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    });
                    break;
                case App.deleteNameMethod:
                    response = await fetch(urlBody, {
                        method: 'DELETE',
                        body: JSON.stringify(sendedFormData),
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    });
                    break;
                default:
                    const msg = `Ошибка обработки события нажатия кнопки в контексте {${App.method}}.`;
                    console.error(msg);
                    alert(msg);
                    break;
            }

            if (response.redirected === true) {
                window.location.href = response.url;
            }

            var result = await response.json();
            var domElement;
            if (response.ok) {
                if (result.success === false) {
                    this.clientAlert(result.info, result.status, 1000, 15000);
                    return;
                }

                if (App.method === App.viewNameMethod) {
                    this.clientAlert('Команда успешно выполнена', 'success');
                }
                else if (App.method === App.createNameMethod) {

                    if (nameButton === this.okButtonName) {
                        this.props.history.push(`/${apiName}/${App.listNameMethod}/`)
                    }
                    else {
                        this.props.history.push(`/${apiName}/${App.viewNameMethod}/${result.tag.id}/`);
                    }

                    return;
                }
                else if (App.method === App.deleteNameMethod) {
                    this.props.history.push(`/${apiName}/${App.listNameMethod}/`);
                }
            }
            else {
                var errorsString = App.mapObjectToArr(result.errors).join('<br/>');
                domElement = jQuery(`<div class="mt-2 alert alert-danger" role="alert"><h4 class="alert-heading">${result.title}</h4><p>${errorsString}</p><hr/><p>traceId: ${result.traceId}</p></div>`);
                jQuery(form).after(domElement.hide().fadeIn(1000, 'swing', function () { domElement.fadeOut(15000); }));
                const msg = `Ошибка обработки HTTP запроса. Status: ${response.status}`;
                console.error(msg);
                return;
            }

            if (nameButton === this.okButtonName) {
                this.props.history.push(`/${apiName}/${App.listNameMethod}/`);
            }
        } catch (error) {
            const msg = `Ошибка: ${error}`;
            console.error(msg);
            alert(msg);
        }
    }

    /** Вкл/Выкл объект */
    async handleClickButtonDisable() {
        const response = await fetch(`${this.apiPrefix}/${App.controller}${this.apiPostfix}`, { method: 'PATCH' });
        if (response.ok) {
            var result = await response.json();
            App.data.isDisabled = result.tag;
            this.forceUpdate();
            this.clientAlert(result.info, result.status);
        }
    }

    cardHeaderPanel() {
        if (App.data.isDisabled === true) {
            return <button onClick={this.handleClickButtonDisable} type="button" title='объект выключен. для включения - нажмите на кнопку' className="badge badge-pill badge-secondary">Выкл</button>;
        }
        else {
            return <button onClick={this.handleClickButtonDisable} title='объект включен. для выключения - нажмите на кнопку' className="badge badge-pill badge-primary">Вкл</button>;
        }
    }

    /** Набор кнопок управления для формы просмотра/редактирования объекта */
    viewButtons() {
        return (<div className="btn-toolbar justify-content-end" role="toolbar" aria-label="Toolbar with button groups">
            <div className="btn-group" role="group" aria-label="First group">
                <button name={this.okButtonName} onClick={this.handleClickButton} type="button" className="btn btn-outline-success" title='Сохранить и перейти к списку'>Ok</button>
                <button name={this.saveButtonName} onClick={this.handleClickButton} type="button" className="btn btn-outline-success" title='Записать в базу данных и продолжить редактирование'>Записать</button>
                <NavLink className='btn btn-outline-primary' to={`/${App.controller}/${App.listNameMethod}/`} role='button' title='Вернуться к списку без сохранения'>Вернуться к списку</NavLink>
                <NavLink className='btn btn-outline-danger' to={`/${App.controller}/${App.deleteNameMethod}/${App.data.id}/`} role='button' title='Удалить объект из базы данных'>Удаление</NavLink>
            </div>
        </div>);
    }

    /** Набор кнопок управления для формы создания объекта */
    createButtons() {
        return (<div className="btn-toolbar justify-content-end" role="toolbar" aria-label="Toolbar with button groups">
            <div className="btn-group" role="group" aria-label="First group">
                <button name={this.okButtonName} onClick={this.handleClickButton} type="button" className="btn btn-outline-success" title='Сохранить и перейти к списку'>Ok</button>
                <button name={this.saveButtonName} onClick={this.handleClickButton} type="button" className="btn btn-outline-success" title='Записать в базу данных и продолжить редактирование'>Записать</button>
                <NavLink className='btn btn-outline-primary' to={`/${App.controller}/${App.listNameMethod}/`} role='button' title='Вернуться к списку без сохранения'>Отмена</NavLink>
            </div>
        </div>);
    }

    /** Набор кнопок управления для формы удаления объекта */
    deleteButtons() {
        return (<>
            <NavLink className='btn btn-primary btn-block' to={`/${App.controller}/${App.listNameMethod}/`} role='button' title='Вернуться к списку'>Отмена</NavLink>
            <button name={this.okButtonName} onClick={this.handleClickButton} type="button" className="btn btn-outline-danger btn-block" title='Подтвердить удаление объекта'>Подтверждение удаления</button>
        </>);
    }
}