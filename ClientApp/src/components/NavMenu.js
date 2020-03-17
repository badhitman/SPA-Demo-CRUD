////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';
import App from '../App';

export class NavMenu extends Component {
    static displayName = NavMenu.name;
    constructor(props) {
        super(props);
        this.toggleNavbar = this.toggleNavbar.bind(this);
        this.state = {
            collapsed: true
        };
    }

    toggleNavbar() {
        this.setState({
            collapsed: !this.state.collapsed
        });
    }
    
    render() {
        const session = App.session;
        return (
            <header>
                <Navbar expand="sm" color="dark" className="navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" dark>
                    <Container>
                        <NavbarBrand tag={Link} to="/"><strong className="text-info">SPA</strong> - <span className="text-primary\"> CRUD demo</span></NavbarBrand>
                        <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
                        <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
                            <ul className="navbar-nav flex-grow navbar-dark bg-dark">
                                {session.isAuthenticated === true ? this.getMenuAuthorizedUser() : this.getMenuAnonymousGuest()}
                            </ul>
                        </Collapse>
                    </Container>
                </Navbar>
            </header>
        );
    }

    getMenuAnonymousGuest() {
        return (
            <NavItem>
                <NavLink tag={Link} to="/signin/">Вход</NavLink>
            </NavItem>
        );
    }

    getMenuAuthorizedUser() {
        return (
            <>
                <NavItem>
                    <NavLink tag={Link} to="/users/list/">Сотрудники</NavLink>
                </NavItem>
                <NavItem>
                    <NavLink tag={Link} to="/departments/list/">Подразделения</NavLink>
                </NavItem>
                <NavItem>
                    <NavLink tag={Link} to="/signin/">Выход</NavLink>
                </NavItem>
            </>
        );
    }
}
