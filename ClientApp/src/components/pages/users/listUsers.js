////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageList } from '../aPageList';
import { NavLink } from 'react-router-dom'
import App from '../../../App';

/** Компонент для отображения списка пользователей */
export class listUsers extends aPageList {
    static displayName = listUsers.name;
    cardTitle = 'Справочник пользователей';

    cardBody() {
        var users = App.data;
        const apiName = App.controller;
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
                            const currentNavLink = user.isDisabled === true
                                ? <del><NavLink className='text-muted' to={`/${apiName}/${App.viewNameMethod}/${user.id}`} title='объект. отключен. кликните для редактирования'>{user.name} {user.Email}</NavLink></del>
                                : <NavLink to={`/${apiName}/${App.viewNameMethod}/${user.id}`} title='кликните для редактирования'>{user.name} {user.Email}</NavLink>

                            return <tr key={user.id}>
                                <td>{user.id} <span className="badge badge-light">{user.role}</span></td>
                                <td>
                                    {currentNavLink}
                                    <NavLink to={`/${apiName}/${App.deleteNameMethod}/${user.id}`} title='удалить объект' className='text-danger ml-3'>del</NavLink>
                                </td>
                                <td><span className="badge badge-light">{user.department}</span></td>
                            </tr>
                        })}
                    </tbody>
                </table>
                {this.cardPaginator()}
            </>
        );
    }
}