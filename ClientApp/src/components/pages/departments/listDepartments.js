////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageList } from '../aPageList';
import { NavLink } from 'react-router-dom'
import App from '../../../App';

/** Компонент для отображения списка департаментов */
export class listDepartments extends aPageList {
    static displayName = listDepartments.name;
    cardTitle = 'Справочник департаментов';

    cardBody() {
        var departments = App.data;
        var apiName = App.controller;
        return (
            <>
                <NavLink to={`/${apiName}/${App.createNameMethod}/`} className="btn btn-primary btn-block" role="button">Создать новый департамент</NavLink>

                <table className='table table-striped mt-4' aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>id</th>
                            <th>Name</th>
                        </tr>
                    </thead>
                    <tbody>
                        {departments.map(function (department) {
                            const currentNavLink = department.isDisabled === true
                                ? <del><NavLink className='text-muted' to={`/${apiName}/${App.viewNameMethod}/${department.id}`} title='кликните для редактирования'>{department.name}</NavLink></del>
                                : <NavLink to={`/${apiName}/${App.viewNameMethod}/${department.id}`} title='кликните для редактирования'>{department.name}</NavLink>

                            return <tr key={department.id}>
                                <td>{department.id}</td>
                                <td>
                                    {currentNavLink}
                                    <NavLink to={`/${apiName}/${App.deleteNameMethod}/${department.id}`} title='удалить объект' className='text-danger ml-3'>del</NavLink>
                                </td>
                            </tr>
                        })}
                    </tbody>
                </table>
                {this.cardPaginator()}
            </>
        );
    }
}
