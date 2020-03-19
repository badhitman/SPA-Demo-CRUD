////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
import React, { Component } from 'react';
import App from '../../App';
import jQuery from 'jquery';

export class SignIn extends Component {
    static displayName = SignIn.name;
    registrationFormName = 'reg';
    authorisationFormName = 'auth';
    logoutFormName = 'logout';

    constructor(props) {
        super(props);

        this.handleClickButton = this.handleClickButton.bind(this);
    }

    async handleClickButton(e) {
        var form = e.target.form;

        var sendedFormData = jQuery(form).serializeArray().reduce(function (obj, item) {
            obj[item.name] = item.value;

            return obj;
        }, {});

        var tmp = jQuery(`#recaptchaWıdget${form.name} textarea`).first().val();
        sendedFormData.g_recaptcha_response = tmp;

        try {
            var fetchInitOptions = {};
            switch (form.name) {
                case this.registrationFormName:
                    fetchInitOptions.method = 'POST';
                    fetchInitOptions.body = JSON.stringify(sendedFormData);
                    fetchInitOptions.headers = { 'Content-Type': 'application/json; charset=utf-8' };
                    break;
                case this.authorisationFormName:
                    fetchInitOptions.method = 'PUT';
                    fetchInitOptions.body = JSON.stringify(sendedFormData);
                    fetchInitOptions.headers = { 'Content-Type': 'application/json; charset=utf-8' };
                    break;
                case this.logoutFormName:
                    fetchInitOptions.method = 'DELETE';
                    break;
                default:
                    fetchInitOptions.method = 'GET';
                    break;
            }

            var result = await fetch('/api/session/', fetchInitOptions);

            if (result.ok) {
                const response = await fetch('/api/session/', { credentials: 'include' });
                const session = await response.json();
                App.session = session;
                this.props.history.push('/');
            }
            else {
                const msg = `Ошибка обработки HTTP запроса. Status: ${result.status}`;
                console.error(msg);
                alert(msg);
            }

        } catch (error) {
            const msg = `Ошибка: ${error}`;
            console.error(msg);
            alert(msg);
        }
    }

    render() {
        if (App.session.isAuthenticated === true) {
            return (
                <form name={this.logoutFormName}>
                    <p>{`Приветсвую Вас, ${App.session.name}! Вы авторизованы в роли [${App.session.role}].`}</p>
                    <button type="button" className="btn btn-outline-danger btn-block" onClick={this.handleClickButton}>Для выхода нажмите на эту кнопку</button>
                </form>
            );
        }
        return (
            <div className="row">
                <div className="col-sm-5 mb-3">
                    <div className="card">
                        <div className="card-body">
                            <fieldset disabled={App.session.allowedWebLogin !== true}>
                                <h5 className="card-title">Вход</h5>
                                <p className="card-text">Вход в существующий акаунт</p>
                                <form name={this.authorisationFormName}>
                                    <div className="form-group">
                                        <label htmlFor="EmailAuth">Email address</label>
                                        <input type="email" className="form-control" name='EmailLogin' id="EmailAuth" aria-describedby="emailAuthHelp" placeholder="Enter email" />
                                        <small id="emailAuthHelp" className="form-text text-muted">Для входа, укажите свой Email</small>
                                    </div>
                                    <div className="form-group">
                                        <label htmlFor="exampleInputPassword1">Password</label>
                                        <input type="password" className="form-control" name='PasswordLogin' id="exampleInputPassword1" placeholder="Password" />
                                    </div>
                                    <button type="button" className="btn btn-primary" onClick={this.handleClickButton}>Вход</button>
                                </form>
                                {this.getRecaptchaDiv(this.authorisationFormName)}
                            </fieldset>
                        </div>
                    </div>
                </div>
                <div className="col-sm-7">
                    <div className="card">
                        <div className="card-body">
                            <fieldset disabled={App.session.allowedWebRegistration !== true}>
                                <h5 className="card-title">Регистрация</h5>
                                <p className="card-text">Регистрация нового акаунта</p>
                                <form name={this.registrationFormName}>
                                    <div className='form-row'>
                                        <div className='col'>
                                            <div className="form-group">
                                                <label htmlFor="EmailRegister">Email address</label>
                                                <input type="email" className="form-control" name='EmailRegister' id="EmailRegister" aria-describedby="emailRegisterHelp" placeholder="Enter email" />
                                                <small id="emailRegisterHelp" className="form-text text-muted">Для регистрации, укажите Email</small>
                                            </div>
                                        </div>
                                        <div className='col'>
                                            <div className="form-group">
                                                <label htmlFor="UsernameRegister">Username</label>
                                                <input type="text" className="form-control" name='UsernameRegister' id="UsernameRegister" aria-describedby="UsernameRegisterHelp" placeholder="Enter public name" />
                                                <small id="UsernameRegisterHelp" className="form-text text-muted">Публичное имя</small>
                                            </div>
                                        </div>
                                    </div>
                                    <div className='form-row'>
                                        <div className='col'>
                                            <div className="form-group">
                                                <label htmlFor="PasswordRegister">Пароль</label>
                                                <input type="password" className="form-control" name='PasswordRegister' id="PasswordRegister" placeholder="Password" />
                                            </div>
                                        </div>
                                        <div className='col'>
                                            <div className="form-group">
                                                <label htmlFor="ConfirmPasswordRegister">Повтор пароля</label>
                                                <input type="password" className="form-control" name='ConfirmPasswordRegister' id="ConfirmPassword" placeholder="Confirm password" />
                                            </div>
                                        </div>
                                    </div>
                                    <button type="button" className="btn btn-primary" onClick={this.handleClickButton}>Регистрация</button>
                                </form>
                                {this.getRecaptchaDiv(this.registrationFormName)}
                            </fieldset>
                        </div>
                    </div>
                </div>
            </div>
        );
    }

