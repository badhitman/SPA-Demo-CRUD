////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React, { Component } from 'react';
import { Route, Switch } from 'react-router';
import App from './App';
import { NotFound } from './components/NotFound';
import { AccessDenied } from './components/AccessDenied';

import { HomePage } from './components/pages/HomePage';
import { SignIn } from './components/pages/SignIn';
import { NavMenu } from './components/NavMenu';

import { listUsers } from './components/pages/users/listUsers';
import { viewUser } from './components/pages/users/viewUser';
import { createUser } from './components/pages/users/createUser';
import { deleteUser } from './components/pages/users/deleteUser';

import { listDepartments } from './components/pages/departments/listDepartments';
import { viewDepartment } from './components/pages/departments/viewDepartment';
import { createDepartment } from './components/pages/departments/createDepartment';
import { deleteDepartment } from './components/pages/departments/deleteDepartment';

import { viewGood } from './components/pages/goods/viewGood';
import { deleteGood } from './components/pages/goods/deleteGood';
//
import { listGroupsGoods } from './components/pages/goods/listGroupsGoods';
import { viewGroupGoods } from './components/pages/goods/viewGroupGoods';
import { deleteGroupGoods } from './components/pages/goods/deleteGroupGoods';
//
import { listUnitsGoods } from './components/pages/unitsgoods/listUnitsGoods';
import { viewUnitGoods } from './components/pages/unitsgoods/viewUnitGoods';
import { deleteUnitGoods } from './components/pages/unitsgoods/deleteUnitGoods';

import { listDeliveries } from './components/pages/delivery/listDeliveries';
import { viewDelivery } from './components/pages/delivery/viewDelivery';
import { createDelivery } from './components/pages/delivery/createDelivery';
import { deleteDelivery } from './components/pages/delivery/deleteDelivery';

import { listTurnovers } from './components/pages/turnovers/listTurnovers';
import { viewTurnover } from './components/pages/turnovers/viewTurnover';
import { createTurnover } from './components/pages/turnovers/createTurnover';
import { deleteTurnover } from './components/pages/turnovers/deleteTurnover';

import { listWarehouses } from './components/pages/warehouses/listWarehouses';
import { viewWarehouse } from './components/pages/warehouses/viewWarehouse';
import { deleteWarehouse } from './components/pages/warehouses/deleteWarehouse';

import { warehouseDocumentsList } from './components/pages/warehouseDocumentsList';

import { viewReceiptWarehouse } from './components/pages/receiptswarehouses/viewReceiptWarehouse';
import { createReceiptWarehouse } from './components/pages/receiptswarehouses/createReceiptWarehouse';
import { deleteReceiptWarehouse } from './components/pages/receiptswarehouses/deleteReceiptWarehouse';

import { viewWarehousesDisplacement } from './components/pages/displacements/viewWarehousesDisplacement';
import { createWarehousesDisplacement } from './components/pages/displacements/createWarehousesDisplacement';
import { deleteWarehousesDisplacement } from './components/pages/displacements/deleteWarehousesDisplacement';

import { WarehousesReports } from './components/pages/WarehousesReports';

import { listFiles } from './components/pages/files/listFiles';
import { viewFile } from './components/pages/files/viewFile';
import { deleteFile } from './components/pages/files/deleteFile';

import { listNotifications } from './components/pages/notifications/listNotifications';
import { viewNotification } from './components/pages/notifications/viewNotification';
import { deleteNotification } from './components/pages/notifications/deleteNotification';

import { listTelegramBotUpdates } from './components/pages/telegram/listTelegramBotUpdates';
import { viewTelegramBotUpdate } from './components/pages/telegram/viewTelegramBotUpdate';
import { deleteTelegramBotUpdate } from './components/pages/telegram/deleteTelegramBotUpdate';

import { listElectrum } from './components/pages/electrum/listElectrum';
import { viewElectrum } from './components/pages/electrum/viewElectrum';

import { viewProfile } from './components/pages/profile/viewProfile';

import { viewServer } from './components/pages/server/viewServer';

export class Hub extends Component {
    static displayName = Hub.name;

