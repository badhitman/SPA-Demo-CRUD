////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { viewDelivery } from './viewDelivery';
import App from '../../../App';

/** Удаление доставки */
export class deleteDelivery extends viewDelivery {
    static displayName = deleteDelivery.name;

    async load() {
        await this.ajax();
        this.cardTitle = `Удаление доставки: [#${App.data.id}] ${App.data.name}`;
        this.setState({ loading: false });
    }

    cardBody() {
        var delivery = App.data;
        return (
            <>
                <div className="alert alert-danger" role="alert">Безвозратное удаление Номенклатуры! Данное дейтсвие нельзя будет отменить!</div>
                <form className="mb-3">
                    {this.mapObjectToReadonlyForm(delivery, ['id'])}
                    {this.deleteButtons()}
                </form>
            </>
        );
    }
}
