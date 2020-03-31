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
        const response = await fetch(`/${this.apiName}/delete/${App.id}`);
        App.data = await response.json();
        this.setState({ cardTitle: 'Удаление файла', loading: false });
    }
    cardBody() {
        const file = App.data;

        return (
            <>
                <div className="alert alert-danger" role="alert">Безвозвратное удаление файла! Данное дейтсвие нельзя будет отменить!</div>
                <form className="mb-3" key='delete-form'>
                    {this.mapObjectToReadonlyForm(file)}
                    {this.deleteButtons()}
                </form>
            </>
        );
    }
}