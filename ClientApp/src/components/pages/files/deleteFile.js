////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageCard } from '../aPageCard';
import App from '../../../App';

/** Удаление объекта/файла */
export class deleteFile extends aPageCard {
    static displayName = deleteFile.name;

    get isFtpFileContext() { return localStorage.getItem('fileContext') === 'ftp'; }

    async ajax() {
        this.apiPostfix = `/info${(this.isFtpFileContext === true ? 'ftp' : 'storage')}`;
        await super.ajax();
    }

    async load() {
        await this.ajax();
        this.cardTitle = `Удаление файла: [#${App.data.id}] ${App.data.name}`;
        this.setState({ loading: false });
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
                {this.deleteButtons()}
            </form>
        );
    }

    cardHeaderPanel() {
        if (this.isFtpFileContext === true) {
            return <></>;
        }
        return super.cardHeaderPanel();
    }

    async handleClickButton(e) {
        this.apiPostfix = '/delete' + (this.isFtpFileContext === true ? 'ftp' : 'storage') + '/' + App.id;
        super.handleClickButton(e);
    }

    /** Вкл/Выкл объект */
    async handleClickButtonDisable() {
        this.apiPostfix = '';
        //this.pushApiQueryParametr('id', App.data.id);
        super.handleClickButtonDisable();
    }
}