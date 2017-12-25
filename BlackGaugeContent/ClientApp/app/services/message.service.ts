import { Inject, Injectable, Output, EventEmitter } from '@angular/core';
import { Http } from '@angular/http';
import { ApiRoutesService, Routes, ApiRoutes } from './apiRoutes.service';
import { Message } from '../models/signals';
import { IComradeEntry } from '../models/users';
import { AuthRequestHandler } from '../handlers/requestHandler';
import { AuthGuard } from '../auth/auth.guard';
import 'rxjs/add/operator/map';

@Injectable()
export class MessageService extends AuthRequestHandler {

	constructor(http: Http, @Inject('BASE_URL') baseUrl: string, auth: AuthGuard,
		private router: ApiRoutesService)
	{
		super(http, baseUrl, auth);
	}

	public getComrades(callback: (r: IComradeEntry[]) => void) {
		if (this.isLoggedIn() === false)
			return;
		let id = this.auth.getLoggedUserIds().id;
		this.fireAuthGet<IComradeEntry[]>(ApiRoutes.GetComrades, callback, id);
	}

	public getLastMessages(otherName: string, callback: (r: Message[]) => void) {
		if (this.isLoggedIn() === false)
			return;
		let id = this.auth.getLoggedUserIds().id;
		this.fireAuthGet<Message[]>(ApiRoutes.GetLastMessages, callback, id, otherName);
	}

	public sendMessage(message: Message, callback: (r: { result: any }) => void) {
		if (this.isLoggedIn() === false)
			return;

		message.userId = this.auth.getLoggedUserIds().id;
		this.fireAuthPost<Message, { result: any }>
			(ApiRoutes.SendMessage, message, callback);
	}

	public readMessage(messageId: number, callback: (r: { result: any }) => void) {
		if (this.isLoggedIn() === false)
			return;

		this.fireAuthPost<{id: number}, { result: any }>
			(ApiRoutes.SeenMessage, {id: messageId}, callback);
	} 
}