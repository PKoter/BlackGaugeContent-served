import { Inject, Injectable, Output, EventEmitter } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { ApiRoutesService, Routes, ApiRoutes } from './apiRoutes.service';
import { GenderModel, IUserId } from '../models/account';
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

	public getGenders(): Observable<GenderModel[]> {
		return this.get<GenderModel[]>('api/User/GetGenders');
	}

	public isLoggedIn(): boolean {
		return this.auth.hasActiveToken();
	}

	public logIn(result: Response) {
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
}