import { Inject, Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { RequestHandler } from './requestHandler';
import { ApiRoutes } from './apiRoutes.service';
import { UniqueRegisterValue } from '../models/account';

@Injectable()
export class AppMetaService extends RequestHandler {

	constructor(http: Http, @Inject('BASE_URL') url: string) {
		super(http, url);
	}

	public checkUniqueness(value: string, type: string,
	callback: (unique: UniqueRegisterValue) => void) 
	{
		this.fireGet<UniqueRegisterValue>(
			ApiRoutes.CheckUniqueness + `/${value}/${type}`, v => callback(v));
	}

	public getTermsOfService(callback: (terms: string) => void) {
		this.fireGet<any>(ApiRoutes.TermsOfService, r => callback(r.terms));
	}
}