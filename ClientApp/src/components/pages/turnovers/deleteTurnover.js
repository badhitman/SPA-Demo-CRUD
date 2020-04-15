////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { viewTurnover } from './viewTurnover';
import App from '../../../App';

/** Удаление документа отгрузки в доставку */
export class deleteTurnover extends viewTurnover {
    static displayName = deleteTurnover.name;

    async load() {
        await this.ajax();
        this.cardTitle = `Удаление документа отгрузки: [#${App.data.id}] ${App.data.name}`;
        this.setState({ loading: false });
    }

    cardBody() {
        var turnover = App.data;
        return (
            <>
                <p className="lead">В процессе разработки...</p>
            </>
        );
    }
}
