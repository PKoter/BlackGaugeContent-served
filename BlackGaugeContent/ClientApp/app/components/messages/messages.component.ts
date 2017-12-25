import { Component, NgModule, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { IComradeEntry } from '../../models/users';
import { IComradeRequest, Message } from '../../models/signals';
import { MessageService } from '../../services/message.service';
import { ApiRoutes, ApiRoutesService } from '../../services/apiRoutes.service';
import { UserImpulsesService } from '../../services/userImpulses.service';


@Component({
	selector: 'comrades',
	templateUrl: './messages.html',
	styleUrls: ['./messages.css', '../../controls/bgcButtons.css', '../../controls/bgcViewSections.css', '../../controls/bgcForms.css', '../../controls/bgcGeneral.css', '../../controls/bgcForms.css']
})

export class MessagesComponent implements OnInit {
	private comrades: IComradeEntry [];
	
	private loading: boolean = true;
	private comrade: string = "";
	private title:   string = "Messages";

	private messages:{ [index: string]: Message[] } = {};
	private currentMessages: Message[];
	private typedMessage:    string;

	constructor(titleService: Title, private messageService: MessageService,
		private router: ApiRoutesService, private impulses: UserImpulsesService
	) {
		titleService.setTitle("BGC Messages");
	}

	ngOnInit() {
		this.loading = true;
		// get comrades
		this.messageService.getComrades(r => {
			this.comrades = r;
			this.loading  = false;
		});
		this.impulses.message.subscribe(this.messageUpdate.bind(this));
	}

	private messageUpdate(message: Message) {
		let senderName = message.otherName;
		if (!senderName)
			return;
		let messages = this.messages[senderName];
		messages.push(message);

		// message is immediately read
		if (this.comrade === senderName) {
			let counts = this.impulses.getCounts();
			counts.popMessage();
			this.impulses.setCounts(counts);
		}
	}

	private selectComradeToChat(index: number) {
		let com      = this.comrades[index];
		this.comrade = com.name;
		this.title   = this.comrade;
		this.loading = true;

		this.messageService.getLastMessages(this.comrade, r => {
			this.loading = false;

			this.messages[this.comrade] = r;
			this.currentMessages        = r;
		});
	}

	private sendMessage() {
		if (!this.typedMessage || this.typedMessage.length === 0 || this.typedMessage.length > 2048)
			return;

		let message       = new Message();
		message.text      = this.typedMessage;
		message.otherName = this.comrade;
		this.typedMessage = '';

		this.messageService.sendMessage(message, r => {});

		message.sent = true;
		this.currentMessages.push(message);
	}
}