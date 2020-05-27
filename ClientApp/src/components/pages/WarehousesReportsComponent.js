////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import { userAlertComponent } from '../userAlertComponent';
import App from '../../App';

/** Отчёты по номенклатуре и складам */
export class WarehousesReportsComponent extends userAlertComponent {
    static displayName = WarehousesReportsComponent.name;
    /** использовать форму выбора типа отчёта или нет */
    reportSelectionFormEnable = true;

    constructor(props) {
        super(props);
        const typeReport = props.typeReport;
        this.reportSelectionFormEnable = typeReport === 0;

        this.state = {
            typeReport,
            loading: true
        }

        /** событие изменения вида отчёта */
        this.handleTypeReportChange = this.handleTypeReportChange.bind(this);
    }

    handleTypeReportChange(e) {
        const targetValue = e.target.value;
        this.setState({ typeReport: targetValue, loading: true });
        this.loadReport(targetValue);
        localStorage.setItem('typeWarehouseReport', targetValue);
    }

    /** получение данных с сервера */
    async getAjaxData(apiUrl) {
        const response = await fetch(apiUrl);

        try {
            if (response.ok === true) {
                const result = await response.json();

                if (result.success === false) {
                    this.clientAlert(result.info, result.status);
                }
                else {
                    return result.tag;
                }
            }
            else {
                const msg = `Ошибка обработки запроса. CODE[${response.status}] Status[${response.statusText}]`;
                console.error(msg);
                this.clientAlert(msg, 'danger');
            }
        }
        catch (err) {
            console.error(err);
            this.clientAlert(err, 'danger');
        }
        return null;
    }

    async loadReportsTypesList() {
        this.types = await this.getAjaxData('/api/WarehousesReports/');
        var storageTypeReport = localStorage.getItem('typeWarehouseReport');
        if (/^\d+$/.test(storageTypeReport)) {
            storageTypeReport = parseInt(storageTypeReport, 10);
        }
        else {
            storageTypeReport = this.types[0].id;
        }
        this.report = await this.getAjaxData(`/api/WarehousesReports/${storageTypeReport}`);
        this.setState({ typeReport: storageTypeReport, loading: false });
    }

    async loadReport(reportId) {
        this.report = await this.getAjaxData(`/api/WarehousesReports/${reportId}`);
        this.setState({ loading: false });
    }

    componentDidMount() {
        if (this.reportSelectionFormEnable === true) {
            this.loadReportsTypesList();
        }
        else {
            //var typeReport = this.state.typeReport;
            //if (typeReport < 1) {
            //    typeReport = localStorage.getItem('typeWarehouseReport');
            //    if (/^\d+$/.test(typeReport)) {
            //        typeReport = parseInt(typeReport, 10);
            //    }
            //    else {
            //        typeReport = this.types[0].id;
            //    }
            //}
            //this.loadReport(typeReport);
        }
    }

    render() {
        if (this.state.loading === true) {
            return <div className="spinner-border spinner-border-sm text-primary" role="status">
                <span className="sr-only">Loading...</span>
            </div>;
        }

        const types = this.types;
        const reportSelectionForm = this.reportSelectionFormEnable === true
            ? <div className="form-group">
                <label htmlFor="typeReport">Вид отчёта</label>
                <select id='typeReport' value={this.state.typeReport} onChange={this.handleTypeReportChange} className="custom-select">
                    {types.map(function (element) {
                        return <option key={element.id} value={element.id}>{element.name}</option>
                    })}
                </select>
            </div>
            : <></>;

        const report = <>
            <table className='table table-hover'>
                <thead>
                    <tr>
                        <th>Наименование</th>
                        <th>Остаток</th>
                    </tr>
                </thead>
                <tbody>
                    {this.report.map(function (element, index) {
                        return <Fragment key={index}>
                            <tr className='table-info'>
                                <td>
                                    <h6>{element.key.name}</h6>
                                </td>
                                <td>
                                    <strong>{element.key.sum}</strong>
                                </td>
                            </tr>
                            {element.groupValues.map(function (subElement, subIndex) {
                                const subKey = `${index}/${subIndex}`;
                                return <tr key={subKey}>
                                    <td>
                                        <><mark className='ml-4'>{subElement.name}</mark></>
                                    </td>
                                    <td>
                                        {subElement.quantity}
                                    </td>
                                </tr>;
                            })}
                        </Fragment>;
                    })}
                </tbody>
            </table>
        </>;

        return <>
            {reportSelectionForm}
            {report}
        </>;
    }
}

WarehousesReportsComponent.propTypes = {
    typeReport: PropTypes.number
};

WarehousesReportsComponent.defaultProps = {
    typeReport: 0
};