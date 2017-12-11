import { ComradeRequest } from './users';

export interface IUserImpulses {
	impulses: IImpulseState;
}

export interface IImpulseState {
	notifyCount: number;
	agreed:      ComradeRequest[];
	received:    ComradeRequest[];
}