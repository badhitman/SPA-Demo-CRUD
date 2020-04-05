////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React, { Component } from 'react';
import App from '../../../App';
import { NavLink } from 'react-router-dom'

/** Пользователи в контексте департаментов */
export class DepatmentUsers extends Component {
    static displayName = DepatmentUsers.name;

    render() {
        if (App.data.users.length === 0) {
            return <footer className='blockquote-footer'>Сотрудников нет. Ни одному сотруднику не назначен департамент: #{App.data.id} "{App.data.name}"</footer>;
        }

        return (
            <>
                <p>В этом департаменте числятся сотрудники:</p>
                <table className="table table-striped table-hover table-sm">
                    <thead>
                        <tr>
                            <th scope="col">#</th>
                            <th scope="col">Имя</th>
                        </tr>
                    </thead>
                    <tbody>
                        {App.data.users.map(function (user) {
                            return (
                                <tr key={user.id}>
                                    <th scope="row">{user.id}</th>
                                    <td><NavLink to={`/users/${App.viewNameMethod}/${user.id}`}>{user.name}</NavLink></td>
                                </tr>)
                        })}
                    </tbody>
                </table>
            </>
        );
    }
}