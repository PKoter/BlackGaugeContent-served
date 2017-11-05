import { Inject } from '@angular/core';
import { Http, Response, RequestOptions, Headers } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';

export class RequestHandler {

	protected http: Http;
	protected baseUrl: string;

	constructor(http: Http, baseUrl: string) {
		this.http = http;
		this.baseUrl = baseUrl;
	}

	public get<T>(route: string): Observable<T> {
		return this.http.get(this.baseUrl + route).map((result: Response) => result.json() as T);
	}

	public getAny(route: string): Observable<any> {
		return this.http.get(this.baseUrl + route).map((result: Response) => result.json());
	}

	public post<T>(route: string, data: T): Observable<Response> {
		let body    = JSON.stringify(data);
		let headers = new Headers({ 'Content-Type': 'application/json' });
		let options = new RequestOptions({ headers: headers });
		return this.http.post(this.baseUrl + route, body, options);
	}
}