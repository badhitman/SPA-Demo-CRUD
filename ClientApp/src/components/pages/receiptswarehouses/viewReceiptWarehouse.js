////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { NavLink } from 'react-router-dom'
import { aPageCard } from '../aPageCard';
import App from '../../../App';

/** Просмотр документа поступления на склад */
export class viewReceiptWarehouse extends aPageCard {
    static displayName = viewReceiptWarehouse.name;
    modelName = 'ReceiptToWarehouseDocumentModel';

    /** навигационная а панель */
    static navTabs() {
        return <ul className="nav nav-tabs">
            <li className="nav-item">
                <NavLink to={`/warehouses/${App.listNameMethod}`} className='nav-link' title='Перейти в справочник складов'>Склады</NavLink>
            </li>
            <li className="nav-item">
                <NavLink to={`/receiptswarehousesdocuments/${App.listNameMethod}`} className='nav-link active'>Поступления</NavLink>
            </li>
            <li className="nav-item">
                <NavLink to={`/displacementsdocuments/${App.listNameMethod}`} className='nav-link' title='Перейти в журнал документов внутреннего перемещения номенклатуры'>Перемещения</NavLink>
            </li>
        </ul>
    }

    constructor(props) {
        super(props);

        this.state.quantity = '0';
        this.state.unitId = 0;
        this.state.goodId = 0;

        /** событие изменения номенклатуры (вновь-добавляемой строки) */
        this.handleGoodChange = this.handleGoodChange.bind(this);
        /** событие изменения единицы измерения (вновь-добавляемой строки) */
        this.handleUnitChange = this.handleUnitChange.bind(this);
        /** событие изменения количества номенклатуры (вновь-добавляемой строки) */
        this.handleQuantityChange = this.handleQuantityChange.bind(this);

        /** событие клика кнопки "добавить" (строку в документ) */
        this.handleAddRowDocument = this.handleAddRowDocument.bind(this);
        /** событие клика кнопки "удалить" (строку документа) */
        this.handleDeleteRowDocument = this.handleDeleteRowDocument.bind(this);
    }

    async ajax() {
        var response = await fetch(`${this.apiPrefix}/receiptswarehousesdocuments/${App.id}`);
        if (response.redirected === true) {
            window.location.href = response.url;
            return;
        }

        try {
            if (response.ok === true) {
                var result = await response.json();
                if (result.success === false) {
                    this.clientAlert(result.info, result.status, 1000, 10000);
                }
                else {
                    App.data = result.tag;
                    response = await fetch(`${this.apiPrefix}/warehouses?pageSize=999`);
                    if (response.ok === true) {
                        result = await response.json();
                        App.data.warehouses = result.tag;
                    }
                    else {
                        App.data = undefined;
                        console.error(`Ошибка обработки запроса. CODE[${response.status}] Status[${response.statusText}]`);
                        this.clientAlert(`Ошибка обработки запроса. CODE[${response.status}] Status[${response.statusText}]`, 'danger', 1000, 10000);
                    }
                }
            }
            else {
                App.data = undefined;
                console.error(`Ошибка обработки запроса. CODE[${response.status}] Status[${response.statusText}]`);
                this.clientAlert(`Ошибка обработки запроса. CODE[${response.status}] Status[${response.statusText}]`, 'danger', 1000, 10000);
            }
        }
        catch (err) {
            console.error(err);
            this.clientAlert(err, 'danger');
        }
    }

    async load() {
        await this.ajax();
        this.cardTitle = `#${App.data.id}-поступление на склад`;

        var goodId = localStorage.getItem(`${this.displayName}/${App.method}/goodNewRow`);
        goodId = (goodId && !isNaN(goodId)) ? parseInt(goodId) : 0;

        var unitId = localStorage.getItem(`${this.displayName}/${App.method}/unitNewRow`);
        unitId = (unitId && !isNaN(unitId)) ? parseInt(unitId) : 0;

        var quantity = localStorage.getItem(`${this.displayName}/${App.method}/quantityNewRow`);
        if (!(/^[\d,.]+$/.test(quantity)) || (quantity.match(/[^\d]/g) || []).length > 1) {
            quantity = `${this.state.quantity}`;
        }
        else {
            quantity = quantity.replace(',', '.');
        }

        this.setState(
            {
                loading: false,
                goodId,
                unitId,
                quantity
            });
    }

    /** обработчик изменения номенклатуры (вновь-добавляемой строки) */
    handleGoodChange(e) {
        const target = e.target;
        const goodId = parseInt(target.value, 10);
        localStorage.setItem(`${this.displayName}/${App.method}/goodNewRow`, goodId);
        this.setState({
            goodId: goodId
        });
    }

    /** обработчик изменения единицы измерения (вновь-добавляемой строки) */
    handleUnitChange(e) {
        const target = e.target;
        const unitId = parseInt(target.value, 10);
        localStorage.setItem(`${this.displayName}/${App.method}/unitNewRow`, unitId);
        this.setState({
            unitId: unitId
        });
    }

    /** обработчик изменения количества номенклатуры (вновь-добавляемой строки) */
    handleQuantityChange(e) {
        const target = e.target;
        var quantity = target.value;
        if (!(/^-?[\d,.]+$/.test(quantity)) || (quantity.match(/[,.]/g) || []).length > 1) {
            quantity = `${this.state.quantity}`;
        }
        else {
            quantity = quantity.replace(',', '.');
        }
        localStorage.setItem(`${this.displayName}/${App.method}/quantityNewRow`, quantity);
        this.setState({
            quantity: quantity
        });
    }

