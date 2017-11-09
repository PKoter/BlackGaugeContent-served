import { Injectable } from '@angular/core';
import { Response, RequestOptions, Headers } from '@angular/http';

@Injectable()
export class AuthGuard {
	private AuthToken = 'auth_token';
	private loginInfo: LoginResult;

	public hasActiveToken(): boolean {
		return localStorage.getItem(this.AuthToken) != null;
	}

	public loggedIn(result: Response) {
		this.loginInfo = result.json() as LoginResult;
		localStorage.setItem(this.AuthToken, this.loginInfo.auth_token);
	}

	public getLoggedUserIds(): { id: number, name: string} {
		return this.loginInfo
			? { id: this.loginInfo.userId, name: this.loginInfo.userName }
			: { id: 0, name: '' };
	}

	public logOut() {
		localStorage.removeItem(this.AuthToken);
		this.loginInfo = new LoginResult();
	}

	/**
	 * Returns headers for authorized get using JwtBearer.
	 */
	public authGetHeaders(): RequestOptions {
		let headers = new Headers();
		headers.append('Content-Type', 'application/json');
		headers.append('Authorization', 'Bearer ' + this.loginInfo.auth_token);
		let options = new RequestOptions({ headers: headers });
		return options;
	}

	/**
	 * Returns headers for authorized post using JwtBearer.
	 */
	public authPostHeaders(): RequestOptions {
		let headers = new Headers();
		headers.append('Content-Type', 'application/json');
		headers.append('Authorization', 'Bearer ' + this.loginInfo.auth_token);
		let options = new RequestOptions({ headers: headers });
		return options;
	}
}

class LoginResult {
	public auth_token: string;
	public userName: string;
	public userId: number;
	public expires_in: number;
}