////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { NavLink } from 'react-router-dom'
import { aPageList } from '../aPageList';
import App from '../../../App';
import { PaginatorComponent } from '../../PaginatorComponent';

/** Отображение существующего склада */
export class viewWarehouse extends aPageList {
    static displayName = viewWarehouse.name;

    constructor(props) {
        super(props);

        this.state.name = '';

        /** изменение значения поля имени склада */
        this.handleNameChange = this.handleNameChange.bind(this);
        /** сброс введёного значения поля имени склада*/
        this.handleResetClick = this.handleResetClick.bind(this);
    }

    /**
     * событие изменения имени склада
     * @param {object} e - object sender
     */
    handleNameChange(e) {
        const target = e.target;

        this.setState({
            name: target.value
        });
    }

    /** сброс формы редактирования склада */
    handleResetClick() {
        this.setState({
            name: App.data.name
        });
    }

    async ajax() {
        this.apiPostfix = `/${App.id}`;
        await super.ajax();
    }

    async load() {
        await this.ajax();
        this.cardTitle = `Склад: [#${App.data.id}] ${App.data.name}`;
        this.setState({ loading: false, name: App.data.name });
    }

    cardBody() {
        const resetButtonDisabled = App.data.name === this.state.name.trim();
        const saveButtonDisabled = resetButtonDisabled === true || this.state.name.length === 0;
        const deleteButtonDisabled = this.servicePaginator.rowsCount > 0 || App.data.noDelete === true;

        const saveButton = saveButtonDisabled === true
            ? <button disabled title='Вы можете ввести новое имя группе' className="btn btn-outline-secondary" type="button">Ξ</button>
            : <button title='Сохранить новое имя группы в БД' onClick={this.handleSaveClick} className="btn btn-success" type="button">Сохранить</button>;

        const resetButton = resetButtonDisabled === true
            ? <></>
            : <button title='Сброс имени на оригинальное значание из БД' onClick={this.handleResetClick} className="btn btn-outline-primary" type="reset">Отмена</button>;

        const deleteButton = deleteButtonDisabled === true
            ? <button disabled title='На объект существуют ссылки' className="btn btn-outline-secondary btn-block mb-3" type="button">Удаление невозможно</button>
            : <NavLink className='btn btn-outline-danger btn-block mb-3' to={`/${App.controller}/${App.deleteNameMethod}/${App.id}`} role='button' title='Диалог удаления объекта'>Удаление</NavLink>;

        var myButtons = <>{saveButton}{resetButton}</>;

        return (
            <>
                <label htmlFor="basic-url">Наименование склада</label>
                <div className="input-group mb-3">
                    <input onChange={this.handleNameChange} value={this.state.name} type="text" className="form-control" placeholder="Введите наименование склада" aria-label="Введите наименование склада" />
                    <div className="input-group-append">
                        {myButtons}
                    </div>
                </div>

                <NavLink to={`/${App.controller}/${App.listNameMethod}/`} className="btn btn-outline-primary btn-block mt-3" role="button" title='Перейти в справочник складов'>Вернуться</NavLink>
                {deleteButton}

                <table className='table table-striped mt-4'>
                    <thead>
                        <tr>
                            <th>id</th>
                            <th>Name</th>
                            <th>About</th>
                        </tr>
                    </thead>
                    <tbody>
                        {App.data.documents.map(function (warehouseDocument) {
                            var apiDocumentContriller = '#';
                            switch (warehouseDocument.discriminator) {
                                case 'ReceiptToWarehouseDocumentModel':
                                    apiDocumentContriller = 'receiptswarehousesdocuments';
                                    break;
                                case 'InternalDisplacementWarehouseDocumentModel':
                                    apiDocumentContriller = 'displacementsdocuments';
                                    break;
                                default:
                                    apiDocumentContriller = '#ошибка#';
                                    break;
                            }

                            const currentNavLink = warehouseDocument.isDisabled === true
                                ? <del><NavLink className='text-muted' to={`/${apiDocumentContriller}/${App.viewNameMethod}/${warehouseDocument.id}`} title='кликните для редактирования'>{warehouseDocument.name}</NavLink></del>
                                : <NavLink to={`/${apiDocumentContriller}/${App.viewNameMethod}/${warehouseDocument.id}`} title='кликните для редактирования'>{warehouseDocument.name}</NavLink>

                            return <tr key={warehouseDocument.id}>
                                <td>{warehouseDocument.id}</td>
                                <td>
                                    {currentNavLink}
                                    <NavLink to={`/${apiDocumentContriller}/${App.deleteNameMethod}/${warehouseDocument.id}`} title='удалить объект' className='text-danger ml-3'>del</NavLink>
                                </td>
                                <td>x{warehouseDocument.countRows}</td>
                            </tr>
                        })}
                    </tbody>
                </table>
                <PaginatorComponent servicePaginator={this.servicePaginator} />
            </>
        );
    }
}