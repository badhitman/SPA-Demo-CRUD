////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageCard } from '../aPageCard';
import App from '../../../App';

/** Отображение/редактирование существующего файла */
export class viewFile extends aPageCard {
    static displayName = viewFile.name;
    apiName = 'files';

    async load() {
        const response = await fetch(`/api/${this.apiName}/${App.id}`);
        App.data = await response.json();
        this.setState({ cardTitle: `Файл: [#${App.data.id}] ${App.data.name}`, loading: false });
    }

    cardBody() {
        const file = App.data;

        return (
            <form className='mb-2' key='view-form'>
                <input name='id' defaultValue={file.id} type='hidden' />
                <div className="form-group">
                    <label htmlFor="file-input">Публичное имя</label>
                    <input name='name' defaultValue={file.name} type="text" className="form-control" id="file-input" placeholder="Новое имя" />
                </div>
                {this.viewButtons()}
            </form>
        );
    }
}