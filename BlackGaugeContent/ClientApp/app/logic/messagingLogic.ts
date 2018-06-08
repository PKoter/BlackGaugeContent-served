import { Message, ChatData, Chatter, MessageCollection } from '../models/chatData';
import { UserImpulsesService } from '../services/userImpulses.service';
import { MessageService } from '../services/message.service';

export class MessagingLogic {
	private readonly PageSize = 10;

	private impulses: UserImpulsesService;
	private chatData: ChatData;

	constructor(impulses: UserImpulsesService, chatData: ChatData) {
		this.impulses = impulses;
		this.chatData = chatData;
	}
	
	public static addMessage(chatData: ChatData, message: Message) {
		if (!chatData || !message.otherName)
			return;

		let chatter = chatData.getChatter(message.otherName);
		chatter.impulses     += 1;
		chatter.interactions += 1;
		let messages = chatter.messages;

		// either there's no messages or last is from signal, which means there's no gap between loaded from server and from signals.
		if (messages.length === 0 || messages[messages.length-1].fromSignal || chatter.impulses === 1)
		{
			messages.push(message);
			message.fromSignal = true;
		}
	}

	private reduceMessageNotifies(chatter: Chatter) {
		let by = chatter.impulses;
		chatter.impulses = 0;

		let counts = this.impulses.getCounts();
		counts.popMessages(by);
		if(by > 0)
			this.impulses.setCounts(counts);
	}

	private getChatter(name: string): Chatter {
		return this.chatData.getChatter(name);
	}

	/**
	 * checks for last message, if criteria are met - reduces count and returns true, otherwise next messages need to be loaded.
	 * @param chatter
	 */
	public tryReduceImpulsesHavingLast(chatter: Chatter): boolean {
		if (chatter.impulses === 0)
			return true;
		let msg = chatter.messages.length>0 ? chatter.messages[chatter.messages.length - 1] :null;
		if (!msg)
			return true;
		if (msg.fromSignal) {
			this.reduceMessageNotifies(chatter);
			return true;
		}
		return false;
	}

	public markLastSeen(messages: Message[], messageService: MessageService) {
		let msg = messages[messages.length - 1];
		if (!msg || !msg.fromSignal)
			return;
		
		if (!msg.sent && msg.seen !== true && msg.id) {

			msg.seen = true;
			messageService.readMessage(msg.id, r => {});
		}
	}

	public tryLoadPreviousMessages(chatterName: string, messageService: MessageService, 
	 callback: (c: Chatter) => void) : boolean
	{
		let chatter = this.getChatter(chatterName);
		let first = chatter.messages.length > 0 ? chatter.messages[0] : null;
		if (!first || first.first || !first.id)
			return false;

		messageService.getPreviousMessages(first.id, chatterName, ms => 
		{
			const chatter = this.getChatter(ms.chatterName);
			new MessageCollection(ms).precedeFirst(chatter.messages);

			this.checkChatStartAndMark(ms.messages, chatter);
			callback(chatter);
		});
		return true;
	}

	public tryLoadNextMessages(chatterName: string, messageService: MessageService, 
	 callback: (c: Chatter) => void): boolean 
	{	
		let chatter = this.getChatter(chatterName);
		
		const last = chatter.messages.length>0 ? chatter.messages[chatter.messages.length - 1] :null;
		// last message loaded, ensure impulses are cancelled
		if (last && last.fromSignal) 
		{
			this.reduceMessageNotifies(chatter);
			return false;
		}
		if (!last || !last.id || chatter.impulses === 0)
			return false;

		messageService.getNextMessages(last.id, chatterName, ms => 
		{
			const chatter = this.getChatter(ms.chatterName);
			new MessageCollection(ms).appendTo(chatter.messages);

			this.checkChatEndAndMark(ms.messages, chatter);
			callback(chatter);
		});
		return true;
	}

	private checkChatStartAndMark(msgs: Message[], chatter: Chatter) {
		if (msgs.length < this.PageSize)
			chatter.messages[0].first = true;
	}

	private checkChatEndAndMark(msgs: Message[], chatter: Chatter) {
		// loaded last, mark last then and 
		if (msgs.length >= this.PageSize)
			return;
		msgs = chatter.messages;
		msgs[msgs.length -1].fromSignal = true;
		this.reduceMessageNotifies(chatter);
	}

}