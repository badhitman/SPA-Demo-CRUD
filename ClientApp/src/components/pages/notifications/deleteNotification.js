////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { viewNotification } from './viewNotification';
import App from '../../../App';

/** Удаление уведомления */
export class deleteNotification extends viewNotification {
    static displayName = deleteNotification.name;

    async load() {
        await this.ajax();
        this.cardTitle = `Удаление уведомления: [#${App.data.id}] ${App.data.name}`;
        this.setState({ loading: false });
    }
    cardBody() {
        var notification = App.data;

        return (
            <>
                <div className="alert alert-danger" role="alert">Безвозвратное удаление уведомления! Данное дейтсвие нельзя будет отменить!</div>
                <form className="mb-3" key='delete-form'>
                    {this.mapObjectToReadonlyForm(notification, ['id'])}
                    {this.deleteButtons()}
                </form>
            </>
        );
    }
}