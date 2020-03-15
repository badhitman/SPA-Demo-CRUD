import React, { Component } from 'react';
import { Route, Switch } from 'react-router';
import { NotFound } from './NotFound';
import { UsersList, viewUser, createUser, deleteUser } from './pages/Users';
import { DepartmentsList, viewDepartment, createDepartment, deleteDepartment } from './pages/Departments';
import App from '../App';

export class Home extends Component {
    static displayName = Home.name;

    render() {
        App.method = this.props.match.params.method;
        App.id = this.props.match.params.id;
        App.data = null;

        if (App.allowsMethods.includes(App.method) !== true) {
            console.error('Недопустимое имя метода: ' + App.method);
            return <NotFound>Ошибка имени метода: {App.method}<br />Доступные имена метдов: {App.allowsMethods.join()}</NotFound>;
        }

        if (App.id && App.id > 0) { /*console.trace('id объекта: [' + Home.id + ']');*/ }
        else { App.id = -1 }

        return (
            <main role="main" className="pb-3">
                <Switch>
                    <Route path={`/users/${App.listNameMethod}/`} component={UsersList} />
                    <Route path={`/users/${App.viewNameMethod}`} component={viewUser} />
                    <Route path={`/users/${App.createNameMethod}`} component={createUser} />
                    <Route path={`/users/${App.deleteNameMethod}`} component={deleteUser} />

                    <Route path={`/departments/${App.listNameMethod}/`} component={DepartmentsList} />
                    <Route path={`/departments/${App.viewNameMethod}`} component={viewDepartment} />
                    <Route path={`/departments/${App.createNameMethod}`} component={createDepartment} />
                    <Route path={`/departments/${App.deleteNameMethod}`} component={deleteDepartment} />

                    <Route component={NotFound} />
                </Switch>
            </main>
        );
    }
}
