////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { NavLink } from 'react-router-dom'
import { viewReceiptWarehouse } from './viewReceiptWarehouse';
import App from '../../../App';

/** Удаление документа поступления на склад */
export class deleteReceiptWarehouse extends viewReceiptWarehouse {
    static displayName = deleteReceiptWarehouse.name;

    async load() {
        await super.load();
        this.cardTitle = `Удаление документа складского поступления: [#${App.data.id}] ${App.data.name}`;
        this.setState({ loading: false });
    }

    //cardBody() {
    //    return (
    //        <>
    //            {viewReceiptWarehouse.navTabs()}
    //            <NavLink className='btn btn-outline-primary btn-block' to={`/${App.controller}/${App.listNameMethod}`} role='button' title='Вернуться в журнал документов поступления номенклатуры'>Вернуться</NavLink>
    //            <p className="lead">В процессе разработки...</p>
    //        </>
    //    );
    //}
}
