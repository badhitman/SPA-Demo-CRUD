////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import React, { Component } from 'react';
import App from '../../App';
import jQuery from 'jquery';

export class SignIn extends Component {
    static displayName = SignIn.name;
    registrationFormName = 'registration';
    authorisationFormName = 'authorization';
    logoutFormName = 'logout';

    constructor(props) {
        super(props);

        this.emailLoginInputRef = React.createRef();
        this.passwordLoginInputRef = React.createRef();

        this.handleClickButton = this.handleClickButton.bind(this);
        this.handleDoubleClickButton = this.handleDoubleClickButton.bind(this);
    }

    handleDoubleClickButton(e) {
        this.emailLoginInputRef.current.value = e.target.innerText;
        this.passwordLoginInputRef.current.value = 'demo';
    }

    async handleClickButton(e) {
        var form = e.target.form;

        var sendedFormData = jQuery(form).serializeArray().reduce(function (obj, item) {
            obj[item.name] = item.value;
            return obj;
        }, {});

        sendedFormData.g_recaptcha_response = jQuery(`#recaptchaWıdget${form.name} textarea`).first().val();

        try {
            var fetchInitOptions = {};

            if (form.name === this.logoutFormName) {
                fetchInitOptions.method = 'DELETE';
            }
            else {
                fetchInitOptions.method = form.name === this.registrationFormName ? 'POST' : 'PUT';
                fetchInitOptions.body = JSON.stringify(sendedFormData);
                fetchInitOptions.headers = { 'Content-Type': 'application/json; charset=utf-8' };
            }

            var response = await fetch(`/api/${this.authorisationFormName}/`, fetchInitOptions);
            App.readSession();

            if (form.name === this.logoutFormName) {
                this.props.history.push('/');
                return;
            }

            var result = await response.json();
            var domElement;
            if (response.ok) {
                domElement = jQuery(`<div class="mt-2 alert alert-${result.status}" role="alert">${result.info}</div>`);

                if (result.success === true) {
                    const history = this.props.history;
                    jQuery(form).after(domElement.hide().fadeIn(1000, 'swing', function () { domElement.fadeOut(100, function () { history.push('/'); }); }));
                }
                else {
                    jQuery(form).after(domElement.hide().fadeIn(1000, 'swing', function () { domElement.fadeOut(10000); }));
                }
            }
            else {
                var errorsString = App.mapObjectToArr(result.errors).join('<br/>');
                domElement = jQuery(`<div class="mt-2 alert alert-danger" role="alert"><h4 class="alert-heading">${result.title}</h4><p>${errorsString}</p><hr/><p>traceId: ${result.traceId}</p></div>`);
                jQuery(form).after(domElement.hide().fadeIn(1000, 'swing', function () { domElement.fadeOut(10000); }));
            }

        } catch (error) {
            const msg = `Ошибка: ${error}`;
            console.error(msg);
            alert(msg);
        }
    }

