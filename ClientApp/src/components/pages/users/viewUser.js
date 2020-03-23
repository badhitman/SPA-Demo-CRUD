////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageCard } from '../aPage';
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
                <div className="form-group">
                    <label htmlFor="user-email">Email</label>
                    <input name='user-email' defaultValue={user.Email} type="email" className="form-control" id="user-email" />
                </div>
                <div className="form-group">
                    <label htmlFor="user-telegram-id">Telegram идентификатор</label>
                    <input readOnly={true} name='user-telegram-id' defaultValue={user.TelegramId} type="number" className="form-control" id="user-telegram-id" />
                </div>
                {this.viewButtons()}
            </form>
        );
    }
}