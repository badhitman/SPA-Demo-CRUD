////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageCard } from '../aPageCard';
import App from '../../../App';

/** Просмотр деталей уведомления */
export class viewNotification extends aPageCard {
    static displayName = viewNotification.name;

    async load() {
        await this.ajax();
        this.cardTitle = `Уведомление: [#${App.data.id}] ${App.data.name}`;
        this.setState({ loading: false });
    }

    cardBody() {
        const notification = App.data;

        return (
            <form className='mb-2' key='view-form'>
                <input name='id' defaultValue={notification.id} type='hidden' />
                
                {this.viewButtons()}
            </form>
        );
    }
}