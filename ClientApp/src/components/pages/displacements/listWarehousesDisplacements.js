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
        var apiName = App.controller;
        return (
            <>
                <ul className="nav nav-tabs">
                    <li className="nav-item">
                        <NavLink to={`/warehouses/${App.listNameMethod}`} className='nav-link'>Склады</NavLink>
                    </li>
                    <li className="nav-item">
                        <NavLink to={`/receiptswarehouses/${App.listNameMethod}`} className='nav-link'>Поступления</NavLink>
                    </li>
                    <li className="nav-item">
                        <NavLink to={`/displacements/${App.listNameMethod}`} className='nav-link active'>Перемещения</NavLink>
                    </li>
                </ul>

                <NavLink to={`/${apiName}/${App.createNameMethod}/`} className="btn btn-primary btn-block mt-3" role="button">Создать внутреннее перемещение</NavLink>

                <p className="lead">В процессе разработки...</p>
            </>
        );
    }
}
