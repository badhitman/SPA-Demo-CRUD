////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageCard } from '../aPageCard';
import App from '../../../App';

/** Отображение/редактирование существующей номенклатуры */
export class viewGood extends aPageCard {
    static displayName = viewGood.name;

    async load() {
        await this.ajax();
        this.cardTitle = `Номенклатура: [#${App.data.id}] ${App.data.name}`;
        this.setState({ loading: false });
    }

    cardBody() {
        var good = App.data;
        return (
            <>
                <form className='mb-2'>
                    <div className="form-group">
                        <input name='id' defaultValue={good.id} type='hidden' />
                        <label htmlFor="departments-input">Наименование</label>
                        <input name='name' defaultValue={good.name} type="text" className="form-control" id="departments-input" placeholder="Новое название" />
                    </div>
                    {this.viewButtons()}
                </form>
            </>
        );
    }
}