    render() {
        App.controller = this.props.match.params.controller;
        App.method = this.props.match.params.method;
        App.id = this.props.match.params.id;
        App.data = null;

        if (App.controller !== undefined && App.allowsControllers.includes(App.controller.toLowerCase()) !== true) {
            console.error('Недопустимое имя контроллера: ' + App.controller);
            return <NotFound><hr /><p>Запрашиваемый контроллер: <span className="badge badge-warning"><b>{App.controller}</b></span></p>Доступные имена контроллеров: {App.allowsControllers.map(function (controllerName) { return <span key={controllerName} className="badge badge-primary mx-1">{controllerName}</span> })}</NotFound>;
        }

        if (App.method !== undefined && App.allowsMethods.includes(App.method) !== true) {
            console.error('Недопустимое имя метода: ' + App.method + '');
            return <NotFound><hr /><p>Ошибка имени метода: <span className="badge badge-warning"><b>{App.method}</b></span></p>Доступные имена метдов: {App.allowsMethods.map(function (methodName) { return <span key={methodName} className="badge badge-primary mx-1">{methodName}</span> })}</NotFound>;
        }

        return (
            <>
                <NavMenu key={App.session.role} />
                <main role="main" className="pb-3">
                    <Switch>
                        <Route exact path='/' component={HomePage} />

                        {/** сессия (logIn/logOut) */}
                        <Route path={'/signin/'} component={SignIn} />

                        {/** пользователи */}
                        <Route path={`/users/${App.listNameMethod}/`} component={listUsers} />
                        <Route path={`/users/${App.viewNameMethod}`} component={viewUser} />
                        <Route path={`/users/${App.createNameMethod}`} component={createUser} />
                        <Route path={`/users/${App.deleteNameMethod}`} component={deleteUser} />

                        {/** департаменты/отделы */}
                        <Route path={`/departments/${App.listNameMethod}/`} component={listDepartments} />
                        <Route path={`/departments/${App.viewNameMethod}`} component={viewDepartment} />
                        <Route path={`/departments/${App.createNameMethod}`} component={createDepartment} />
                        <Route path={`/departments/${App.deleteNameMethod}`} component={deleteDepartment} />

                        {/** группы номенклатуры */}
                        <Route path={`/goods/${App.listNameMethod}`} component={listGroupsGoods} />

                        <Route path={`/groupsgoods/${App.listNameMethod}/`} component={listGroupsGoods} />
                        <Route path={`/groupsgoods/${App.viewNameMethod}/`} component={viewGroupGoods} />
                        <Route path={`/groupsgoods/${App.deleteNameMethod}/`} component={deleteGroupGoods} />
                        {/** номенклатура */}
                        <Route path={`/goods/${App.viewNameMethod}`} component={viewGood} />
                        <Route path={`/goods/${App.deleteNameMethod}`} component={deleteGood} />
                        {/** единицы измерения номенклатуры */}
                        <Route path={`/unitsgoods/${App.listNameMethod}/`} component={listUnitsGoods} />
                        <Route path={`/unitsgoods/${App.viewNameMethod}`} component={viewUnitGoods} />
                        <Route path={`/unitsgoods/${App.deleteNameMethod}`} component={deleteUnitGoods} />

                        {/** доставка (настройки служб и методов) */}
                        <Route path={`/delivery/${App.listNameMethod}/`} component={listDeliveries} />
                        <Route path={`/delivery/${App.viewNameMethod}`} component={viewDelivery} />
                        <Route path={`/delivery/${App.createNameMethod}`} component={createDelivery} />
                        <Route path={`/delivery/${App.deleteNameMethod}`} component={deleteDelivery} />

                        {/** отгрузка номенклатуры (в доставке/продаже) */}
                        <Route path={`/turnovers/${App.listNameMethod}/`} component={listTurnovers} />
                        <Route path={`/turnovers/${App.viewNameMethod}`} component={viewTurnover} />
                        <Route path={`/turnovers/${App.createNameMethod}`} component={createTurnover} />
                        <Route path={`/turnovers/${App.deleteNameMethod}`} component={deleteTurnover} />

                        {/** журнал складских документов */}
                        <Route path={`/warehouseDocuments/${App.listNameMethod}`} component={warehouseDocumentsList} />
                        <Route path={`/receiptsWarehousesDocuments/${App.listNameMethod}`} component={warehouseDocumentsList} />
                        <Route path={`/displacementsDocuments/${App.listNameMethod}`} component={warehouseDocumentsList} />

                        {/** внутреннее перемещение (между складами) */}
                        <Route path={`/displacementsdocuments/${App.viewNameMethod}`} component={viewWarehousesDisplacement} />
                        <Route path={`/displacementsdocuments/${App.createNameMethod}`} component={createWarehousesDisplacement} />
                        <Route path={`/displacementsdocuments/${App.deleteNameMethod}`} component={deleteWarehousesDisplacement} />

                        {/** поступление на склад */}
                        <Route path={`/receiptswarehousesdocuments/${App.viewNameMethod}`} component={viewReceiptWarehouse} />
                        <Route path={`/receiptswarehousesdocuments/${App.createNameMethod}`} component={createReceiptWarehouse} />
                        <Route path={`/receiptswarehousesdocuments/${App.deleteNameMethod}`} component={deleteReceiptWarehouse} />

                        {/** складские отчёты */}
                        <Route path={`/warehousesreports`} component={WarehousesReports} />

                        {/** склады учёта */}
                        <Route path={`/warehouses/${App.listNameMethod}/`} component={listWarehouses} />
                        <Route path={`/warehouses/${App.viewNameMethod}`} component={viewWarehouse} />
                        <Route path={`/warehouses/${App.deleteNameMethod}`} component={deleteWarehouse} />

                        {/** пользовательские уведомления */}
                        <Route path={`/notifications/${App.listNameMethod}/`} component={listNotifications} />
                        <Route path={`/notifications/${App.viewNameMethod}`} component={viewNotification} />
                        <Route path={`/notifications/${App.deleteNameMethod}`} component={deleteNotification} />

                        {/** файловое хранилище */}
                        <Route path={`/files/${App.listNameMethod}/`} component={listFiles} />
                        <Route path={`/files/${App.viewNameMethod}`} component={viewFile} />
                        <Route path={`/files/${App.deleteNameMethod}`} component={deleteFile} />

                        {/** telegram bot */}
                        <Route path={`/telegram/${App.listNameMethod}/`} component={listTelegramBotUpdates} />
                        <Route path={`/telegram/${App.viewNameMethod}`} component={viewTelegramBotUpdate} />
                        <Route path={`/telegram/${App.deleteNameMethod}`} component={deleteTelegramBotUpdate} />

                        {/** Electrum wallet */}
                        <Route path={`/electrum/${App.listNameMethod}/`} component={listElectrum} />
                        <Route path={`/electrum/${App.viewNameMethod}`} component={viewElectrum} />

                        {/** работа с собственным профилем */}
                        <Route path={`/profile/${App.viewNameMethod}`} component={viewProfile} />

                        {/** Настройки/состояние сервера */}
                        <Route path={`/server/${App.viewNameMethod}`} component={viewServer} />

                        <Route path={'/accessdenied/'} component={AccessDenied} />
                        <Route component={NotFound} />
                    </Switch>
                </main>
            </>
        );
    }
}
