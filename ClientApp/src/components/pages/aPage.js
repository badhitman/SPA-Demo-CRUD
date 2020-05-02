////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React, { Component } from 'react';
import { NavLink } from 'react-router-dom';
import jQuery from 'jquery';
import App from '../../App';

/** Базовый (типа абстрактный) компонент */
export class aPage extends Component {
    static displayName = aPage.name;
    stub = <div className="alert alert-danger" role="alert">Заглушка. Требуется переопределения в потомке</div>

    /** Префикс/приставка url тела запроса к api */
    apiPrefix = '/api';
    /** Постфикс/окончание url тела запроса к api  */
    apiPostfix = '';
    /** Строка запроса к api (та что: ?nameParam=valueParam&nameParam2=valueParam2) */
    apiQuery = '';

    /** имя кнопки отправки данных на сервер (с последующим переходом к списку) */
    okButtonName = 'okButton';
    /** имя кнопки только отправки данных на сервер (без последующего перехода)*/
    saveButtonName = 'saveButton';

    /** Текст заголовка карточки */
    cardTitle = 'Заголовок карточки';

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
            loading: true
        };

        /** Обработчик нажатия кнопки "Ок" */
        this.handleClickButton = this.handleClickButton.bind(this);
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
            if (item.name.toLowerCase() === 'id' || form[item.name].type.toLowerCase() === 'number' || form[item.name].tagName.toLowerCase() === 'select' || form[item.name].attributes.isinteger) {
                obj[item.name] = parseInt(item.value, 10);
            }
            else if (form[item.name].attributes.isdouble) {
                obj[item.name] = parseFloat(item.value.replace(',', '.'));
            }
            else if (form[item.name].type.toLowerCase() === 'checkbox') {
                obj[item.name] = form[item.name].checked === true || form[item.name].value.toLowerCase() === 'on' || form[item.name].value.toLowerCase() === 'checked' || form[item.name].value.toLowerCase() === 'true';
            }
            else {
                obj[item.name] = item.value;
            }
            return obj;
        }, {});

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
                        this.props.history.push(`/${apiName}/${App.viewNameMethod}/${result.tag}/`);
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

    componentDidMount() {
        this.load();
    }

    pushApiQueryParametr(paramName, paramValue) {
        this.apiQuery = this.apiQuery.trim();
        if (this.apiQuery && this.apiQuery.length > 0) {
            this.apiQuery = this.apiQuery + `&${paramName}=${paramValue}`;
        }
        else {
            this.apiQuery = `${paramName}=${paramValue}`;
        }
    }

    /** получение данных с сервера */
    async ajax() {
        const response = await fetch(`${this.apiPrefix}/${App.controller}${this.apiPostfix}${this.apiQuery ? '?' + this.apiQuery : ''}`);
        if (response.redirected === true) {
            window.location.href = response.url;
            return;
        }

        try {
            if (response.ok === true) {
                const result = await response.json()
                App.data = result.tag;

                if (result.success === false) {
                    this.clientAlert(result.info, result.status, 1000, 10000);
                }
            }
            else {
                App.data = undefined;
                console.error(`Ошибка обработки запроса. CODE[${response.status}] Status[${response.statusText}]`);
                this.clientAlert(`Ошибка обработки запроса. CODE[${response.status}] Status[${response.statusText}]`, 'danger', 1000, 10000);
            }
        }
        catch (err) {
            console.error(err);
            this.clientAlert(err, 'danger');
        }
    }

    /**
     * загрузка окна
     * @param {boolean} continueLoading - true если требуется дополнительная работа перед финальной отрисовкой. false - если окно готово к финальной отрисовке
     */
    async load(continueLoading = false) {
        await this.ajax();
        this.setState(
            {
                name: App.data.name,
                information: App.data.information,

                loading: continueLoading === true,

                isReadonly: App.data.isReadonly === true,
                isDisabled: App.data.isDisabled === true,
                isGlobalFavorite: App.data.isGlobalFavorite === true
            });
    }

    /**
     * Отправка уведомления клиенту в виде ALERT
     * @param {string} message - текст сообщения
     * @param {string} status - bootstrap статус (цветовое оформление: primary, secondary, success, danger, warning)
     * @param {number} fadeIn - скорость вывода сообщения
     * @param {number} fadeOut - скорость увядания сообщения
     * @param {string} fadeBehavior - jquery флаг поведения анимации
     */
    clientAlert(message, status = 'secondary', fadeIn = 1000, fadeOut = 5000, fadeBehavior = 'swing') {
        var domElement = jQuery(`<div class="mt-2 alert alert-${status}" role="alert">${message}</div>`);
        jQuery('footer').last().after(domElement.fadeIn(fadeIn, fadeBehavior, function () { domElement.fadeOut(fadeOut); }));
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
                        <input name={keyName} id={keyName} readOnly={true} defaultValue={(obj[keyName].name ? obj[keyName].name : obj[keyName])} className='form-control' type='text' />
                    </div>
                </div>
        })
    }

    /** Рендер функциональной панели, размещённого в заголовочной части карточки (прижата к правой части) */
    cardHeaderPanel() {
        return <></>;
    }

    /** Рендер тела карточки страницы */
    cardBody() {
        return this.stub;
    }

    rootPanelObject() {
        if (App.session.role.toLowerCase() === 'root') {
            return <>
                <div className="custom-control custom-checkbox">
                    <input type="checkbox" className="custom-control-input" id="customCheck1" name='isReadonly' defaultChecked={(App.data.isReadonly === true)} />
                    <label className="custom-control-label" htmlFor="customCheck1">только для чтения</label>
                </div>
                <div className="custom-control custom-checkbox">
                    <input type="checkbox" className="custom-control-input" id="customCheck2" name='isDisabled' defaultChecked={(App.data.isDisabled === true)} />
                    <label className="custom-control-label" htmlFor="customCheck2">отключить/деактивировать</label>
                </div>
                <div className="custom-control custom-checkbox">
                    <input type="checkbox" className="custom-control-input" id="customCheck3" name='IsGlobalFavorite' defaultChecked={(App.data.isGlobalFavorite === true)} />
                    <label className="custom-control-label" htmlFor="customCheck3">Избранное (для всех)</label>
                </div>
            </>
        }

        return <></>;
    }

    getInformation() {
        return <div className="form-group">
            <label htmlFor="infirmationFormControlTextarea">Комментарий</label>
            <textarea defaultValue={App.data.information} id="infirmationFormControlTextarea" name='information' className="form-control" rows="3" placeholder='Комментарий/примечание'></textarea>
        </div>
    }

    /** Набор кнопок управления для формы просмотра/редактирования объекта */
    viewButtons(allowDelete = true) {
        return (<div className="btn-toolbar justify-content-end" role="toolbar" aria-label="Toolbar with button groups">
            <div className="btn-group" role="group" aria-label="First group">
                <button name={this.okButtonName} onClick={this.handleClickButton} type="button" className="btn btn-outline-success" title='Сохранить и перейти к списку'>Ok</button>
                <button name={this.saveButtonName} onClick={this.handleClickButton} type="button" className="btn btn-outline-success" title='Записать в базу данных и продолжить редактирование'>Запись</button>
                <NavLink className='btn btn-outline-primary' to={`/${App.controller}/${App.listNameMethod}/`} role='button' title='Вернуться к списку без сохранения'>Выйти</NavLink>
                {allowDelete === true
                    ? <NavLink className='btn btn-outline-danger' to={`/${App.controller}/${App.deleteNameMethod}/${App.data.id}/`} role='button' title='Удалить объект из базы данных'>Удаление</NavLink>
                    : <button disabled className='btn btn-outline-danger' title='Удалить объект из базы данных нельзя'>Удаление недоступно</button>}
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
    deleteButtons(allowDelete = true) {
        return allowDelete === true
            ? (<>
                <NavLink className='btn btn-primary btn-block' to={`/${App.controller}/${App.viewNameMethod}/${App.id}`} role='button' title='Перейти к редактированию объекта'>Редактировать</NavLink>
                <NavLink className='btn btn-outline-primary btn-block' to={`/${App.controller}/${App.listNameMethod}/`} role='button' title='Вернуться к списку/справочнику'>Отмена</NavLink>
                <button name={this.okButtonName} onClick={this.handleClickButton} type="button" className="btn btn-outline-danger btn-block" title='Подтвердить удаление объекта'>Подтверждение удаления</button>
            </>)

            : (<>
                <NavLink className='btn btn-primary btn-block' to={`/${App.controller}/${App.viewNameMethod}/${App.id}`} role='button' title='Перейти к редактированию объекта'>Редактировать</NavLink>
                <NavLink className='btn btn-outline-primary btn-block' to={`/${App.controller}/${App.listNameMethod}/`} role='button' title='Вернуться к списку'>Отмена</NavLink>
                <button disabled type="button" className="btn btn-outline-danger btn-block" title='Удаление объекта недоступно'>Удаление невозможно</button>
            </>);
    }

    render() {
        if (this.state.loading === true) {
            return <p>load...</p>;
        }
        const data = App.data;
        if (data === null) {// || App.data === undefined
            this.load();
            return <p>ajax...</p>;
        }

        if (data === undefined) {
            return <p>ajax context tag-data is undefined...</p>;
        }

        return (
            <>
                <div className="card">
                    <div className="card-header">
                        <div className="d-flex">
                            <div className="mr-auto p-2">{this.cardTitle}</div>
                            <div className="p-2">{this.cardHeaderPanel()}</div>
                        </div>
                    </div>
                    <div className="card-body pb-3">
                        {this.cardBody()}
                    </div>
                </div>
            </>
        );
    }
}