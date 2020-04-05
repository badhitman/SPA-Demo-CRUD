////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageCard } from '../aPageCard';
import { NavLink } from 'react-router-dom';
import App from '../../../App';

/** Отображение/редактирование существующего файла */
export class viewFile extends aPageCard {
    static displayName = viewFile.name;

    get isFtpFileContext() { return localStorage.getItem('fileContext') === 'ftp'; }

    async ajax() {
        this.apiPostfix = `/info${(this.isFtpFileContext === true ? 'ftp' : 'storage')}`;
        await super.ajax();
    }

    async load() {
        await this.ajax();
        this.cardTitle = `Файл: [#${App.data.id}] ${App.data.name}`;
        this.setState({ loading: false });
    }

    cardHeaderPanel() {
        if (this.isFtpFileContext === true) {
            return <></>;
        }
        return super.cardHeaderPanel();
    }

    getExtPropsDom(label, value) {
        return <div key={label} className='form-group row'>
            <label className='col-sm-2 col-form-label'>{label}</label>
            <div className='col-sm-10'>
                <input type="text" readOnly className='form-control-plaintext' defaultValue={value} />
            </div>
        </div>
    }

    cardBody() {
        const file = App.data;
        const altInfo = file.name;
        const srcImg = `/${App.controller}/src${this.isFtpFileContext === true ? 'ftp' : 'storage'}?id=${this.isFtpFileContext === true ? file.name : file.id}`;
        const extPropsFile = [];
        if (file.creationTime) {
            extPropsFile.push(this.getExtPropsDom('Дата создания', file.creationTime));
        }
        if (file.isDisabled === true) {
            extPropsFile.push(this.getExtPropsDom('isDisabled', 'Объект имеет маркировку: "отключённый"'));
        }
        if (file.readonly === true) {
            extPropsFile.push(this.getExtPropsDom('Readonly', 'Объект имеет маркировку: "только для чтения"'));
        }
        return (
            <form className='mb-2' key='view-form'>
                {extPropsFile.map(function (element) { return element; })}
                <input name='id' defaultValue={file.id} type='hidden' />
                <div className="form-group">
                    <img src={srcImg} alt={altInfo} className="img-thumbnail" />
                </div>
                {this.viewButtons()}
            </form>
        );
    }

    /** Набор кнопок управления для формы просмотра/редактирования объекта */
    viewButtons() {
        return (<div className="btn-toolbar justify-content-end" role="toolbar" aria-label="Toolbar with button groups">
            <div className="btn-group" role="group" aria-label="First group">
                <NavLink className='btn btn-outline-primary' to={`/${App.controller}/${App.listNameMethod}/`} role='button' title='Вернуться к списку без сохранения'>Вернуться к списку</NavLink>
                <NavLink className='btn btn-outline-danger' to={`/${App.controller}/${App.deleteNameMethod}/${App.data.id}/`} role='button' title='Удалить объект из базы данных'>Удаление</NavLink>
            </div>
        </div>);
    }
}