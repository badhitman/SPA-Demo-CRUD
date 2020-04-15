////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageList } from '../aPageList';
import { NavLink } from 'react-router-dom'
import App from '../../../App';
import { PaginatorComponent } from '../../PaginatorComponent';

/** Компонент для отображения журнала документов поступления на склад */
export class listReceiptsWarehouses extends aPageList {
    static displayName = listReceiptsWarehouses.name;
    cardTitle = 'Журнал документов поступления на склад';

    cardBody() {
        var apiName = App.controller;
        return (
            <>
                <ul className="nav nav-tabs">
                    <li className="nav-item">
                        <NavLink className='nav-link'>Склады</NavLink>
                    </li>
                    <li className="nav-item">
                        <NavLink className='nav-link active'>Поступления</NavLink>
                    </li>
                    <li className="nav-item">
                        <NavLink className='nav-link'>Перемещения</NavLink>
                    </li>
                </ul>

                <NavLink to={`/${apiName}/${App.createNameMethod}/`} className="btn btn-primary btn-block" role="button">Создать поступление на склад</NavLink>

                <p className="lead">В процессе разработки...</p>
            </>
        );
    }
}
