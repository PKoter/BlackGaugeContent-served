import { Inject, Injectable, Output, EventEmitter } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { ApiRoutesService, Routes, ApiRoutes } from './apiRoutes.service';
import { GenderModel, AccountFeedback, FeedResult, IUserId, AccountDetails, LoginModel, RegistrationModel } from '../models/account';
import { IUserInfo, ComradeRequest, IComradeRelations, ComradeRequestFeedback, SeenComradeRequest } from '../models/users';
import { AuthRequestHandler } from '../handlers/requestHandler';
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
			.subscribe(() => {
				this.router.redirect(Routes.Home);
				this.logged.emit(true);
			}, errors => console.warn(errors));
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
			ApiRoutes.AccountDetails, callback, userId);
	}

	public saveAccountDetails(details: AccountDetails, callback: (r: AccountFeedback) => void) {
		if (this.isLoggedIn() === false)
			return;
		details.userId = this.getUserIds().id;
		this.fireAuthPost<AccountDetails, AccountFeedback>(
			ApiRoutes.SetAccountDetails, details, callback);
	}

	public findUser(userName: string, callback: (r: IUserInfo) => void) {
		if (this.isLoggedIn() === false)
			return;
		let id = this.getUserIds().id;
		this.fireAuthGet<IUserInfo>(ApiRoutes.GetUserInfo, callback, id, userName);
	}

	public sendComradeRequest(userName: string, callback: (r: {result: FeedResult}) => void) {
		if (this.isLoggedIn() === false)
			return;
		let id = this.getUserIds().id;
		let request = new ComradeRequest(id, userName);
		this.fireAuthPost<ComradeRequest, { result: FeedResult }>
			(ApiRoutes.SendComradeRequest, request, callback);
	}

	public getComradeRelations(callback: (r: IComradeRelations) => void) {
		if (this.isLoggedIn() === false)
			return;
		let id = this.getUserIds().id;
		this.fireAuthGet<IComradeRelations>(ApiRoutes.GetComradeRelations, callback, id);
	}

	public confirmComradeRequest(requestId: number, otherName: string, 
		callback: (r: { result: FeedResult }) => void)
	{
		if (this.isLoggedIn() === false)
			return;
		let id = this.getUserIds().id;
		let request = new ComradeRequestFeedback(requestId, id, otherName);
		this.fireAuthPost<ComradeRequestFeedback, { result: FeedResult }>
			(ApiRoutes.ConfirmComradeRequest, request, callback);
	}

	public seenComradeRequest(requestId: number, callback: (r: { result: FeedResult }) => void) {
		if (this.isLoggedIn() === false)
			return;
		let request = new SeenComradeRequest(requestId, true);
		this.fireAuthPost<SeenComradeRequest, { result: FeedResult }>
			(ApiRoutes.SeenComradeRequest, request, callback);
	}
}