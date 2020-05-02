////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { NavLink } from 'react-router-dom';
import { viewWarehousesDisplacement } from './viewWarehousesDisplacement';
import App from '../../../App';

/** Удаление внутреннего перемещения */
export class deleteWarehousesDisplacement extends viewWarehousesDisplacement {
    static displayName = deleteWarehousesDisplacement.name;

    async load() {
        await this.ajax();
        this.cardTitle = `Удаление документа внутреннего перемещения: [#${App.data.id}] ${App.data.name}`;
        this.setState({ loading: false });
    }

    cardBody() {
        return (
            <>
                <p className="lead">В процессе разработки...</p>
                Перейти <NavLink to={`/${App.controller}/${App.listNameMethod}`}>к списку </NavLink>
            </>
        );
    }
}
