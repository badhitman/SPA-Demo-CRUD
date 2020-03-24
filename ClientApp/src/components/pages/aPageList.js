////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import App from '../../App';
import { aPage } from './aPage';

/** Списки/Справочники. Базовый (типа абстрактный) компонент */
export class aPageList extends aPage {
    static displayName = aPageList.name;
    apiName = '';
    listCardHeader = '';

    async load() {
        const response = await fetch(`/api/${this.apiName}/`);
        if (response.redirected === true) {
            window.location.href = response.url;
        }
        try {
            App.data = await response.json();
            this.setState({ cardTitle: this.listCardHeader, loading: false, cardContents: this.body() });
        }
        catch (err) {
            this.setState({
                cardTitle: `Ошибка...`, loading: false, cardContents: <p>{err}</p>
            });
        }
    }
}