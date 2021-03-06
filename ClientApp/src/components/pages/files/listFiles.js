////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { aPageList } from '../aPageList';
import { NavLink } from 'react-router-dom'
import App from '../../../App';
import { PaginatorComponent } from '../../PaginatorComponent';

/** Компонент для отображения списка файлов */
export class listFiles extends aPageList {
    static displayName = listFiles.name;

    cardTitle = 'Файловое хранилище';

    apiPrefix = '';
    apiPostfix = '/ftp';

    async ajax() {
        this.apiPostfix = this.isFtpFileContext === true ? '/ftp' : '/storage';
        await super.ajax();
    }

    get isFtpFileContext() { return localStorage.getItem('fileContext') === 'ftp'; }

    constructor(props) {
        super(props);

        this.getBaseButtons = this.getBaseButtons.bind(this);

        this.state.selectedFiles = [];
        this.state.labelFileInput = 'Выберете файлы для отправки';
        this.onChangeHandlerInputFiles = this.onChangeHandlerInputFiles.bind(this);
        this.handleClickSendFile = this.handleClickSendFile.bind(this);
        this.handleClickResetFile = this.handleClickResetFile.bind(this);

        this.handleClickToggleFileContext = this.handleClickToggleFileContext.bind(this);
        this.handleClickToggleViewStyle = this.handleClickToggleViewStyle.bind(this);
        this.handleClickTransferFile = this.handleClickTransferFile.bind(this);
    }

    /**
    * Обработчик сброса выбранных файлов
    * @param {any} e - context handle button
    */
    async handleClickResetFile(e) {
        const form = e.target.form;
        const fileInput = form['inputGroupFile'];
        fileInput.value = '';
        this.setState({ selectedFiles: [], labelFileInput: 'Выберете файлы для отправки' });
    }

    /**
    * Обработчик отправки выбранных файлов на сервер
    * @param {any} e - context handle button
    */
    async handleClickSendFile(e) {
        const form = e.target.form;
        var formData;
        for (var x = 0; x < this.state.selectedFiles.length; x++) {
            formData = new FormData();
            formData.append('file', this.state.selectedFiles[x]);

            try {
                const response = await fetch(`/${App.controller}/UploadFileToFtp`, {
                    method: 'POST',
                    body: formData
                });
                const result = await response.json();
                this.clientAlert(result.info, result.status);
            } catch (error) {
                console.error('Ошибка:', error);
            }
        }

        const fileInput = form['inputGroupFile'];
        fileInput.value = '';
        await this.ajax();
        this.setState({ selectedFiles: [], labelFileInput: 'Выберете файлы для отправки' });
    }

    /**
     * Обработчик выбора файлов для отправки
     * @param {any} e - context handle button
     */
    async onChangeHandlerInputFiles(e) {
        var labelFileInput = e.target.files.length === 0 ? 'Выберете файлы для отправки' : `Выбрано ${e.target.files.length} файл(ов)`;

        this.setState({ selectedFiles: e.target.files, labelFileInput });
    }

    /**
 * Обработчик нажатия кнопки трансфера файла ftp<=>storage
 * @param {any} e - context handle button
 */
    async handleClickTransferFile(e) {
        var idObject = `${e.target.id}`;
        if (/file-id-.+/.test(idObject) !== true) {
            this.clientAlert('не корректный id файла', 'warning');
            return;
        }
        idObject = idObject.substring(8);

        const response = await fetch(`/${App.controller}/MoveFileTo${this.isFtpFileContext === true ? 'Storage' : 'Ftp'}/${idObject}`);
        if (response.redirected === true) {
            window.location.href = response.url;
            return;
        }
        const result = await response.json();
        if (result.success === false) {
            this.clientAlert(result.info, result.status, 1500, 15000);
        }

        await this.ajax();
        this.forceUpdate();
    }

    /**
     * Обработчик нажатия кнопки переключения контекста данных (FTP/Storage)
     * @param {any} e - context handle button
     */
    async handleClickToggleFileContext(e) {
        if (this.isFtpFileContext === true) {
            localStorage.setItem('fileContext', 'storage');
        }
        else {
            localStorage.setItem('fileContext', 'ftp');
        }
        this.apiPostfix = this.isFtpFileContext === true ? '/ftp' : '/storage';
        this.load();
    }

