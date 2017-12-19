import { Output, EventEmitter } from '@angular/core';
import { HubConnection } from '@aspnet/signalr-client';
import { IComradeRequest, ImpulsesState, IImpulsesState as IImpulseState, ImpulseTypes } from '../models/signals';

export interface IImpulseHandler {
	comradeRequest: EventEmitter<IComradeRequest>;
	broadcast:      EventEmitter<string>;
	activeCounts:   EventEmitter<IImpulseState>;

	getCounts(): IImpulseState;
	/**
	 * sets updated active impulses count and emits its value;
	 * @param counts 
	 * @returns {} 
	 */
	setCounts(counts: IImpulseState):void;
}

export class ImpulseHandler implements IImpulseHandler {
	public comradeRequest: EventEmitter<IComradeRequest> = new EventEmitter();
	public broadcast:      EventEmitter<string>          = new EventEmitter();
	public activeCounts:   EventEmitter<IImpulseState>   = new EventEmitter();

	protected hub:         HubConnection;
	protected counts:      ImpulsesState;

	public getCounts(): IImpulseState {
		return this.counts;
	}
	public setCounts(counts: IImpulseState): void {
		let cs = counts as ImpulsesState;
		if (!cs)
			return;
		this.counts = new ImpulsesState(cs.notifyCount, cs.comradeRequestCount);
		this.activeCounts.emit(this.counts as IImpulseState);
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