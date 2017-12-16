import { Component, OnInit } from '@angular/core';
import { ApiRoutesService, Routes } from '../../services/apiRoutes.service';
import { UserService } from '../../services/user.service';
import { UserImpulsesService} from '../../services/userImpulses.service';
import { QuickActionItem } from '../../controls/bgcQuickUserActions/bgcQuickUserActions.control';
import { ICountImpulses} from '../../models/signals'

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
		// wait for notification count update
		this.impulseService.activeCounts.subscribe(this.updateImpulseCount);
	}

	private updateImpulseCount(counts: ICountImpulses) {
		this.userActions[1].badge = counts.comradeRequestCount;
		this.notifications        = counts.countAll;
	}

	/**
	 * feeds quick user actions with currect notification count.
	 * @returns {} 
	 */
	private get getImpulsesCount(): number | string {
		let impulses = this.notifications;
		if (impulses === 0)
			return '';
		return impulses;
	}

	private userLogged(really: boolean) {
		this.userName = really ? this.userService.getUserIds().name : '';
	}

	/**
	 * called by quick user actions
	 */
	private logout():boolean {
		this.userService.logOut();
		return true;
	}
}
