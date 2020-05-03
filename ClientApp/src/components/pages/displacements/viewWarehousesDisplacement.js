////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { NavLink } from 'react-router-dom'
import { aPageCard } from '../aPageCard';
import App from '../../../App';

/** Отображение существующего документа внутреннего перемещения */
export class viewWarehousesDisplacement extends aPageCard {
    static displayName = viewWarehousesDisplacement.name;

    async load() {
        await this.ajax();
        this.cardTitle = `Документ внутреннего перемещения: [#${App.data.id}] ${App.data.name}`;
        this.setState({ loading: false });
    }

    cardBody() {
        return (
            <>
                <NavLink to={`/${App.controller}/${App.listNameMethod}/`} className="btn btn-primary btn-block mt-3" role="button">Вернуться</NavLink>

                <table className='table table-striped mt-4'>
                    <thead>
                        <tr>
                            <th>id</th>
                            <th>Name</th>
                            <th>About</th>
                        </tr>
                    </thead>
                    <tbody>
                        {App.data.rows.map(function (warehouseDisplacement) {
                            const currentNavLink = warehouseDisplacement.isDisabled === true
                                ? <del><NavLink className='text-muted' to={`/${App.controller}/${App.viewNameMethod}/${warehouseDisplacement.id}`} title='кликните для редактирования'>{warehouseDisplacement.name}</NavLink></del>
                                : <NavLink to={`/${App.controller}/${App.viewNameMethod}/${warehouseDisplacement.id}`} title='кликните для редактирования'>{warehouseDisplacement.name}</NavLink>

                            return <tr key={warehouseDisplacement.id}>
                                <td>{warehouseDisplacement.id}</td>
                                <td>
                                    {currentNavLink}
                                    <NavLink to={`/${App.controller}/${App.deleteNameMethod}/${warehouseDisplacement.id}`} title='удалить объект' className='text-danger ml-3'>del</NavLink>
                                </td>
                                <td>x{warehouseDisplacement.countRows}</td>
                            </tr>
                        })}
                    </tbody>
                </table>
            </>
        );
    }

    cardHeaderPanel() {
        return <></>;
    }
}