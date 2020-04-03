////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { viewFile } from './viewFile';
import App from '../../../App';

/** Удаление объекта/файла */
export class deleteFile extends viewFile {
    static displayName = deleteFile.name;



    async load() {
        this.apiPostfix = this.isFtpFileContext === true ? 'ftp' : 'storage';
        await super.load();
        this.setState({ cardTitle: 'Удаление файла', loading: false });
    }

    viewButtons() {
        return this.deleteButtons();
    }
}