    /** обработчик добавления строки в документ */
    async handleAddRowDocument() {
        var Quantity = this.state.quantity;
        if (/[,.]/.test(Quantity)) {
            Quantity = `0${Quantity}0`;
        }
        Quantity = parseFloat(parseFloat(Quantity.replace(',', '.')).toFixed(2));

        const sendedFormData = { GoodId: this.state.goodId, UnitId: this.state.unitId, Quantity };
        const response = await fetch(`${this.apiPrefix}/receiptswarehousesdocuments/${App.id}`, {
            method: 'PATCH',
            body: JSON.stringify(sendedFormData),
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            }
        });

        if (response.redirected === true) {
            window.location.href = response.url;
            return;
        }

        try {
            if (response.ok === true) {
                const result = await response.json();

                if (result.success === false) {
                    this.clientAlert(result.info, result.status, 1000, 10000);
                }
                else if (result.success === true) {
                    localStorage.setItem(`${this.displayName}/${App.method}/quantityNewRow`, '0');
                    await this.load();
                }
            }
            else {
                console.error(`Ошибка обработки запроса. CODE[${response.status}] Status[${response.statusText}]`);
                this.clientAlert(`Ошибка обработки запроса. CODE[${response.status}] Status[${response.statusText}]`, 'danger', 1000, 10000);
            }
        }
        catch (err) {
            console.error(err);
            this.clientAlert(err, 'danger');
        }
    }

    /** обработчик удаления строки документа */
    async handleDeleteRowDocument(e) {
        const target = e.target;
        const row = parseInt(target.attributes.row.value, 10);


        //const sendedFormData = { row };
        const response = await fetch(`${this.apiPrefix}/receiptswarehousesdocuments/${App.id}`, {
            method: 'OPTIONS',
            body: JSON.stringify(row),
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            }
        });

        if (response.redirected === true) {
            window.location.href = response.url;
            return;
        }

        try {
            if (response.ok === true) {
                const result = await response.json();

                if (result.success === false) {
                    this.clientAlert(result.info, result.status, 1000, 10000);
                }
                else if (result.success === true) {
                    await this.load();
                }
            }
            else {
                console.error(`Ошибка обработки запроса. CODE[${response.status}] Status[${response.statusText}]`);
                this.clientAlert(`Ошибка обработки запроса. CODE[${response.status}] Status[${response.statusText}]`, 'danger', 1000, 10000);
            }
        }
        catch (err) {
            console.error(err);
            this.clientAlert(err, 'danger');
        }
    }

    cardHeaderPanel() {
        return <></>;
    }

    cardBody() {
        const stylePointerCursor = { cursor: 'pointer' };
        const data = App.data;
        const warehouses = data.warehouses;
        var rows = data.rows;

        if (Array.isArray(rows) && rows.length > 0) {
            rows = rows.map(function (row) {
                return <tr key={row.id}>
                    <td><NavLink to={`/goods/${App.viewNameMethod}/${row.good.id}`} title={row.good.information}>{row.good.name}</NavLink></td>
                    <td title={row.unit.information}>{row.unit.name}</td>
                    <td>{row.quantity}</td>
                    <td><span onClick={this.handleDeleteRowDocument} row={row.id} style={stylePointerCursor} className="badge badge-danger" title='удалить строку'>del</span></td>
                </tr>
            }, this);
        }
        else {
            rows = <tr><td colSpan={4}><div className='d-flex justify-content-center' title='табличная часть данных документа не заполнена'>~ таблица пуста ~</div></td></tr>;
        }

        const buttonAddRowDisabled = this.state.quantity === '0' || this.state.quantity === '' || isNaN(this.state.quantity);

        return (
            <>
                {viewReceiptWarehouse.navTabs()}
                <form className='mt-3'>
                    <div className="form-group">
                        <label htmlFor="warehouseId">Склад поступления</label>
                        <select name='warehouseId' id='warehouseId' defaultValue={App.data.warehouseId} className="custom-select">
                            {warehouses.map(function (element) {
                                return <option key={element.id} title={element.information} value={element.id}>{element.name}</option>
                            })}
                        </select>
                    </div>
                    {this.getInformation()}
                    {this.viewButtons()}
                </form>
                <br />
                <div className='card'>
                    <div className='card-body'>
                        <div className="input-group">
                            <select value={this.state.goodId} onChange={this.handleGoodChange} className="custom-select" aria-label="Выбор номенклатуры">
                                {App.data.groupsGoods.map(function (element) {
                                    return <optgroup key={element.id} label={element.name}>
                                        {element.goods.map(function (goodElement) {
                                            return <option key={goodElement.id} value={goodElement.id}>
                                                {goodElement.name}
                                            </option>
                                        })}
                                    </optgroup>
                                })}
                            </select>
                            <select value={this.state.unitId} onChange={this.handleUnitChange} className="custom-select" aria-label="Выбор еденицы измерения">
                                {App.data.units.map(function (element) { return <option key={element.id} value={element.id} title={element.information}>{element.name}</option> })}
                            </select>
                            <input value={this.state.quantity} onChange={this.handleQuantityChange} type='text' placeholder='Кол-во' title='Количество номенклатуры' />
                            <div className="input-group-append">
                                <button onClick={this.handleAddRowDocument} disabled={buttonAddRowDisabled} className="btn btn-outline-secondary" type="button">Добавить</button>
                            </div>
                        </div>

                        <table className='table table-striped mt-4'>
                            <thead>
                                <tr>
                                    <th>Good</th>
                                    <th>Unit</th>
                                    <th>Quantity</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                {rows}
                            </tbody>
                        </table>
                    </div>
                </div>
            </>
        );
    }
}
