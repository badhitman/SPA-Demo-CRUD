////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { viewTurnover } from './viewTurnover';
import App from '../../../App';

/** Создание новго документа доставки */
export class createTurnover extends viewTurnover {
    static displayName = createTurnover.name;

    async load() {
        this.cardTitle = 'Создание нового документа доставки';
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
