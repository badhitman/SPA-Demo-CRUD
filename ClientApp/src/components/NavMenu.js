////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink, DropdownMenu, UncontrolledDropdown, DropdownToggle, DropdownItem } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';

export class NavMenu extends Component {
    static displayName = NavMenu.name;
    static myMenu = [{ htmlDomId: "guestMenuItem", title: "Вход", href: "/signin/", tooltip: "Авторизация/Регистрация", childs: undefined }];

    constructor(props) {
        super(props);
        this.toggleNavbar = this.toggleNavbar.bind(this);

        /** Создание метода Array.isArray(), если он ещё не реализован в браузере. */
        if (!Array.isArray) {
            Array.isArray = function (arg) {
                return Object.prototype.toString.call(arg) === '[object Array]';
            };
        }

        this.state = {
            collapsed: true,
            loading: true
        };
    }

    componentDidMount() {
        this.loadMenu();
    }

    async loadMenu() {
        const response = await fetch('/api/getmenu/');
        NavMenu.myMenu = await response.json();
        this.setState({ loading: false });
    }

    toggleNavbar() {
        this.setState({
            collapsed: !this.state.collapsed
        });
    }

    render() {
        var dividersKeys = 0;
        var uncontrolledDropdown = 0;
        const menuItems = this.state.loading === true
            ? <li>Загрузка меню... <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span></li>
            : NavMenu.myMenu.map(function (item) {
                if (Array.isArray(item.childs)) {
                    return (
                        <UncontrolledDropdown key={uncontrolledDropdown++} nav inNavbar>
                            <DropdownToggle nav caret>{item.title}</DropdownToggle>
                            <DropdownMenu right>
                                {item.childs.map(function (subItem) {
                                    return subItem === null
                                        ? <DropdownItem className='text-secondary' key={dividersKeys++} divider />
                                        :
                                        <DropdownItem key={subItem.href}>
                                            <NavItem title={subItem.tooltip}>
                                                <NavLink disabled={subItem.isDisabled} className='text-dark' tag={Link} to={subItem.href}>{subItem.title}</NavLink>
                                            </NavItem>
                                        </DropdownItem>
                                })}
                            </DropdownMenu>
                        </UncontrolledDropdown>
                    )
                }
                else {
                    return (<NavItem key={item.href}><NavLink tag={Link} to={item.href} tooltip={item.tooltip}>{item.title}</NavLink></NavItem>)
                }
            });

        return (
            <header>
                <Navbar expand="sm" color="dark" className="navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" dark>
                    <Container>
                        <NavbarBrand tag={Link} to="/"><strong className="text-info">SPA</strong> - <span className="text-primary\"> CRUD demo</span></NavbarBrand>
                        <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
                        <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
                            <ul className="navbar-nav flex-grow navbar-dark bg-dark">
                                {menuItems}
                            </ul>
                        </Collapse>
                    </Container>
                </Navbar>
            </header>
        );
    }
}
