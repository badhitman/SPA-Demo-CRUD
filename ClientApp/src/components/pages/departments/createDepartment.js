////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { viewDepartment } from './viewDepartment';
import App from '../../../App';

/** Создание нового департамента */
export class createDepartment extends viewDepartment {
    static displayName = createDepartment.name;

    async load() {
        this.cardTitle = 'Создание нового департамента';
        App.data = {};
        this.setState({ loading: false });
    }

    cardHeaderPanel() {
        return <></>;
    }

    cardBody() {
        return (
            <>
                <form>
                    <div className="form-group">
                        <label htmlFor="departments-input">Наименование</label>
                        <input name='name' type="text" className="form-control" id="departments-input" placeholder="Название нового департамента" />
                    </div>
                    {this.getInformation()}
                    {this.rootPanelObject()}
                    {this.createButtons()}
                </form>
            </>
        );
    }
}
