import { Component, OnInit } from '@angular/core';
import { ApiRoutesService, Routes } from '../../services/apiRoutes.service';
import { UserService } from '../../services/user.service';
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

	private userActions: QuickActionItem[] = [
		new QuickActionItem('Find user', ''),
		new QuickActionItem('Compadres', ''),
		new QuickActionItem('Messages', ''),
		new QuickActionItem('Manage account', ''),
		new QuickActionItem('Sign out', '/', this.logout.bind(this))
	];

	constructor(private userService: UserService) {
	}

	ngOnInit() {
		this.userService.logged.subscribe((input: any) => this.userLogged(input));
		this.userName = this.userService.getUserIds().name;
	}

	private userLogged(really: boolean) {
		this.userName = really ? this.userService.getUserIds().name : '';
	}

	private logout():boolean {
		this.userService.logOut();
		return true;
	}
}
