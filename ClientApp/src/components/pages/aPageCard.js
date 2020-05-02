////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import App from '../../App';

import { aPage } from './aPage';

/** Карточка объекта. Базовый (типа абстрактный) компонент */
export class aPageCard extends aPage {
    static displayName = aPageCard.name;

    async ajax() {
        if (App.id) {
            this.apiPostfix = `${this.apiPostfix}/${App.id}`;
        }

        await super.ajax();
    }

    cardHeaderPanel() {
        return <></>;
    }
}