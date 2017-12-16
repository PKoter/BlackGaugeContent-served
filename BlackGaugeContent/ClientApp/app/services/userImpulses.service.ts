import { Injectable, Inject, Output, EventEmitter } from '@angular/core';
import { HubConnection } from '@aspnet/signalr-client';
import { IImpulseState, IComradeRequest, IRuntimeImpulse } from '../models/signals';
import { UserService } from './user.service';
import { AuthGuard } from '../auth/auth.guard';
import { ImpulseHandler } from '../handlers/impulseHandler';

@Injectable()
export class UserImpulsesService extends ImpulseHandler {
	private impulsesEndpoint: string;
	private state:            IImpulseState;

	@Output() public impulsed : EventEmitter<IRuntimeImpulse> = new EventEmitter();

	constructor(private userService: UserService, @Inject('BASE_URL') baseUrl: string, private auth: AuthGuard)
	{
		super();
		this.impulsesEndpoint = baseUrl + 'bgcImpulses';
		this.userService.logged.subscribe((x: any) => this.connectIfLogged(x));
	}


	private connectIfLogged(logged: boolean) {
		if (logged) {
			let aut = this.auth.getAuthorization();
			this.subscribe = new HubConnection(
				this.impulsesEndpoint + `?auth=${aut.auth}`);
			
			this.hub.start()
				.then(() => {
					console.log('impulse sniff established');
				})
				.catch(errors => console.warn(errors));
		}
	}

	public digestState(state: any) {
		let impulses = (state as { impulses: IImpulseState });
		if (impulses) {
			this.state = impulses.impulses;
			//this.impulsed.emit();
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