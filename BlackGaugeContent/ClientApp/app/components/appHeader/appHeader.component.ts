import { Component, OnInit } from '@angular/core';
import { ApiRoutesService, Routes } from '../../services/apiRoutes.service';
import { UserService } from '../../services/user.service';
import { UserImpulsesService} from '../../services/userImpulses.service';
import { QuickActionItem } from '../../controls/bgcQuickUserActions/bgcQuickUserActions.control';

@Component({
	selector: 'app-header',
	templateUrl: './appHeader.html',
	styleUrls: ['./appHeader.css']
})

export class AppHeaderComponent implements OnInit {
	private readonly loginRoute    = [`/${Routes.Login}`];
	private readonly memeListRoute = [`/${Routes.MemeList}`];
	private readonly homeRoute     = [`/${Routes.Home}`];

	private userName = '';
	private notifications: number = 0;
	private comradeRequests: number;

	private userActions: QuickActionItem[] = [
		new QuickActionItem('Find user', Routes.FindUsers),
		new QuickActionItem('Comrades', Routes.Comrades),
		new QuickActionItem('Messages', ''),
		new QuickActionItem('Manage account', Routes.ManageAccount),
		new QuickActionItem('Sign out', '/', this.logout.bind(this))
	];

	constructor(private userService: UserService, private impulseService: UserImpulsesService) {
	}

	ngOnInit() {
		this.userService.logged.subscribe((input: any) => this.userLogged(input));
		this.userName = this.userService.getUserIds().name;

		this.impulseService.impulsed.subscribe(() => this.userNotification());
		this.impulseService.comradeRequest.subscribe((value: any) => {
			if (value && value != null) {
				let badge: number = +this.userActions[1].badge;
				this.userActions[1].badge = +badge + 1;
				this.notifications += 1;
			}
		});
	}

	private get getAllImpulseCount(): number | string {
		// impulses  = this.impulseService.getAllImpulseCount();
		let impulses = this.notifications;
		if (impulses === 0)
			return '';
		return impulses;
	}

	private userNotification() {
		this.comradeRequests = this.impulseService.getComradeRequestCount();
		this.notifications = this.impulseService.getAllImpulseCount();
		this.userActions[1].badge = this.comradeRequests;
	}

	private userLogged(really: boolean) {
		this.userName = really ? this.userService.getUserIds().name : '';
	}

	private logout():boolean {
		this.userService.logOut();
		return true;
	}
}
