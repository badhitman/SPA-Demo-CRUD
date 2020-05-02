////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { NavLink } from 'react-router-dom';
import { viewWarehousesDisplacement } from './viewWarehousesDisplacement';
import App from '../../../App';

/** Создание нового документа внутреннего перемещения */
export class createWarehousesDisplacement extends viewWarehousesDisplacement {
    static displayName = createWarehousesDisplacement.name;

    async load() {
        this.cardTitle = 'Новое складское перемещение';
        App.data = {};
        this.setState({ loading: false });
    }

    cardHeaderPanel() {
        return <></>;
    }

    cardBody() {
        return (
            <>
                <form>
                    <p className="lead">В процессе разработки...</p>
                    Перейти <NavLink to={`/${App.controller}/${App.listNameMethod}`}>к списку </NavLink>
                </form>
            </>
        );
    }
}
