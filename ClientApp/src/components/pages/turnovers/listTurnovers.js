////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageList } from '../aPageList';
import { NavLink } from 'react-router-dom'
import App from '../../../App';
import { PaginatorComponent } from '../../PaginatorComponent';

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

                <p className="lead">В процессе разработки...</p>
            </>
        );
    }
}
