////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { viewGroupGoods } from './viewGroupGoods';
import { NavLink } from 'react-router-dom'
import App from '../../../App';

/** Компонент для отображения удаления номенклатурной группы */
export class deleteGroupGoods extends viewGroupGoods {
    static displayName = deleteGroupGoods.name;
    cardTitle = 'Удаление группы номенклатуры';

    constructor(props) {
        super(props);

        /** удаление объекта из бд */
        this.handleDeleteClick = this.handleDeleteClick.bind(this);
    }

    /** удаление */
    async handleDeleteClick() {

        const response = await fetch(`/api/${App.controller}/${App.id}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            }
        });

        if (response.ok === true) {
            try {
                const result = await response.json();
                if (result.success === true) {
                    this.props.history.push(`/groupsgoods/${App.listNameMethod}`);
                }
                else {
                    this.clientAlert(result.info, result.status);
                }
            }
            catch (err) {
                this.clientAlert(err);
            }
        }
    }

    cardBody() {
        const goods = App.data.goods;

        const deleteButtonDisabled = (Array.isArray(goods) === false || goods.length > 0) || App.data.noDelete === true;
        const deleteButton = deleteButtonDisabled === true
            ? <button disabled title='Что бы удалить объект - предварительно перенесите номенклатуру в другие группы' className="btn btn-outline-secondary btn-block mb-3" type="button">Удаление невозможно</button>
            : <button onClick={this.handleDeleteClick} type="button" className="btn btn-outline-danger btn-block">Подтверждение удаления</button>;

        return (
            <>
                <label htmlFor="basic-url">Удаление группы номенклатуры</label>
                <input readOnly defaultValue={App.data.name} type="text" className="form-control mb-3" />
                {deleteButton}
                <NavLink className='btn btn-outline-primary btn-block' to={`/${App.controller}/${App.viewNameMethod}/${App.id}`} role='button' title='Редактирование/просмотр объекта'>Отмена</NavLink>
                <NavLink className='btn btn-outline-primary btn-block' to={`/${App.controller}/${App.listNameMethod}`} role='button' title='Вернуться в список/справочник'>Вернуться к списку</NavLink>
            </>
        );
    }
}
