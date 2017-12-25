export class ImpulseTypes {
	public static readonly Broadcast      = 'notifyAll';
	public static readonly ComradeRequest = 'comradeRequest';
	public static readonly Message        = 'message';
}

export interface IUserImpulses {
	impulses: IImpulsesState;
}

export interface IImpulsesState {
	notifySum:         number;
	messageSum:        number;
	comradeRequestSum: number;

	popComradeRequest(): void;
	popMessage(): void;
}

export interface IComradeRequest {
	requestId: number;
	agreed:    boolean;
}

export class ImpulsesState implements IImpulsesState {
	public notifyCount:         number = 0;
	public messageCount:        number = 0;
	public comradeRequestCount: number = 0;

	constructor(notifyCount: number, crCount: number, msgCount: number) {
		this.notifyCount         = notifyCount;
		this.messageCount        = msgCount;
		this.comradeRequestCount = crCount;
	}

	public get notifySum(): number { return this.notifyCount; }

	public get messageSum(): number { return this.messageCount; }

	public get comradeRequestSum(): number { return this.comradeRequestCount; }

	public popComradeRequest(): void {
		this.comradeRequestCount--;
		this.notifyCount--;
	}

	public pushComradeRequest() {
		this.comradeRequestCount++;
		this.notifyCount++;
	}

	public popMessage(): void {
		this.messageCount--;
		this.notifyCount--;
	}

	public pushMessage() {
		this.messageCount++;
		this.notifyCount++;
	}
}

export class Message {
	public id:     number |null;
	public userId: number |null;
	public sent:   boolean|null;

	public otherName: string|null;
	public text:      string;
	
}