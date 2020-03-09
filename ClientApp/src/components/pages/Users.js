import React from 'react';
import { aPageList, aPageCard } from './aPage';
import { NavLink } from 'react-router-dom'
import { Home } from '../Home';

/** Компонент для отображения списка пользователей */
export class UsersList extends aPageList {
    static displayName = UsersList.name;
    apiName = 'users';
    listCardHeader = 'Справочник пользователей';

    listRender() {
        var users = Home.data;
        const apiName = this.apiName;
        return (
            <>
                <NavLink to={`/${apiName}/${Home.createNameMethod}/`} className="btn btn-primary btn-block" role="button" >Создать нового пользователя</NavLink>

                <table className='table table-striped mt-4' aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>id</th>
                            <th>Name</th>
                            <th>Department</th>
                        </tr>
                    </thead>
                    <tbody>
                        {users.map(function (user) {
                            return <tr key={user.id}>
                                <td>{user.id}</td>
                                <td>
                                    <NavLink to={`/${apiName}/${Home.viewNameMethod}/${user.id}`} title='кликните для редактирования'>
                                        {user.name}
                                    </NavLink>
                                    <NavLink to={`/${apiName}/${Home.deleteNameMethod}/${user.id}`} title='удалить объект' className='text-danger ml-3'>del</NavLink>
                                </td>
                                <td><span className="badge badge-light">{user.department}</span></td>
                            </tr>
                        })}
                    </tbody>
                </table>
            </>
        );
    }
}

/** Компонент для отображения и управления конкретными объектами/пользователями */
export class UserCard extends aPageCard {
    static displayName = UserCard.name;
    apiName = 'users';

    async viewLoad() {
        const response = await fetch(`/api/${this.apiName}/${Home.id}`);
        Home.data = await response.json();
        this.setState({ cartTitle: `Пользователь: [#${Home.data.id}] ${Home.data.name}`, loading: false, cartContents: this[Home.method + 'Render']() });
    }
    viewRender() {
        var user = Home.data;
        var departments = Home.data.departments;
        return (
            <>
                <form className='mb-2'>
                    <input name='id' defaultValue={user.id} type='hidden' />
                    <div className="form-group">
                        <label htmlFor="user-input">Пользователь</label>
                        <input name='name' defaultValue={user.name} type="text" className="form-control" id="user-input" placeholder="Новое имя" />
                    </div>
                    <div className="form-group">
                        <select name='DepartmentId' className='custom-select' defaultValue={user.departmentId}>
                            {departments.map(function (department) {
                                return <option key={department.id} value={department.id}>{department.name}</option>
                            })}
                        </select>
                    </div>
                    {this.viewButtons()}
                </form>
            </>
        );
    }

    async createLoad() {
        this.setState({ cartTitle: 'Создание нового пользователя', loading: false, cartContents: this[Home.method + 'Render']() });
    }
    createRender() {
        return (
            <>
                <form>
                    <div className="form-group">
                        <label htmlFor="user-input">Имя/ФИО</label>
                        <input name='name' type="text" className="form-control" id="user-input" placeholder="Имя нового пользователя" />
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
        var user = Home.data;
        return (
            <>
                <div className="alert alert-danger" role="alert">Данное дейтсвие нельзя будет отменить!</div>
                <form className="mb-3">
                    {
                        Object.keys(user).map((keyName, i) => (
                            <div className='form-group row' key={i}>
                                <label htmlFor={keyName} className='col-sm-2 col-form-label'>{keyName}</label>
                                <div className='col-sm-10'>
                                    <input name={keyName} id={keyName} readOnly={true} defaultValue={user[keyName]} className='form-control' type='text' />
                                </div>
                            </div>
                        ))
                    }
                    {this.deleteButtons()}
                </form>
            </>
        );
    }
}