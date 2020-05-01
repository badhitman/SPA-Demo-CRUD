////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React, { Component } from 'react';
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

    /** Последнее значение строки запроса (this.props.location.search) */
    static prewQuery = '';

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
            name: '',
            information: '',

            loading: true,

            isReadonly: false,
            isDisabled: false,
            isGlobalFavorite: false
        };

        this.rootPanelCheckboxChangeHandler = this.rootPanelCheckboxChangeHandler.bind(this);
        this.InformationTextareaChangeHandler = this.InformationTextareaChangeHandler.bind(this);
    }

    InformationTextareaChangeHandler(e) {
        const target = e.target;
        this.setState({ information: target.value });
    }

    rootPanelCheckboxChangeHandler(e) {
        const target = e.target;
        const checkboxName = target.name;
        switch (checkboxName.toLowerCase()) {
            case 'isreadonly':
                this.setState({ isReadonly: target.checked === true });
                break;
            case 'isdisabled':
                this.setState({ isDisabled: target.checked === true });
                break;
            case 'isglobalfavorite':
                this.setState({ isGlobalFavorite: target.checked === true });
                break;
            default:
                console.error('Произошла ошибка во время отработки события переключения чекбокс-ов в root панели ([isReadonly][isdisabled][isGlobalFavorite]). Неизвестное имя поля формы: ' + checkboxName.toLowerCase());
                break;
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
        return this.stub;
    }

    /** Рендер тела карточки страницы */
    cardBody() {
        return this.stub;
    }

    rootPanelObject() {
        if (App.session.role.toLowerCase() === 'root') {
            return <>
                <div className="custom-control custom-checkbox">
                    <input type="checkbox" className="custom-control-input" id="customCheck1" name='isReadonly' checked={this.state.isReadonly} onChange={this.rootPanelCheckboxChangeHandler} />
                    <label className="custom-control-label" htmlFor="customCheck1">только для чтения</label>
                </div>
                <div className="custom-control custom-checkbox">
                    <input type="checkbox" className="custom-control-input" id="customCheck2" name='isDisabled' checked={this.state.isDisabled} onChange={this.rootPanelCheckboxChangeHandler} />
                    <label className="custom-control-label" htmlFor="customCheck2">отключить/деактивировать</label>
                </div>
                <div className="custom-control custom-checkbox">
                    <input type="checkbox" className="custom-control-input" id="customCheck3" name='IsGlobalFavorite' checked={this.state.isGlobalFavorite} onChange={this.rootPanelCheckboxChangeHandler} />
                    <label className="custom-control-label" htmlFor="customCheck3">Избранное (для всех)</label>
                </div>
            </>
        }

        return <></>;
    }

    getInformation() {
        return <div className="form-group">
            <label htmlFor="infirmationFormControlTextarea">Комментарий</label>
            <textarea value={this.state.information} onChange={this.InformationTextareaChangeHandler} id="infirmationFormControlTextarea" name='information' className="form-control" rows="3" placeholder='Комментарий/примечание'></textarea>
        </div>
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

        //const search = this.props.location.search;

        //if (search !== aPage.prewQuery) {
        //    aPage.prewQuery = search;
        //    this.load();
        //    return <p>reload...</p>;
        //}

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