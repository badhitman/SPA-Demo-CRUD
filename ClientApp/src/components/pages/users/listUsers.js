////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageList } from '../aPage';
import { NavLink } from 'react-router-dom'
import App from '../../../App';

/** Компонент для отображения списка пользователей */
export class listUsers extends aPageList {
    static displayName = listUsers.name;
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
                                <td>{user.id} <span className="badge badge-light">{user.role}</span></td>
                                <td>
                                    <NavLink to={`/${apiName}/${App.viewNameMethod}/${user.id}`} title='кликните для редактирования'>
                                        {user.name} {user.Email}
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