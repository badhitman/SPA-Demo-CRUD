////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React from 'react';
import { viewTelegramBotUpdate } from './viewTelegramBotUpdate';
import App from '../../../App';

/** Удаление TelegramBot Update */
export class deleteTelegramBotUpdate extends viewTelegramBotUpdate {
    static displayName = deleteTelegramBotUpdate.name;

    async load() {
        await this.ajax();
        this.cardTitle = `Удаление TelegramBot Update: [#${App.data.id}] ${App.data.name}`;
        this.setState({ loading: false });
    }
    cardBody() {
        var telegramBotUpdate = App.data;

        return (
            <>
                <div className="alert alert-danger" role="alert">Безвозвратное удаление TelegramBot Update! Данное дейтсвие нельзя будет отменить!</div>
                <form className="mb-3" key='delete-form'>
                    {this.mapObjectToReadonlyForm(telegramBotUpdate)}
                    {this.deleteButtons()}
                </form>
            </>
        );
    }
}