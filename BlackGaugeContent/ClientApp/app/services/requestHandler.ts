import { Http, Response, RequestOptions, Headers } from '@angular/http';
import { AuthGuard } from '../auth/auth.guard';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';


export class RequestHandler {

	protected http: Http;
	protected baseUrl: string;

	constructor(http: Http, baseUrl: string) {
		this.http = http;
		this.baseUrl = baseUrl;
	}

	/**
	 * performs http get from given route and maps result to T type.
	 * @param route
	 */
	public get<T>(route: string, options?: RequestOptions): Observable<T> {
		return this.http.get(this.baseUrl + route, options).map((result: Response) => result.json() as T);
	}

	/**
	 * performs get and maps result to json.
	 * @param route
	 */
	public getAny(route: string): Observable<any> {
		return this.http.get(this.baseUrl + route).map((result: Response) => result.json());
	}

	public post<T>(route: string, data: T): Observable<Response> {
		let body    = JSON.stringify(data);
		let headers = new Headers({ 'Content-Type': 'application/json' });
		let options = new RequestOptions({ headers: headers });
		return this.http.post(this.baseUrl + route, body, options);
	}

	/**
	 * performs post with no authentication and maps result to R type.
	 * @param route
	 * @param data
	 */
	public postClassic<T, R>(route: string, data: T): Observable<R> {
		let body = JSON.stringify(data);
		let headers = new Headers({ 'Content-Type': 'application/json' });
		let options = new RequestOptions({ headers: headers });
		return this.http.post(this.baseUrl + route, body, options)
			.map((response: Response) => response.json() as R);
	}
	/*
	public postEmpty(route: string): Observable<Response> {
		let body = JSON.stringify({});
		let headers = new Headers({ 'Content-Type': 'application/json' });
		let options = new RequestOptions({ headers: headers });
		return this.http.post(this.baseUrl + route, body, options);
	}*/
}

export class AuthRequestHandler extends RequestHandler {

	protected auth: AuthGuard;

	constructor(http: Http, baseUrl: string, auth: AuthGuard) {
		super(http, baseUrl);
		this.auth = auth;
	}

	/**
	 * performs post at given route with headers
	 * @param route
	 * @param options special auth headers?
	 */
	protected postWithHeaders<T, R>(route: string, data: T, options: RequestOptions): Observable<R>
	{
		let body = JSON.stringify(data);
		return this.http.post(this.baseUrl + route, body, options)
			.map((response: Response) => response.json() as R);
	}

	/**
	 * performs empty post at given route with headers
	 * @param route
	 * @param options special auth headers?
	 */ /*
	protected postEmptyWithHeaders(route: string, options: RequestOptions): Observable<Response> {
		let body = JSON.stringify({});
		return this.http.post(this.baseUrl + route, body, options);
	}*/

	/**
	 * performs post with authentication and maps result to R type.
	 * @param route
	 * @param data
	 */
	protected authPost<T, R>(route: string, data: T): Observable<R> {
		let body = JSON.stringify(data);
		let options = this.auth.authPostHeaders();
		return this.http.post(this.baseUrl + route, body, options)
			.map((response: Response) => response.json() as R);
	}

	protected authGet<R>(route: string): Observable<R> {
		let options = this.auth.authGetHeaders();
		return this.http.get(this.baseUrl + route, options)
			.map((response: Response) => response.json() as R);
	}
	/*
	protected authPostEmpty(route: string): Observable<Response> {
		return this.postEmptyWithHeaders(route, this.auth.authPostHeaders());
	}*/
}