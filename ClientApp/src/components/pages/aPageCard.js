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
    static form;

    apiName = '';
    /** имя кнопки отправки данных на сервер (с последующим переходом к списку) */
    okButtonName = 'okButton';
    /** имя кнопки только отправки данных на сервер (без последующего перехода)*/
    saveButtonName = 'saveButton';

    constructor(props) {
        super(props);

        this.handleClickButton = this.handleClickButton.bind(this);
    }

    /**
     * Обработчик нажатия кнопки сохранения данных
     * @param {any} e - context handle button
     */
    async handleClickButton(e) {
        var nameButton = e.target.name;
        aPageCard.form = e.target.form;

        var response;
        const apiName = this.apiName;

        var sendedFormData = jQuery(aPageCard.form).serializeArray().reduce(function (obj, item) {
            if (item.name.toLowerCase() === 'id' || aPageCard.form[item.name].type.toLowerCase() === 'number' || aPageCard.form[item.name].tagName.toLowerCase() === 'select') {
                obj[item.name] = parseInt(item.value);
            }
            else {
                obj[item.name] = item.value;
            }
            return obj;
        }, {});

        try {
            switch (App.method) {
                case App.viewNameMethod:
                    response = await fetch(`/api/${apiName}/${App.data.id}`, {
                        method: 'PUT',
                        body: JSON.stringify(sendedFormData),
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    });
                    break;
                case App.createNameMethod:
                    response = await fetch(`/api/${apiName}/`, {
                        method: 'POST',
                        body: JSON.stringify(sendedFormData),
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    });
                    break;
                case App.deleteNameMethod:
                    response = await fetch(`/api/${apiName}/${App.data.id}`, {
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
                    domElement = jQuery(`<div class="mt-2 alert alert-${result.status}" role="alert">${result.info}</div>`);
                    jQuery(aPageCard.form).after(domElement.hide().fadeIn(1000, 'swing', function () { domElement.fadeOut(15000); }));
                    return;
                }

                if (App.method === App.viewNameMethod) {
                    domElement = jQuery('<div class="alert alert-success" role="alert">Команда успешно выполнена: <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button></div>');
                    jQuery(aPageCard.form).after(domElement.hide().fadeIn(1000, 'swing', function () { domElement.fadeOut(1000); }));
                }
                else if (App.method === App.createNameMethod) {
                    this.props.history.push(`/${apiName}/${App.viewNameMethod}/${result.id}/`);
                    return;
                }
                else if (App.method === App.deleteNameMethod) {
                    this.props.history.push(`/${apiName}/${App.listNameMethod}/`);
                }
            }
            else {
                var errorsString = App.mapObjectToArr(result.errors).join('<br/>');
                domElement = jQuery(`<div class="mt-2 alert alert-danger" role="alert"><h4 class="alert-heading">${result.title}</h4><p>${errorsString}</p><hr/><p>traceId: ${result.traceId}</p></div>`);
                jQuery(aPageCard.form).after(domElement.hide().fadeIn(1000, 'swing', function () { domElement.fadeOut(15000); }));
                const msg = `Ошибка обработки HTTP запроса. Status: ${response.status}`;
                console.error(msg);
                return;
            }

            aPageCard.form = undefined;

            if (nameButton === this.okButtonName) {
                this.props.history.push(`/${apiName}/${App.listNameMethod}/`);
            }
        } catch (error) {
            const msg = `Ошибка: ${error}`;
            console.error(msg);
            alert(msg);
        }
    }

    /**
     * Отрисовка объекьа в виде тела формы
     * @param {object} obj - объект для отрисовки
     * @param {array} skipFields - перечень полей, которые отрисовывать не нужно
     */
    mapObjectToReadonlyForm(obj, skipFields = []) {
        return Object.keys(obj).map((keyName, i) => {
            return Array.isArray(obj[keyName]) || skipFields.includes(keyName)
                ?
                <React.Fragment key={i}></React.Fragment>
                :
                <div className='form-group row' key={i}>
                    <label htmlFor={keyName} className='col-sm-2 col-form-label'>{keyName}</label>
                    <div className='col-sm-10'>
                        <input name={keyName} id={keyName} readOnly={true} defaultValue={obj[keyName]} className='form-control' type='text' />
                    </div>
                </div>
        })
    }

    /** Набор кнопок управления для формы просмотра/редактирования объекта */
    viewButtons() {
        return (<div className="btn-toolbar justify-content-end" role="toolbar" aria-label="Toolbar with button groups">
            <div className="btn-group" role="group" aria-label="First group">
                <button name={this.okButtonName} onClick={this.handleClickButton} type="button" className="btn btn-outline-success" title='Сохранить и перейти к списку'>Ok</button>
                <button name={this.saveButtonName} onClick={this.handleClickButton} type="button" className="btn btn-outline-success" title='Записать в базу данных и продолжить редактирование'>Записать</button>
                <NavLink className='btn btn-outline-primary' to={`/${this.apiName}/${App.listNameMethod}/`} role='button' title='Вернуться к списку без сохранения'>Вернуться к списку</NavLink>
                <NavLink className='btn btn-outline-danger' to={`/${this.apiName}/${App.deleteNameMethod}/${App.data.id}/`} role='button' title='Удалить объект из базы данных'>Удаление</NavLink>
            </div>
        </div>);
    }

    /** Набор кнопок управления для формы создания объекта */
    createButtons() {
        return (<div className="btn-toolbar justify-content-end" role="toolbar" aria-label="Toolbar with button groups">
            <div className="btn-group" role="group" aria-label="First group">
                <button name={this.okButtonName} onClick={this.handleClickButton} type="button" className="btn btn-outline-success" title='Сохранить и перейти к списку'>Ok</button>
                <button name={this.saveButtonName} onClick={this.handleClickButton} type="button" className="btn btn-outline-success" title='Записать в базу данных и продолжить редактирование'>Записать</button>
                <NavLink className='btn btn-outline-primary' to={`/${this.apiName}/${App.listNameMethod}/`} role='button' title='Вернуться к списку без сохранения'>Отмена</NavLink>
            </div>
        </div>);
    }

    /** Набор кнопок управления для формы удаления объекта */
    deleteButtons() {
        return (<>
            <NavLink className='btn btn-primary btn-block' to={`/${this.apiName}/${App.listNameMethod}/`} role='button' title='Вернуться к списку'>Отмена</NavLink>
            <button name={this.okButtonName} onClick={this.handleClickButton} type="button" className="btn btn-outline-danger btn-block" title='Подтвердить удаление объекта'>Подтверждение удаления</button>
        </>);
    }
}