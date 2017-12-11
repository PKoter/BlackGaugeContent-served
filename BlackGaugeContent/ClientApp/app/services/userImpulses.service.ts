import { Injectable, Output, EventEmitter } from '@angular/core';
import { IImpulseState } from '../models/signals';

@Injectable()
export class UserImpulsesService {
	state: IImpulseState;
	@Output() public impulsed = new EventEmitter();

	public digestState(state: any) {
		let impulses = (state as { impulses: IImpulseState });
		if (impulses) {
			this.state = impulses.impulses;
			this.impulsed.emit();
		}
	}

	public digestImpulses(impulses: IImpulseState) {
		this.state = impulses;
	}

	public getAllImpulseCount(): number {
		if (this.state)
			return this.state.notifyCount;
		return 0;
	}

	public getComradeRequestCount(): number {
		if (this.state)
			return this.state.agreed.length + this.state.received.length;
		return 0;
	}

	public getEachComradeRequestCount(): IImpulseState {
		return this.state;
	}
}