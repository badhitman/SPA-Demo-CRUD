////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { viewReceiptWarehouse } from './viewReceiptWarehouse';
import App from '../../../App';

/** Удаление документа поступления на склад */
export class deleteReceiptWarehouse extends viewReceiptWarehouse {
    static displayName = deleteReceiptWarehouse.name;

    async load() {
        await this.ajax();
        this.cardTitle = `Удаление документа складского поступления: [#${App.data.id}] ${App.data.name}`;
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
