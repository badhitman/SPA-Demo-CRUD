////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import App from '../../../App';
import { aWarehouseDocumentViewing } from '../aWarehouseDocumentViewing';

/** Отображение существующего документа внутреннего перемещения */
export class viewWarehousesDisplacement extends aWarehouseDocumentViewing {
    static displayName = viewWarehousesDisplacement.name;
    userFriendlyNameDocument = 'Внутреннее перемещение';

    constructor(props) {
        super(props);

        /** событие изменения склада списания */
        this.handleDebitingWarehouseChange = this.handleDebitingWarehouseChange.bind(this);
    }

    /** обработчик изменения склада списания */
    handleDebitingWarehouseChange(e) {
        const target = e.target;
        const id = parseInt(target.value, 10);
        if (id > 0 && App.data.warehouseReceipt.id > 0 && id === App.data.warehouseReceipt.id) {
            this.clientAlert('Склад отгрузки не может совпадать со складом поступления', 'danger');
        }
        else {
            App.data.warehouseDebiting = { id };
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
                <select name='warehouseDebitingId' id='warehouseDebitingId' defaultValue={warehouseDebitingId} onChange={this.handleDebitingWarehouseChange} aria-describedby="debitingWarehouseHelp" className="custom-select">
                    {warehouseDebitingId > 0 ? <></> : <option disabled value={0}>склад списания</option>}
                    {warehouses.map(function (element) {
                        return <option key={element.id} title={element.information} value={element.id}>{element.name}</option>
                    })}
                </select>
                <small id="debitingWarehouseHelp" className="form-text text-muted">укажите склад списания</small>
            </div>
            <div className="form-group col-md-6">
                <label htmlFor="warehouseReceiptId">Склад поступления</label>
                <select name='warehouseReceiptId' id='warehouseReceiptId' defaultValue={data.warehouseReceipt.id} onChange={this.handleReceiptWarehouseChange} aria-describedby="receiptWarehouseHelp" className="custom-select">
                    {data.warehouseReceipt.id > 0 ? <></> : <option disabled value={0}>склад поступления</option>}
                    {warehouses.map(function (element) {
                        return <option key={element.id} title={element.information} value={element.id}>{element.name}</option>
                    })}
                </select>
                <small id="receiptWarehouseHelp" className="form-text text-muted">укажите склад назначения</small>
            </div>
        </div>;
    }
}