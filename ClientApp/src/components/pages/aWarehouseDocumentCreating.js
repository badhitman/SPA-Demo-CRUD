////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { NavLink } from 'react-router-dom';
import { aWarehouseDocumentViewing } from './aWarehouseDocumentViewing';
import App from '../../App';

/** базовый (типа абстрактный) компонент для создания нового складского документа */
export class aWarehouseDocumentCreating extends aWarehouseDocumentViewing {
    static displayName = aWarehouseDocumentCreating.name;
    /** имя хранимой модели временного складского документа (в таблице БД складских документов).
     * Этим именем будет назван объект временного/скрытого документа.
     * Пример: 'ReceiptToWarehouseDocumentModel' */
    modelName = '';

    get apiAddressDocumentRowManage() {
        return `${this.apiPrefix}/WarehouseDocuments/${this.modelName}`;
    }

    constructor(props) {
        super(props);

        /** событие изменения склада поступления */
        this.handleReceiptWarehouseChange = this.handleReceiptWarehouseChange.bind(this);
        /** событие потери фокуса поля ввода примечания */
        this.handleInformationBlur = this.handleInformationBlur.bind(this);
        /** событие изменения номенклатуры для вновь добавляемой строки документа */
        this.handleGoodChange = this.handleGoodChange.bind(this);
        /** событие изменения единицы измерения для вновь добавляемой строки документа */
        this.handleUnitChange = this.handleUnitChange.bind(this);
        /** событие изменения количества для вновь добавляемой строки документа */
        this.handleQuantityChange = this.handleQuantityChange.bind(this);

        /** событие клика кнопки "создать" (документ) */
        this.handleCreateDocument = this.handleCreateDocument.bind(this);
    }

    async handleReceiptWarehouseChange(e) {
        super.handleReceiptWarehouseChange(e);
        await this.saveDocumentState();
    }

    async handleInformationBlur(e) {
        await this.saveDocumentState();
    }

    /**
     * Сохранение состояния временного документа (в БД на сервере и/или в browser storage)
     * @param {string} information - комменатрий/примечание к документу
     * @param {number} warehouseReceiptId - склад поступления
     */
    async saveDocumentState() {
        const data = App.data;
        const warehouseDebitingId = data.warehouseDebiting ? data.warehouseDebiting.id : undefined;
        if (warehouseDebitingId) {
            localStorage.setItem('warehouseDebitingId', warehouseDebitingId);
        }
        else if (localStorage.getItem('warehouseDebitingId')) {
            localStorage.removeItem('warehouseDebitingId');
        }

        const information = data.information;
        const warehouseReceiptId = data.warehouseReceipt.id;

        const localStorageInformationMark = `${this.constructor.name}/${App.method}/information`;
        if (warehouseReceiptId < 1) {
            localStorage.setItem(localStorageInformationMark, information);
            return;
        }
        else if (localStorage.getItem(localStorageInformationMark)) {
            localStorage.removeItem(localStorageInformationMark);
        }
        const sendedFormData =
        {
            warehouseReceiptId,
            information: data.information,
            name: '~'
        };
        const response = await fetch(`${this.apiPrefix}/WarehouseDocuments/${this.modelName}`, {
            method: 'PUT',
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

    /** обработчик изменения номенклатуры (вновь-добавляемой строки) */
    handleGoodChange(e) {
        super.handleGoodChange(e);
        localStorage.setItem(`${this.constructor.name}/${App.method}/goodNewRow`, App.data.goodId);
    }

    /** обработчик изменения единицы измерения (вновь-добавляемой строки) */
    handleUnitChange(e) {
        super.handleUnitChange(e);
        localStorage.setItem(`${this.constructor.name}/${App.method}/unitNewRow`, App.data.unitId);
    }

    /** обработчик изменения количества номенклатуры (вновь-добавляемой строки) */
    handleQuantityChange(e) {
        super.handleQuantityChange(e);
        localStorage.setItem(`${this.constructor.name}/${App.method}/quantityNewRow`, App.data.quantity);
    }

    /** обработчик создания документа */
    async handleCreateDocument() {
        const data = App.data;
        const warehouseDebitingId = data.warehouseDebiting ? data.warehouseDebiting.id : 0;
        const WarehouseReceiptId = data.warehouseReceipt.id;

        const response = await fetch(`${this.apiPrefix}/${App.controller}`, {
            method: 'POST',
            body: JSON.stringify({
                name: '~',
                warehouseDebitingId,
                WarehouseReceiptId,
                information: data.information
            }),
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
                App.data.quantity = 0;
                localStorage.removeItem(`${this.constructor.name}/${App.method}/information`);
                if (result.success === false) {
                    this.clientAlert(result.info, result.status, 1000, 10000);
                }
                else if (result.success === true && !isNaN(result.tag.id)) {
                    this.props.history.push(`/${App.controller}/${App.viewNameMethod}/${result.tag.id}`)
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
        this.cardTitle = `Создание документа: ${this.userFriendlyNameDocument}`;
        this.readStateFormAddingNewRow();

        if (App.data.warehouseReceiptId < 1) {
            var storageInformation = localStorage.getItem(`${this.constructor.name}/${App.method}/information`);
            if (storageInformation === undefined || storageInformation === null) {
                storageInformation = '';
            }
            App.data.information = storageInformation;
        }
        this.apiPostfix = '';
        this.setState({ loading: false });
    }

    viewButtons() {
        const data = App.data;
        const warehouseDebitingId = data.warehouseDebiting ? data.warehouseDebiting.id : -1;
        return <><NavLink className='btn btn-outline-primary btn-block' to={`/${App.controller}/${App.listNameMethod}`} role='button' title='Вернуться в журнал документов поступления номенклатуры'>В журнал</NavLink>
            {data.warehouseReceipt.id > 0 && Array.isArray(data.rows) && data.rows.length > 0 && warehouseDebitingId !== 0
                ? <button onClick={this.handleCreateDocument} type="button" className='btn btn-dark btn-block'>Создать: {this.userFriendlyNameDocument}</button>
                : <button disabled type="button" className='btn btn-outline-secondary btn-block'>Укажите склад и добавьте строки</button>}</>;
    }
}