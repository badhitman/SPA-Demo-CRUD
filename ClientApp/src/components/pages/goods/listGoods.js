////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageList } from '../aPageList';
import { NavLink } from 'react-router-dom'
import App from '../../../App';

/** Компонент для отображения справочника номенклатуры */
export class listGoods extends aPageList {
    static displayName = listGoods.name;
    cardTitle = 'Справочник номенклатуры';

    cardBody() {
        var goods = App.data;
        var apiName = App.controller;
        return (
            <>
                <label htmlFor="basic-url">Создание новой группы/папки номенклатуры</label>
                <div className="input-group mb-3">
                    <div className="input-group-prepend">
                        <span className="input-group-text" id="basic-addon3">имя новой папки/группы</span>
                    </div>
                    <input type="text" className="form-control" placeholder="Введите название новой группы" aria-label="Введите название новой группы" aria-describedby="button-addon4" />
                    <div className="input-group-append" id="button-addon4">
                        <button className="btn btn-outline-secondary" type="button">Создать</button>
                        <button className="btn btn-outline-secondary" type="button">Сброс</button>
                    </div>
                </div>

                <NavLink to={`/${apiName}/${App.createNameMethod}/`} className="btn btn-primary btn-block" role="button">Создать новую номенклатуру</NavLink>

                <table className='table table-striped mt-4' aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>id</th>
                            <th>Name</th>
                        </tr>
                    </thead>
                    <tbody>
                        {goods.map(function (good) {
                            const currentNavLink = good.isDisabled === true
                                ? <del><NavLink className='text-muted' to={`/${apiName}/${App.viewNameMethod}/${good.id}`} title='кликните для редактирования'>{good.name}</NavLink></del>
                                : <NavLink to={`/${apiName}/${App.viewNameMethod}/${good.id}`} title='кликните для редактирования'>{good.name}</NavLink>

                            return <tr key={good.id}>
                                <td>{good.id}</td>
                                <td>
                                    {currentNavLink}
                                    <NavLink to={`/${apiName}/${App.deleteNameMethod}/${good.id}`} title='удалить объект' className='text-danger ml-3'>del</NavLink>
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
