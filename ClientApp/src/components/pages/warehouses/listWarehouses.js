////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageList } from '../aPageList';
import { NavLink } from 'react-router-dom'
import App from '../../../App';
import { PaginatorComponent } from '../../PaginatorComponent';

/** Компонент для отображения справочника складов */
export class listWarehouses extends aPageList {
    static displayName = listWarehouses.name;
    cardTitle = 'Справочник складов';

    constructor(props) {
        super(props);

        this.state.nameNewWarehouse = '';

        /** изменение значения поля имени склада */
        this.handleNameChange = this.handleNameChange.bind(this);
        /** сброс введёного значения поля имени склада*/
        this.handleResetClick = this.handleResetClick.bind(this);
        /** создание в бд склада */
        this.handleCreateClick = this.handleCreateClick.bind(this);
    }

    /**
     * событие изменения имени склада
     * @param {object} e - object sender
     */
    handleNameChange(e) {
        const target = e.target;

        this.setState({
            nameNewWarehouse: target.value
        });
    }

    /** сброс формы редактирования склада */
    handleResetClick() {
        this.setState({
            nameNewWarehouse: ''
        });
    }

    /** добавление склада в БД */
    async handleCreateClick() {
        var sendedFormData =
        {
            name: this.state.nameNewWarehouse
        };

        const response = await fetch(`${this.apiPrefix}/${App.controller}`, {
            method: 'POST',
            body: JSON.stringify(sendedFormData),
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            }
        });

        if (response.ok === true) {
            try {
                const result = await response.json();
                if (result.success === true) {
                    await this.ajax();
                    this.setState({ loading: false, name: ''});
                }
                else {
                    this.clientAlert(result.info, result.status);
                }
            }
            catch (err) {
                this.clientAlert(err);
            }
        }
    }

    cardBody() {
        const myButtons = this.state.nameNewWarehouse.length === 0
            ? <><button disabled className="btn btn-outline-secondary" type="button">Создать</button>
                <button disabled className="btn btn-outline-secondary" type="reset">Сброс</button></>
            : <><button onClick={this.handleCreateClick} className="btn btn-outline-success" type="button">Создать</button>
                <button onClick={this.handleResetClick} className="btn btn-outline-primary" type="reset">Сброс</button></>

        return (
            <>
                <ul className="nav nav-tabs">
                    <li className="nav-item">
                        <NavLink to={`/warehouses/${App.listNameMethod}`} className='nav-link active'>Склады</NavLink>
                    </li>
                    <li className="nav-item">
                        <NavLink to={`/receiptswarehousesdocuments/${App.listNameMethod}`} className='nav-link' title='Перейти в журнал документов поступления номенклатуры'>Поступления</NavLink>
                    </li>
                    <li className="nav-item">
                        <NavLink to={`/displacementsdocuments/${App.listNameMethod}`} className='nav-link' title='Перейти в журнал документов внутреннего перемещения номенклатуры'>Перемещения</NavLink>
                    </li>
                </ul>
                <br />
                <label htmlFor="basic-url">Создание нового склада</label>
                <div title='Для создания нового склада введите его название и нажмите кнопку - Создать' className="input-group mb-3">
                    <input onChange={this.handleNameChange} value={this.state.nameNewWarehouse} type="text" className="form-control" placeholder="Введите название нового склада" aria-label="Введите название нового склада" />
                    <div className="input-group-append">
                        {myButtons}
                    </div>
                </div>

                <table className='table table-striped mt-4'>
                    <thead>
                        <tr>
                            <th>id</th>
                            <th>Name</th>
                            <th>About</th>
                        </tr>
                    </thead>
                    <tbody>
                        {App.data.map(function (warehouse) {
                            const currentNavLink = warehouse.isDisabled === true
                                ? <del><NavLink className='text-muted' to={`/${App.controller}/${App.viewNameMethod}/${warehouse.id}`} title='кликните для редактирования'>{warehouse.name}</NavLink></del>
                                : <NavLink to={`/${App.controller}/${App.viewNameMethod}/${warehouse.id}`} title='кликните для редактирования'>{warehouse.name}</NavLink>

                            return <tr key={warehouse.id}>
                                <td>{warehouse.id}</td>
                                <td>
                                    {currentNavLink}
                                    <NavLink to={`/${App.controller}/${App.deleteNameMethod}/${warehouse.id}`} title='удалить объект' className='text-danger ml-3'>del</NavLink>
                                </td>
                                <td></td>
                            </tr>
                        })}
                    </tbody>
                </table>
                <PaginatorComponent servicePaginator={this.servicePaginator} />
            </>
        );
    }
}
