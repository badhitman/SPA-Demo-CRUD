////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageCard } from '../aPageCard';
import App from '../../../App';

/** Просмотр собственного профиля */
export class viewProfile extends aPageCard {
    static displayName = viewProfile.name;

    async load() {
        await this.ajax();
        this.cardTitle = `Профиль: [#${App.data.id}] ${App.data.name}`;
        this.setState({ loading: false });
    }

    cardBody() {
        const notification = App.data;

        return (
            <form className='mb-2' key='view-form'>
                <input name='id' defaultValue={notification.id} type='hidden' />
                {this.mapObjectToReadonlyForm(notification)}
            </form>
        );
    }

    cardHeaderPanel() {
        return <></>;
    }
}