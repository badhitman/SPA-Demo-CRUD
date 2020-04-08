////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageCard } from '../aPageCard';
import App from '../../../App';

/** Просмотр состояние сервера */
export class viewServer extends aPageCard {
    static displayName = viewServer.name;
    cardTitle = 'Состояние сервера';

    async load() {
        await this.ajax();

        this.setState({ loading: false });
    }

    cardBody() {
        const server = App.data;

        return (
            <form className='mb-2' key='view-form'>
                {this.mapObjectToReadonlyForm(server)}
            </form>
        );
    }

    cardHeaderPanel() {
        return <></>;
    }
}