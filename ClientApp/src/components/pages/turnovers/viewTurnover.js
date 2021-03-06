////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
//import { NavLink } from 'react-router-dom';
import { aPageCard } from '../aPageCard';
import App from '../../../App';

/** Отображение существующего документа отгрузки в доставку */
export class viewTurnover extends aPageCard {
    static displayName = viewTurnover.name;

    async load() {
        await this.ajax();
        this.cardTitle = `Документ отгрузки в доставку: [#${App.data.id}] ${App.data.name}`;
        this.setState({ loading: false });
    }

    cardBody() {
        //var turnover = App.data;
        return App.emptyCardBody;
    }

    cardHeaderPanel() {
        return <></>;
    }
}