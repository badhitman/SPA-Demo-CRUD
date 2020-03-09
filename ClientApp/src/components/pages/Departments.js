import React from 'react';
import { aPageList, aPageCard } from './aPage';
import { NavLink } from 'react-router-dom'
import { Home } from '../Home';
import { DepatmentUsers } from './DepatmentUsers';

/** Компонент для отображения списка департаментов */
export class DepartmentsList extends aPageList {
    static displayName = DepartmentsList.name;
    apiName = 'departments';
    listCardHeader = 'Справочник департаментов';

    listRender() {
        var departments = Home.data;
        var apiName = this.apiName;
        return (
            <>
                <NavLink to={`/${apiName}/${Home.createNameMethod}/`} className="btn btn-primary btn-block" role="button">Создать новый департамент</NavLink>

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
                                    <NavLink to={`/${apiName}/${Home.viewNameMethod}/${department.id}`} title='кликните для редактирования'>
                                        {department.name}
                                    </NavLink>
                                    <NavLink to={`/${apiName}/${Home.deleteNameMethod}/${department.id}`} title='удалить объект' className='text-danger ml-3'>del</NavLink>
                                </td>
                            </tr>
                        })}
                    </tbody>
                </table>
            </>
        );
    }
}

/** Компонент для отображения и управления конкретными объектами/департаментами */
export class DepartmentCard extends aPageCard {
    static displayName = DepartmentCard.name;
    apiName = 'departments';

    async viewLoad() {
        const response = await fetch(`/api/${this.apiName}/${Home.id}`);
        Home.data = await response.json();
        this.setState({ cartTitle: `Департамент: [#${Home.data.id}] ${Home.data.name}`, loading: false, cartContents: this[Home.method + 'Render']() });
    }
    viewRender() {
        var department = Home.data;
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

    async createLoad() {
        this.setState({ cartTitle: 'Создание нового департамента', loading: false, cartContents: this[Home.method + 'Render']() });
    }
    createRender() {
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

    async deleteLoad() {
        const response = await fetch(`/api/${this.apiName}/${Home.id}`);
        Home.data = await response.json();
        this.setState({ cartTitle: 'Удаление объекта', loading: false, cartContents: this[Home.method + 'Render']() });
    }
    deleteRender() {
        var department = Home.data;
        return (
            <>
                <div className="alert alert-danger" role="alert">Безвозратное удаление департамента и связаных с ним пользователей! Данное дейтсвие нельзя будет отменить!</div>
                <form className="mb-3">
                    {
                        Object.keys(department).map((keyName, i) => (
                            <div className='form-group row' key={i}>
                                <label htmlFor={keyName} className='col-sm-2 col-form-label'>{keyName}</label>
                                <div className='col-sm-10'>
                                    <input name={keyName} id={keyName} readOnly={true} defaultValue={department[keyName]} className='form-control' type='text' />
                                </div>
                            </div>
                        ))
                    }
                    {this.deleteButtons()}
                </form>
                <DepatmentUsers/>
            </>
        );
    }
}