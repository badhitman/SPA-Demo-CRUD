////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageCard } from '../aPageCard';
import App from '../../../App';

/** Отображение/редактирование существующей службы/метода доставки */
export class viewDelivery extends aPageCard {
    static displayName = viewDelivery.name;

    async load() {
        await this.ajax();
        this.cardTitle = `Доставка: [#${App.data.id}] ${App.data.name}`;
        this.setState({ loading: false });
    }

    cardBody() {
        var delivery = App.data;
        return (
            <>
                <form className='mb-2'>
                    <div className="form-group">
                        <input name='id' defaultValue={delivery.id} type='hidden' />
                        <label htmlFor="departments-input">Наименование</label>
                        <input name='name' defaultValue={delivery.name} type="text" className="form-control" id="departments-input" placeholder="Новое название" />
                    </div>
                    {this.viewButtons()}
                </form>
            </>
        );
    }
}