////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageCard } from '../aPageCard';
import App from '../../../App';

/** Просмотр существующего TelegramBot Update */
export class viewTelegramBotUpdate extends aPageCard {
    static displayName = viewTelegramBotUpdate.name;

    async load() {
        await this.ajax();
        this.cardTitle = `TelegramBot Update: [#${App.data.id}] ${App.data.name}`;
        this.setState({ loading: false });
    }

    cardBody() {
        const telegramBotUpdate = App.data;

        return (
            <form className='mb-2' key='view-form'>
                <input name='id' defaultValue={telegramBotUpdate.id} type='hidden' />
            </form>
        );
    }
}