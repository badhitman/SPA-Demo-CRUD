////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageList } from '../aPageList';
import { NavLink } from 'react-router-dom'
import App from '../../../App';
import { PaginatorComponent } from '../../PaginatorComponent';

/** Компонент для отображения журнала документов внутреннего перемещения */
export class listWarehousesDisplacements extends aPageList {
    static displayName = listWarehousesDisplacements.name;
    cardTitle = 'Журнал документов внутреннего перемещения';

    cardBody() {
        return (
            <>
                <ul className="nav nav-tabs">
                    <li className="nav-item">
                        <NavLink to={`/warehouses/${App.listNameMethod}`} className='nav-link' title='Перейти в справочник складов номенклатуры'>Склады</NavLink>
                    </li>
                    <li className="nav-item">
                        <NavLink to={`/receiptswarehousesdocuments/${App.listNameMethod}`} className='nav-link' title='Перейти в журнал документов поступления номенклатуры'>Поступления</NavLink>
                    </li>
                    <li className="nav-item">
                        <NavLink to={`/displacementsdocuments/${App.listNameMethod}`} className='nav-link active'>Перемещения</NavLink>
                    </li>
                </ul>

                <NavLink to={`/${App.controller}/${App.createNameMethod}/`} className="btn btn-primary btn-block mt-3" role="button" title='Создание нового документа внутреннего перемещения номенклатуры'>Новое перемещение</NavLink>

                <table className='table table-striped mt-4'>
                    <thead>
                        <tr>
                            <th>id</th>
                            <th>Name</th>
                            <th>About</th>
                        </tr>
                    </thead>
                    <tbody>
                        {App.data.map(function (warehouseDisplacement) {
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
                <PaginatorComponent servicePaginator={this.servicePaginator} />
            </>
        );
    }
}
