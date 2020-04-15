////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { viewTurnover } from './viewTurnover';
import App from '../../../App';

/** Создание новго документа отгрузки в доствку */
export class createTurnover extends viewTurnover {
    static displayName = createTurnover.name;

    async load() {
        this.cardTitle = 'Создание нового документа доставки';
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
