////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageList } from '../aPageList';
import { NavLink } from 'react-router-dom'
import App from '../../../App';
import { PaginatorComponent } from '../../PaginatorComponent';
import { viewReceiptWarehouse } from './viewReceiptWarehouse';

/** Компонент для отображения журнала документов поступления на склад */
export class listReceiptsWarehouses extends aPageList {
    static displayName = listReceiptsWarehouses.name;
    cardTitle = 'Журнал документов поступления на склад';

    cardBody() {
        var apiName = App.controller;
        return (
            <>
                {viewReceiptWarehouse.navTabs()}
                <NavLink to={`/${apiName}/${App.createNameMethod}/`} className="btn btn-primary btn-block mt-3" role="button" title='Создание документа поступления на склад'>Новое поступление на склад</NavLink>

                <table className='table table-striped mt-4'>
                    <thead>
                        <tr>
                            <th>id</th>
                            <th>Склад поступления</th>
                            <th>About</th>
                        </tr>
                    </thead>
                    <tbody>
                        {App.data.map(function (receiptWarehouse) {
                            const currentNavLink = receiptWarehouse.isDisabled === true
                                ? <del><NavLink className='text-muted' to={`/${App.controller}/${App.viewNameMethod}/${receiptWarehouse.id}`} title='кликните для редактирования'>{receiptWarehouse.name}</NavLink></del>
                                : <NavLink to={`/${App.controller}/${App.viewNameMethod}/${receiptWarehouse.id}`} title='кликните для редактирования'>{receiptWarehouse.name}</NavLink>

                            return <tr key={receiptWarehouse.id}>
                                <td>{receiptWarehouse.id}</td>
                                <td>
                                    {currentNavLink}
                                    <NavLink to={`/${App.controller}/${App.deleteNameMethod}/${receiptWarehouse.id}`} title='удалить объект' className='text-danger ml-3'>del</NavLink>
                                </td>
                                <td>x{receiptWarehouse.countRows}</td>
                            </tr>
                        })}
                    </tbody>
                </table>
                <PaginatorComponent servicePaginator={this.servicePaginator} />
            </>
        );
    }
}
