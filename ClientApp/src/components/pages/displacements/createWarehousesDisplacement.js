////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aWarehouseDocumentCreating } from '../aWarehouseDocumentCreating';
//import { viewWarehousesDisplacement } from './viewWarehousesDisplacement';
import App from '../../../App';

/** Создание нового документа внутреннего перемещения */
export class createWarehousesDisplacement extends aWarehouseDocumentCreating {
    static displayName = createWarehousesDisplacement.name;
    modelName = 'InternalDisplacementWarehouseDocumentModel';
    userFriendlyNameDocument = 'Внутреннее перемещение';

    constructor(props) {
        super(props);

        /** событие изменения склада списания */
        this.handleDebitingWarehouseChange = this.handleDebitingWarehouseChange.bind(this);
    }

    async ajax() {
        await super.ajax();
        var warehouseDebitingId = localStorage.getItem('warehouseDebitingId');
        if (!warehouseDebitingId || isNaN(warehouseDebitingId)) {
            warehouseDebitingId = 0;
        }
        else {
            warehouseDebitingId = parseInt(warehouseDebitingId, 10);
        }
        App.data.warehouseDebiting = { id: warehouseDebitingId };
    }

    async handleCreateDocument() {
        super.handleCreateDocument();
        if (localStorage.getItem('warehouseDebitingId')) {
            localStorage.removeItem('warehouseDebitingId');
        }
    }

    /** обработчик изменения склада списания */
    handleDebitingWarehouseChange(e) {
        const target = e.target;
        const data = App.data;
        const id = parseInt(target.value, 10);
        if (id > 0 && data.warehouseReceipt.id > 0 && id === data.warehouseReceipt.id) {
            //App.data.warehouseDebiting = { id: 0 };
            this.clientAlert('Склад отгрузки не может совпадать со складом поступления', 'danger');
        }
        else {
            App.data.warehouseDebiting = { id };
            this.saveDocumentState();
        }
        this.forceUpdate();
    }

    get getTopBodyForm() {
        const data = App.data;
        const warehouses = data.warehouses;
        const warehouseDebitingId = data.warehouseDebiting
            ? data.warehouseDebiting.id
            : 0;

        return <div className="form-row">
            <div className="form-group col-md-6">
                <label htmlFor="warehouseDebitingId">Склад списания</label>
                <select name='warehouseDebitingId' id='warehouseDebitingId' value={warehouseDebitingId} onChange={this.handleDebitingWarehouseChange} aria-describedby="debitingWarehouseHelp" className="custom-select">
                    {warehouseDebitingId > 0 ? <></> : <option disabled value={0}>склад списания</option>}
                    {warehouses.map(function (element) {
                        return <option key={element.id} title={element.information} value={element.id}>{element.name}</option>
                    })}
                </select>
                <small id="debitingWarehouseHelp" className={`form-text text-${warehouseDebitingId === 0 ? 'danger' : 'muted'}`}>укажите склад списания</small>
            </div>
            <div className="form-group col-md-6">
                <label htmlFor="warehouseReceiptId">Склад поступления</label>
                <select name='warehouseReceiptId' id='warehouseReceiptId' value={data.warehouseReceipt.id} onChange={this.handleReceiptWarehouseChange} aria-describedby="receiptWarehouseHelp" className="custom-select">
                    {data.warehouseReceipt.id > 0 ? <></> : <option disabled value={0}>склад поступления</option>}
                    {warehouses.map(function (element) {
                        return <option key={element.id} title={element.information} value={element.id}>{element.name}</option>
                    })}
                </select>
                <small id="receiptWarehouseHelp" className={`form-text text-${data.warehouseReceipt.id > 0 ? 'muted' : 'danger'}`}>укажите склад назначения</small>
            </div>
        </div>;
    }
}
