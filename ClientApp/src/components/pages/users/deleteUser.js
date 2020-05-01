////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { viewUser } from './viewUser';
import App from '../../../App';

/** Удаление объекта/пользователя */
export class deleteUser extends viewUser {
    static displayName = deleteUser.name;

    async load() {
        await this.ajax();
        this.cardTitle = `Удаление пользователя: [#${App.data.id}] ${App.data.name}`;
        this.setState({ loading: false });
    }
    cardBody() {
        var user = App.data;

        user.role = user.roles.find(function (element, index, array) { return element.id === user.role });
        if (user.role === undefined) {
            user.role = '<битая ссылка>';
        }

        return (
            <>
                <div className="alert alert-danger" role="alert">{(App.data.noDelete === false ? 'Безвозвратное удаление пользователя! Данное дейтсвие нельзя будет отменить!' : 'Объект невозможно удалить. На него существуют ссылки')}</div>
                <form className="mb-3" key='delete-form'>
                    {this.mapObjectToReadonlyForm(user, ['departmentId', 'id', 'avatar'])}
                    {this.deleteButtons(App.data.noDelete === false)}
                </form>
            </>
        );
    }
}