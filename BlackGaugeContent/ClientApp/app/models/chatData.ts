import { IComradeEntry } from './users';
import { ChatImpulse } from './signals';
import { HashSet } from '../commonTypes.api';

export class ChatData {
	public  list:    Chatter[];
	private hashset: HashSet<Chatter>;

	constructor(impulses: ChatImpulse[]) {
		this.hashset = new HashSet<Chatter>();

		for (var impulse of impulses) {
			let name    = impulse.comrade;
			let chatter = new Chatter(name, impulse.impulses);
			this.hashset.add(name, chatter);
		}
	}

	/**
	 * puts comrades as chatters in comrade list. 
	 * @param comrades
	 */
	public zipComrades(comrades: IComradeEntry[]) {
		if (!comrades)
			return;
		this.list = new Array<Chatter>(comrades.length);
		for (let i = 0; i < comrades.length; i++) {
			let name = comrades[i].name;
			let chatter = this.hashset.item(name);

			if (!chatter) {
				chatter = new Chatter(name, 0);
				this.hashset.add(name, chatter);
			}
			chatter.interactions = comrades[i].interactions;
			this.list[i] = chatter;
		}
	}

	public comradesLoaded(): boolean {
		return this.list && this.list.length > 0;
	}

	/**
	 * Checks, if chatter exists, if not, creates one with name and returns it.
	 * @param name
	 */
	public getChatter(name: string): Chatter {
		let chatter = this.hashset.item(name);
		if (!chatter)
			chatter = new Chatter(name, 0);
		return chatter;
	}
}

export class Chatter {
	public messages: Message[];
	public impulses: number;
	public comrade:  string;
	public interactions: number;
	private onList:  boolean = false;

	constructor(comrade: string, impulses: number) {
		this.comrade      = comrade;
		this.impulses     = impulses;
		this.interactions = 0;
		this.messages     = new Array<Message>();
	}

	zipIfNotIn(array: Chatter[]) {
		if (this.onList)
			return;
		array.push(this);
		this.onList = true;
	}
}

export class Message {
	public id:     number |null;
	public userId: number |null;
	public sent:   boolean|null;
	public seen:   boolean|null;
	public first:  boolean|null;
	public fromSignal: boolean|null;

	public otherName: string|null;
	public text:      string;
	
}