import { Component, OnInit } from '@angular/core';
import { ApiRoutesService, Routes } from '../../services/apiRoutes.service';
import { UserService } from '../../services/user.service';

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

	constructor(private userService: UserService) {
	}

	ngOnInit() {
		this.userService.logged.subscribe((input: any) => this.userLogged(input));
		this.userName = this.userService.getUserIds().name;
	}

	private userLogged(really: boolean) {
		this.userName = really ? this.userService.getUserIds().name : '';
	}

	private logout() {
		this.userService.logOut();
	}
}
