////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { NavLink } from 'react-router-dom'
import { PaginatorComponent } from '../../PaginatorComponent';
import { viewGood } from './viewGood';
import App from '../../../App';

/** Удаление Номенклатуры */
export class deleteGood extends viewGood {
    static displayName = deleteGood.name;

    constructor(props) {
        super(props);

        /** удаление объекта из бд */
        this.handleDeleteClick = this.handleDeleteClick.bind(this);
    }

    /** удаление */
    async handleDeleteClick() {

        const response = await fetch(`/api/${App.controller}/${App.id}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            }
        });

        if (response.ok === true) {
            try {
                const result = await response.json();
                if (result.success === true) {
                    this.props.history.push(`/groupsgoods/${App.viewNameMethod}/${App.data.groupId}`);
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

    async load() {
        await super.load();
        this.cardTitle = `Удаление номенклатуры: [#${App.data.id}] ${App.data.name}`;
        this.setState({ loading: false });
    }

    cardBody() {
        var good = App.data;
        const buttonDeleteEnabled = (this.servicePaginator.rowsCount === 0 && good.noDelete !== true)
        const buttonDelete = buttonDeleteEnabled === true
            ? <button onClick={this.handleDeleteClick} type="button" className="btn btn-outline-danger btn-block">Подтверждение удаления</button>
            : <button disabled type="button" className="btn btn-outline-secondary btn-block" title='Вероятно на объект имеются ссылки'>Удаление невозможно</button>;

        return (
            <>
                <div className="alert alert-danger" role="alert">{(buttonDeleteEnabled === true ? 'Безвозратное удаление Номенклатуры! Данное дейтсвие нельзя будет отменить!' : 'Номенклатуру нельзя удалить. Существуют ссылки в других объектах')}</div>
                <NavLink to={`/${App.controller}/${App.viewNameMethod}/${App.id}`} className='btn btn-primary btn-block' role="button">Отмена</NavLink>
                {buttonDelete}
                <form className="my-3">
                    <div className='form-group row'>
                        <label className='col-sm-2 col-form-label'>Наименование</label>
                        <div className='col-sm-10'>
                            <input readOnly={true} defaultValue={good.name} className='form-control' type='text' />
                        </div>
                    </div>
                    <div className='form-group row'>
                        <label className='col-sm-2 col-form-label'>Группа</label>
                        <div className='col-sm-10'>
                            <input readOnly={true} defaultValue={good.groups.find(function (element, index, array) { return element.id === good.groupId }).name} className='form-control' type='text' />
                        </div>
                    </div>
                    <div className='form-group row'>
                        <label className='col-sm-2 col-form-label'>Единица измерения</label>
                        <div className='col-sm-10'>
                            <input readOnly={true} defaultValue={good.units.find(function (element, index, array) { return element.id === good.unitId }).name} className='form-control' type='text' />
                        </div>
                    </div>
                    <div className='form-group row'>
                        <label className='col-sm-2 col-form-label'>Цена</label>
                        <div className='col-sm-10'>
                            <input readOnly={true} defaultValue={good.price} className='form-control' type='text' />
                        </div>
                    </div>
                </form>
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
}
