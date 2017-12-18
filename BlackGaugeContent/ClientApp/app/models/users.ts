export interface IUserInfo {
	userName:    string;
	genderName:  string;
	motto:       string;
	respek:      number;
	isComrade:          boolean;
	requestReceived:    boolean;
	comradeRequestSent: boolean;
}

export class ComradeRequest {
	public id:       number |null;
	public timeSpan: string |null;
	public agreed:   boolean|null;
	public received: boolean|null;
	public seen:     boolean|null;

	constructor(
		public senderId:  number,
		public otherName: string
	) { }
}

export interface IComradeRelations {
	comrades: IComradeEntry [];
	received: ComradeRequest[];
	sent:     ComradeRequest[];
}

export interface IComradeEntry {
	name: string;
	interactions: number;
}

export class SeenComradeRequest {
	constructor(
		public id:   number,
		public seen: boolean
	) { }
}

export class ComradeRequestFeedback {
	constructor(
		public id: number,
		public receiverId: number
	) { }
}