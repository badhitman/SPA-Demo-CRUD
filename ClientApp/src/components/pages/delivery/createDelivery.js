////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { viewDelivery } from './viewDelivery';
import App from '../../../App';

/** Создание новой доставки */
export class createDelivery extends viewDelivery {
    static displayName = createDelivery.name;

    async load() {
        this.cardTitle = 'Создание новой доставки';
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
