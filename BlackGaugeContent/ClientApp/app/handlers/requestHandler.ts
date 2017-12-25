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
	protected get<T>(route: string, options?: RequestOptions): Observable<T> {
		return this.http.get(this.baseUrl + route, options)
			.map((result: Response) => result.json() as T);
	}

	public fireGet<T>(route: string, callback: (r : T) => void, options?: RequestOptions) {
		this.get<T>(route, options).subscribe(r => callback(r), errors => console.warn(errors));
	}

	protected postAny<T>(route: string, data: T): Observable<Response> {
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
	protected post<T, R>(route: string, data: T): Observable<R> {
		let body    = JSON.stringify(data);
		let headers = new Headers({ 'Content-Type': 'application/json' });
		let options = new RequestOptions({ headers: headers });
		return this.http.post(this.baseUrl + route, body, options)
			.map((response: Response) => response.json() as R);
	}

	public firePost<T, R>(route: string, data: T, callback: (r: R) => void) {
		this.post<T, R>(route, data)
			.subscribe(r => callback(r), errors => console.warn(errors));
	}
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

	protected fireAuthPost<T, R>(route: string, data: T, callback: (r: R) => void,
		...params: (string | number)[])
	{
		for (let i = 0; i < params.length; i++)
			route += `/${params[i]}`;
		this.authPost<T, R>(route, data)
			.subscribe(r => callback(r), errors => console.warn(errors));
	}

	/**
	 * Sends user request.
	 * @param route
	 * @param callback controller callback when method finishes
	 * @param params optional route parameters
	 */
	protected fireAuthGet<T>(route: string, callback: (r: T) => void, ...params: (string|number)[]) {
		for (let i = 0; i < params.length; i++)
			route += `/${params[i]}`;
		this.authGet<T>(route)
			.subscribe(r => callback(r), errors => console.warn(errors));
	}

	public isLoggedIn(): boolean {
		return this.auth.hasActiveToken();
	}
}