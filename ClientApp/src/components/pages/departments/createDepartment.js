////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { viewDepartment } from './viewDepartment';

/** Создание нового департамента */
export class createDepartment extends viewDepartment {
    static displayName = createDepartment.name;

    async load() {
        this.setState({ cardTitle: 'Создание нового департамента', loading: false });
    }
    cardBody() {
        return (
            <>
                <form>
                    <div className="form-group">
                        <label htmlFor="departments-input">Наименование</label>
                        <input name='name' type="text" className="form-control" id="departments-input" placeholder="Название нового департамента" />
                    </div>
                    {this.createButtons()}
                </form>
            </>
        );
    }
}
