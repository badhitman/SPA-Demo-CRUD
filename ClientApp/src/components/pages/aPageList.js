////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPage } from './aPage';
import { PaginatorComponent } from '../PaginatorComponent';
import App from '../../App';

/** Списки/Справочники. Базовый (типа абстрактный) компонент */
export class aPageList extends aPage {
    static displayName = aPageList.name;
    servicePaginator = new PaginatorComponent();

    async ajax() {
        this.servicePaginator = new PaginatorComponent(this.props);
        const urlAddress = `${this.apiPrefix}/${App.controller + this.apiPostfix}`;
        this.servicePaginator.readPagination(urlAddress);
        this.apiQuery = this.servicePaginator.urlQuery;
        await super.ajax();
        this.servicePaginator.readPagination();
        if (App.id) {
            this.servicePaginator.urlRequestAddress = `/${App.controller}/${App.method}/${App.id}`;
        }
        else {
            this.servicePaginator.urlRequestAddress = `/${App.controller}/${App.method}`;
        }
    }

    cardHeaderPanel() {
        return <></>;
    }
}