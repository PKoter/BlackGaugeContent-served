import { Component, NgModule, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { IComradeEntry } from '../../models/users';
import { ChatData, Message, Chatter } from '../../models/chatData';
import { MessageService } from '../../services/message.service';
import { ApiRoutes, ApiRoutesService } from '../../services/apiRoutes.service';
import { UserImpulsesService } from '../../services/userImpulses.service';


@Component({
	selector: 'comrades',
	templateUrl: './messages.html',
	styleUrls: ['./messages.css', '../../controls/bgcButtons.css', '../../controls/bgcViewSections.css', '../../controls/bgcForms.css', '../../controls/bgcGeneral.css', '../../controls/bgcForms.css']
})

export class MessagesComponent implements OnInit {	
	private readonly pageSize = 10;

	private loading:  boolean = false;
	private msgLoad:  boolean = false;
	private title:    string  = "Messages";
	private comrades: Chatter[] = [];

	private chatters:        ChatData;
	private chatterName:     string = "";
	private currentMessages: Message[];
	private typedMessage:    string;

	constructor(titleService: Title, private messageService: MessageService,
		private router: ApiRoutesService, private impulses: UserImpulsesService
	) {
		titleService.setTitle("BGC Messages");
	}

	ngOnInit() {
		this.chatters = this.impulses.getChatters();

		// get comrades
		if (this.chatters.comradesLoaded() !== true) {
			this.loading = true;

			this.messageService.getComrades(r => {
				this.chatters.zipComrades(r);
				this.comrades = this.chatters.list;
				this.loading  = false;
			});
		}
		this.impulses.message.subscribe(this.messageUpdate.bind(this));
	}

	/**
	 * moved message/impulse handling to impulseHandler
	 * @param message
	 */
	private messageUpdate(message: Message) {
		let senderName = message.otherName;
		if (!senderName)
			return;

		if (senderName === this.chatterName) 
			this.orderByInteractions();
	}

	private selectComradeToChat(index: number) {
		if (this.msgLoad || this.loading)
			return;

		let chatter = this.chatters.list[index];
		this.chatterName = chatter.comrade;
		this.title       = this.chatterName;
		// if there are any messages, they were most likely loaded already.
		if (chatter.messages.length > 0) {
			this.currentMessages = chatter.messages;
			return;
		}

		this.loading = true;
		this.messageService.getLastMessages(this.chatterName, (r : Message[]) => {
			this.loading  = false;
			let data      = this.chatters.getChatter(this.chatterName);
			data.messages = r;
			data.impulses = 0;

			this.currentMessages = r;
		});
	}

	/**
	 * Occurs when user clicks on typing field - they may read all messages and are preparing to answer - better to have this kind of indication that message is read than nothing.
	 */
	private readAndReady() {
		if (this.chatterName === '')
			return;
		if (!this.currentMessages || this.currentMessages.length === 0)
			return;

		// delete impulses for that chatter
		let chatter = this.chatters.getChatter(this.chatterName);
		if (chatter.impulses > 0)
		{
			this.reduceMessageNotifies(chatter.impulses);
			chatter.impulses = 0;
		}

		let msg = this.currentMessages[this.currentMessages.length - 1];
		if (!msg.sent && msg.seen !== true && msg.id) {

			msg.seen = true;
			this.messageService.readMessage(msg.id, r => {});
		}
	}

	private sendMessage() {
		if (!this.typedMessage || this.typedMessage.length === 0)
			return;

		let message       = new Message();
		message.text      = this.typedMessage;
		message.otherName = this.chatterName;
		this.typedMessage = '';

		this.messageService.sendMessage(message, r => {});

		message.sent = true;
		this.currentMessages.push(message);

		let chatter = this.chatters.getChatter(this.chatterName);
		chatter.interactions += 1;
		this.orderByInteractions();
	}

	private orderByInteractions() {
		// sorts comrades descending on interaction count
		this.chatters.list.sort((a, b) => b.interactions - a.interactions);
		this.comrades = this.chatters.list;
	}

	private reduceMessageNotifies(by: number) {
		let counts = this.impulses.getCounts();
		counts.popMessages(by);
		this.impulses.setCounts(counts);
	}

	private loadPrevious() {
		console.log('dupa');
		if (this.currentMessages.length === 0 || this.msgLoad)
			return;

		let first = this.currentMessages[0];
		if (first.first || !first.id)
			return;

		this.msgLoad = true;
		this.messageService.getPreviousMessages(first.id, this.chatterName, ms => {
			this.msgLoad = false;

			let chatter = this.chatters.getChatter(this.chatterName);
			for (let i = ms.length - 1; i >= 0; i--) {
				chatter.messages.unshift(ms[i]);
			}

			this.currentMessages = chatter.messages;
			if (ms.length < this.pageSize)
				this.currentMessages[0].first = true;
		});
	}

	private loadNext() {
		
	}
}