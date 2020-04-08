////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { viewGood } from './viewGood';
import App from '../../../App';

/** Создание новой Номенклатуры */
export class createGood extends viewGood {
    static displayName = createGood.name;

    async load() {
        this.cardTitle = 'Создание новой Номенклатуры';
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
                        <input name='name' type="text" className="form-control" id="departments-input" placeholder="Название новой номенклатуры" />
                    </div>
                    {this.createButtons()}
                </form>
            </>
        );
    }
}
