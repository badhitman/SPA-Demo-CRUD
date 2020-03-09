import React, { Component } from 'react';
import { Route, Switch } from 'react-router';
import { Redirect } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { NotFound } from './components/NotFound';

import './custom.css'

export default class App extends Component {
    static displayName = App.name;

    render() {
        return (
            <Layout>
                <Switch>
                    <Route path='/:controller/:method/:id?' component={Home} />
                    <Redirect from='/' to={`/users/${Home.listNameMethod}`} />
                    <Route component={NotFound} />
                </Switch>
            </Layout>
        );
    }
}
