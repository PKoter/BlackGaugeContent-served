import { EventEmitter } from '@angular/core';
import { HubConnection } from '@aspnet/signalr-client';
import { ImpulsesState, IImpulsesState, ImpulseTypes, ChatImpulse } from '../models/signals';
import { Message, ChatData } from '../models/chatData';
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
	// chat data is persistent throughout live of app to reduce server requests and data transfer.
	protected chatData:    ChatData;

	constructor() {
		this.counts = new ImpulsesState(0, 0, 0);
	}

	public getCounts(): IImpulsesState {
		return this.counts;
	}

	public getChatters(): ChatData {
		if (!this.chatData)
			this.chatData = new ChatData([] as ChatImpulse[]);
		return this.chatData;
	}

	public setCounts(counts: IImpulsesState): void {
		let cs = counts as ImpulsesState;
		if (!cs)
			return;
		this.counts = new ImpulsesState(cs.notifyCount, cs.comradeRequestCount, cs.messageCount);
		this.activeCounts.emit(this.counts as IImpulsesState);

		// this should happen only at digest state after login
		if (cs.chatImpulses ) {
			this.chatData = new ChatData(cs.chatImpulses);
			delete cs.chatImpulses;
		}
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
		alert(`broadcast message from ${name}: ${message}`);
	}

	private onComradeRequest(request: ComradeRequest) {
		this.counts.pushComradeRequest();
		this.activeCounts.emit(this.counts);
		this.comradeRequest.emit(request);
	}

	private onMessage(message: Message) {
		// update impulses here comrades component may not be created, and we would lose data.
		if (this.chatData && message.otherName) {
			let msg = this.chatData.getChatter(message.otherName);
			msg.impulses     += 1;
			msg.interactions += 1;
			let messages = msg.messages;
			// either there's no messages or last is from signal, which means there's no gap between loaded from server and from signals.
			if (messages.length === 0 || messages[messages.length - 1].fromSignal || msg.impulses === 1) 
			{
				messages.push(message);
				message.fromSignal = true;
			}
		}

		this.counts.pushMessage();
		this.activeCounts.emit(this.counts);
		this.message.emit(message);
	}
}