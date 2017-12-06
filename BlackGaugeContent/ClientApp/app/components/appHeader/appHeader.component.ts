import { Component, OnInit } from '@angular/core';
import { ApiRoutesService, Routes } from '../../services/apiRoutes.service';
import { UserService } from '../../services/user.service';
import { UserImpulsesService} from '../../services/userImpulses.service';
import { BgcQuickUserActionsControl, QuickActionItem} from '../../controls/bgcQuickUserActions/bgcQuickUserActions.control';

@Component({
	selector: 'app-header',
	templateUrl: './appHeader.html',
	styleUrls: ['./appHeader.css']
})

export class AppHeaderComponent implements OnInit {
	private readonly registerRoute = [`/${Routes.Register}`];
	private readonly loginRoute    = [`/${Routes.Login}`];
	private readonly memeListRoute = [`/${Routes.MemeList}`];
	private readonly homeRoute     = [`/${Routes.Home}`];

	private userName = '';
	private notifications: number;
	private comradeRequests: number;

	private userActions: QuickActionItem[] = [
		new QuickActionItem('Find user', Routes.FindUsers),
		new QuickActionItem('Compadre Requests', ''),
		new QuickActionItem('Compadres', ''),
		new QuickActionItem('Messages', ''),
		new QuickActionItem('Manage account', Routes.ManageAccount),
		new QuickActionItem('Sign out', '/', this.logout.bind(this))
	];

	constructor(private userService: UserService, private userImpulses: UserImpulsesService) {
	}

	ngOnInit() {
		this.userService.logged.subscribe((input: any) => this.userLogged(input));
		this.userImpulses.impulsed.subscribe(() => this.userNotification());
		this.userName = this.userService.getUserIds().name;
	}

	private userNotification() {
		this.comradeRequests = this.userImpulses.getComradeRequestCount();
		this.notifications = this.userImpulses.getAllImpulseCount();
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
