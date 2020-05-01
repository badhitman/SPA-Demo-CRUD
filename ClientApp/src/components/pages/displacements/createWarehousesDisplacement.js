////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
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
                </form>
            </>
        );
    }
}
