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
	comradeRequestSum: number;

	popComradeRequest(): void;
}

export interface IComradeRequest {
	requestId: number;
	agreed:    boolean;
}

export class ImpulsesState implements IImpulsesState {
	public notifyCount:         number = 0;
	public comradeRequestCount: number = 0;

	constructor(notifyCount: number, crCount: number) {
		this.notifyCount = notifyCount;
		this.comradeRequestCount = crCount;
	}

	public get notifySum(): number { return this.notifyCount; }

	public get comradeRequestSum(): number { return this.comradeRequestCount; }

	public popComradeRequest(): void {
		this.comradeRequestCount--;
		this.notifyCount--;
	}

	public pushComradeRequest() {
		this.comradeRequestCount++;
		this.notifyCount++;
	}
}