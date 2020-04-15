////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { viewWarehouse } from './viewWarehouse';
import App from '../../../App';

/** Создание новго склада */
export class createWarehouse extends viewWarehouse {
    static displayName = createWarehouse.name;

    async load() {
        this.cardTitle = 'Создание нового склада';
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
