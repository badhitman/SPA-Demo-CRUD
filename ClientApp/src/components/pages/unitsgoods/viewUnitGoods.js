////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageList } from '../aPageList';
import { NavLink } from 'react-router-dom'
import App from '../../../App';
import { PaginatorComponent } from '../../PaginatorComponent';

/** Компонент для отображения единицы измерения номенклатуры */
export class viewUnitGoods extends aPageList {
    static displayName = viewUnitGoods.name;
    cardTitle = 'Единица измерения';

    async ajax() {
        this.apiPostfix = `/${App.id}`;
        await super.ajax();
    }

    cardBody() {
        var goods = App.data.goods;

        const dataTableDom = Array.isArray(goods) === true && goods.length > 0
            ? <><div className='card mt-4'>
                <div className='card-body'>
                    <legend>Номенклатура с этой еденицей измерения</legend>
                    {this.tableGoods}
                    <PaginatorComponent servicePaginator={this.servicePaginator} />
                </div>
            </div></>
            : <><div className='card mt-2'>
                <div className='card-body'>
                    <legend>с этой еденицей измерения номенклатуры нет</legend>
                </div>
            </div></>

        return (
            <>
                <form>
                    <input name='id' defaultValue={App.data.id} type='hidden' />
                    <div className="form-group">
                        <label htmlFor="departments-input">Наименование</label>
                        <input name='name' defaultValue={App.data.name} type="text" className="form-control" id="departments-input" placeholder="Новое название" />
                    </div>
                    {this.getInformation()}
                    {this.viewButtons()}
                </form>
                {dataTableDom}
            </>
        );
    }

    /** Получить таблицу данных */
    get tableGoods() {
        var goods = App.data.goods;
        return <table className='table table-striped mt-4' aria-labelledby="tabelLabel">
            <thead>
                <tr>
                    <th>id</th>
                    <th>Name</th>
                </tr>
            </thead>
            <tbody>
                {goods.map(function (good) {
                    const currentNavLink = good.isDisabled === true
                        ? <del><NavLink className='text-muted' to={`/${App.controller}/${App.viewNameMethod}/${good.id}`} title='кликните для редактирования'>{good.name}</NavLink></del>
                        : <NavLink to={`/goods/${App.viewNameMethod}/${good.id}`} title='кликните для редактирования'>{good.name}</NavLink>

                    return <tr key={good.id}>
                        <td>{good.id}</td>
                        <td>
                            {currentNavLink}
                            <NavLink to={`/goods/${App.deleteNameMethod}/${good.id}`} title='удалить объект' className='text-danger ml-3'>del</NavLink>
                        </td>
                    </tr>
                })}
            </tbody>
        </table>
    }

    cardHeaderPanel() {
        return <NavLink className='btn btn-outline-primary btn-sm' to={`/groupsgoods/${App.listNameMethod}/`} role='button' title='Перейти к справочнику номенклатуры'>Номенклатура</NavLink>;
    }
}
