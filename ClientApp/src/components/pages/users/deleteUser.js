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
        const response = await fetch(`/api/${this.apiName}/${App.id}`, { credentials: 'include' });
        App.data = await response.json();
        this.setState({ cardTitle: 'Удаление объекта', loading: false, cardContents: this.body() });
    }
    body() {
        var user = App.data;
        user.departmen = user.departments[user.departmentId].name;
        return (
            <>
                <div className="alert alert-danger" role="alert">Безвозвратное удаление пользователя! Данное дейтсвие нельзя будет отменить!</div>
                <form className="mb-3" key='delete-form'>
                    {this.mapObjectToReadonlyForm(user, ['departmentId', 'id'])}
                    {this.deleteButtons()}
                </form>
            </>
        );
    }
}