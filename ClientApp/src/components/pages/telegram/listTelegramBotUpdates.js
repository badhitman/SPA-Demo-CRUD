////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageList } from '../aPageList';
import App from '../../../App';
import { PaginatorComponent } from '../../PaginatorComponent';

/** Компонент для отображения списка TelegramBot Updates */
export class listTelegramBotUpdates extends aPageList {
    static displayName = listTelegramBotUpdates.name;
    cardTitle = 'Список TelegramBot обновлений';

    cardBody() {
        var telegramBotUpdates = App.data;
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
                        {telegramBotUpdates.map(function (telegramBotUpdate) {                            
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