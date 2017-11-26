import { Inject, Injectable, Output, EventEmitter } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { ApiRoutesService, Routes, ApiRoutes } from './apiRoutes.service';
import { GenderModel, AccountFeedback, FeedResult, IUserId, AccountDetails, LoginModel, RegistrationModel } from '../models/account';
import { AuthRequestHandler } from './requestHandler';
import { AuthGuard } from '../auth/auth.guard';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';

@Injectable()
export class UserService extends AuthRequestHandler {

	constructor(http: Http, @Inject('BASE_URL') baseUrl: string, auth: AuthGuard,
		private router: ApiRoutesService)
	{
		super(http, baseUrl, auth);
	}

	@Output() public logged = new EventEmitter();

	public getGenders(callback: (r: GenderModel[])=> void) {
		this.fireGet<GenderModel[]>('api/User/GetGenders', callback);
	}

	public register(model: RegistrationModel, callback: (r: AccountFeedback) => void) {
		this.firePost<RegistrationModel, AccountFeedback>(ApiRoutes.Register, model, callback);
	}

	public isLoggedIn(): boolean {
		return this.auth.hasActiveToken();
	}

	public loginRequest(model: LoginModel, callback: (r: AccountFeedback) => void) {
		this.firePost<LoginModel, any>(ApiRoutes.Login, model, r => {
			callback(r as AccountFeedback);
			if (r.result === FeedResult.success)
				this.logIn(r);
		});
	}

	protected logIn(result: Response) {
		this.auth.loggedIn(result);
		this.authGet(ApiRoutes.EnsureAuth)
			.subscribe(() => {}, errors => console.warn(errors));
		this.router.redirect(Routes.Home);
		this.logged.emit(true);
	}

	public logOut() {
		this.authPost(ApiRoutes.Logout, { })
			.subscribe(() => {
					this.authGet(ApiRoutes.EnsureAuth)
						.subscribe(() => {}, errors => console.warn(errors));
				},
				errors => console.warn(errors)
			);
		this.auth.logOut();
		this.router.redirect(Routes.Home);
		this.logged.emit(false);
	}

	public getUserIds(): IUserId {
		return this.auth.getLoggedUserIds();
	}

	public getAccountDetails(callback: (r: AccountDetails) => void) {
		if (this.isLoggedIn() === false)
			return;
		let userId = this.getUserIds().id;
		this.fireAuthGet<AccountDetails>(
			ApiRoutes.AccountDetails + `/${userId}`, callback);
	}

	public saveAccountDetails(details: AccountDetails, callback: (r: AccountFeedback) => void) {
		if (this.isLoggedIn() === false)
			return;
		details.userId = this.getUserIds().id;
		this.fireAuthPost<AccountDetails, AccountFeedback>(
			ApiRoutes.SetAccountDetails, details, callback);
	}
}