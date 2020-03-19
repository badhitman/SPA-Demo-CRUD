////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
import React from 'react';
import { viewDepartment } from './viewDepartment';
import App from '../../../App';
import { DepatmentUsers } from './DepatmentUsers';

/** Удаление департамента */
export class deleteDepartment extends viewDepartment {
    static displayName = deleteDepartment.name;

    async load() {
        const response = await fetch(`/api/${this.apiName}/${App.id}`, { credentials: 'include' });
        App.data = await response.json();
        this.setState({ cardTitle: 'Удаление объекта', loading: false, cardContents: this.body() });
    }
    body() {
        var department = App.data;
        return (
            <>
                <div className="alert alert-danger" role="alert">Безвозратное удаление департамента и связаных с ним пользователей! Данное дейтсвие нельзя будет отменить!</div>
                <form className="mb-3">
                    {this.mapObjectToReadonlyForm(department, ['id'])}
                    {this.deleteButtons()}
                </form>
                <DepatmentUsers />
            </>
        );
    }
}
