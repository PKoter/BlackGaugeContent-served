import { Injectable, Inject, Output, EventEmitter } from '@angular/core';
import { HubConnection } from '@aspnet/signalr-client';
import { IImpulseState, IComradeRequest, IRuntimeImpulse } from '../models/signals';
import { UserService } from './user.service';
import { AuthGuard } from '../auth/auth.guard';
import { ImpulseHandler } from '../handlers/impulseHandler';

@Injectable()
export class UserImpulsesService extends ImpulseHandler {
	private impulsesEndpoint: string;
	private connected:        boolean = false;

	constructor(private userService: UserService, @Inject('BASE_URL') baseUrl: string, private auth: AuthGuard)
	{
		super();
		this.impulsesEndpoint = baseUrl + 'bgcImpulses';
		this.userService.logged.subscribe((x: any) => this.connectIfLogged(x));
		if (this.userService.isLoggedIn()) {
			this.connectIfLogged(true);
		}
	}


	private connectIfLogged(logged: boolean) {
		if (logged && !this.connected) {
			let aut = this.auth.getAuthorization();
			this.subscribe = new HubConnection(
				this.impulsesEndpoint + `?auth=${aut.auth}`);
			
			this.hub.start()
				.then(() => {
					this.connected = true;
					console.log('impulse sniff established');
				})
				.catch(errors => {
					this.connected = false;
					console.warn(errors);
				});
		}
	}
}