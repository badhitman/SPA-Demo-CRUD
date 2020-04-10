////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { viewTurnover } from './viewTurnover';
import App from '../../../App';

/** Удаление/сторнирование документа доставки */
export class deleteTurnover extends viewTurnover {
    static displayName = deleteTurnover.name;

    async load() {
        await this.ajax();
        this.cardTitle = `Удаление документа отгрузки: [#${App.data.id}] ${App.data.name}`;
        this.setState({ loading: false });
    }

    cardBody() {
        var turnover = App.data;
        return (
            <>
                <div className="alert alert-danger" role="alert">Удалить документ нельзя - будет создан сторнирующий документ! Документ будет заполнен противоположными показателями. Его можно будет отредактировать и сохранить</div>
                <form className="mb-3">
                    {this.mapObjectToReadonlyForm(turnover, ['id'])}
                    {this.deleteButtons()}
                </form>
            </>
        );
    }
}
