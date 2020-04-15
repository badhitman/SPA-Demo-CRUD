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

        this.state.groupName = '';
        this.state.buttonsDisabled = true;
        this.state.newGoodName = '';
        this.state.newGoodUnitId = 0;
        this.state.newGoodButtonsDisabled = true;

        /** изменение значения селектора еденицы измеренеия новой номенклатуры */
        this.handleNewGoodUnitChange = this.handleNewGoodUnitChange.bind(this);
        /** изменение значения поля имени новой номенклатуры */
        this.handleNewGoodNameChange = this.handleNewGoodNameChange.bind(this);
        /** сброс введёного значения поля новой номенклатуры на исходное */
        this.handleNewGoodResetClick = this.handleNewGoodResetClick.bind(this);
        /** сохранение новой номенклатуры в бд (текущая группа) */
        this.handleSaveNewGoodClick = this.handleSaveNewGoodClick.bind(this);

        /** изменение значения поля нового имени группы */
        this.handleGroupNameChange = this.handleGroupNameChange.bind(this);
        /** сброс введёного значения поля имени группы на исходное*/
        this.handleResetClick = this.handleResetClick.bind(this);
        /** сохранение в бд группы номенклатуры */
        this.handleSaveClick = this.handleSaveClick.bind(this);
    }


    /**
     * событие изменения единицы измерения "создаваемой" номенклатуры
     * @param {object} e - object sender
     */
    handleNewGoodUnitChange(e) {
        const target = e.target;
        const newGoodButtonsDisabled = target.value === 0;

        this.setState({
            newGoodUnitId: target.value,
            newGoodButtonsDisabled: newGoodButtonsDisabled
        });
    }

    /**
     * событие изменения имени "создаваемой" номенклатуры
     * @param {object} e - object sender
     */
    handleNewGoodNameChange(e) {
        const target = e.target;
        var newGoodButtonsDisabled = true;
        if (target.value) {
            if (target.value.length > 0 && target.value.trim() !== App.data.name) {
                newGoodButtonsDisabled = false;
            }
        }

        this.setState({
            newGoodName: target.value,
            newGoodButtonsDisabled: newGoodButtonsDisabled
        });
    }

    /** Сброс формы создания новой номенклатуры */
    handleNewGoodResetClick() {
        this.setState({
            newGoodName: '',
            newGoodUnitId: 0,
            newGoodButtonsDisabled: true
        });
    }

    /** добавление в бд новой номенклатуры */
    async handleSaveNewGoodClick() {
        var sendedFormData = { name: this.state.newGoodName, price: 0, unitId: parseInt(this.state.newGoodUnitId, 10), groupId: parseInt(App.id, 10) };
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

    /**
     * событие изменения имени "создаваемой" номенклатурной группы
     * @param {object} e - object sender
     */
    handleGroupNameChange(e) {
        const target = e.target;
        var buttonsDisabled = true;
        if (target.value) {
            if (target.value.length > 0 && target.value.trim() !== App.data.name) {
                buttonsDisabled = false;
            }
        }

        this.setState({
            groupName: target.value,
            buttonsDisabled: buttonsDisabled
        });
    }

    /** Сброс формы создания новой группы номенклатуры */
    handleResetClick() {
        this.setState({
            groupName: App.data.name,
            buttonsDisabled: true
        });
    }

    /** сохранение имени номенклатурной группы */
    async handleSaveClick() {
        var sendedFormData = { id: parseInt(App.id, 10), name: this.state.groupName };
        const response = await fetch(`${this.apiPrefix}/${App.controller}/${App.id}`, {
            method: 'PUT',
            body: JSON.stringify(sendedFormData),
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            }
        });

        if (response.ok === true) {
            try {
                const result = await response.json();
                if (result.success === true) {
                    result.tag.goods = App.data.goods;
                    result.tag.units = App.data.units;
                    App.data = result.tag;
                    this.handleResetClick();
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
        //this.servicePaginator
        this.setState({ groupName: App.data.name });
    }

    cardBody() {
        var apiName = App.controller;

        const goods = App.data.goods;
        const units = App.data.units;
        this.servicePaginator.urlRequestAddress = `/${App.controller}/${App.viewNameMethod}/${App.id}`;

        const saveButtonDisabled = this.state.buttonsDisabled === true;
        const resetButtonDisabled = saveButtonDisabled && this.state.groupName.length > 0;
        const deleteButtonDisabled = (Array.isArray(goods) === false || goods.length > 0);

        const saveButton = saveButtonDisabled === true
            ? <button disabled title='Вы можете ввести новое имя группе' className="btn btn-outline-secondary" type="button">Ξ</button>
            : <button title='Сохранить новое имя группы в БД' onClick={this.handleSaveClick} className="btn btn-outline-success" type="button">Ok</button>;

        const resetButton = resetButtonDisabled === true
            ? <></>
            : <button title='Сброс имени на оригинальное значание из БД' onClick={this.handleResetClick} className="btn btn-outline-primary" type="reset">Отмена</button>;

        const deleteButton = deleteButtonDisabled === true
            ? <button disabled title='Что бы удалить объект - предварительно перенесите номенклатуру в другие группы' className="btn btn-outline-secondary btn-block mb-3" type="button">Удаление невозможно</button>
            : <NavLink className='btn btn-outline-danger btn-block' to={`/${App.controller}/${App.deleteNameMethod}/${App.id}`} role='button' title='Диалог удаления объекта'>Удаление</NavLink>;

        const saveNewGoodButtonDisabled = this.state.newGoodButtonsDisabled === true || this.state.newGoodName.length === 0 || this.state.newGoodUnitId === 0 || this.state.newGoodUnitId === '0';
        const resetNewGoodButtonDisabled = (this.state.newGoodName.length === 0 && (this.state.newGoodUnitId === 0 || this.state.newGoodUnitId === '0'));

        const saveNewGoodButton = saveNewGoodButtonDisabled ? <button disabled title='Укажите наименование и единицу измерения номенклатуре' className="btn btn-outline-secondary" type="button">•</button> : <button title='Сохранить номенклатуру в БД' onClick={this.handleSaveNewGoodClick} className="btn btn-outline-success" type="button">Add</button>;
        const resetNewGoodButton = resetNewGoodButtonDisabled ? <></> : <button title='Сброс формы' onClick={this.handleNewGoodResetClick} className="btn btn-outline-primary" type="reset">Сброс</button>;

        var myButtons = <>{saveButton}{resetButton}</>;
        return (
            <>
                <label htmlFor="basic-url">Имя группы номенклатуры</label>
                <div className="input-group mb-3">
                    <input onChange={this.handleGroupNameChange} value={this.state.groupName} type="text" className="form-control" placeholder="Введите название группы" aria-label="Введите название группы" aria-describedby="button-addon4" />
                    <div className="input-group-append" id="button-addon4">
                        {myButtons}
                    </div>
                </div>
                <NavLink className='btn btn-outline-primary btn-block' to={`/${App.controller}/${App.listNameMethod}/`} role='button' title='Перейти к списку групп номенклатуры'>К списку групп</NavLink>
                {deleteButton}
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
                                                <input onChange={this.handleNewGoodNameChange} title='Название номенклатуры' value={this.state.newGoodName} type="text" className="form-control" placeholder="Наименование новой номенклатуры" aria-describedby="button-addon4" />
                                                <select onChange={this.handleNewGoodUnitChange} value={this.state.newGoodUnitId} title='Единица измерения' className="custom-select col-md-2" id="inputGroupSelect01">
                                                    <option value='0'>Ед.изм.</option>
                                                    {units.map(function (unit) { return <option key={unit.id} value={unit.id}>{unit.name}</option> })}
                                                </select>
                                                <div className="input-group-append" id="button-addon4">
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
