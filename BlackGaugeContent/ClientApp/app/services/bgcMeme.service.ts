import { Inject, Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { ApiRoutesService, Routes, ApiRoutes } from './apiRoutes.service';
import { AuthRequestHandler } from '../handlers/requestHandler';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import { MemeModel, MemeState, MemeReaction } from '../models/memes';
import { AuthGuard } from '../auth/auth.guard';

@Injectable()
export class BgcMemeService extends AuthRequestHandler {

	constructor(http: Http, @Inject('BASE_URL') baseUrl: string, auth: AuthGuard,
		private router: ApiRoutesService)
	{
		super(http, baseUrl, auth);
	}

	public setUserMemeReaction(reaction: MemeReaction, callback: (r: MemeState) => void)
	{
		this.fireAuthPost<MemeReaction, MemeState>(ApiRoutes.MemeReaction, reaction, callback);
	}

	public getMemePage(pageIndex: number, callback: (r: MemeModel[]) => void)
	{
		let userId = this.auth.getLoggedUserIds().id;
		this.fireGet<MemeModel[]>(ApiRoutes.PageMemes + `/${pageIndex}/${userId}`, callback);
	}

	public getNewMemeCount(pageIndex: number, memes: MemeModel[], callback: (r: number) => void)
	{
		let firstMemeId = memes[0].core.id;
		this.fireGet<ItemCount>(ApiRoutes.CountNewMemes + `/${pageIndex}/${firstMemeId}`, 
			r => callback(r.count));
	}
}

interface ItemCount {
	count: number;
}