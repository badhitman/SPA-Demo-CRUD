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

    constructor(props) {
        super(props);

        this.state.unitName = '';
        this.state.unitInfo = '';
        this.state.buttonsDisabled = true;

        /** изменение значения поля полного имени */
        this.handleUnitInfoChange = this.handleUnitInfoChange.bind(this);
        /** изменение значения поля краткого имени */
        this.handleUnitNameChange = this.handleUnitNameChange.bind(this);
        /** сброс введёного значения поля на исходное: имя */
        this.handleResetClick = this.handleResetClick.bind(this);
        /** сохранение в бд группы номенклатуры */
        this.handleSaveClick = this.handleSaveClick.bind(this);
    }

    /**
     * событие изменения краткого имени единицы измерения
     * @param {object} e - object sender
     */
    handleUnitNameChange(e) {
        const target = e.target;
        const data = App.data;
        var buttonsDisabled = true;
        if (target.value) {
            if (target.value.length > 0 && target.value.trim() !== data.name && this.state.unitInfo.length > 0) {
                buttonsDisabled = false;
            }
        }

        this.setState({
            unitName: target.value,
            buttonsDisabled: buttonsDisabled
        });
    }

    /**
     * событие изменения полного имени единицы измерения
     * @param {object} e - object sender
     */
    handleUnitInfoChange(e) {
        const target = e.target;
        const data = App.data;
        var buttonsDisabled = true;
        if (target.value) {
            if (target.value.length > 0 && target.value.trim() !== data.information && this.state.unitName.length > 0) {
                buttonsDisabled = false;
            }
        }

        this.setState({
            unitInfo: target.value,
            buttonsDisabled: buttonsDisabled
        });
    }

    /** Сброс формы создания новой группы номенклатуры */
    handleResetClick() {
        const data = App.data;
        this.setState({
            unitName: data.name,
            unitInfo: data.information,
            buttonsDisabled: true
        });
    }

    /** сохранение имени номенклатурной группы */
    async handleSaveClick() {
        var sendedFormData = { id: parseInt(App.id, 10), name: this.state.unitName, information: this.state.unitInfo };
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
        const data = App.data;
        this.setState({ unitName: data.name, unitInfo: data.information });
    }

    cardBody() {
        var goods = App.data.goods;

        const buttonsDisabled = this.state.buttonsDisabled === true;
        const resetButtonDisabled = buttonsDisabled === true && this.state.unitName.length > 0 && this.state.unitInfo.length > 0;

        const saveButton = buttonsDisabled === true
            ? <button disabled className="btn btn-outline-secondary" type="button">Записать</button>
            : <button onClick={this.handleSaveClick} className="btn btn-outline-success" type="button">Записать</button>;

        const resetButton = resetButtonDisabled === true
            ? <button disabled className="btn btn-outline-secondary" type="reset">Сброс</button>
            : <button onClick={this.handleResetClick} className="btn btn-outline-primary" type="reset">Сброс</button>;

        const deleteButton = Array.isArray(goods) === true && goods.length === 0 && App.data.noDelete !== true
            ? <NavLink className='btn btn-outline-danger btn-block' to={`/${App.controller}/${App.deleteNameMethod}/${App.id}`} role='button' title='Удалить единицу измерения'>Удаление</NavLink>
            : <button disabled title='Нельзя удалять ед.изм., на которую есть ссылки в объектах справочника номенклатуры' type="button" className="btn btn-outline-secondary btn-block">Удаление невозможно</button>

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
                <label htmlFor="basic-url">Название единицы измерения</label>
                <div className="input-group mb-3">
                    <input onChange={this.handleUnitNameChange} value={this.state.unitName} type="text" className="form-control" placeholder="Введите краткое название единицы измерения" aria-label="Введите краткое название ед.изм." />
                    <input onChange={this.handleUnitInfoChange} value={this.state.unitInfo} type="text" className="form-control" placeholder="Введите полное наименование единицы измерения" aria-label="Введите полное наименование ед.изм." />
                    <div className="input-group-append" id="button-addon4">
                        {saveButton}
                        {resetButton}
                    </div>
                </div>
                <NavLink className='btn btn-outline-info btn-block' to={`/${App.controller}/${App.listNameMethod}/`} role='button' title='Вернуться к списку единиц измерения номенклатуры'>К перечню едениц измерения</NavLink>
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
