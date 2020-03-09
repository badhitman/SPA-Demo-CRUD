import React, { Component } from 'react';
import { Home } from '../Home';
import jQuery from 'jquery';
import { NavLink } from 'react-router-dom'

/** Базовый (типа абстрактный) компонент */
export class aPage extends Component {
    static displayName = aPage.name;

    constructor(props) {
        super(props);

        this.state =
        {
            cartTitle: 'Loading...',
            cartContents: <></>,
            loading: true
        };
    }

    componentDidMount() {
        this[Home.method + 'Load']();
    }

    render() {
        if (this.state.loading) {
            return <p><em>Loading...</em></p>;
        }

        return (
            <>
                <div className="card">
                    <div className="card-header">
                        {this.state.cartTitle}
                    </div>
                    <div className="card-body">
                        {this.state.cartContents}
                    </div>
                </div>
            </>
        );
    }
}

/** Списки/Справочники. Базовый (типа абстрактный) компонент */
export class aPageList extends aPage {
    static displayName = aPageList.name;
    apiName = '';
    listCardHeader = '';

    async listLoad() {
        const response = await fetch(`/api/${this.apiName}/`);
        Home.data = await response.json();
        this.setState({ cartTitle: this.listCardHeader, loading: false, cartContents: this.listRender() });
    }
}

/** Карточка объекта. Базовый (типа абстрактный) компонент */
export class aPageCard extends aPage {
    static displayName = aPageCard.name;
    apiName = '';

    constructor(props) {
        super(props);

        /** имя кнопки отправки данных на сервер (с последующим переходом к списку) */
        this.okButtonName = 'okButton';
        /** имя кнопки отправки данных на сервер */
        this.saveButtonName = 'saveButton';

        this.handleClickButton = this.handleClickButton.bind(this);
    }

    /**
     * Обработчик нажатия кнопки сохранения данных
     * @param {any} e - context handle button
     */
    async handleClickButton(e) {
        var nameButton = e.target.name;
        var form = e.target.form;
        //var formData;
        var result;
        const apiName = this.apiName;

        var sendedFormData = jQuery(form).serializeArray().reduce(function (obj, item) {
            const inputName = item.name.toLowerCase();

            // TODO: нужно избавиться от этих костылей
            if (inputName === 'id' || inputName === 'departmentid') {
                obj[item.name] = parseInt(item.value);
            }
            else {
                obj[item.name] = item.value;
            }

            return obj;
        }, {});

        try {
            switch (Home.method) {
                case Home.viewNameMethod:
                    result = await fetch(`/api/${apiName}/${Home.data.id}`, {
                        method: 'PUT',
                        body: JSON.stringify(sendedFormData),
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    });
                    break;
                case Home.createNameMethod:
                    result = await fetch(`/api/${apiName}/`, {
                        method: 'POST',
                        body: JSON.stringify(sendedFormData),
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    });
                    break;
                case Home.deleteNameMethod:
                    result = await fetch(`/api/${apiName}/${Home.data.id}`, {
                        method: 'DELETE',
                        body: JSON.stringify(sendedFormData),
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    });
                    break;
                default:
                    console.error('ошибка обработки события нажатия кнопки');
                    break;
            }

            if (result.ok) {
                if (Home.method === Home.viewNameMethod) {
                    var domElement = jQuery('<div class="alert alert-success" role="alert">Команда успешно выполнена: <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button></div>');
                    jQuery(form).after(domElement.hide().fadeIn(1000, 'swing', function () { domElement.fadeOut(1000); }));
                }
                else if (Home.method === Home.createNameMethod) {
                    this.props.history.push(`/${apiName}/${Home.viewNameMethod}/${result.id}/`);
                }
                else if (Home.method === Home.deleteNameMethod) {
                    this.props.history.push(`/${apiName}/${Home.listNameMethod}/`);
                }
            }
            else {
                alert(`Ошибка обработки запроса. Status: ${result.status}`);
            }

            if (nameButton === this.okButtonName) {
                this.props.history.push(`/${apiName}/${Home.listNameMethod}/`);
            }
        } catch (error) {
            console.error('Ошибка:', error);
        }
    }

    /** Набор кнопок для режима просмотра/редактирования объекта */
    viewButtons() {
        return (<div className="btn-toolbar justify-content-end" role="toolbar" aria-label="Toolbar with button groups">
            <div className="btn-group" role="group" aria-label="First group">
                <button name={this.okButtonName} onClick={this.handleClickButton} type="button" className="btn btn-outline-success" title='Сохранить и перейти к списку'>Ok</button>
                <button name={this.saveButtonName} onClick={this.handleClickButton} type="button" className="btn btn-outline-success" title='Записать в базу данных и продолжить редактирование'>Записать</button>
                <NavLink className='btn btn-outline-primary' to={`/${this.apiName}/${Home.listNameMethod}/`} role='button' title='Вернуться к списку без сохранения'>Вернуться к списку</NavLink>
            </div>
        </div>);
    }

    /** Набор кнопок для режима создания объекта */
    createButtons() {
        return (<div className="btn-toolbar justify-content-end" role="toolbar" aria-label="Toolbar with button groups">
            <div className="btn-group" role="group" aria-label="First group">
                <button name={this.okButtonName} onClick={this.handleClickButton} type="button" className="btn btn-outline-success" title='Сохранить и перейти к списку'>Ok</button>
                <NavLink className='btn btn-outline-primary' to={`/departments/${Home.listNameMethod}/`} role='button' title='Вернуться к списку без сохранения'>Отмена</NavLink>
            </div>
        </div>);
    }

    /** Набор кнопок для режима удаления объекта */
    deleteButtons() {
        return (<><NavLink className='btn btn-primary btn-block' to={`/${this.apiName}/${Home.listNameMethod}/`} role='button' title='Вернуться к списку'>Отмена</NavLink>
            <button name={this.okButtonName} onClick={this.handleClickButton} type="button" className="btn btn-outline-danger btn-block" title='Подтвердить удаление объекта'>Подтверждение удаления</button></>);
    }
}