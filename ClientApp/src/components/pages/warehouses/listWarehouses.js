////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageList } from '../aPageList';
import { NavLink } from 'react-router-dom'
import App from '../../../App';
import { PaginatorComponent } from '../../PaginatorComponent';

/** Компонент для отображения справочника складов */
export class listWarehouses extends aPageList {
    static displayName = listWarehouses.name;
    cardTitle = 'Справочник складов';

    cardBody() {
        
        var apiName = App.controller;
        return (
            <>
                <ul className="nav nav-tabs">
                    <li className="nav-item">
                        <NavLink className='nav-link active'>Склады</NavLink>
                    </li>
                    <li className="nav-item">
                        <NavLink className='nav-link'>Поступления</NavLink>
                    </li>
                    <li className="nav-item">
                        <NavLink className='nav-link'>Перемещения</NavLink>
                    </li>
                </ul>

                <NavLink to={`/${apiName}/${App.createNameMethod}/`} className="btn btn-primary btn-block" role="button">Создать склад</NavLink>

                <p className="lead">В процессе разработки...</p>
            </>
        );
    }
}
