////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageCard } from '../aPageCard';
import App from '../../../App';

/** Отображение существующего документа поступления на склад */
export class viewReceiptWarehouse extends aPageCard {
    static displayName = viewReceiptWarehouse.name;

    async load() {
        await this.ajax();
        this.cardTitle = `Документ поступления на склад: [#${App.data.id}] ${App.data.name}`;
        this.setState({ loading: false });
    }

    cardBody() {
        return (
            <>
                <p className="lead">В процессе разработки...</p>
            </>
        );
    }
}