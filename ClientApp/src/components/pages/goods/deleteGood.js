////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { viewGood } from './viewGood';
import App from '../../../App';

/** Удаление Номенклатуры */
export class deleteGood extends viewGood {
    static displayName = deleteGood.name;

    async load() {
        await this.ajax();
        this.cardTitle = `Удаление номенклатуры: [#${App.data.id}] ${App.data.name}`;
        this.setState({ loading: false });
    }

    cardBody() {
        var good = App.data;
        return (
            <>
                <div className="alert alert-danger" role="alert">Безвозратное удаление Номенклатуры! Данное дейтсвие нельзя будет отменить!</div>
                <form className="mb-3">
                    {this.mapObjectToReadonlyForm(good, ['id'])}
                    {this.deleteButtons()}
                </form>
            </>
        );
    }
}
