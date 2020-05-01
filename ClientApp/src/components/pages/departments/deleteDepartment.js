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
        const allowDelete = (department.isReadonly === false || App.session.role.toLowerCase() === 'root') && Array.isArray(department.users) && department.users.length === 0;

        return (
            <>
                <div className="alert alert-danger" role="alert">{(allowDelete === true ? 'Безвозратное удаление департамента! Данное дейтсвие нельзя будет отменить!' : 'Департамент с сотрудниками или помеченый как "только для чтения" нельзя удалять')}</div>
                <form className="mb-3">
                    {this.mapObjectToReadonlyForm(department, ['id', 'avatar'])}
                    {this.deleteButtons(allowDelete)}
                </form>
                <DepatmentUsers />
            </>
        );
    }
}
