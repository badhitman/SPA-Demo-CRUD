////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
import React from 'react';
import { viewUser } from './viewUser';

/** Создание нового объекта/пользователя */
export class createUser extends viewUser {
    static displayName = createUser.name;

    async load() {
        this.setState({ cardTitle: 'Создание нового пользователя', loading: false, cardContents: this.body() });
    }
    body() {
        return (
            <>
                <form key='create-form'>
                    <div className="form-group">
                        <label htmlFor="user-input">Имя/ФИО</label>
                        <input name='name' type="text" className="form-control" id="user-input" placeholder="Имя нового пользователя" />
                    </div>
                    {this.createButtons()}
                </form>
            </>
        );
    }
}