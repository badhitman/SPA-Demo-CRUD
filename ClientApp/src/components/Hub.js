////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React, { Component } from 'react';
import { Route, Switch } from 'react-router';
import { HomePage } from './pages/HomePage';
import { NotFound } from './NotFound';

import { listUsers } from './pages/users/listUsers';
import { viewUser } from './pages/users/viewUser';
import { createUser } from './pages/users/createUser';
import { deleteUser } from './pages/users/deleteUser';

import { listDepartments } from './pages/departments/listDepartments';
import { viewDepartment } from './pages/departments/viewDepartment';
import { createDepartment } from './pages/departments/createDepartment';
import { deleteDepartment } from './pages/departments/deleteDepartment';

import { SignIn } from './pages/SignIn';
import { NavMenu } from './NavMenu';
import App from '../App';
import { AccessDenied } from './AccessDenied';

export class Hub extends Component {
    static displayName = Hub.name;

    render() {
        App.method = this.props.match.params.method;
        App.id = this.props.match.params.id;
        App.data = { id: '', name: '' };

        if (App.method !== undefined && App.allowsMethods.includes(App.method) !== true) {
            console.error('Недопустимое имя метода: ' + App.method);
            return <NotFound>Ошибка имени метода: {App.method}<br />Доступные имена метдов: {App.allowsMethods.join()}</NotFound>;
        }

        if (App.id && App.id > 0) { /*console.trace('id объекта: [' + App.id + ']');*/ }
        else { App.id = -1 }

        return (
            <>
                <NavMenu key={App.session.role} />
                <main role="main" className="pb-3">
                    <Switch>
                        <Route exact path='/' component={HomePage} />
                        <Route path={'/signin/'} component={SignIn} />

                        <Route path={`/users/${App.listNameMethod}/`} component={listUsers} />
                        <Route path={`/users/${App.viewNameMethod}`} component={viewUser} />
                        <Route path={`/users/${App.createNameMethod}`} component={createUser} />
                        <Route path={`/users/${App.deleteNameMethod}`} component={deleteUser} />

                        <Route path={`/departments/${App.listNameMethod}/`} component={listDepartments} />
                        <Route path={`/departments/${App.viewNameMethod}`} component={viewDepartment} />
                        <Route path={`/departments/${App.createNameMethod}`} component={createDepartment} />
                        <Route path={`/departments/${App.deleteNameMethod}`} component={deleteDepartment} />

                        <Route path={'/accessdenied/'} component={AccessDenied} />
                        <Route component={NotFound} />
                    </Switch>
                </main>
            </>
        );
    }
}
