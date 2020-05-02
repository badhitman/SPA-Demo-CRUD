////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { NavLink } from 'react-router-dom'
import { aPageList } from '../aPageList';
import App from '../../../App';
import { PaginatorComponent } from '../../PaginatorComponent';

/** Отображение существующего склада */
export class viewWarehouse extends aPageList {
    static displayName = viewWarehouse.name;

    async ajax() {
        this.apiPostfix = `/${App.id}`;
        await super.ajax();
    }

    async load() {
        this.apiPostfix = `/${App.id}`;
        await super.load();
        this.cardTitle = `Склад: [#${App.data.id}] ${App.data.name}`;
        this.setState({ loading: false });
    }

    cardBody() {
        const deleteButtonDisabled = this.servicePaginator.rowsCount > 0 || App.data.noDelete === true;

        const deleteButton = deleteButtonDisabled === true
            ? <button disabled title='На объект существуют ссылки' className="btn btn-outline-secondary btn-block mb-3" type="button">Удаление невозможно</button>
            : <NavLink className='btn btn-outline-danger btn-block mb-3' to={`/${App.controller}/${App.deleteNameMethod}/${App.id}`} role='button' title='Диалог удаления объекта'>Удаление</NavLink>;

        const warehouse = App.data;

        return (
            <>
                <form>
                    <input name='id' defaultValue={warehouse.id} type='hidden' />
                    <div className="form-group">
                        <label htmlFor="departments-input">Наименование</label>
                        <input name='name' defaultValue={warehouse.name} type="text" className="form-control" id="departments-input" placeholder="Новое название" />
                    </div>
                    <div className="form-group">
                        {this.getInformation()}
                    </div>
                    <div className="form-group">
                        {this.rootPanelObject()}
                    </div>
                    {this.viewButtons()}
                </form>
                <div className='card mt-3'>
                    <div className='card-body'>
                        <legend>Журнал регистров</legend>
                        <table className='table table-striped mt-4'>
                            <thead>
                                <tr>
                                    <th>id</th>
                                    <th>Name</th>
                                    <th>About</th>
                                </tr>
                            </thead>
                            <tbody>
                                {App.data.documents.map(function (warehouseDocument) {
                                    var apiDocumentContriller = '#';
                                    switch (warehouseDocument.discriminator) {
                                        case 'ReceiptToWarehouseDocumentModel':
                                            apiDocumentContriller = 'receiptswarehousesdocuments';
                                            break;
                                        case 'InternalDisplacementWarehouseDocumentModel':
                                            apiDocumentContriller = 'displacementsdocuments';
                                            break;
                                        default:
                                            apiDocumentContriller = '#ошибка#';
                                            break;
                                    }

                                    const currentNavLink = warehouseDocument.isDisabled === true
                                        ? <del><NavLink className='text-muted' to={`/${apiDocumentContriller}/${App.viewNameMethod}/${warehouseDocument.id}`} title='кликните для редактирования'>{warehouseDocument.name}</NavLink></del>
                                        : <NavLink to={`/${apiDocumentContriller}/${App.viewNameMethod}/${warehouseDocument.id}`} title='кликните для редактирования'>{warehouseDocument.name}</NavLink>

                                    return <tr key={warehouseDocument.id}>
                                        <td>{warehouseDocument.id}</td>
                                        <td>
                                            {currentNavLink}
                                            <NavLink to={`/${apiDocumentContriller}/${App.deleteNameMethod}/${warehouseDocument.id}`} title='удалить объект' className='text-danger ml-3'>del</NavLink>
                                        </td>
                                        <td>x{warehouseDocument.countRows}</td>
                                    </tr>
                                })}
                            </tbody>
                        </table>
                        <PaginatorComponent servicePaginator={this.servicePaginator} />
                    </div>
                </div>
            </>
        );
    }
}