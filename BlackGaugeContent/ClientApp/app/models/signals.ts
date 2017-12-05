export interface IUserImpulses {
	impulses: IImpulseState;
}

export interface IImpulseState {
	notifyCount: number;
	requestsAgreed: ComradeRequest[];
	requestsReceived: ComradeRequest[];
}

export class ComradeRequest {
	constructor(
		public senderId: number,
		public otherName: string,
		public since: Date,
		public agreed: boolean
	) {}
}