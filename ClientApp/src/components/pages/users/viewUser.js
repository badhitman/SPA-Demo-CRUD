////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageCard } from '../aPageCard';
import App from '../../../App';

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
        const user = App.data;
        const departments = user.departments;
        const roles = user.roles;

        return (
            <form className='mb-2' key='view-form'>
                <input name='id' defaultValue={user.id} type='hidden' />
                <div className="form-group">
                    <label htmlFor="user-input">Публичное имя</label>
                    <input name='name' defaultValue={user.name} type="text" className="form-control" id="user-input" placeholder="Новое имя" />
                </div>
                <div className='form-row'>
                    <div className='col'>
                        <div className="form-group">
                            <label htmlFor="DepartmentId">Отдел/Департамент</label>
                            <select name='departmentId' id='DepartmentId' className='custom-select' defaultValue={user.departmentId}>
                                {departments.map(function (department) {
                                    return <option key={department.id} value={department.id}>{department.name}</option>
                                })}
                            </select>
                        </div>
                    </div>
                    <div className='col'>
                        <label htmlFor="RoleId">Роль (права доступа)</label>
                        <select name='role' id='RoleId' className='custom-select' defaultValue={user.role}>
                            {roles.map(function (role) {
                                return <option key={role.id} value={role.id}>{role.name}</option>
                            })}
                        </select>
                    </div>
                </div>

                <div className="form-group">
                    <label htmlFor="user-email">Email/Login для входа</label>
                    <input name='email' defaultValue={user.email} type="email" className="form-control" id="user-email" />
                </div>
                <div className="form-group">
                    <label htmlFor="user-telegram-id">Telegram идентификатор</label>
                    <input readOnly={true} name='telegramId' defaultValue={user.telegramId} type="number" className="form-control" id="user-telegram-id" />
                </div>
                {this.viewButtons()}
            </form>
        );
    }

    isPrime(element, index, array) {
        array.forEach(element => console.log(element));
        return false;
    }
}