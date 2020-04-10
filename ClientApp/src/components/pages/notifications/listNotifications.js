////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageList } from '../aPageList';
import { NavLink } from 'react-router-dom'
import App from '../../../App';

/** Компонент для отображения списка уведомлений */
export class listNotifications extends aPageList {
    static displayName = listNotifications.name;
    cardTitle = 'Уведомления';

    cardBody() {
        const notifications = App.data;
        return (
            <>
                <table className='table table-striped mt-4' aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>id</th>
                            <th>Subject</th>
                            <th>Information</th>
                        </tr>
                    </thead>
                    <tbody>
                        {notifications.map(function (notification) {
                            return <tr key={notification.id}>
                                <td>{notification.id}</td>
                                <td>{notification.name}</td>
                                <td>{notification.information}</td>
                            </tr>
                        })}
                    </tbody>
                </table>
                {this.cardPaginator()}
            </>
        );
    }
}