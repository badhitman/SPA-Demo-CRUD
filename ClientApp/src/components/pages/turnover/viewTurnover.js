////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageCard } from '../aPageCard';
import App from '../../../App';

/** Отображение существующего документа доставки */
export class viewTurnover extends aPageCard {
    static displayName = viewTurnover.name;

    async load() {
        await this.ajax();
        this.cardTitle = `отгрузка: [#${App.data.id}] ${App.data.name}`;
        this.setState({ loading: false });
    }

    cardBody() {
        var turnover = App.data;
        return (
            <>
                <form className='mb-2'>
                    <div className="form-group">
                        <input name='id' defaultValue={turnover.id} type='hidden' />
                        <label htmlFor="departments-input">Наименование</label>
                        <input name='name' defaultValue={turnover.name} type="text" className="form-control" id="departments-input" placeholder="Новое название" />
                    </div>
                    {this.viewButtons()}
                </form>
            </>
        );
    }
}