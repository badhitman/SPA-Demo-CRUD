////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { Pagination, PaginationItem, PaginationLink, Dropdown, DropdownToggle, DropdownMenu, DropdownItem, NavLink } from 'reactstrap';
import App from '../../App';
import { aPage } from './aPage';
import { PaginationNavElement } from '../PaginationNavElement';
import { PaginationTypesButton } from '../PaginationTypesButton';
import { Link } from 'react-router-dom';

import jquery from 'jquery';
import '../../jquery.cookie.js';

/** Списки/Справочники. Базовый (типа абстрактный) компонент */
export class aPageList extends aPage {
    static displayName = aPageList.name;
    apiName = '';
    /** Заголовок карточки (списка) */
    listCardHeader = '';

    constructor(props) {
        super(props);

        this.toggle = this.toggle.bind(this);
        this.state.dropdownOpen = false;

        /** Количество строк в списке всего */
        this.rowsCount = 0;
        /** количество страниц исходя из размерности страницы и количества строк в коллекции */
        this.pagesCount = 1;
        /** размерность страницы пагинатора */
        this.pageSize = 10;
        /** текущий номер страницы */
        this.pageNum = 1;
    }

    toggle() {
        this.setState(prevState => ({
            dropdownOpen: !prevState.dropdownOpen
        }));
    }

    /** Чтение состояния пагинатора.
     * Возвращает: true - если состояние пагинатора изменилось,
     * false - если изменений в пагинаторе не замечено */
    readPagination() {
        const regExp = /^\d+$/;

        var rowsCount = jquery.cookie('rowsCount');
        if (regExp.test(rowsCount)) {
            rowsCount = parseInt(rowsCount, 10);
        }
        else {
            rowsCount = 0;
        }

        const search = this.props.location.search;
        const paginationParams = new URLSearchParams(search);

        var pageNum = paginationParams.get('pageNum');
        if (regExp.test(pageNum)) {
            pageNum = parseInt(pageNum, 10);
        }
        else {
            pageNum = 1;
        }

        var pageSize = paginationParams.get('pageSize');
        if (regExp.test(pageSize)) {
            pageSize = parseInt(pageSize, 10);
        }
        else {
            pageSize = 10;
        }

        var pagesCount;
        if (rowsCount <= pageSize) {
            pagesCount = 1;
        }
        else {
            pagesCount = Math.ceil(rowsCount / pageSize);
        }

        const isModifiedState = rowsCount !== this.rowsCount || pagesCount !== this.pagesCount || pageSize !== this.pageSize || pageNum !== this.pageNum;

        this.rowsCount = rowsCount;
        this.pagesCount = pagesCount;
        this.pageSize = pageSize;
        this.pageNum = pageNum;

        return isModifiedState;
    }

    async ajax() {
        const response = await fetch(`/api/${this.apiName}/?pageNum=${this.pageNum}&pageSize=${this.pageSize}`);
        if (response.redirected === true) {
            window.location.href = response.url;
            return;
        }

        try {
            App.data = await response.json();
        }
        catch (err) {
            console.error(err);
            alert(err);
        }
    }

    async load() {
        this.readPagination();
        await this.ajax();
        this.readPagination();
        this.setState({ cardTitle: this.listCardHeader, loading: false });
    }

    cardHeaderPanel() {
        return <span title='реализация фильтров запланирована следующим этапом'>фильтры</span>;
    }

