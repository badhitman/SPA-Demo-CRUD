////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import App from '../../../App';
import { viewUser } from './viewUser';

/** Создание нового объекта/пользователя */
export class createUser extends viewUser {
    static displayName = createUser.name;
    usersmetadata = {};

    async load() {
        const response = await fetch('/api/usersmetadata/');
        this.usersmetadata = await response.json();
        this.cardTitle = 'Создание нового пользователя';
        App.data = {};
        this.setState({ loading: false });
    }

    cardHeaderPanel() {
        return <></>;
    }

    cardBody() {
        const departments = this.usersmetadata.departments;
        const roles = this.usersmetadata.roles;

        return (
            <>
                <form key='create-form'>
                    <div className="form-group">
                        <label htmlFor="user-email">Email/Login для входа</label>
                        <input name='email' type="email" className="form-control" id="user-email" />
                    </div>
                    <div className="form-group">
                        <label htmlFor="user-input">Публичное имя</label>
                        <input name='name' type="text" className="form-control" id="user-input" placeholder="Имя нового пользователя" />
                    </div>
                    <div className='form-row'>
                        <div className='col'>
                            <div className="form-group">
                                <label htmlFor="DepartmentId">Отдел/Департамент</label>
                                <select name='departmentId' id='DepartmentId' className='custom-select'>
                                    {departments.map(function (department) {
                                        return <option key={department.id} value={department.id}>{department.name}</option>
                                    })}
                                </select>
                            </div>
                        </div>
                        <div className='col'>
                            <label htmlFor="RoleId">Роль (права доступа)</label>
                            <select name='role' id='RoleId' className='custom-select'>
                                {roles.map(function (role) {
                                    return <option key={role.id} value={role.id}>{role.name}</option>
                                })}
                            </select>
                        </div>
                    </div>
                    {this.createButtons()}
                </form>
            </>
        );
    }
}