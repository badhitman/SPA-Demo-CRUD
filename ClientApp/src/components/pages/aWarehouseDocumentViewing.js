////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { NavLink } from 'react-router-dom';
import { aPageCard } from './aPageCard';
import App from '../../App';

/** базовый (типа абстрактный) компонент для просмотра/редактирования складского документа */
export class aWarehouseDocumentViewing extends aPageCard {
    static displayName = aWarehouseDocumentViewing.name;
    /** название документа для отображения пользователю. Например: 'Поступление на склад' */
    userFriendlyNameDocument = '';

    /** api адрес для управления строками складского документа */
    get apiAddressDocumentRowManage() {
        return `${this.apiPrefix}/${App.controller}/${App.id}`;
    }

    constructor(props) {
        super(props);

        /** событие изменения склада поступления */
        this.handleReceiptWarehouseChange = this.handleReceiptWarehouseChange.bind(this);

        /** событие клика кнопки "добавить" (строку в документ) */
        this.handleAddRowDocument = this.handleAddRowDocument.bind(this);
        /** событие клика кнопки "удалить" (строку документа) */
        this.handleDeleteRowDocument = this.handleDeleteRowDocument.bind(this);
        /** событие изменения количества номенклатуры */
        this.handleQuantityChange = this.handleQuantityChange.bind(this);
        /** событие изменения единицы измерения номенклатуры */
        this.handleUnitChange = this.handleUnitChange.bind(this);
        /** событие изменения номенклатуры */
        this.handleGoodChange = this.handleGoodChange.bind(this);
    }

    /** чтение состояния "формы добавления новой строки складского документа" */
    readStateFormAddingNewRow() {
        const goodId = localStorage.getItem(`${this.constructor.name}/${App.method}/goodNewRow`);
        App.data.goodId = (goodId && !isNaN(goodId)) ? parseInt(goodId) : 0;

        const unitId = localStorage.getItem(`${this.constructor.name}/${App.method}/unitNewRow`);
        App.data.unitId = (unitId && !isNaN(unitId)) ? parseInt(unitId) : 0;

        const quantity = localStorage.getItem(`${this.constructor.name}/${App.method}/quantityNewRow`);
        if (!(/^[\d,.]+$/.test(quantity)) || (quantity.match(/[^\d]/g) || []).length > 1) {
            App.data.quantity = 0;
        }
        else {
            App.data.quantity = quantity.replace(',', '.');
        }
    }

    /** обработчик изменения склада поступления */
    handleReceiptWarehouseChange(e) {
        const target = e.target;
        const data = App.data;
        const id = parseInt(target.value, 10);

        if (id > 0 && data.warehouseDebiting) {
            if (data.warehouseDebiting.id > 0 && id === data.warehouseDebiting.id) {
                this.clientAlert('Склад отгрузки не может совпадать со складом поступления', 'danger');
            }
            else {
                App.data.warehouseReceipt = { id };
            }
        }
        else {
            App.data.warehouseReceipt = { id };
            //this.saveDocumentState();
        }
        this.forceUpdate();
    }

    /** обработчик изменения примечания к документу (поступления номенклатуры) */
    handleInformationChange(e) {
        const target = e.target;
        const information = target.value;
        App.data.information = information;
        //this.forceUpdate();
    }

    /** обработчик изменения номенклатуры (вновь-добавляемой строки) */
    handleGoodChange(e) {
        const target = e.target;
        const goodId = parseInt(target.value, 10);
        App.data.goodId = goodId;
        this.forceUpdate();
    }

    /** обработчик изменения единицы измерения (вновь-добавляемой строки) */
    handleUnitChange(e) {
        const target = e.target;
        const unitId = parseInt(target.value, 10);
        App.data.unitId = unitId;
        this.forceUpdate();
    }

    /** обработчик изменения количества номенклатуры (вновь-добавляемой строки) */
    handleQuantityChange(e) {
        const target = e.target;
        var quantity = target.value;
        if (!(/^-?[\d,.]+$/.test(quantity)) || (quantity.match(/[,.]/g) || []).length > 1) {
            quantity = `${App.data.quantity}`;
        }
        else {
            quantity = quantity.replace(',', '.');
        }
        App.data.quantity = quantity;
        this.forceUpdate();
    }

