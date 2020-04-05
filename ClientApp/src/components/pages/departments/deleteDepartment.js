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
        await this.ajax();
        this.cardTitle = `Удаление объекта: [#${App.data.id}] ${App.data.name}`;
        this.setState({ loading: false });
    }

    cardBody() {
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
