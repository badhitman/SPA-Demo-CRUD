////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageList } from '../aPageList';
import { NavLink } from 'react-router-dom'
import App from '../../../App';
import { PaginatorComponent } from '../../PaginatorComponent';

/** Компонент для отображения справочника единиц измерения номенклатуры */
export class listUnitsGoods extends aPageList {
    static displayName = listUnitsGoods.name;
    cardTitle = 'Единицы измерения';

    constructor(props) {
        super(props);

        this.state.newName = '';
        this.state.newInfo = '';
        this.state.buttonsDisabled = true;

        /** изменение значения поля полного наименования */
        this.handleNewInfoChange = this.handleNewInfoChange.bind(this);
        /** изменение значения поля нового короткого имени */
        this.handleNewNameChange = this.handleNewNameChange.bind(this);
        /** сброс введёного значения поля: имя */
        this.handleResetClick = this.handleResetClick.bind(this);
        /** добавление в бд новой группы номенклатуры */
        this.handleCreateClick = this.handleCreateClick.bind(this);
    }

    /**
     * событие изменения имени "создаваемой" единицы измерения
     * @param {object} e - object sender
     */
    handleNewInfoChange(e) {
        const target = e.target;
        var buttonsDisabled = true;
        if (target.value) {
            if (target.value.length > 0 && this.state.newName.length > 0) {
                buttonsDisabled = false;
            }
        }

        this.setState({
            newInfo: target.value,
            buttonsDisabled: buttonsDisabled
        });
    }

    /**
     * событие изменения имени "создаваемой" единицы измерения
     * @param {object} e - object sender
     */
    handleNewNameChange(e) {
        const target = e.target;
        var buttonsDisabled = true;
        if (target.value) {
            if (target.value.length > 0 && this.state.newInfo.length > 0) {
                buttonsDisabled = false;
            }
        }

        this.setState({
            newName: target.value,
            buttonsDisabled: buttonsDisabled
        });
    }

    /** Сброс формы создания новой единицы измерения */
    handleResetClick() {
        this.setState({
            newName: '',
            newInfo: '',
            buttonsDisabled: true
        });
    }

    /** Создание единицы измерения */
    async handleCreateClick() {
        var sendedFormData = { name: this.state.newName, information: this.state.newInfo };
        const response = await fetch(`${this.apiPrefix}/${App.controller}`, {
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

    cardBody() {
        var units = App.data;
        var apiName = App.controller;
        const myButtons = this.state.buttonsDisabled === true
            ? <><button disabled className="btn btn-outline-secondary" type="button">Создать</button>
                <button disabled className="btn btn-outline-secondary" type="reset">Сброс</button></>
            : <><button onClick={this.handleCreateClick} className="btn btn-outline-success" type="button">Создать</button>
                <button onClick={this.handleResetClick} className="btn btn-outline-primary" type="reset">Сброс</button></>
        return (
            <>
                <label htmlFor="basic-url">Создание новой единицы измерения</label>
                <div className="input-group mb-3">
                    <input onChange={this.handleNewNameChange} value={this.state.newName} type="text" className="form-control" placeholder="Наименование (краткое) новой ед.изм." aria-label="Введите краткое название новой единицы измерения" />
                    <input onChange={this.handleNewInfoChange} value={this.state.newInfo} type="text" className="form-control" placeholder="Наименование (полное) новой ед.изм." aria-label="Введите полное название новой единицы измерения" />
                    <div className="input-group-append">
                        {myButtons}
                    </div>

                </div>

                <table className='table table-striped mt-4' aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>id</th>
                            <th>Name</th>
                        </tr>
                    </thead>
                    <tbody>
                        {units.map(function (unit) {
                            const currentNavLink = unit.isDisabled === true
                                ? <del><NavLink className='text-muted' to={`/${apiName}/${App.viewNameMethod}/${unit.id}`} title='кликните для редактирования'>{unit.name} ({unit.information})</NavLink></del>
                                : <NavLink to={`/${apiName}/${App.viewNameMethod}/${unit.id}`} title='кликните для редактирования'>{unit.name} <mark>({unit.information})</mark></NavLink>

                            return <tr key={unit.id}>
                                <td>{unit.id}</td>
                                <td>
                                    {currentNavLink}
                                    <NavLink to={`/${apiName}/${App.deleteNameMethod}/${unit.id}`} title='удалить объект' className='text-danger ml-3'>del</NavLink>
                                </td>
                            </tr>
                        })}
                    </tbody>
                </table>
                <PaginatorComponent servicePaginator={this.servicePaginator} />
            </>
        );
    }

    cardHeaderPanel() {
        return <NavLink className='btn btn-outline-primary btn-sm' to={`/groupsgoods/${App.listNameMethod}/`} role='button' title='Перейти в справочник номенклатуры'>Номенклатура</NavLink>;
    }
}