    /** обработчик добавления строки в документ */
    async handleAddRowDocument() {
        var Quantity = App.data.quantity;
        if (/[,.]/.test(Quantity)) {
            Quantity = `0${Quantity}0`;
        }
        Quantity = parseFloat(parseFloat(Quantity.replace(',', '.')).toFixed(2));
        const BodyDocumentId = isNaN(App.id) ? 0 : parseInt(App.id, 10);
        //
        const sendedFormData =
        {
            BodyDocumentId,
            GoodId: App.data.goodId,
            UnitId: App.data.unitId,
            Quantity
        };
        //
        await this.sendDataRowDocument(sendedFormData);
    }

    /** обработчик удаления строки документа */
    async handleDeleteRowDocument(e) {
        const target = e.target;

        const id = parseInt(target.attributes.row.value, 10);
        const BodyDocumentId = isNaN(App.id) ? 0 : parseInt(App.id, 10);
        //
        const sendedFormData = { id, BodyDocumentId };
        //
        await this.sendDataRowDocument(sendedFormData);
    }

    /**
     * Отправка на сервер команды управления строкой складского документа
     * @param {object} sendedFormData - отправляемые данные. Если указан id => команда удаления строки документа, в противном случае команда воспринимается серврером как добавление новой строки
     */
    async sendDataRowDocument(sendedFormData) {
        const response = await fetch(this.apiAddressDocumentRowManage, {
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
                    App.data.quantity = 0;
                    await this.load();
                }
            }
            else {
                const msg = `Ошибка обработки команды [${(sendedFormData.id > 0 ? 'удаления' : 'добавления')} строки складского документа: ${this.userFriendlyNameDocument}] . CODE[${response.status}] Status[${response.statusText}]`;
                console.error(msg);
                this.clientAlert(msg, 'danger', 1000, 10000);
            }
        }
        catch (err) {
            console.error(err);
            this.clientAlert(err, 'danger');
        }
    }

    /** обработчик потери фокуса поля примечания к документу (поступления номенклатуры) */
    async handleInformationBlur(e) {
        // переопределить в потомке (создание нового документа)
    }

    async ajax() {
        var response = await fetch(this.apiAddressDocumentRowManage);
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
                        const msg = `Ошибка обработки запроса ajax загрузки складского документа: ${this.userFriendlyNameDocument}. CODE[${response.status}] Status[${response.statusText}]`;
                        console.error(msg);
                        this.clientAlert(msg, 'danger', 1000, 10000);
                    }
                }
            }
            else {
                App.data = undefined;
                const msg = `Ошибка обработки запроса ajax загрузки складского документа: ${this.userFriendlyNameDocument}. CODE[${response.status}] Status[${response.statusText}]`;
                console.error(msg);
                this.clientAlert(msg, 'danger', 1000, 10000);
            }
        }
        catch (err) {
            console.error(err);
            this.clientAlert(err, 'danger');
        }
    }

    async load() {
        await this.ajax();
        this.cardTitle = `${this.userFriendlyNameDocument} №${App.id}`;
        this.readStateFormAddingNewRow();
        this.apiPostfix = `/${App.id}`;
        this.setState({ loading: false });
    }

    cardHeaderPanel() {
        return <></>;
    }

    /** Получить основную часть формы настройки реквизитов складского документа */
    get getTopBodyForm() {
        const data = App.data;
        const warehouses = data.warehouses;
        return <div className="form-group">
            <label htmlFor="warehouseReceiptId">Склад поступления</label>
            <select name='warehouseReceiptId' id='warehouseId' value={data.warehouseReceipt.id} onChange={this.handleReceiptWarehouseChange} aria-describedby="receiptWarehouseHelp" className="custom-select">
                {data.warehouseReceipt.id > 0
                    ? <></>
                    : <option disabled value={0}>укажите склад</option>}
                {warehouses.map(function (element) {
                    return <option key={element.id} title={element.information} value={element.id}>{element.name}</option>
                })}
            </select>
            <small id="receiptWarehouseHelp" className="form-text text-muted">Для создания нового поступления - укажите склад назначения</small>
        </div>;
    }

    cardBody() {
        const data = App.data;

        const buttonAddRowDisabled =
            data.quantity === 0 || data.quantity === '0' || data.quantity === '' || isNaN(data.quantity) ||
            data.warehouseReceiptId < 1 || data.warehouseReceiptId === '0' || data.warehouseReceiptId === '' ||
            data.goodId < 1 || data.goodId === '0' || data.goodId === '' ||
            data.unitId < 1 || data.unitId === '0' || data.unitId === '';

        var rows = data.rows;
        if (Array.isArray(rows) && rows.length > 0) {
            rows = rows.map(function (row) {
                return <tr key={row.id}>
                    <td>
                        <NavLink to={`/goods/${App.viewNameMethod}/${row.good.id}`} title={row.good.information}>{row.good.name}</NavLink>
                        <span onClick={this.handleDeleteRowDocument} row={row.id} style={{ cursor: 'pointer' }} className="badge badge-danger ml-2" title='удалить строку'>del</span>
                    </td>
                    <td>{row.unit.name}</td>
                    <td>{row.quantity}</td>
                </tr>
            }, this);
        }
        else {
            rows = <tr><td colSpan={4}><div className='d-flex justify-content-center' title='табличная часть данных документа не заполнена'>~ таблица пуста ~</div></td></tr>;
        }
        //this.apiPostfix = App.id;
        return (
            <>
                <form>
                    {data.id > 0
                        ? <>
                            <input type='hidden' name='id' value={data.id} />
                            <input type='hidden' name='name' value='~' />
                        </>
                        : <></>}
                    {this.getTopBodyForm}
                    <div className="form-group">
                        <label htmlFor="information">Комментарий</label>
                        <textarea defaultValue={data.information} onChange={this.handleInformationChange} onBlur={this.handleInformationBlur} className="form-control" id="information" name="information" rows="3"></textarea>
                    </div>
                    {this.viewButtons()}
                    <br />
                </form>
                <div className='card'>
                    <div className='card-body'>
                        <div className="input-group">
                            <select defaultValue={data.goodId} onChange={this.handleGoodChange} className="custom-select" aria-label="Выбор номенклатуры">
                                {
                                    data.goodId > 0
                                        ? <></>
                                        : <option key='0' disabled value={0} title='укажите номенклатуру'>товар</option>
                                }
                                {data.groupsGoods.map(function (element) {
                                    return <optgroup key={element.id} label={element.name}>
                                        {element.goods.map(function (goodElement) {
                                            return <option key={goodElement.id} value={goodElement.id}>
                                                {goodElement.name}
                                            </option>
                                        })}
                                    </optgroup>
                                })}
                            </select>
                            <select defaultValue={data.unitId} onChange={this.handleUnitChange} className="custom-select" aria-label="Выбор еденицы измерения">
                                {
                                    data.unitId > 0
                                        ? <></>
                                        : <option key='0' disabled value={0} title='укажите единицу измерения'>ед.изм.</option>
                                }
                                {App.data.units.map(function (element) { return <option key={element.id} value={element.id} title={element.information}>{element.name}</option> })}
                            </select>
                            <input className='form-control' value={data.quantity} onChange={this.handleQuantityChange} type='text' placeholder='Кол-во' title='Количество номенклатуры' />
                            <div className="input-group-append">{/*buttonAddRowDisabled === true secondary*/}
                                <button onClick={this.handleAddRowDocument} disabled={buttonAddRowDisabled} className={`btn btn-outline-${(buttonAddRowDisabled === true ? 'secondary' : 'primary')}`} type="button">Добавить</button>
                            </div>
                        </div>

                        <table className='table table-striped mt-4'>
                            <thead>
                                <tr>
                                    <th>Good</th>
                                    <th>Unit</th>
                                    <th>x</th>
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