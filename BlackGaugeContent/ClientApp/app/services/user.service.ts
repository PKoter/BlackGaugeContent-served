import { Inject, Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { GenderModel } from '../models/account';
import { RequestHandler } from '../components/requestHandler';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';

@Injectable()
export class UserService extends RequestHandler {

	constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
		super(http, baseUrl);
	}

	public getGenders(): Observable<GenderModel[]> {
		return this.get<GenderModel[]>('api/User/GetGenders');
	}
}