    /**
     * Обработчик нажатия кнопки переключения стиля отображения список/плитка
     * @param {any} e - context handle button
     */
    async handleClickToggleViewStyle(e) {
        const isTilesModeView = localStorage.getItem('modeView') === 'tiles';
        if (isTilesModeView === true) {
            localStorage.setItem('modeView', 'table');
        }
        else {
            localStorage.setItem('modeView', 'tiles');
        }
        this.load();
    }

    /** Рендер функциональной панели, размещённого в заголовочной части карточки (прижата к правой части) */
    cardHeaderPanel() {
        const isTilesModeView = localStorage.getItem('modeView') === 'tiles';
        const modeViewInfo = isTilesModeView === true ? 'плитка' : 'таблица';

        const fileContextInfo = this.isFtpFileContext === true ? 'FTP' : 'Storage';

        return <>
            <button onClick={this.handleClickToggleFileContext} className="btn btn-sm btn-outline-info mr-2" title={`текущий контекст данных [${fileContextInfo}]. Для переключения на другой контекст - кликните по кнопке`} type="button">{fileContextInfo}</button>
            <button onClick={this.handleClickToggleViewStyle} className="btn btn-sm btn-outline-primary" title={`текущий режим отображения [${modeViewInfo}]. Для переключения на другой стиль - кликните по кнопке`} type="button">{modeViewInfo}</button>
        </>;
    }

    /** Рендер тела карточки страницы */
    cardBody() {
        //if (this.servicePaginator.readPagination() === true) {
        //    this.load();
        //    return <></>;
        //}
        const files = App.data;
        const isTilesModeView = localStorage.getItem('modeView') === 'tiles';

        var filesToUpload = [];
        for (var x = 0; x < this.state.selectedFiles.length; x++) {
            filesToUpload.push(this.state.selectedFiles[x]);
        }

        const filesNotSelected = this.state.selectedFiles.length === 0;

        /** Рендер набора инструментов для загрузки файлов через web интерфейс. Загружать файлы таким образом можно только в "начальную/uploads" папку.
         * В "хранимую/storage" папку файлы помещаются из числа уже загруженных файлов */
        const uploadNewFileButton = this.isFtpFileContext === true
            ? <form>
                <div className="input-group mb-2">
                    <div className="custom-file">
                        <input disabled multiple type="file" className="custom-file-input" id="inputGroupFile" aria-describedby="inputGroupFileAddon" onChange={this.onChangeHandlerInputFiles} title='Функционал кнопки станет доступен после того как найдётся причина блокировки загруженных файлов, в связи с чем загруженые файлы остаются "заблокированными" до перезапуская приложения. Без перезагрузки файлы недоступны для манипуляций: The process cannot access the file because it is being used by another process. На этот период файлы можно загружать через FTP. Загружать нужно непосредственно в папку "Uploads" (этот параметр есть в конфигурационном файле). Это и есть начальная папка появления файлов в системе извне. Дальнейшая манипуляция файлами производится через Web интерфейс (просмотр, удаление, перенос и т.д.)' />
                        <label className="custom-file-label" htmlFor="inputGroupFile">{this.state.labelFileInput}</label>
                    </div>
                    <div className="input-group-append">
                        <button disabled={filesNotSelected} className="btn btn-outline-secondary" type="button" onClick={this.handleClickSendFile} id="inputGroupFileAddon">Отправить</button>
                        <button disabled={filesNotSelected} className="btn btn-outline-secondary" type="button" onClick={this.handleClickResetFile}>Сброс</button>
                    </div>
                </div>
                {filesToUpload.length === 0 ? <></> : <ul>{filesToUpload.map(function (element, index) { return <li key={index} title={element.type}>{element.name} <b>{App.SizeDataAsString(element.size)}</b></li> })}</ul>}
            </form>
            : <></>;

        if (isTilesModeView === true) {
            return (<>
                {uploadNewFileButton}
                {this.getFilesAsTiles(files)}
            </>);
        }
        else {
            return (<>
                {uploadNewFileButton}
                {this.getFilesAsTable(files)}
            </>);
        }
    }

