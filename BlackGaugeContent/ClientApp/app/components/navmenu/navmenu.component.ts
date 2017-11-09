import { Component, OnInit } from '@angular/core';
import { ApiRoutesService, Routes } from '../../services/apiRoutes.service';
import { UserService } from '../../services/user.service';

@Component({
	selector: 'nav-menu',
	templateUrl: './navmenu.html',
	styleUrls: ['./navmenu.css']
})
export class NavMenuComponent implements OnInit {
	private registerRoute = [`/${Routes.Register}`];
	private loginRoute = [`/${Routes.Login}`];
	private memeListRoute = [`/${Routes.MemeList}`];
	private homeRoute = [`/${Routes.Home}`];

	private userName = '';

	constructor(private userService: UserService) {
	}

	ngOnInit() {
		this.userService.logged.subscribe((input: any) => this.userName = input ? this.userService.getUserIds().name : '');
		this.userName = this.userService.getUserIds().name;
	}

	private userLogged(really: boolean) {
		this.userName = really ? this.userService.getUserIds().name : '';
	}

	private logout() {
		this.userService.logOut();
	}
}
