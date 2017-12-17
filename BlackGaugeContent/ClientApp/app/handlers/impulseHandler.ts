import { Output, EventEmitter } from '@angular/core';
import { HubConnection } from '@aspnet/signalr-client';
import { IComradeRequest, ImpulseCounts, ICountImpulses, ImpulseTypes } from '../models/signals';

export interface IImpulseHandler {
	comradeRequest: EventEmitter<IComradeRequest>;
	broadcast:      EventEmitter<string>;
	activeCounts:   EventEmitter<ICountImpulses>;
	getCounts(): ICountImpulses;
	/**
	 * sets updated active impulses count and emits its value;
	 * @param counts 
	 * @returns {} 
	 */
	setCounts(counts: ICountImpulses):void;
}

export class ImpulseHandler implements IImpulseHandler {
	public comradeRequest: EventEmitter<IComradeRequest> = new EventEmitter();
	public broadcast:      EventEmitter<string>          = new EventEmitter();
	public activeCounts:   EventEmitter<ICountImpulses>  = new EventEmitter();

	protected hub:         HubConnection;
	protected counts = new ImpulseCounts();

	public getCounts(): ICountImpulses {
		return this.counts;
	}
	public setCounts(counts: ICountImpulses): void {
		this.counts = counts as ImpulseCounts;
		this.activeCounts.emit(this.counts);
	}

	/**
	 * Sets hub and subscribes to handled events.
	 * @param hub 
	 * @returns {} 
	 */
	protected set subscribe(hub: HubConnection) {
		this.hub = hub;

		hub.on(ImpulseTypes.Broadcast, this.onRumor.bind(this));
		hub.on(ImpulseTypes.ComradeRequest, this.onComradeRequest.bind(this));
	}

	private onRumor(name: string, message: string) {
		console.log(`broadcast message from ${name}: ${message}`);
	}

	private onComradeRequest(request: IComradeRequest) {
		console.log('comrade requests updated');
		this.counts.pushComradeRequest();
		this.activeCounts.emit(this.counts);
		this.comradeRequest.emit(request);
	}
}