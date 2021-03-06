////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

//import React from 'react';
//import { NavLink } from 'react-router-dom';
import { viewWarehouse } from './viewWarehouse';
import App from '../../../App';

/** Удаление склада */
export class deleteWarehouse extends viewWarehouse {
    static displayName = deleteWarehouse.name;

    async load() {
        await this.ajax();
        this.cardTitle = `Удаление склада: [#${App.data.id}] ${App.data.name}`;
        this.setState({ loading: false });
    }

    cardBody() {
        return App.emptyCardBody;
    }
}
