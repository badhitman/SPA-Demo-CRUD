////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { NavLink } from 'react-router-dom'
import { aPageList } from '../aPageList';
import { PaginatorComponent } from '../../PaginatorComponent';
import App from '../../../App';

/** Отображение/редактирование существующей номенклатуры */
export class viewGood extends aPageList {
    static displayName = viewGood.name;

    constructor(props) {
        super(props);

        /** изменение цены */
        this.handlePriceChange = this.handlePriceChange.bind(this);
        this.handleClickButton = this.handleClickButton.bind(this);

        this.priceRef = React.createRef();
    }

    /**
    * событие изменения цены
    * @param {object} e - object sender
    */
    handlePriceChange(e) {
        var targetValue = e.target.value;
        if (!(/^[\d,.]+$/.test(targetValue)) || (targetValue.match(/[^\d]/g) || []).length > 1) {
            if (this.lastPrice) {
                e.target.value = this.lastPrice;
            }
            else {
                const data = App.data;
                e.target.value = data.price;
            }
            return;
        }

        targetValue = targetValue.replace(',', '.');
        e.target.value = targetValue;
        this.lastPrice = targetValue;
    }

    /** сохранение */
    async handleClickButton(e) {
        var priceValue = `${this.priceRef.current.value}`;
        if (/^[,.]/.test(priceValue)) {
            priceValue = `0${priceValue}`;
        }
        if (/[,.]$/.test(priceValue)) {
            priceValue = `${priceValue}0`;
        }

        this.priceRef.current.value = priceValue;
        super.handleClickButton(e);
    }

    async load(continueLoading = false) {
        this.apiPostfix = `/${App.id}`;
        await super.load(true);
        this.cardTitle = `Номенклатура: [#${App.data.id}] ${App.data.name}`;
        const data = App.data;
        this.setState({
            loading: false,
            unitId: data.unitId,
            groupId: data.groupId,
            price: data.price
        });
    }

    cardBody() {
        const good = App.data;

        return (
            <>
                <form className='mb-2'>
                    <input name='id' defaultValue={good.id} type='hidden' />
                    <div className="form-row">
                        <div className="col">
                            <label>Наименование</label>
                            <input defaultValue={good.name} name='name' type="text" className="form-control" placeholder="Новое название" />
                        </div>
                        <div className="col">
                            <label>Группа</label>
                            <select defaultValue={good.groupId} name='groupId' className="custom-select">
                                {good.groups.map(function (element) {
                                    return <option key={element.id} value={element.id} title={element.information}>{element.name}</option>
                                })}
                            </select>
                        </div>
                    </div>
                    <div className="form-row mb-2">
                        <div className="col">
                            <label>Ед. измерения</label>
                            <select defaultValue={good.unitId} name='unitId' className="custom-select" aria-describedby="unitHelp">
                                {good.units.map(function (element) {
                                    return <option key={element.id} value={element.id} title={element.information}>{element.name}</option>
                                })}
                            </select>
                            <small id="unitHelp" className="form-text text-muted">Ед.изм. по умолчанию.</small>
                        </div>
                        <div className="col">
                            <label>Цена</label>
                            <input ref={this.priceRef} defaultValue={good.price} onChange={this.handlePriceChange} isdouble='true' name='price' type="text" className="form-control" placeholder="0.0" aria-describedby="priceHelp" />
                            <small id="priceHelp" className="form-text text-muted">Цена продажи</small>
                        </div>
                    </div>
                    {this.getInformation()}
                    {this.rootPanelObject()}
                    <hr />
                    {this.viewButtons()}
                </form>                
                <div className='card'>
                    <div className='card-body'>
                        <legend>Регистры оборотов по номенклатуре</legend>
                        <table className='table table-striped mt-4'>
                            <thead>
                                <tr>
                                    <th>id</th>
                                    <th>Info</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                {good.registers.map(function (register) {
                                    const document = register.document;
                                    const urlDocument = `/${document.apiName}/${App.viewNameMethod}/${document.id}?row=${register.id}`;
                                    const authorDom = <small className='text-muted'>(автор: <NavLink to={`/users/${App.viewNameMethod}/${document.author.id}`} title='автор документа движения'>{document.author.name}</NavLink>)</small>;
                                    var extDataAboutDocument = <></>;
                                    switch (document.apiName) {
                                        case 'movementgoodswarehouses':
                                            const warehouse = document.warehouse;
                                            extDataAboutDocument = <small className='text-muted'>(склад: <NavLink to='#' /*to={`/warehouses/${App.viewNameMethod}/${warehouse.id}`}*/ title='склад поступления'>{warehouse.name}</NavLink>)</small>;
                                            break;
                                        case 'movementturnoverdeliverydocumentmodel':
                                            const buyer = document.buyer
                                                ? <><>buyer </><NavLink to={`/users/${App.viewNameMethod}/${document.buyer.id}`} title='покупатель'>[{document.buyer.name}]</NavLink></>
                                                : <></>;

                                            const deliveryMethod = <><>(отгрузка: </><NavLink to='#' /*to={`/deliverymethods/${App.viewNameMethod}/`}*/>{document.deliveryMethod.name}</NavLink></>;

                                            const deliveryService = document.deliveryService
                                                ? <>/<NavLink to='#' /*to={`/deliveryservices/${App.viewNameMethod}/${document.deliveryService.id}`}*/>{document.deliveryService.name}</NavLink></>
                                                : <>)</>;

                                            const DeliveryAddress1 = document.DeliveryAddress1
                                                ? <>адрес: {document.DeliveryAddress1}</>
                                                : <></>;

                                            const DeliveryAddress2 = document.DeliveryAddress2
                                                ? <>, {document.DeliveryAddress2}</>
                                                : <></>;

                                            extDataAboutDocument = <small className='text-muted'>{deliveryMethod} {deliveryService} {DeliveryAddress1} {DeliveryAddress2} {buyer}: <NavLink /*to={`/warehouses/${App.viewNameMethod}/${warehouse.id}`}*/ title='склад поступления'>{warehouse.name}</NavLink>)</small>;
                                            break;
                                        default:

                                            break;
                                    }
                                    const currentNavLink = register.isDisabled === true
                                        ? <del><NavLink className='text-muted' to={urlDocument} title='открыть документ'>{document.about}</NavLink></del>
                                        : <><NavLink to='#' /*to={urlDocument}*/ title='открыть документ'>{document.name}</NavLink> {authorDom} {extDataAboutDocument}</>

                                    return <tr title='строка/регистр документа' key={register.id}>
                                        <td>{register.id}</td>
                                        <td>
                                            {currentNavLink}
                                        </td>
                                        <td>{register.quantity} {good.units.find(function (element, index, array) { return element.id === register.unitId }).name}</td>
                                    </tr>
                                })}
                            </tbody>
                        </table>
                    </div>
                </div>
                <PaginatorComponent servicePaginator={this.servicePaginator} />
            </>
        );
    }

    cardHeaderPanel() {
        return <NavLink className='btn btn-outline-info btn-sm' to={`/unitsgoods/${App.listNameMethod}/`} role='button' title='Перейти в справочник единиц измерения'>Единицы измерения</NavLink>;
    }
}