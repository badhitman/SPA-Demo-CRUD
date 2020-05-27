////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { Dropdown, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap';
import { NavLink, Link } from 'react-router-dom'
import { aPageList } from '../aPageList';
import App from '../../../App';
import { PaginatorComponent } from '../../PaginatorComponent';

/** Компонент для отображения справочника складов */
export class listWarehouses extends aPageList {
    static displayName = listWarehouses.name;
    cardTitle = 'Справочник складов';

    constructor(props) {
        super(props);

        this.state.nameWarehouse = '';
        this.state.dropdownOpenDropdownMenu = false;

        /** изменение значения поля имени склада */
        this.handleNameChange = this.handleNameChange.bind(this);
        /** сброс введёного значения поля имени склада*/
        this.handleResetClick = this.handleResetClick.bind(this);
        /** создание в бд склада */
        this.handleCreateClick = this.handleCreateClick.bind(this);

        this.toggleDropdownMenu = this.toggleDropdownMenu.bind(this);
    }

    toggleDropdownMenu() {
        this.setState({
            dropdownOpenDropdownMenu: !this.state.dropdownOpenDropdownMenu
        });
    }

    /**
     * событие изменения имени склада
     * @param {object} e - object sender
     */
    handleNameChange(e) {
        const target = e.target;

        this.setState({
            nameWarehouse: target.value
        });
    }

    /** сброс формы редактирования склада */
    handleResetClick() {
        this.setState({
            nameWarehouse: ''
        });
    }

    /** добавление склада в БД */
    async handleCreateClick() {
        var sendedFormData =
        {
            name: this.state.nameWarehouse
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
                    this.setState({ loading: false, nameWarehouse: '' });
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
        const myButtons = this.state.nameWarehouse.length === 0
            ? <><button disabled className="btn btn-outline-secondary" type="button">Создать</button>
                <button disabled className="btn btn-outline-secondary" type="reset">Сброс</button></>
            : <><button onClick={this.handleCreateClick} className="btn btn-outline-success" type="button">Создать</button>
                <button onClick={this.handleResetClick} className="btn btn-outline-primary" type="reset">Сброс</button></>
        
        const controllerName = App.controller.toLowerCase();
        return (
            <>
                <nav className="nav nav-pills nav-fill">
                    <NavLink to={`/warehouses/${App.listNameMethod}`} className='nav-item nav-link active'>Склады</NavLink>
                    <Dropdown nav isOpen={this.state.dropdownOpenDropdownMenu} toggle={this.toggleDropdownMenu}>
                        <DropdownToggle nav caret title='Журнал складских документов'>
                            Журнал
                        </DropdownToggle>
                        <DropdownMenu>
                            <DropdownItem className={(controllerName === 'warehousedocuments' ? 'font-weight-bold shadow rounded' : '')}><NavLink tag={Link} to={`/WarehouseDocuments/${App.listNameMethod}`} title='Полный журнал складских поступлений и перемещений'>Все (складские документы)</NavLink></DropdownItem>
                            <DropdownItem className={(controllerName === 'receiptswarehousesdocuments' ? 'font-weight-bold shadow rounded' : '')}><NavLink tag={Link} to={`/ReceiptsWarehousesDocuments/${App.listNameMethod}`} title='Журнал документов поступления на склад'>Поступления на склад</NavLink></DropdownItem>
                            <DropdownItem className={(controllerName === 'displacementsdocuments' ? 'font-weight-bold shadow rounded' : '')}><NavLink tag={Link} to={`/DisplacementsDocuments/${App.listNameMethod}`} title='Журнал внутренних перемещений со склада на склад'>Внутренние перемещения</NavLink></DropdownItem>
                        </DropdownMenu>
                    </Dropdown>
                    <NavLink to={`/warehousesReports/${App.listNameMethod}`} className='nav-item nav-link' title='Остатки номенклатуры' role='button'>Отчёты</NavLink>
                </nav>
                <br />
                <label htmlFor="basic-url">Создание нового склада</label>
                <div title='Для создания нового склада введите его название и нажмите кнопку - Создать' className="input-group mb-3">
                    <input onChange={this.handleNameChange} value={this.state.nameWarehouse} type="text" className="form-control" placeholder="Введите название нового склада" aria-label="Введите название нового склада" />
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
