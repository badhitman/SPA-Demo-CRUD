////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageList } from '../aPageList';
import { NavLink } from 'react-router-dom'
import App from '../../../App';
import { PaginatorComponent } from '../../PaginatorComponent';

/** Компонент для отображения справочника доставки */
export class listDeliveries extends aPageList {
    static displayName = listDeliveries.name;
    cardTitle = 'Справочник доставки';

    cardBody() {
        var deliveries = App.data;
        var apiName = App.controller;
        return (
            <>
                <NavLink to={`/${apiName}/${App.createNameMethod}/`} className="btn btn-primary btn-block" role="button">Создать новую доставку</NavLink>

                <table className='table table-striped mt-4' aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>id</th>
                            <th>Name</th>
                        </tr>
                    </thead>
                    <tbody>
                        {deliveries.map(function (deliveriy) {
                            const currentNavLink = deliveriy.isDisabled === true
                                ? <del><NavLink className='text-muted' to={`/${apiName}/${App.viewNameMethod}/${deliveriy.id}`} title='кликните для редактирования'>{deliveriy.name}</NavLink></del>
                                : <NavLink to={`/${apiName}/${App.viewNameMethod}/${deliveriy.id}`} title='кликните для редактирования'>{deliveriy.name}</NavLink>

                            return <tr key={deliveriy.id}>
                                <td>{deliveriy.id}</td>
                                <td>
                                    {currentNavLink}
                                    <NavLink to={`/${apiName}/${App.deleteNameMethod}/${deliveriy.id}`} title='удалить объект' className='text-danger ml-3'>del</NavLink>
                                </td>
                            </tr>
                        })}
                    </tbody>
                </table>
                <PaginatorComponent servicePaginator={this.servicePaginator} />
            </>
        );
    }
}
