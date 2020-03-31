////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageList } from '../aPageList';
import { NavLink } from 'react-router-dom'
import App from '../../../App';

/** Компонент для отображения списка файлов */
export class listFiles extends aPageList {
    static displayName = listFiles.name;
    apiName = 'files';
    listCardHeader = 'Файловое хранилище';

    apiPrefix = '';
    apiPostfix = 'ftp';

    cardBody() {
        if (this.readPagination() === true) {
            this.load();
            return <></>;
        }
        var files = App.data;
        return (
            <>
                <table className='table table-striped mt-4' aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Length</th>
                        </tr>
                    </thead>
                    <tbody>
                        {files.map(function (file) {
                            return <tr key={file.name}>
                                <td>{file.name}</td>
                                <td>{file.length}</td>
                            </tr>
                        })}
                    </tbody>
                </table>
                {this.cardPaginator()}
            </>
        );
    }
}