    onloadCallback() {
        if (App.session.AllowedWebLogin === false && App.session.AllowedWebRegistration === false) {
            return;
        }

        var PublicKey = this.getRecaptchaPublicKey();
        if (App.session.isAuthenticated !== true && PublicKey !== null && PublicKey.length > 0) {
            if (jQuery.find(`#recaptchaWıdget${this.authorisationFormName}`).length) {
                grecaptcha.render(`recaptchaWıdget${this.authorisationFormName}`);
            }
            //
            if (jQuery.find(`#recaptchaWıdget${this.registrationFormName}`).length) {
                grecaptcha.render(`recaptchaWıdget${this.registrationFormName}`);
            }
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

        var script = document.createElement('script');
        script.src = "https://www.google.com/recaptcha/api.js?onload=onloadCallback";
        document.head.appendChild(script);

        const spanStyle = {
            cursor: 'pointer'
        };
        return (
            <div className="row">
                {App.session.isDemo === true
                    ?
                    <div className='alert alert-info mr-3 ml-3' role='alert'>
                        <h5 className="alert-heading">Demo доступ</h5>
                        <p>Для демо доступа используйте один из нижеперечисленных имён (пароль у всех: demo). Имена в данном случае соотсветсвуют присвоеным им ролям</p>
                        <div title='двойной клик для заполнения формы входа'>
                            <span className="badge badge-light" style={spanStyle} onDoubleClick={this.handleDoubleClickButton}>auth</span>&nbsp;
                            <span className="badge badge-light" style={spanStyle} onDoubleClick={this.handleDoubleClickButton}>verified</span>&nbsp;
                            <span className="badge badge-light" style={spanStyle} onDoubleClick={this.handleDoubleClickButton}>privileged</span>&nbsp;
                            <span className="badge badge-light" style={spanStyle} onDoubleClick={this.handleDoubleClickButton}>manager</span>&nbsp;
                            <span className="badge badge-light" style={spanStyle} onDoubleClick={this.handleDoubleClickButton}>admin</span>&nbsp;
                            <span className="badge badge-light" style={spanStyle} onDoubleClick={this.handleDoubleClickButton}>root</span>
                        </div>
                    </div>
                    : <></>}
                <div className="col-sm-5 mb-3">
                    <div className="card">
                        <div className="card-body">
                            <fieldset disabled={App.session.AllowedWebLogin !== true}>
                                <h5 className="card-title">Вход</h5>
                                <p className="card-text">Вход в существующий акаунт</p>
                                <form name={this.authorisationFormName}>
                                    <div className="form-group">
                                        <label htmlFor="EmailAuth">Email address</label>
                                        <input ref={this.emailLoginInputRef} type="email" className="form-control" name='EmailLogin' id="EmailLogin" aria-describedby="emailAuthHelp" placeholder="Enter email" />
                                        <small id="emailAuthHelp" className="form-text text-muted">Для входа, укажите свой Email</small>
                                    </div>
                                    <div className="form-group">
                                        <label htmlFor="PasswordLogin">Password</label>
                                        <input ref={this.passwordLoginInputRef} type="password" className="form-control" name='PasswordLogin' id="PasswordLogin" placeholder="Password" />
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
                            <fieldset disabled={App.session.AllowedWebRegistration !== true}>
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
                                                <label htmlFor="PublicNameRegister">Username</label>
                                                <input type="text" className="form-control" name='PublicNameRegister' id="PublicNameRegister" aria-describedby="PublicNameRegisterHelp" placeholder="Enter public name" />
                                                <small id="PublicNameRegisterHelp" className="form-text text-muted">Публичное имя</small>
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
        if (App.session.AllowedWebLogin !== true && prefix === this.authorisationFormName) {
            return <div className="alert alert-warning mt-3 mb-1" role="alert">Авторизация при помощи Web формы отключена администратором!</div>;
        }
        if (App.session.AllowedWebRegistration !== true && prefix === this.registrationFormName) {
            return <div className="alert alert-warning mt-3 mb-1" role="alert">Регистрация при помощи Web формы отключена администратором!</div>;
        }

        const PublicKey = this.getRecaptchaPublicKey();
        if (App.session.isAuthenticated !== true && PublicKey !== null && PublicKey.length > 0 &&
            (App.session.AllowedWebLogin === true || App.session.AllowedWebRegistration === true)) {
            return (<div id={`recaptchaWıdget${prefix}`} className="g-recaptcha mt-2" data-sitekey={PublicKey}></div>);
        }

        return <></>;
    }

    /** Получить публичный ключ reCaptcha (null если ни одного ключа не установлено). Приоритет/порядок проверки наличия установленного ключа: 1) Invisible 2) Widget */
    getRecaptchaPublicKey() {
        if (App.session.AllowedWebLogin === false && App.session.AllowedWebRegistration === false) {
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
}