////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPage } from '../aPage';
import App from '../../../App';
import { DepatmentUsers } from './DepatmentUsers';

/** Отображение/редактирование существующего департамента */
export class viewDepartment extends aPage {
    static displayName = viewDepartment.name;

    async load() {
        await super.load(true);
        this.cardTitle = `Департамент: [#${App.data.id}] ${App.data.name}`;
        this.setState({ loading: false });
    }

    cardBody() {
        var department = App.data;
        
        return (
            <>
                <form className='mb-2'>
                    <input name='id' defaultValue={department.id} type='hidden' />
                    <div className="form-group">
                        <label htmlFor="departments-input">Наименование</label>
                        <input name='name' defaultValue={department.name} type="text" className="form-control" id="departments-input" placeholder="Новое название" />
                    </div>
                    {this.getInformation()}
                    {this.rootPanelObject()}
                    {this.viewButtons()}
                </form>
                <DepatmentUsers />
            </>
        );
    }
}