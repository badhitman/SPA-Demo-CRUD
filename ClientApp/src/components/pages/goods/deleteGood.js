////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { NavLink } from 'react-router-dom'
import { viewGood } from './viewGood';
import App from '../../../App';

/** Удаление Номенклатуры */
export class deleteGood extends viewGood {
    static displayName = deleteGood.name;

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
                    this.props.history.push(`/groupsgoods/${App.viewNameMethod}/${App.data.groupId}`);
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

    async load() {
        await this.ajax();
        this.cardTitle = `Удаление номенклатуры: [#${App.data.id}] ${App.data.name}`;
        this.setState({ loading: false });
    }

    cardBody() {
        const buttonDelete = ((this.servicePaginator.rowsCount === 0 || this.servicePaginator.rowsCount === '0') && App.data.noDelete !== true)
            ? <button onClick={this.handleDeleteClick} type="button" className="btn btn-outline-danger btn-block">Подтверждение удаления</button>
            : <button disabled type="button" className="btn btn-outline-secondary btn-block" title='Вероятно на объект имеются ссылки'>Удаление невозможно</button>;

        var good = App.data;
        return (
            <>
                <div className="alert alert-danger" role="alert">Безвозратное удаление Номенклатуры! Данное дейтсвие нельзя будет отменить!</div>
                <NavLink to={`/${App.controller}/${App.viewNameMethod}/${App.id}`} className='btn btn-primary btn-block' role="button">Отмена</NavLink>
                {buttonDelete}
                <form className="my-3">
                    <div className='form-group row'>
                        <label className='col-sm-2 col-form-label'>Наименование</label>
                        <div className='col-sm-10'>
                            <input readOnly={true} defaultValue={good.name} className='form-control' type='text' />
                        </div>
                    </div>
                    <div className='form-group row'>
                        <label className='col-sm-2 col-form-label'>Группа</label>
                        <div className='col-sm-10'>
                            <input readOnly={true} defaultValue={good.groups.find(function (element, index, array) { return element.id === good.groupId }).name} className='form-control' type='text' />
                        </div>
                    </div>
                    <div className='form-group row'>
                        <label className='col-sm-2 col-form-label'>Единица измерения</label>
                        <div className='col-sm-10'>
                            <input readOnly={true} defaultValue={good.units.find(function (element, index, array) { return element.id === good.unitId }).name} className='form-control' type='text' />
                        </div>
                    </div>
                    <div className='form-group row'>
                        <label className='col-sm-2 col-form-label'>Цена</label>
                        <div className='col-sm-10'>
                            <input readOnly={true} defaultValue={good.price} className='form-control' type='text' />
                        </div>
                    </div>
                </form>
            </>
        );
    }
}
