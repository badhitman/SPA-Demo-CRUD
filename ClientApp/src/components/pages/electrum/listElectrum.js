////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageList } from '../aPageList';
import App from '../../../App';
import { PaginatorComponent } from '../../PaginatorComponent';

/** Компонент для отображения списка Electrum транзакций */
export class listElectrum extends aPageList {
    static displayName = listElectrum.name;
    cardTitle = 'Список Electrum транзакций';

    cardBody() {
        var electrum = App.data;
        return (
            <>
                <table className='table table-striped mt-4' aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>id</th>
                            <th>Information</th>
                        </tr>
                    </thead>
                    <tbody>
                        {electrum.map(function (telegramBotUpdate) {
                            return <tr key={telegramBotUpdate.id}>
                                <td>{telegramBotUpdate.id}</td>
                                <td>{telegramBotUpdate.information}</td>
                            </tr>
                        })}
                    </tbody>
                </table>
                <PaginatorComponent servicePaginator={this.servicePaginator} />
            </>
        );
    }
}