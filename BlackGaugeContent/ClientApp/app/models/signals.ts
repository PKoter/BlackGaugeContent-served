import { ComradeRequest } from './users';

export class ImpulseTypes {
	public static readonly Broadcast = 'notifyAll';
	public static readonly ComradeRequest = 'comradeRequest';
	public static readonly Message = 'message';

}

export interface IUserImpulses {
	impulses: IImpulseState;
}

export interface IImpulseState {
	notifyCount: number;
	agreed:      ComradeRequest[];
	received:    ComradeRequest[];
}

export interface ICountImpulses {
	countAll: number;
	comradeRequestCount: number;

	popComradeRequests(): void;
}

export interface IRuntimeImpulse {
	
}

export interface IComradeRequest {
	requestId: number;
	agreed:    boolean;
}

export class ImpulseCounts implements ICountImpulses {
	public  count:   number;
	private crCount: number;


	public get countAll(): number { return this.count; }

	public get comradeRequestCount(): number { return this.crCount; }

	public popComradeRequests(): void {
		this.crCount--;
		this.count--;
	}

	public pushComradeRequest() {
		this.crCount++;
		this.count++;
	}
}