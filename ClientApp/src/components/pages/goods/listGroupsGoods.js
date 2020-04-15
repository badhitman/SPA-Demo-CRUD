////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageList } from '../aPageList';
import { NavLink } from 'react-router-dom'
import App from '../../../App';
import { PaginatorComponent } from '../../PaginatorComponent';

/** Компонент для отображения справочника групп номенклатуры */
export class listGroupsGoods extends aPageList {
    static displayName = listGroupsGoods.name;
    cardTitle = 'Группы номенклатуры';

    constructor(props) {
        super(props);

        this.state.newName = '';
        this.state.buttonsDisabled = true;
        /** изменение значения поля нового имени */
        this.handleNewNameChange = this.handleNewNameChange.bind(this);
        /** сброс введёного значения поля: имя */
        this.handleResetClick = this.handleResetClick.bind(this);
        /** добавление в бд новой группы номенклатуры */
        this.handleCreateClick = this.handleCreateClick.bind(this);
    }

    /**
     * событие изменения имени "создаваемой" номенклатурной группы
     * @param {object} e - object sender
     */
    handleNewNameChange(e) {
        const target = e.target;
        var buttonsDisabled = true;
        if (target.value) {
            if (target.value.length > 0) {
                buttonsDisabled = false;
            }
        }

        this.setState({
            newName: target.value,
            buttonsDisabled: buttonsDisabled
        });
    }

    /** Сброс формы создания новой группы номенклатуры */
    handleResetClick() {
        this.setState({
            newName: '',
            buttonsDisabled: true
        });
    }

    /** Создание номенклатурной группы */
    async handleCreateClick() {
        var sendedFormData = { name: this.state.newName };
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
                    await this.ajax();
                    App.data = null;
                    this.setState({ loading: false, newName: '', buttonsDisabled: true});
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
        var goods = App.data;
        const myButtons = this.state.buttonsDisabled === true
            ? <><button disabled className="btn btn-outline-secondary" type="button">Создать</button>
                <button disabled className="btn btn-outline-secondary" type="reset">Сброс</button></>
            : <><button onClick={this.handleCreateClick} className="btn btn-outline-success" type="button">Создать</button>
                <button onClick={this.handleResetClick} className="btn btn-outline-primary" type="reset">Сброс</button></>
        return (
            <>
                <label htmlFor="basic-url">Создание новой группы номенклатуры</label>
                <div title='Для создания новой группы введите её название и нажмите кнопку - Создать' className="input-group mb-3">
                    <input onChange={this.handleNewNameChange} value={this.state.newName} type="text" className="form-control" placeholder="Введите название новой группы" aria-label="Введите название новой группы" aria-describedby="button-addon4" />
                    <div className="input-group-append" id="button-addon4">
                        {myButtons}
                    </div>
                </div>

                <table className='table table-striped mt-4'>
                    <thead>
                        <tr>
                            <th>id</th>
                            <th>Name</th>
                            <th>About</th>
                        </tr>
                    </thead>
                    <tbody>
                        {goods.map(function (good) {
                            const currentNavLink = good.isDisabled === true
                                ? <del><NavLink className='text-muted' to={`/${App.controller}/${App.viewNameMethod}/${good.id}`} title='кликните для редактирования'>{good.name}</NavLink></del>
                                : <NavLink to={`/${App.controller}/${App.viewNameMethod}/${good.id}`} title='кликните для редактирования'>{good.name}</NavLink>

                            return <tr key={good.id}>
                                <td>{good.id} <span className="badge badge-secondary">группа</span></td>
                                <td>
                                    {currentNavLink}
                                    <NavLink to={`/${App.controller}/${App.deleteNameMethod}/${good.id}`} title='удалить объект' className='text-danger ml-3'>del</NavLink>
                                </td>
                                <td>x{good.count}</td>
                            </tr>
                        })}
                    </tbody>
                </table>
                <PaginatorComponent servicePaginator={this.servicePaginator} />
            </>
        );
    }

    cardHeaderPanel() {
        return <NavLink className='btn btn-outline-info btn-sm' to={`/unitsgoods/${App.listNameMethod}/`} role='button' title='Перейти в справочник единиц измерения'>Единицы измерения</NavLink>;
    }
}
