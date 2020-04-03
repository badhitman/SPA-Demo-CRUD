////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React, { Component } from 'react';
import { Route, Switch } from 'react-router';
import { HomePage } from './components/pages/HomePage';
import { NotFound } from './components/NotFound';

import { listUsers } from './components/pages/users/listUsers';
import { viewUser } from './components/pages/users/viewUser';
import { createUser } from './components/pages/users/createUser';
import { deleteUser } from './components/pages/users/deleteUser';

import { listDepartments } from './components/pages/departments/listDepartments';
import { viewDepartment } from './components/pages/departments/viewDepartment';
import { createDepartment } from './components/pages/departments/createDepartment';
import { deleteDepartment } from './components/pages/departments/deleteDepartment';

import { listFiles } from './components/pages/files/listFiles';
import { viewFile } from './components/pages/files/viewFile';
import { deleteFile } from './components/pages/files/deleteFile';

import { SignIn } from './components/pages/SignIn';
import { NavMenu } from './components/NavMenu';
import App from './App';
import { AccessDenied } from './components/AccessDenied';

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

                        <Route path={`/files/${App.listNameMethod}/`} component={listFiles} />
                        <Route path={`/files/${App.viewNameMethod}`} component={viewFile} />
                        <Route path={`/files/${App.deleteNameMethod}`} component={deleteFile} />

                        <Route path={'/accessdenied/'} component={AccessDenied} />
                        <Route component={NotFound} />
                    </Switch>
                </main>
            </>
        );
    }
}
