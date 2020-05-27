////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { WarehousesReportsComponent } from './WarehousesReportsComponent';
import { Dropdown, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap';
import { NavLink, Link } from 'react-router-dom'
import { aPageCard } from './aPageCard';
import App from '../../App';

/** Отчёты по номенклатуре и складам */
export class WarehousesReports extends aPageCard {
    static displayName = WarehousesReports.name;
    cardTitle = 'Складские отчёты';

    constructor(props) {
        super(props);
        this.state.dropdownOpenDropdownMenu = false;
        this.toggleDropdownMenu = this.toggleDropdownMenu.bind(this);
    }

    async ajax() {
        App.data = {};
    }

    toggleDropdownMenu() {
        this.setState({
            dropdownOpenDropdownMenu: !this.state.dropdownOpenDropdownMenu
        });
    }

    cardBody() {
        const controllerName = App.controller.toLowerCase();
        return <>
            <nav className="nav nav-pills nav-fill">
                <NavLink to={`/warehouses/${App.listNameMethod}`} className='nav-item nav-link'>Склады</NavLink>
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
                <NavLink to={`/warehousesReports/${App.listNameMethod}`} className='nav-item nav-link active' title='Остатки номенклатуры' role='button'>Отчёты</NavLink>
            </nav>
            <br />
            <WarehousesReportsComponent />
        </>;
    }
}