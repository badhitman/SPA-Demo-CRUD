////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageCard } from '../aPageCard';
import App from '../../../App';

/** Отображение существующего документа внутреннего перемещения */
export class viewWarehousesDisplacement extends aPageCard {
    static displayName = viewWarehousesDisplacement.name;

    async load() {
        await this.ajax();
        this.cardTitle = `перемещение: [#${App.data.id}] ${App.data.name}`;
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