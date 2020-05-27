////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import { Pagination, PaginationItem, PaginationLink, Dropdown, DropdownToggle, DropdownMenu, DropdownItem, NavLink } from 'reactstrap';
import { PaginationNavElement } from './PaginationNavElement';
import { PaginationTypesButton } from './PaginationTypesButton';
import Cookies from 'universal-cookie';

/** Компонент Пагинатор */
export class PaginatorComponent extends Component {
    static displayName = PaginatorComponent.name;

    /** адресс запроса (всё что до: ?x=1&y=2) */
    urlRequestAddress = '';
    /** пользовательская строка запроса (та что: ?x=1&y=2). К пользовательским будут добавлены автоматически-рассчитанные: pageNum=XX&pageSize=YY */
    urlQuery = '';
    /** Количество строк в списке всего */
    rowsCount = 0;
    /** количество страниц исходя из размерности страницы и количества строк в коллекции */
    pagesCount = 1;
    /** размерность страницы пагинатора */
    pageSize = 10;
    /** текущий номер страницы */
    pageNum = 1;

    constructor(props) {
        super(props);

        if (!props) {
            return;
        }

        const servicePaginator = this.props.servicePaginator;
        if (servicePaginator) {
            this.urlRequestAddress = servicePaginator.urlRequestAddress;

            this.rowsCount = servicePaginator.rowsCount;
            this.pagesCount = servicePaginator.pagesCount;
            this.pageSize = servicePaginator.pageSize;
            this.pageNum = servicePaginator.pageNum;
        }

        this.state = { dropdownOpenPaginator: false };
        this.togglePaginator = this.togglePaginator.bind(this);
    }

    /**
     * Чтение состояния пагинатора. Возвращает: true - если состояние пагинатора изменилось, false - если изменений в пагинаторе не замечено
     * @param {string} urlRequestAddress - адресс запроса (всё что до: ?x=1&y=2)
     * @param {string} urlQuery - пользовательская строка запроса (та что: ?x=1&y=2). К пользовательским будут добавлены автоматически-рассчитанные: pageNum=XX&pageSize=YY
     */
    readPagination(urlRequestAddress = '', urlQuery = '') {
        const regExp = /^\d+$/;
        const cookies = new Cookies();
        var rowsCount = cookies.get('rowsCount');
        if (regExp.test(rowsCount)) {
            rowsCount = parseInt(rowsCount, 10);
            if (rowsCount < 0) {
                rowsCount = 0;
            }
        }
        else {
            rowsCount = 0;
        }

        const paginationParams = new URLSearchParams(this.props.location.search);

        var pageNum = paginationParams.get('pageNum');
        if (regExp.test(pageNum)) {
            pageNum = parseInt(pageNum, 10);
            if (pageNum <= 0) {
                pageNum = 1;
            }
        }
        else {
            pageNum = 1;
        }

        var pageSize = paginationParams.get('pageSize');
        if (regExp.test(pageSize)) {
            pageSize = parseInt(pageSize, 10);
            if (pageSize <= 0) {
                pageSize = 1;
            }
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

        if (urlQuery && urlQuery.length > 0) {
            urlQuery = `${urlQuery}&pageNum=${pageNum}&pageSize=${pageSize}`;
        }
        else {
            urlQuery = `pageNum=${pageNum}&pageSize=${pageSize}`;
        }

        const isModifiedState = urlQuery !== this.urlQuery || rowsCount !== this.rowsCount || pagesCount !== this.pagesCount || pageSize !== this.pageSize || pageNum !== this.pageNum || this.urlRequestAddress !== urlRequestAddress;

        this.rowsCount = rowsCount;
        this.pagesCount = pagesCount;
        this.pageSize = pageSize;
        this.pageNum = pageNum;
        this.urlRequestAddress = urlRequestAddress;
        this.urlQuery = urlQuery;

        return isModifiedState;
    }

    togglePaginator() {
        this.setState({
            dropdownOpenPaginator: !this.state.dropdownOpenPaginator
        });
    }

    render() {
        var urlTmpl = `${this.urlRequestAddress}?pageSize=${this.pageSize}&pageNum=`;
        const PageNum = this.pageNum;

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

        const clearQuery = (true && this.urlQuery && this.urlQuery.length > 0) ? this.urlQuery + '&' : '';
        return (
            <div className="d-flex justify-content-end mt-2">
                <Dropdown className='mr-2' size="sm" isOpen={this.state.dropdownOpenPaginator} toggle={this.togglePaginator}>
                    <DropdownToggle title='Размерность пагинатора' caret>{this.pageSize}</DropdownToggle>
                    <DropdownMenu>
                        <DropdownItem className={(this.pageSize === 10 ? 'font-weight-bold shadow rounded' : '')}><NavLink tag={Link} to={`${this.urlRequestAddress}?${clearQuery}pageSize=10&pageNum=1`} title=''>10</NavLink></DropdownItem>
                        <DropdownItem className={(this.pageSize === 20 ? 'font-weight-bold shadow rounded' : '')}><NavLink tag={Link} to={`${this.urlRequestAddress}?${clearQuery}pageSize=20&pageNum=1`} title=''>20</NavLink></DropdownItem>
                        <DropdownItem className={(this.pageSize === 50 ? 'font-weight-bold shadow rounded' : '')}><NavLink tag={Link} to={`${this.urlRequestAddress}?${clearQuery}pageSize=50&pageNum=1`} title=''>50</NavLink></DropdownItem>
                        <DropdownItem className={(this.pageSize === 100 ? 'font-weight-bold shadow rounded' : '')}><NavLink tag={Link} to={`${this.urlRequestAddress}?${clearQuery}pageSize=100&pageNum=1`} title=''>100</NavLink></DropdownItem>
                        <DropdownItem className={(this.pageSize === 200 ? 'font-weight-bold shadow rounded' : '')}><NavLink tag={Link} to={`${this.urlRequestAddress}?${clearQuery}pageSize=200&pageNum=1`} title=''>200</NavLink></DropdownItem>
                    </DropdownMenu>
                </Dropdown>
                <Pagination size="sm" aria-label="Page navigation example">
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
                                    return <PaginationItem key={index} active><PaginationLink>{item.title}</PaginationLink></PaginationItem>;
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