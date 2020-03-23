////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
import React from 'react';
import { aPageCard } from '../aPage';
import App from '../../../App';
import { DepatmentUsers } from './DepatmentUsers';

/** Отображение/редактирование существующего департамента */
export class viewDepartment extends aPageCard {
    static displayName = viewDepartment.name;
    apiName = 'departments';

    async load() {
        const response = await fetch(`/api/${this.apiName}/${App.id}`);
        App.data = await response.json();
        this.setState({ cardTitle: `Департамент: [#${App.data.id}] ${App.data.name}`, loading: false, cardContents: this.body() });
    }
    body() {
        var department = App.data;
        return (
            <>
                <form className='mb-2'>
                    <div className="form-group">
                        <input name='id' defaultValue={department.id} type='hidden' />
                        <label htmlFor="departments-input">Наименование</label>
                        <input name='name' defaultValue={department.name} type="text" className="form-control" id="departments-input" placeholder="Новое название" />
                    </div>
                    {this.viewButtons()}
                </form>
                <DepatmentUsers />
            </>
        );
    }
}