    cardPaginator() {
        var urlTmpl = `/${this.apiName}/${App.listNameMethod}/`;
        const PageNum = this.pageNum;
        this.paginationQueryTmpl = `?pageSize=${this.pageSize}&pageNum=`;

        urlTmpl = urlTmpl + this.paginationQueryTmpl;
        const CountPages = this.pagesCount;
        const pagunationItems = [new PaginationNavElement(PaginationTypesButton.Back, (PageNum > 1), undefined, (PageNum > 1 ? urlTmpl + (PageNum - 1) : undefined))];

        for (var i = 1; CountPages >= i; i++) {
            const isActive = (PageNum === i);
            if (CountPages > 7) {
                if (PageNum < 5) {
                    if (i === CountPages - 1) {
                        pagunationItems.push(new PaginationNavElement(PaginationTypesButton.Separator, false, '⁞', undefined));
                    }
                    else if (i <= 5 || i === CountPages) {
                        pagunationItems.push(new PaginationNavElement(PaginationTypesButton.Numb, isActive, i.toString(), urlTmpl + i.toString()));
                    }
                    else {
                        continue;
                    }
                }
                else if (PageNum > (CountPages - 4)) {
                    if (i === 2) {
                        pagunationItems.push(new PaginationNavElement(PaginationTypesButton.Separator, false, '⁞', undefined));
                    }
                    else if (i === 1 || i >= (CountPages - 4)) {
                        pagunationItems.push(new PaginationNavElement(PaginationTypesButton.Numb, isActive, i.toString(), urlTmpl + i.toString()));
                    }
                    else {
                        continue;
                    }
                }
                else {
                    if (i === 2 || i === CountPages - 1) {
                        pagunationItems.push(new PaginationNavElement(PaginationTypesButton.Separator, false, '⁞', undefined));
                    }
                    else if ((i === 1 || i === CountPages) || (i === PageNum - 1 || i === PageNum || i === PageNum + 1)) {
                        pagunationItems.push(new PaginationNavElement());
                        pagunationItems.push(new PaginationNavElement(PaginationTypesButton.Numb, isActive, i.toString(), urlTmpl + i.toString()));
                    }
                    else {
                        continue;
                    }
                }
            }
            else {
                pagunationItems.push(new PaginationNavElement(PaginationTypesButton.Numb, isActive, i.toString(), urlTmpl + i.toString()));
            }
        }
        pagunationItems.push(new PaginationNavElement(PaginationTypesButton.Next, false, undefined, (PageNum >= CountPages ? undefined : urlTmpl + (PageNum + 1).toString())));

        return (
            <div className="d-flex justify-content-end">
                <Dropdown className='mr-2' size="sm" isOpen={this.state.dropdownOpen} toggle={this.toggle}>
                    <DropdownToggle title='Размерность пагинатора' caret>{this.pageSize}</DropdownToggle>
                    <DropdownMenu>
                        <DropdownItem><NavLink tag={Link} to={`/${this.apiName}/${App.listNameMethod}/?pageSize=10&pageNum=1`} title=''>10</NavLink></DropdownItem>
                        <DropdownItem><NavLink tag={Link} to={`/${this.apiName}/${App.listNameMethod}/?pageSize=20&pageNum=1`} title=''>20</NavLink></DropdownItem>
                        <DropdownItem><NavLink tag={Link} to={`/${this.apiName}/${App.listNameMethod}/?pageSize=50&pageNum=1`} title=''>50</NavLink></DropdownItem>
                        <DropdownItem><NavLink tag={Link} to={`/${this.apiName}/${App.listNameMethod}/?pageSize=100&pageNum=1`} title=''>100</NavLink></DropdownItem>
                        <DropdownItem><NavLink tag={Link} to={`/${this.apiName}/${App.listNameMethod}/?pageSize=200&pageNum=1`} title=''>200</NavLink></DropdownItem>
                    </DropdownMenu>
                </Dropdown>
                <Pagination key={this.props.location.search} size="sm" aria-label="Page navigation example">
                    {pagunationItems.map(function (item, index) {
                        switch (item.typeButton) {
                            case PaginationTypesButton.Back:
                                if (item.href) {
                                    return <PaginationItem key={index}><PaginationLink previous tag={Link} to={item.href} /></PaginationItem>;
                                }
                                else {
                                    return <PaginationItem disabled key={index}><PaginationLink previous /></PaginationItem>;
                                }
                            case PaginationTypesButton.Next:
                                if (item.href) {
                                    return <PaginationItem key={index}><PaginationLink next tag={Link} to={item.href} /></PaginationItem>;
                                }
                                else {
                                    return <PaginationItem disabled key={index}><PaginationLink next /></PaginationItem>;
                                }
                            case PaginationTypesButton.Numb:
                                if (item.isActive) {
                                    return <PaginationItem key={index} active><PaginationLink tag={Link} to={item.href}>{item.title}</PaginationLink></PaginationItem>;
                                }
                                else {
                                    return <PaginationItem key={index}><PaginationLink tag={Link} to={item.href}>{item.title}</PaginationLink></PaginationItem>;
                                }
                            case PaginationTypesButton.Separator:
                                return <PaginationItem key={index} disabled><PaginationLink>{item.title}</PaginationLink></PaginationItem>;
                            default:
                                return <>ошибка пагинатора</>;
                        }
                    })}
                </Pagination>
            </div>
        );
    }
}