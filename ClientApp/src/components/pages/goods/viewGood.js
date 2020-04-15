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

        this.state.unitId = 0;
        this.state.groupId = 0;
        this.state.price = 0;
        this.state.name = '';

        /** изменение цены */
        this.handlePriceChange = this.handlePriceChange.bind(this);
        /** изменение единицы измерения */
        this.handleUnitChange = this.handleUnitChange.bind(this);
        /** изменение группы */
        this.handleGroupChange = this.handleGroupChange.bind(this);
        /** изменение имени */
        this.handleNameChange = this.handleNameChange.bind(this);
        /** сброс формы в исходное состояние */
        this.handleResetClick = this.handleResetClick.bind(this);
        /** сохранение в бд */
        this.handleSaveClick = this.handleSaveClick.bind(this);
    }

    /**
     * событие изменения имени
     * @param {object} e - object sender
     */
    handleNameChange(e) {
        const target = e.target;
        this.setState({
            name: target.value
        });
    }

    /**
    * событие изменения группы
    * @param {object} e - object sender
    */
    handleGroupChange(e) {
        const target = e.target;

        try {
            const targetValue = parseInt(target.value, 10);
            this.setState({
                groupId: targetValue
            });
        }
        catch (err) {
            this.clientAlert(err, 'danger');
        }
    }

    /**
    * событие изменения единицы измерения
    * @param {object} e - object sender
    */
    handleUnitChange(e) {
        const target = e.target;

        try {
            const targetValue = parseInt(target.value, 10);
            this.setState({
                unitId: targetValue
            });
        }
        catch (err) {
            this.clientAlert(err, 'danger');
        }
    }

    /**
    * событие изменения цены
    * @param {object} e - object sender
    */
    handlePriceChange(e) {
        var targetValue = e.target.value;
        if (!(/^[\d,.]+$/.test(targetValue)) || (targetValue.match(/[^\d]/g) || []).length > 1) {
            return;
        }

        targetValue = targetValue.replace(',', '.');

        try {
            this.setState({
                price: targetValue
            });
        }
        catch (err) {
            this.clientAlert(err, 'danger');
        }
    }

    /** Сброс формы */
    handleResetClick() {
        const data = App.data;
        this.setState({
            name: data.name,
            unitId: data.unitId,
            price: data.price,
            groupId: data.groupId,
            loading: false
        });
    }

    /** сохранение */
    async handleSaveClick() {
        var priceValue = `${this.state.price}`;
        if (/[,.]/.test(priceValue)) {
            priceValue = `0${priceValue}0`;
        }
        var sendedFormData =
        {
            id: parseInt(App.id, 10),
            name: this.state.name,
            unitId: this.state.unitId,
            groupId: this.state.groupId,
            price: parseFloat(parseFloat(priceValue.replace(',', '.')).toFixed(2))
        };
        const response = await fetch(`${this.apiPrefix}/${App.controller}/${App.id}`, {
            method: 'PUT',
            body: JSON.stringify(sendedFormData),
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            }
        });

        if (response.ok === true) {
            try {
                const result = await response.json();
                if (result.success === true) {
                    App.data.name = result.tag.name;
                    App.data.price = result.tag.price;
                    App.data.groupId = result.tag.groupId;
                    App.data.unitId = result.tag.unitId;
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

    async ajax() {
        this.apiPostfix = `/${App.id}`;
        await super.ajax();
    }

    async load() {
        await this.ajax();
        this.cardTitle = `Номенклатура: [#${App.data.id}] ${App.data.name}`;
        this.handleResetClick();
    }

    cardBody() {
        const good = App.data;

        const buttonSaveEnable = this.state.name.length > 0 && (this.state.name !== App.data.name || this.state.price !== App.data.price || this.state.unitId !== App.data.unitId || this.state.groupId !== App.data.groupId);
        const buttonResetEnable = this.state.name.length >= 0 && (this.state.name !== App.data.name || this.state.price !== App.data.price || this.state.unitId !== App.data.unitId || this.state.groupId !== App.data.groupId);
        const buttonDeleteEnable = (this.servicePaginator.rowsCount === 0 || this.servicePaginator.rowsCount === '0') && App.data.noDelete !== true;

        const buttonSave = buttonSaveEnable === true
            ? <button onClick={this.handleSaveClick} type="button" className="btn btn-outline-success btn-block" title='Сохранить объект в БД'>Сохранить</button>
            : <button disabled type="button" className="btn btn-outline-secondary btn-block" title='Объект в исходном состоянии'>Сохранение</button>;

        const buttonReset = buttonResetEnable === true
            ? <button onClick={this.handleResetClick} type="button" className="btn btn-outline-dark btn-block" title='Сброс значений формы на исходное'>Сбросить</button>
            : <button disabled type="button" className="btn btn-outline-secondary btn-block" title='Сброс значений формы на исходное'>Сбросить</button>;

        const buttonDelete = buttonDeleteEnable === true
            ? <NavLink className='btn btn-outline-danger btn-block' to={`/${App.controller}/${App.deleteNameMethod}/${App.id}`} role='button' title='Диалог удаления объекта'>Удаление</NavLink>
            : <button disabled type="button" className="btn btn-outline-secondary btn-block" title='Объект удалить невозможно'>Удаление невозможно</button>;

        const buttonBack = <NavLink className='btn btn-outline-primary btn-block' to={`/groupsgoods/${App.viewNameMethod}/${good.groupId}`} role='button' title='Вернуться в группу номенклатуры'>В группу</NavLink>

        return (
            <>
                <form className='mb-2'>
                    <input name='id' defaultValue={good.id} type='hidden' />
                    <div className="form-row">
                        <div className="col">
                            <label>Наименование</label>
                            <input value={this.state.name} onChange={this.handleNameChange} name='name' type="text" className="form-control" placeholder="Новое название" />
                        </div>
                        <div className="col">
                            <label>Группа</label>
                            <select value={this.state.groupId} onChange={this.handleGroupChange} className="custom-select">
                                {good.groups.map(function (element) {
                                    return <option key={element.id} value={element.id} title={element.information}>{element.name}</option>
                                })}
                            </select>
                        </div>
                    </div>
                    <div className="form-row">
                        <div className="col">
                            <label>Единица измерения</label>
                            <select value={this.state.unitId} onChange={this.handleUnitChange} className="custom-select">
                                {good.units.map(function (element) {
                                    return <option key={element.id} value={element.id} title={element.information}>{element.name}</option>
                                })}
                            </select>
                        </div>
                        <div className="col">
                            <label>Цена</label>
                            <input value={this.state.price} onChange={this.handlePriceChange} name='name' type="text" className="form-control" placeholder="0.0" />
                        </div>
                    </div>
                </form>
                <hr />
                <div className='form-row my-2'>
                    <div className='col'>{buttonSave}</div>
                    <div className='col'>{buttonReset}</div>
                </div>
                <div className='form-row mb-3'>
                    <div className='col'>{buttonDelete}</div>
                    <div className='col'>{buttonBack}</div>
                </div>
                <hr />
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
                                        : <><NavLink to='#' /*to={urlDocument}*/ title='открыть документ'>{document.about}</NavLink> {authorDom} {extDataAboutDocument}</>

                                    return <tr title='строка/регистр документа' key={register.id}>
                                        <td>{register.id}</td>
                                        <td>
                                            {currentNavLink}
                                        </td>
                                        <td>{register.quantity} {good.units.find(function (element, index, array) { return element.id === register.unitId }).name} x ${register.price}</td>
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