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

export interface IRuntimeImpulse {
	
}

export interface IComradeRequest {
	requestId: number;
	agreed:    boolean;
}