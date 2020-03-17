////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
import React from 'react';
import { aPageList, aPageCard } from './aPage';
import { NavLink } from 'react-router-dom'
import App from '../../App';
import { DepatmentUsers } from './DepatmentUsers';

/** Компонент для отображения списка департаментов */
export class DepartmentsList extends aPageList {
    static displayName = DepartmentsList.name;
    apiName = 'departments';
    listCardHeader = 'Справочник департаментов';

    body() {
        var departments = App.data;
        var apiName = this.apiName;
        return (
            <>
                <NavLink to={`/${apiName}/${App.createNameMethod}/`} className="btn btn-primary btn-block" role="button">Создать новый департамент</NavLink>

                <table className='table table-striped mt-4' aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>id</th>
                            <th>Name</th>
                        </tr>
                    </thead>
                    <tbody>
                        {departments.map(function (department) {
                            return <tr key={department.id}>
                                <td>{department.id}</td>
                                <td>
                                    <NavLink to={`/${apiName}/${App.viewNameMethod}/${department.id}`} title='кликните для редактирования'>
                                        {department.name}
                                    </NavLink>
                                    <NavLink to={`/${apiName}/${App.deleteNameMethod}/${department.id}`} title='удалить объект' className='text-danger ml-3'>del</NavLink>
                                </td>
                            </tr>
                        })}
                    </tbody>
                </table>
            </>
        );
    }
}

/** Отображение/редактирование существующего департамента */
export class viewDepartment extends aPageCard {
    static displayName = viewDepartment.name;
    apiName = 'departments';

    async load() {
        const response = await fetch(`/api/${this.apiName}/${App.id}`, { credentials: 'include' });
        App.data = await response.json();
        this.setState({ cardTitle: `Департамент: [#${App.data.id}] ${App.data.name}`, loading: false, cardContents: this.body() });
    }
    body() {
        var department = App.data;
        return (
            <>
                <form className='mb-2'>
                    <div className="form-group">
                        <input name='id' defaultValue={department.id} type='hidden' />
                        <label htmlFor="departments-input">Наименование</label>
                        <input name='name' defaultValue={department.name} type="text" className="form-control" id="departments-input" placeholder="Новое название" />
                    </div>
                    {this.viewButtons()}
                </form>
                <DepatmentUsers />
            </>
        );
    }
}

/** Создание нового департамента */
export class createDepartment extends viewDepartment {
    static displayName = createDepartment.name;

    async load() {
        this.setState({ cardTitle: 'Создание нового департамента', loading: false, cardContents: this.body() });
    }
    body() {
        return (
            <>
                <form>
                    <div className="form-group">
                        <label htmlFor="departments-input">Наименование</label>
                        <input name='name' type="text" className="form-control" id="departments-input" placeholder="Название нового департамента" />
                    </div>
                    {this.createButtons()}
                </form>
            </>
        );
    }
}

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
