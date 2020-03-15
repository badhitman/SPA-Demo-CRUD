import React from 'react';
import { aPageList, aPageCard } from './aPage';
import { NavLink } from 'react-router-dom'
import App from '../../App';

/** Компонент для отображения списка пользователей */
export class UsersList extends aPageList {
    static displayName = UsersList.name;
    apiName = 'users';
    listCardHeader = 'Справочник пользователей';

    body() {
        var users = App.data;
        const apiName = this.apiName;
        return (
            <>
                <NavLink to={`/${apiName}/${App.createNameMethod}/`} className="btn btn-primary btn-block" role="button" >Создать нового пользователя</NavLink>

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
                                    <NavLink to={`/${apiName}/${App.viewNameMethod}/${user.id}`} title='кликните для редактирования'>
                                        {user.name}
                                    </NavLink>
                                    <NavLink to={`/${apiName}/${App.deleteNameMethod}/${user.id}`} title='удалить объект' className='text-danger ml-3'>del</NavLink>
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

/** Отображение/редактирование существующего объекта/пользователя */
export class viewUser extends aPageCard {
    static displayName = viewUser.name;
    apiName = 'users';

    async load() {
        const response = await fetch(`/api/${this.apiName}/${App.id}`);
        App.data = await response.json();
        this.setState({ cardTitle: `Пользователь: [#${App.data.id}] ${App.data.name}`, loading: false, cardContents: this.body() });
    }
    body() {
        var user = App.data;
        var departments = user.departments;
        return (
            <form className='mb-2' key='view-form'>
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
        );
    }
}

/** Создание нового объекта/пользователя */
export class createUser extends viewUser {
    static displayName = createUser.name;

    async load() {
        this.setState({ cardTitle: 'Создание нового пользователя', loading: false, cardContents: this.body() });
    }
    body() {
        return (
            <>
                <form key='create-form'>
                    <div className="form-group">
                        <label htmlFor="user-input">Имя/ФИО</label>
                        <input name='name' type="text" className="form-control" id="user-input" placeholder="Имя нового пользователя" />
                    </div>
                    {this.createButtons()}
                </form>
            </>
        );
    }
}

/** Удаление объекта/пользователя */
export class deleteUser extends viewUser {
    static displayName = deleteUser.name;

    async load() {
        const response = await fetch(`/api/${this.apiName}/${App.id}`);
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