import { Component, NgModule, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { SiteTitleService } from '../../services/title.service';
import { ChatData, Message, Chatter } from '../../models/chatData';
import { MessageService } from '../../services/message.service';
import { ApiRoutes, ApiRoutesService } from '../../services/apiRoutes.service';
import { UserImpulsesService } from '../../services/userImpulses.service';
import { MessagingLogic } from '../../logic/messagingLogic';


@Component({
	selector: 'comrades',
	templateUrl: './messages.html',
	styleUrls: ['./messages.css', '../../controls/bgcButtons.css', '../../controls/bgcViewSections.css', '../../controls/bgcForms.css', '../../controls/bgcGeneral.css', '../../controls/bgcForms.css']
})

export class MessagesComponent implements OnInit {	
	logic:    MessagingLogic;

	loading:  boolean = false;
	msgLoad:  boolean = false;
	title:    string  = "Messages";
	comrades: Chatter[] = [];

	chatters:        ChatData;
	chatterName:     string = "";
	currentMessages: Message[];
	typedMessage:    string;

	constructor(titleService: SiteTitleService, private messageService: MessageService,
		private router: ApiRoutesService, private impulses: UserImpulsesService
	) {
		titleService.setTitle("BGC Messages");
	}

	ngOnInit() {
		this.chatters = this.impulses.getChatters();
		this.logic = new MessagingLogic(this.impulses, this.chatters);

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

	private anySelectedOrMessages(): boolean {
		if (this.chatterName === '')
			return false;
		if (!this.currentMessages || this.currentMessages.length == 0)
			return false;
		return true;
	}

	/**
	 * moved message/impulse handling to impulseHandler
	 * @param message
	 */
	private messageUpdate(message: Message) {
		let senderName = message.otherName;
		if (!senderName)
			return;

		//if (senderName === this.chatterName) 
		this.orderByInteractions();
	}

	private selectComradeToChat(index: number) {
		if (this.loading)
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
			this.loading = false;
			let chatter1      = this.chatters.getChatter(this.chatterName);
			chatter1.messages = r;
			this.currentMessages = r;
		});
	}

	/**
	 * Occurs when user clicks on typing field - they may read all messages and are preparing to answer - better to have this kind of indication that message is read than nothing.
	 */
	readAndReady() {
		if (! this.anySelectedOrMessages())
			return;

		// delete impulses for that chatter
		let chatter = this.chatters.getChatter(this.chatterName);
		if (! this.logic.tryReduceImpulsesHavingLast(chatter)) {
			this.loadNext();
			return;
		}

		this.logic.markLastSeen(this.currentMessages, this.messageService);
	}

	sendMessage() {
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

	loadPrevious() {
		if (!this.anySelectedOrMessages() || this.msgLoad)
			return;

		if (!this.logic.tryLoadPreviousMessages(this.chatterName, this.messageService, 
			(r: any) => {this.msgLoad = false;})
		)
			return;
		this.msgLoad = true;
	}

	loadNext() {
		if (!this.anySelectedOrMessages() || this.msgLoad)
			return;

		if (!this.logic.tryLoadNextMessages(this.chatterName, this.messageService, 
			(r: any) => {this.msgLoad = false;})
		)
			return;
		this.msgLoad = true;
	}
}