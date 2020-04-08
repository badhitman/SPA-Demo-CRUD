////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageCard } from '../aPageCard';
import App from '../../../App';

/** Просмотр существующего Electrum BTC TX */
export class viewElectrum extends aPageCard {
    static displayName = viewElectrum.name;

    async load() {
        await this.ajax();
        this.cardTitle = `Electrum BTC TX: [#${App.data.id}] ${App.data.name}`;
        this.setState({ loading: false });
    }

    cardBody() {
        const electrum = App.data;

        return (
            <form className='mb-2' key='view-form'>
                <input name='id' defaultValue={electrum.id} type='hidden' />
            </form>
        );
    }
}