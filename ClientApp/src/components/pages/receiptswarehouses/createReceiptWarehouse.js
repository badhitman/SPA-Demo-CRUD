////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { viewReceiptWarehouse } from './viewReceiptWarehouse';
import App from '../../../App';

/** Создание новго документа поступления на склад */
export class createReceiptWarehouse extends viewReceiptWarehouse {
    static displayName = createReceiptWarehouse.name;

    async load() {
        this.cardTitle = 'Создание поступления на склад';
        App.data = {};
        this.setState({ loading: false });
    }

    cardHeaderPanel() {
        return <></>;
    }

    cardBody() {
        return (
            <>
                <p className="lead">В процессе разработки...</p>
            </>
        );
    }
}