    /**
     * Рендеринг списка файлов в виде таблицы
     * @param {[{id,name}]} files - перечень файлов для рендеринга
     */
    getFilesAsTiles(files) {
        const spanStyle = {
            float: 'left',
            margin: '5px',
            padding: '5px'
        };
        const apiPostfix = this.apiPostfix;
        const isFtpFileContext = this.isFtpFileContext === true;
        const getBaseButtons = this.getBaseButtons;
        return (<>
            <div className="card">
                <div className="card-body">
                    {files.map(function (file, index) {
                        const fileExtension = App.getFileExtension(file.name);
                        const isImageFile = App.fileNameIsImage(fileExtension);
                        const aboutFile = `name:${file.name}; size:${file.size};`;

                        const domObject = isImageFile === true
                            ? <>
                                <img src={`/${App.controller}/src${apiPostfix.substring(1)}?thumb=true&id=${isFtpFileContext === true ? file.name : file.id + App.getFileExtension(file.name)}`} alt={aboutFile} className="in-turns-loading-images" />
                                {getBaseButtons(file.id, file.name)}
                            </>
                            : <><span className="badge badge-info">{fileExtension}</span> {getBaseButtons(file.id, file.name)}</>

                        return <span className='border' style={spanStyle} title={aboutFile} key={index}>
                            {domObject}
                        </span>
                    })}
                </div>
            </div >
            {this.servicePaginator.render()}
        </>
        );
    }

    /**
     * Рендеринг списка файлов в виде плиток
     * @param {[{id,name}]} files - перечень файлов для рендеринга
     */
    getFilesAsTable(files) {
        const getBaseButtons = this.getBaseButtons;
        return (
            <>
                <table className='table table-striped table-sm mt-4'>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Size</th>
                        </tr>
                    </thead>
                    <tbody>
                        {files.map(function (file, index) {
                            return <tr key={index}>
                                <td>{getBaseButtons(file.id, file.name, true)}</td>
                                <td>{file.size}</td>
                            </tr>
                        })}
                    </tbody>
                </table>
                <PaginatorComponent servicePaginator={this.servicePaginator} />
            </>
        );
    }

    /**
     * Рендеринг индивидульного набора кнопок управления для управления файлом: выгрузка/загрузка, просмотр карточки файла и форма запроса удаления для файла
     * @param {string} id - идентификатор файла на сервере. Значение имеет смысл только для файлов из "хранилища/storage" - "идентификатор/ключ" для доступа к записи в БД.
     * Для получения "связанного/реального" имни файла в папке "защищённого хранилища storage", к идентификатору нужно добавить расширение файла из имени.
     * Для файлов "начальной/uploads" папки этот параметр undefended
     * @param {string} name - имя файла. В случае с доступом к "начальной/uploads" папке имя файла совпадает с реальным именем в папке на диске.
     * @param {boolean} inLine - признак использования компановки "InLine"
     */
    getBaseButtons(id, name, inLine = false) {
        const isFtpFileContext = this.isFtpFileContext === true;
        const apiName = App.controller;
        const fileExtension = App.getFileExtension(name);
        const trueFileName = isFtpFileContext === true ? '' : name;
        if (inLine === true) {
            return <p className='mb-0'><NavLink className='badge badge-light mr-2' role='button' to={`/${apiName}/${App.viewNameMethod}/${isFtpFileContext === true ? name : id + fileExtension}`} title='просмотр файла'>{name}</NavLink>
                    <NavLink className='badge badge-dark mr-1' role='button' to={`/${apiName}/${App.deleteNameMethod}/${isFtpFileContext === true ? name : id + fileExtension}`} title='удаление файла'>del</NavLink>
                <button id={`file-id-${isFtpFileContext === true ? name : id + fileExtension}`} onClick={this.handleClickTransferFile} title={isFtpFileContext === true ? 'сохранить файл в хранилище' : 'выгрузить файл из хранилища'} className={`badge badge-${isFtpFileContext === true ? 'info' : 'primary'}`}>{isFtpFileContext === true ? 'save' : 'unload'}</button>
                {trueFileName}
                </p>;
        }
        else {
            return <>
                <p className='mb-0'><NavLink className='badge badge-light mr-2' role='button' to={`/${apiName}/${App.viewNameMethod}/${isFtpFileContext === true ? name : id + fileExtension}`} title='просмотр файла'>{name}</NavLink></p>
                <NavLink className='badge badge-dark mr-1' role='button' to={`/${apiName}/${App.deleteNameMethod}/${isFtpFileContext === true ? name : id + fileExtension}`} title='удаление файла'>del</NavLink>
                <button id={`file-id-${isFtpFileContext === true ? name : id + fileExtension}`} onClick={this.handleClickTransferFile} title={isFtpFileContext === true ? 'сохранить файл в хранилище' : 'выгрузить файл из хранилища'} className={`badge badge-${isFtpFileContext === true ? 'info' : 'primary'}`}>{isFtpFileContext === true ? 'save' : 'unload'}</button>
                {trueFileName}
            </>;
        }
    }
}