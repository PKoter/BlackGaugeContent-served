import { EventEmitter } from '@angular/core';
import { HubConnection } from '@aspnet/signalr-client';
import { ImpulsesState, IImpulsesState, ImpulseTypes, Message } from '../models/signals';
import { ComradeRequest } from '../models/users';

export interface IImpulseHandler {
	comradeRequest: EventEmitter<ComradeRequest>;
	broadcast:      EventEmitter<string>;
	message:        EventEmitter<Message>;
	activeCounts:   EventEmitter<IImpulsesState>;


	getCounts(): IImpulsesState;
	/**
	 * sets updated active impulses count and emits its value;
	 * @param counts 
	 * @returns {} 
	 */
	setCounts(counts: IImpulsesState):void;
}

export class ImpulseHandler implements IImpulseHandler {
	public comradeRequest: EventEmitter<ComradeRequest> = new EventEmitter();
	public broadcast:      EventEmitter<string>         = new EventEmitter();
	public message:        EventEmitter<Message>        = new EventEmitter();

	public activeCounts:   EventEmitter<IImpulsesState> = new EventEmitter();

	protected hub:         HubConnection;
	protected counts:      ImpulsesState;

	constructor() {
		this.counts = new ImpulsesState(0, 0, 0);
	}

	public getCounts(): IImpulsesState {
		return this.counts;
	}
	public setCounts(counts: IImpulsesState): void {
		let cs = counts as ImpulsesState;
		if (!cs)
			return;
		this.counts = new ImpulsesState(cs.notifyCount, cs.comradeRequestCount, cs.messageCount);
		this.activeCounts.emit(this.counts as IImpulsesState);
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
		hub.on(ImpulseTypes.Message, this.onMessage.bind(this));
	}

	private onRumor(name: string, message: string) {
		console.log(`broadcast message from ${name}: ${message}`);
	}

	private onComradeRequest(request: ComradeRequest) {
		console.log('comrade requests updated');
		this.counts.pushComradeRequest();
		this.activeCounts.emit(this.counts);
		this.comradeRequest.emit(request);
	}

	private onMessage(message: Message) {
		console.log('messages updated');
		this.counts.pushMessage();
		this.activeCounts.emit(this.counts);
		this.message.emit(message);
	}
}