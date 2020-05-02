////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { NavLink } from 'react-router-dom';
import { viewReceiptWarehouse } from './viewReceiptWarehouse';
import App from '../../../App';

/** Удаление документа поступления на склад */
export class deleteReceiptWarehouse extends viewReceiptWarehouse {
    static displayName = deleteReceiptWarehouse.name;

    async load() {
        await super.load();
        this.cardTitle = `Удаление документа складского поступления: [#${App.data.id}] ${App.data.name}`;
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
