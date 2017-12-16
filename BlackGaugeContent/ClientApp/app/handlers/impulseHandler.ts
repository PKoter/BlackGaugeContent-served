import { Output, EventEmitter } from '@angular/core';
import { HubConnection } from '@aspnet/signalr-client';
import { IComradeRequest, ImpulseTypes } from '../models/signals';

export interface IImpulseHandler {
	comradeRequest: EventEmitter<IComradeRequest>;
	broadcast:      EventEmitter<string>;
}

export class ImpulseHandler implements IImpulseHandler {
	public comradeRequest: EventEmitter<IComradeRequest> = new EventEmitter();
	public broadcast:      EventEmitter<string>          = new EventEmitter();
	protected hub:         HubConnection;

	/**
	 * Sets hub and subscribes to handled events.
	 * @param hub 
	 * @returns {} 
	 */
	protected set subscribe(hub: HubConnection) {
		this.hub = hub;

		hub.on(ImpulseTypes.Broadcast, (name: string, message: string) => {
			console.log(`broadcast message from ${name}: ${message}`);
		});
		hub.on(ImpulseTypes.ComradeRequest, (arg: IComradeRequest) => {
			console.log('comrade requests updated');
			this.comradeRequest.emit(arg);
		});
	}
}