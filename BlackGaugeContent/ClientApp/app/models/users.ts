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
	public since:  Date|null;
	public agreed: boolean|null;

	constructor(
		public senderId:  number,
		public otherName: string
	) { }
}