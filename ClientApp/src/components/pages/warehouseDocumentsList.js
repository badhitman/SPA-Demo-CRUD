////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { Dropdown, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap';
import { aPageList } from './aPageList';
import { NavLink, Link } from 'react-router-dom'
import App from '../../App';
import { PaginatorComponent } from '../PaginatorComponent';

/** Компонент для отображения журнала складских документов */
export class warehouseDocumentsList extends aPageList {
    static displayName = warehouseDocumentsList.name;
    cardTitle = 'Складские документы';

    constructor(props) {
        super(props);

        this.state.dropdownOpenPaginator = false;
        this.togglePaginator = this.togglePaginator.bind(this);
    }

    togglePaginator() {
        this.setState({
            dropdownOpenPaginator: !this.state.dropdownOpenPaginator
        });
    }

    cardBody() {
        this.servicePaginator.urlRequestAddress = "";
        const controllerName = App.controller.toLowerCase();

        var typeDocumentTitle = '<empty>';
        var type2DocumentTitle = '<empty>';
        switch (controllerName) {
            case 'warehousedocuments':
                typeDocumentTitle = 'Все';
                break;
            case 'receiptswarehousesdocuments':
                typeDocumentTitle = 'Поступл.';
                type2DocumentTitle = 'поступление на склад';
                break;
            case 'displacementsdocuments':
                typeDocumentTitle = 'Внутр.';
                type2DocumentTitle = 'внутреннее перемещение';
                break;
            default:
                typeDocumentTitle = '<error>';
                break;
        }

        const createNewDocument = App.controller.toLowerCase() === 'warehousedocuments'
            ? <button disabled type="button" className="btn btn-outline-secondary btn-block">Укажите тип документа</button>
            : <NavLink className="btn btn-primary btn-block" to={`/${App.controller}/${App.createNameMethod}`} role="button">Создать {type2DocumentTitle}</NavLink>;
        
        return (
            <>
                <nav className="nav nav-pills nav-fill">
                    <NavLink to={`/warehouses/${App.listNameMethod}`} className='nav-item nav-link'>Склады</NavLink>
                    <Dropdown nav isOpen={this.state.dropdownOpenPaginator} toggle={this.togglePaginator}>
                        <DropdownToggle style={{ 'color': '#fff', 'backgroundColor': '#007bff'}} nav caret>
                            {typeDocumentTitle}
                        </DropdownToggle>
                        <DropdownMenu>
                            <DropdownItem className={(controllerName === 'warehousedocuments' ? 'font-weight-bold shadow rounded' : '')}><NavLink tag={Link} to={`/WarehouseDocuments/${App.listNameMethod}`} title='Полный журнал складских поступлений и перемещений'>Все (складские документы)</NavLink></DropdownItem>
                            <DropdownItem className={(controllerName === 'receiptswarehousesdocuments' ? 'font-weight-bold shadow rounded' : '')}><NavLink tag={Link} to={`/ReceiptsWarehousesDocuments/${App.listNameMethod}`} title='Журнал документов поступления на склад'>Поступления на склад</NavLink></DropdownItem>
                            <DropdownItem className={(controllerName === 'displacementsdocuments' ? 'font-weight-bold shadow rounded' : '')}><NavLink tag={Link} to={`/DisplacementsDocuments/${App.listNameMethod}`} title='Журнал внутренних перемещений со склада на склад'>Внутренние перемещения</NavLink></DropdownItem>
                        </DropdownMenu>
                    </Dropdown>
                    <NavLink to={`/warehousesReports/`} className='nav-item nav-link' title='Остатки номенклатуры' role='button'>Отчёты</NavLink>
                </nav>
                <br />
                {createNewDocument}
                <table className='table table-striped mt-4'>
                    <thead>
                        <tr>
                            <th>id</th>
                            <th>Name</th>
                            <th>About</th>
                        </tr>
                    </thead>
                    <tbody>
                        {App.data.map(function (docWarehouse) {

                            var controllerName = '';
                            var aboutInfo = <></>;
                            switch (docWarehouse.discriminator.toLowerCase()) {
                                case 'receipttowarehousedocumentmodel':
                                    controllerName = 'receiptswarehousesdocuments';
                                    aboutInfo = App.controller.toLowerCase() === 'warehousedocuments'
                                        ? <span title='автор документа' className="badge badge-light">{docWarehouse.author.name}</span>
                                        : <>→ {docWarehouse.warehouseReceipt.name}</>;
                                    break;
                                case 'internaldisplacementwarehousedocumentmodel':
                                    controllerName = 'displacementsdocuments';
                                    if (docWarehouse.warehouseDebiting) {
                                        aboutInfo = <>{docWarehouse.warehouseDebiting.name} → {docWarehouse.warehouseReceipt.name}</>;
                                    }
                                    else {
                                        aboutInfo = <span title='автор документа' className="badge badge-light">{docWarehouse.author.name}</span>;
                                    }

                                    break;
                                default:
                                    controllerName = 'error';
                                    aboutInfo = <span title='автор документа' className="badge badge-light">{docWarehouse.author.name}</span>;
                                    console.error('ошибка определения типа документа (складской документ)');
                                    break;
                            }

                            return <tr key={docWarehouse.id}>
                                <td>{docWarehouse.id}</td>
                                <td>
                                    <NavLink to={`/${controllerName}/${App.viewNameMethod}/${docWarehouse.id}`} title='кликните для редактирования'>{docWarehouse.name}</NavLink>
                                </td>
                                <td><b>x{docWarehouse.countRows}</b> {aboutInfo}</td>
                            </tr>
                        })}
                    </tbody>
                </table>
                <PaginatorComponent servicePaginator={this.servicePaginator} />
            </>
        );
    }
}