    /**
     * Получить блок формы reCaptcha
     * @param {any} prefix - префикс: регистрация или авторизация
     */
    getRecaptchaDiv(prefix) {
        if (App.session.allowedWebLogin !== true && prefix === this.authorisationFormName) {
            return <div className="alert alert-warning mt-3 mb-1" role="alert">Авторизация при помощи Web формы отключена администратором!</div>;
        }
        if (App.session.allowedWebRegistration !== true && prefix === this.registrationFormName) {
            return <div className="alert alert-warning mt-3 mb-1" role="alert">Регистрация при помощи Web формы отключена администратором!</div>;
        }

        const PublicKey = this.getRecaptchaPublicKey();

        if (App.session.isAuthenticated === false && PublicKey !== null && PublicKey.length > 0 &&
            (App.session.allowedWebLogin === true || App.session.allowedWebRegistration === true)) {
            return (<div id={`recaptchaWıdget${prefix}`} className="g-recaptcha mt-2" data-sitekey={PublicKey}></div>);
        }

        return <></>;
    }

    /** Получить публичный ключ reCaptcha (null если ни одного ключа не установлено). Приоритет/порядок проверки наличия установленного ключа: 1) Invisible 2) Widget */
    getRecaptchaPublicKey() {
        if (App.session.allowedWebLogin === false && App.session.allowedWebRegistration === false) {
            return null;
        }

        // firs priority try Invisible version reCaptcha
        var PublicKey = App.session.reCaptchaV2InvisiblePublicKey;
        if (PublicKey && PublicKey.length > 0) {
            return PublicKey;
        }

        // second try Widget version reCaptcha
        PublicKey = App.session.reCaptchaV2PublicKey;
        if (PublicKey && PublicKey.length > 0) {
            return PublicKey;
        }

        return null;
    }

    componentDidMount() {
        if (App.session.allowedWebLogin === false && App.session.allowedWebRegistration === false) {
            return;
        }

        var PublicKey = this.getRecaptchaPublicKey();
        if (App.session.isAuthenticated === false && PublicKey !== null && PublicKey.length > 0) {
            if (jQuery.find(`#recaptchaWıdget${this.authorisationFormName}`).length) {
                grecaptcha.render(`recaptchaWıdget${this.authorisationFormName}`);
            }
            //
            if (jQuery.find(`#recaptchaWıdget${this.registrationFormName}`).length) {
                grecaptcha.render(`recaptchaWıdget${this.registrationFormName}`);
            }
        }
    }
}