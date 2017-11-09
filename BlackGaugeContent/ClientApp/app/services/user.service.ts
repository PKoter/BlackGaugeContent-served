import { Inject, Injectable, Output, EventEmitter } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { ApiRoutesService, Routes, ApiRoutes } from './apiRoutes.service';
import { GenderModel } from '../models/account';
import { RequestHandler } from '../components/requestHandler';
import { AuthGuard } from '../auth/auth.guard';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';

@Injectable()
export class UserService extends RequestHandler {

	constructor(http: Http, @Inject('BASE_URL') baseUrl: string, private auth: AuthGuard, private router: ApiRoutesService) {
		super(http, baseUrl);
	}

	@Output() public logged = new EventEmitter();

	public getGenders(): Observable<GenderModel[]> {
		return this.get<GenderModel[]>('api/User/GetGenders');
	}

	public isLoggedIn(): boolean {
		return this.auth.hasActiveToken();
	}

	public logIn(result: any) {
		this.auth.loggedIn(result);
		this.router.redirect(Routes.Home);
		this.logged.emit(true);
	}

	public logOut() {
		this.authorizedPost(ApiRoutes.Logout).subscribe();
		this.auth.logOut();
		this.router.redirect(Routes.Home);
		this.logged.emit(false);
	}

	public getUserIds(): { id: number, name: string } {
		return this.auth.getLoggedUserIds();
	}

	private authorizedGet<T>(route: string): Observable<T> {
		let options = this.auth.authGetHeaders();
		return this.get<T>(route, options);
	}

	private authorizedPost(route: string): Observable<Response> {
		let options = this.auth.authPostHeaders();
		return this.postWithHeaders(route, options);
	}
}