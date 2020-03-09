import React, { Component } from 'react';
import { Route, Switch } from 'react-router';
import { NotFound } from './NotFound';
import { UsersList, UserCard } from './pages/Users';
import { DepartmentsList, DepartmentCard } from './pages/Departments';

export class Home extends Component {
    static displayName = Home.name;

    /** доступные типы данных (список REST котроллеров) */
    static allowsControllers = ['users', 'departments'];

    /** Имя метода для отображения списка объектов */
    static listNameMethod = 'list';
    /** Имя метода для отображения карточки объекта */
    static viewNameMethod = 'view';
    /** Имя метода для отображения формы создания объекта */
    static createNameMethod = 'create';
    /** Имя метода для отображения формы удаления объекта*/
    static deleteNameMethod = 'delete';
    /** Список доступных CRUD методов */
    static allowsMethods = [Home.listNameMethod, Home.viewNameMethod, Home.createNameMethod, Home.deleteNameMethod];

    /** Имя текущего метода */
    static method;
    /** ID текущего объекта */
    static id;

    /** context json data */
    static data = null;

    render() {
        Home.method = this.props.match.params.method;
        Home.id = this.props.match.params.id;
        Home.data = null;

        if (Home.allowsMethods.includes(Home.method) !== true) {
            console.error('Недопустимое имя метода: ' + Home.method);
            return <NotFound>Ошибка имени метода: {Home.method}<br />Доступные имена метдов: {Home.allowsMethods.join()}</NotFound>;
        }

        if (Home.id && Home.id > 0) { /*console.trace('id объекта: [' + Home.id + ']');*/ }
        else { Home.id = -1 }

        return (
            <main role="main" className="pb-3">
                <Switch>
                    <Route path={`/users/${Home.listNameMethod}/`} component={UsersList} />
                    <Route path='/users/' component={UserCard} />

                    <Route path={`/departments/${Home.listNameMethod}/`} component={DepartmentsList} />
                    <Route path='/departments/' component={DepartmentCard} />

                    <Route component={NotFound} />
                </Switch>
            </main>
        );
    }
}
