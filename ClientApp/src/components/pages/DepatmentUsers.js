import React, { Component } from 'react';
import { Home } from '../Home';
import { NavLink } from 'react-router-dom'

/** Пользователи в контексте департаментов */
export class DepatmentUsers extends Component {
    static displayName = DepatmentUsers.name;
    apiName = 'usersbydepartments';

    render() {
        if (Home.data.users.length === 0) {
            return <footer className='blockquote-footer'>Сотрудников нет. Ни одному сотруднику не назначен департамент: #{Home.data.id} "{Home.data.name}"</footer>;
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
                        {Home.data.users.map(function (user) {
                            return (
                                <tr key={user.id}>
                                    <th scope="row">{user.id}</th>
                                    <td><NavLink to={`/users/${Home.viewNameMethod}/${user.id}`}>{user.name}</NavLink></td>
                                </tr>)
                        })}
                    </tbody>
                </table>
            </>
        );
    }
}