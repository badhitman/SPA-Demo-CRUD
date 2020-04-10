////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageList } from '../aPageList';
import { NavLink } from 'react-router-dom'
import App from '../../../App';

/** Компонент для отображения журнала документов отгрузки в доставку */
export class listTurnovers extends aPageList {
    static displayName = listTurnovers.name;
    cardTitle = 'Журнал документов отгрузки в доставку';

    cardBody() {
        var turnovers = App.data;
        var apiName = App.controller;
        return (
            <>
                <NavLink to={`/${apiName}/${App.createNameMethod}/`} className="btn btn-primary btn-block" role="button">Создать отгрузку в доставку</NavLink>

                <table className='table table-striped mt-4' aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>id</th>
                            <th>Name</th>
                        </tr>
                    </thead>
                    <tbody>
                        {turnovers.map(function (turnover) {
                            const currentNavLink = turnover.isDisabled === true
                                ? <del><NavLink className='text-muted' to={`/${apiName}/${App.viewNameMethod}/${turnover.id}`} title='кликните для редактирования'>{turnover.name}</NavLink></del>
                                : <NavLink to={`/${apiName}/${App.viewNameMethod}/${turnover.id}`} title='кликните для редактирования'>{turnover.name}</NavLink>

                            return <tr key={turnover.id}>
                                <td>{turnover.id}</td>
                                <td>
                                    {currentNavLink}
                                    <NavLink to={`/${apiName}/${App.deleteNameMethod}/${turnover.id}`} title='удалить объект' className='text-danger ml-3'>del</NavLink>
                                </td>
                            </tr>
                        })}
                    </tbody>
                </table>
                {this.cardPaginator()}
            </>
        );
    }
}
