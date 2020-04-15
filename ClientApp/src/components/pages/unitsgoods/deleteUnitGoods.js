////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { viewUnitGoods } from './viewUnitGoods';
import { NavLink } from 'react-router-dom'
import App from '../../../App';
import { PaginatorComponent } from '../../PaginatorComponent';

/** Компонент для отображения удаления единицы измерения номенклатуры */
export class deleteUnitGoods extends viewUnitGoods {
    static displayName = deleteUnitGoods.name;
    cardTitle = 'Удаление единицы измерения';

    constructor(props) {
        super(props);

        /** сохранение в бд группы номенклатуры */
        this.handleDeleteButtonClick = this.handleDeleteButtonClick.bind(this);
    }

    async handleDeleteButtonClick() {
        var urlBody = `${this.apiPrefix}/${App.controller}${this.apiPostfix}`;
        var response = await fetch(urlBody, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            }
        });

        if (response.redirected === true) {
            window.location.href = response.url;
        }

        var result = await response.json();
        if (response.ok) {
            if (result.success === false) {
                this.clientAlert(result.info, result.status, 1000, 15000);
                return;
            }

            this.props.history.push(`/${App.controller}/${App.listNameMethod}/`);
        }
        else {
            var errorsString = App.mapObjectToArr(result.errors).join('<br/>');
            this.clientAlert(errorsString, result.status, 1000, 15000);

            const msg = `Ошибка обработки HTTP запроса. Status: ${response.status}`;
            console.error(msg);
            return;
        }

        this.props.history.push(`/${App.controller}/${App.listNameMethod}/`);
    }

    cardBody() {
        var goods = App.data.goods;

        const deleteButton = Array.isArray(goods) === true && goods.length === 0
            ? <button onClick={this.handleDeleteButtonClick} className='btn btn-outline-danger btn-block' title='Подтверждение удаления единицы измерения'>Подтверждение удаления</button>
            : <button disabled title='Нельзя удалять ед.изм., на которую есть ссылки в объектах справочника номенклатуры' type="button" className="btn btn-outline-secondary btn-block">Удаление невозможно</button>

        const dataTableDom = Array.isArray(goods) === true && goods.length > 0
            ? <><div className='card mt-4'>
                <div className='card-body'>
                    <legend>Номенклатура с этой еденицей измерения</legend>
                    {this.tableGoods}
                    <PaginatorComponent servicePaginator={this.servicePaginator} />
                </div>
            </div></>
            : <></>

        return (
            <>
                <div className="form-row">
                    <div className="form-group col-md-6">
                        <label htmlFor="inputName">Краткое название</label>
                        <input id='inputName' readOnly defaultValue={App.data.name} type="text" className="form-control" aria-label="Краткое название ед.изм." />
                    </div>
                    <div className="form-group col-md-6">
                        <label htmlFor='inputInfo'>Полное наименование</label>
                        <input id='inputInfo' readOnly defaultValue={App.data.information} type="text" className="form-control" aria-label="Полное наименование ед.изм." />
                    </div>
                </div>
                <NavLink className='btn btn-outline-info btn-block' to={`/${App.controller}/${App.listNameMethod}`} role='button' title='Вернуться к списку единиц измерения номенклатуры'>К перечню едениц измерения</NavLink>
                <NavLink className='btn btn-outline-primary btn-block' to={`/${App.controller}/${App.viewNameMethod}/${App.id}`} role='button' title='Вернуться в карточку единицы измерения'>Отмена</NavLink>
                {deleteButton}
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
