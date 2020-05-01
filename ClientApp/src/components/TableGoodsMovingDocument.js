////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React, { Component } from 'react';
import App from '../App';

/** Табличная часть документа движения номенклатуры */
export class TableGoodsMovingDocument extends Component {
    static displayName = TableGoodsMovingDocument.name;

    rows = [];
    goodsGroups = [];
    units = [];

    constructor(props) {
        super(props);

        this.state =
        {
            loading: true
        };

        //this.state.warehouseId = 0;
        //this.state.information = '';

        //this.state.quantity = '0';
        //this.state.unitId = 0;
        //this.state.goodId = 0;

        ///** событие изменение склада поступления */
        //this.handleWarehouseChange = this.handleWarehouseChange.bind(this);
        ///** событие изменение примечания к документу */
        //this.handleInformationChange = this.handleInformationChange.bind(this);

        ///** событие изменения номенклатуры (вновь-добавляемой строки) */
        //this.handleGoodChange = this.handleGoodChange.bind(this);
        ///** событие изменения единицы измерения (вновь-добавляемой строки) */
        //this.handleUnitChange = this.handleUnitChange.bind(this);
        ///** событие изменения количества номенклатуры (вновь-добавляемой строки) */
        //this.handleQuantityChange = this.handleQuantityChange.bind(this);

        ///** событие клика кнопки "сохранить" (документ) */
        //this.handleSaveDocument = this.handleSaveDocument.bind(this);
        ///** событие клика кнопки "сбросить форму" (документ) */
        //this.handleResetDocument = this.handleResetDocument.bind(this);

        ///** событие клика кнопки "добавить" (строку в документ) */
        //this.handleAddRowDocument = this.handleAddRowDocument.bind(this);
        ///** событие клика кнопки "удалить" (строку документа) */
        //this.handleDeleteRowDocument = this.handleDeleteRowDocument.bind(this);
    }

    componentDidMount() {
        this.load();
    }

    async load() {


        var response = await fetch(`/api/${this.props.apiName}/${App.id}`);
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
                    this.data = result.tag;
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




        //this.cardTitle = 'Создание поступления на склад';

        //var goodId = localStorage.getItem(`${this.displayName}/${App.method}/goodNewRow`);
        //goodId = (goodId && !isNaN(goodId)) ? parseInt(goodId) : 0;

        //var unitId = localStorage.getItem(`${this.displayName}/${App.method}/unitNewRow`);
        //unitId = (unitId && !isNaN(unitId)) ? parseInt(unitId) : 0;

        //var quantity = localStorage.getItem(`${this.displayName}/${App.method}/quantityNewRow`);
        //if (!(/^[\d,.]+$/.test(quantity)) || (quantity.match(/[^\d]/g) || []).length > 1) {
        //    quantity = `${this.state.quantity}`;
        //}
        //else {
        //    quantity = quantity.replace(',', '.');
        //}

        //if (App.data.warehouseId < 1) {
        //    const storageInformation = localStorage.getItem(`${this.displayName}/${App.method}/information`);
        //    this.setState({ loading: false, information: storageInformation, warehouseId: App.data.warehouseId, goodId, unitId, quantity });
        //}
        //else {
        //    this.setState({ loading: false, information: App.data.information, warehouseId: App.data.warehouseId, goodId, unitId, quantity });
        //}
        this.setState({ loading: false });
    }

    render() {
        if (this.state.loading === true) {
            return <>загрузка данных...</>;
        }

        return <>table document</>;
    }
}
TableGoodsMovingDocument.defaultProps = {
    /** имя api контроллера */
    apiName: 'WarehouseDocuments',

    /** отображать ли форму добавления новой строки в документ */
    showForm: true,

    /** признак дотупности (вкл/выкл) формы добавления новой строки (равно как и кнопки удаления строки) */
    formDisabled: false,

    /** строка/примечание к таблице */
    captionTableText: null
};