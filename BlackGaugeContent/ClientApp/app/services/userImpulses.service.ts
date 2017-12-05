import { Inject, Injectable, Output, EventEmitter } from '@angular/core';
import { ApiRoutesService, Routes, ApiRoutes } from './apiRoutes.service';
import { IImpulseState} from '../models/signals';
import 'rxjs/add/operator/map';

@Injectable()
export class UserImpulsesService {
	private state: IImpulseState;
	@Output() public impulsed = new EventEmitter();

	constructor(private router: ApiRoutesService) {
	}

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
			return this.state.requestsAgreed.length + this.state.requestsReceived.length;
		return 0;
	}

	public getEachComradeRequestCount(): { agreed: number, received: number } {
		if (this.state)
			return { agreed: this.state.requestsAgreed.length, received: this.state.requestsReceived.length };
		return { agreed: 0, received: 0 };
	}
}