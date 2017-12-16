import { Injectable, Inject } from '@angular/core';
import { Response, RequestOptions, Headers } from '@angular/http';
import { LocalStorage } from '../viewApi/localStorage';
import { IUserId } from '../models/account';

@Injectable()
export class AuthGuard {
	private AuthToken = 'auth_token.bgc77cyc';
	private loginInfo: LoginResult;

	constructor(@Inject(LocalStorage) private storage: any) {
		this.loginInfo = new LoginResult(0);
	}

	public getLoggedUserIds(): IUserId {
		if (this.hasActiveToken() === false)
			return { id: 0, name: '' };

		if (this.loginInfo.userId === 0) {
			
			let token                 = this.storage.getItem(this.AuthToken);
			let name                  = this.storage.getItem(this.AuthToken + 'usN');
			let id                    = this.storage.getItem(this.AuthToken + 'uId');
			this.loginInfo.auth_token = token != null ? token : '';
			this.loginInfo.userName   = name != null ? name : '';
			this.loginInfo.userId     = id != null ? +id : 0;
		}

		return { id: this.loginInfo.userId, name: this.loginInfo.userName };
	}

	public hasActiveToken(): boolean {
		if (this.loginInfo.userId > 0)
			return true;
		return this.storage.getItem(this.AuthToken) != null;

	}

	public loggedIn(result: any) {
		this.loginInfo = result as LoginResult;

		this.storage.setItem(this.AuthToken, this.loginInfo.auth_token);
		this.storage.setItem(this.AuthToken + 'usN', this.loginInfo.userName);
		this.storage.setItem(this.AuthToken + 'uId', this.loginInfo.userId.toString());
	}

	public logOut() {
		this.storage.removeItem(this.AuthToken);
		this.loginInfo = new LoginResult(0);
	}

	public getAuthorization(): { auth: string } {
		if (this.hasActiveToken())
			return { auth: this.loginInfo.auth_token };
		return {} as { auth: string };
	}

	/**
	 * Returns headers for authorized get using JwtBearer.
	 */
	public authGetHeaders(): RequestOptions {
		let headers = new Headers();
		headers.append('Content-Type', 'application/json');

		if (this.hasActiveToken())
			headers.append('Authorization', 'Bearer ' + this.loginInfo.auth_token);

		let options = new RequestOptions({ headers: headers });
		return options;
	}

	/**
	 * Returns headers for authorized post using JwtBearer.
	 */
	public authPostHeaders(): RequestOptions {
		return this.authGetHeaders();
	}
}

class LoginResult {
	public auth_token: string;
	public userName: string;
	public userId: number;
	public expires_in: number;

	constructor(id: number) {
		this.userId = id;
	}
}