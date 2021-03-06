////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageList } from '../aPageList';
import { NavLink } from 'react-router-dom'
import App from '../../../App';
import { PaginatorComponent } from '../../PaginatorComponent';

/** Компонент для отображения номенклатурного состава группы */
export class viewGroupGoods extends aPageList {
    static displayName = viewGroupGoods.name;
    cardTitle = 'Номенклатурная группа';
    /** номенклатура группы */
    goods = [];
    /** единицы измерения номенклатуры */
    units = [];

    constructor(props) {
        super(props);

        this.state.newGoodName = '';
        this.state.newGoodUnitId = 0;

        /** изменение значения селектора еденицы измеренеия новой номенклатуры */
        this.handleNewGoodUnitChange = this.handleNewGoodUnitChange.bind(this);
        /** изменение значения поля имени новой номенклатуры */
        this.handleNewGoodNameChange = this.handleNewGoodNameChange.bind(this);
        /** сброс введёного значения поля новой номенклатуры на исходное */
        this.handleNewGoodResetClick = this.handleNewGoodResetClick.bind(this);
        /** сохранение новой номенклатуры в бд (текущая группа) */
        this.handleSaveNewGoodClick = this.handleSaveNewGoodClick.bind(this);
    }

    /**
     * событие изменения единицы измерения "создаваемой" номенклатуры
     * @param {object} e - object sender
     */
    handleNewGoodUnitChange(e) {
        const target = e.target;

        this.setState({
            newGoodUnitId: target.value
        });
    }

    /**
     * событие изменения имени "создаваемой" номенклатуры
     * @param {object} e - object sender
     */
    handleNewGoodNameChange(e) {
        const target = e.target;

        this.setState({
            newGoodName: target.value
        });
    }

    /** Сброс формы создания новой номенклатуры */
    handleNewGoodResetClick() {
        this.setState({
            newGoodName: '',
            newGoodUnitId: 0
        });
    }

    /** запись (создание) в бд новой номенклатуры */
    async handleSaveNewGoodClick() {
        var sendedFormData =
        {
            name: this.state.newGoodName,
            price: 0,
            unitId: parseInt(this.state.newGoodUnitId, 10),
            groupId: parseInt(App.id, 10)
        };

        const response = await fetch(`${this.apiPrefix}/goods/${App.id}`, {
            method: 'POST',
            body: JSON.stringify(sendedFormData),
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            }
        });

        if (response.ok === true) {
            try {
                const result = await response.json();
                if (result.success === true) {
                    App.data = null;
                    this.handleNewGoodResetClick();
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

    async ajax() {
        this.apiPostfix = `/${App.id}`;
        await super.ajax();
        this.setState({ groupName: App.data.name });
    }

    cardBody() {
        var apiName = App.controller;
        const data = App.data;
        const goods = data.goods;
        const units = data.units;
        this.servicePaginator.urlRequestAddress = `/${App.controller}/${App.viewNameMethod}/${App.id}`;

        const saveNewGoodButtonDisabled = this.state.newGoodName.length === 0 || this.state.newGoodUnitId === 0 || this.state.newGoodUnitId === '0';
        const resetNewGoodButtonDisabled = (this.state.newGoodName.length === 0 && (this.state.newGoodUnitId === 0 || this.state.newGoodUnitId === '0'));
        //
        const saveNewGoodButton = saveNewGoodButtonDisabled ? <button disabled title='Укажите наименование и единицу измерения номенклатуре' className="btn btn-outline-secondary" type="button">•</button> : <button title='Сохранить номенклатуру в БД' onClick={this.handleSaveNewGoodClick} className="btn btn-outline-success" type="button">Add</button>;
        const resetNewGoodButton = resetNewGoodButtonDisabled ? <></> : <button title='Сброс формы' onClick={this.handleNewGoodResetClick} className="btn btn-outline-primary" type="reset">Сброс</button>;

        return (
            <>
                <form>
                    <input name='id' defaultValue={data.id} type='hidden' />
                    <div className="form-group">
                        <label htmlFor="exampleInputEmail1">Имя группы номенклатуры</label>
                        <input defaultValue={data.name} name='name' type="text" aria-label="Введите название группы" className="form-control" id="exampleInputEmail1" placeholder="Введите название группы" />
                    </div>
                    {this.getInformation()}
                    {this.rootPanelObject()}
                    <br />
                    {this.viewButtons()}
                </form>
                <div className="card mt-3">
                    <div className="card-body">
                        <fieldset>
                            <legend>Состав группы: Номенклатура</legend>
                            <table className='table table-striped mt-4' aria-labelledby="tabelLabel">
                                <thead>
                                    <tr>
                                        <th>id</th>
                                        <th>Name</th>
                                        <th>Unit</th>
                                        <th>About</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr title='создание новой номенклатуры внутри текущей группы/папки'>
                                        <td colSpan='4'>
                                            <label>Ввод новой номенклатуры в справочник</label>
                                            <div className="input-group input-group-sm">
                                                <input onChange={this.handleNewGoodNameChange} title='Название номенклатуры' value={this.state.newGoodName} type="text" className="form-control" placeholder="Наименование новой номенклатуры" />
                                                <select onChange={this.handleNewGoodUnitChange} value={this.state.newGoodUnitId} title='Единица измерения' className="custom-select col-md-2" id="inputGroupSelect01">
                                                    <option value='0'>Ед.изм.</option>
                                                    {units.map(function (unit) { return <option key={unit.id} value={unit.id}>{unit.name}</option> })}
                                                </select>
                                                <div className="input-group-append">
                                                    {saveNewGoodButton}
                                                    {resetNewGoodButton}
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                    {goods.map(function (good) {
                                        const unit = units.find(function (element, index, array) { return element.id === good.unitId; });
                                        const currentNavLink = good.isDisabled === true
                                            ? <del><NavLink className='text-muted' to={`/${apiName}/${App.viewNameMethod}/${good.id}`} title='кликните для редактирования'>{good.name}</NavLink></del>
                                            : <NavLink to={`/goods/${App.viewNameMethod}/${good.id}`} title='кликните для редактирования'>{good.name}</NavLink>

                                        return <tr key={good.id}>
                                            <td>{good.id}</td>
                                            <td>
                                                {currentNavLink}
                                                <NavLink to={`/goods/${App.deleteNameMethod}/${good.id}`} title='удалить объект' className='text-danger ml-3'>del</NavLink>
                                            </td>
                                            <td title={unit.information}>{unit.name}</td>
                                            <td className={good.price > 0 ? '' : 'table-danger'}>${good.price}</td>
                                        </tr>
                                    })}
                                </tbody>
                            </table>
                        </fieldset>
                        <PaginatorComponent servicePaginator={this.servicePaginator} />
                    </div>
                </div>
            </>
        );
    }

    cardHeaderPanel() {
        return <NavLink className='btn btn-outline-info btn-sm' to={`/unitsgoods/${App.listNameMethod}/`} role='button' title='Перейти в справочник единиц измерения'>Единицы измерения</NavLink>;
    }
}
