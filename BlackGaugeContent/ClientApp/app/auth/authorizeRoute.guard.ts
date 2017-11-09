import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';
import { UserService } from '../services/user.service';
import { ApiRoutesService, Routes } from '../services/apiRoutes.service';

@Injectable() 
export class AuthorizeRouteGuard implements CanActivate {

	constructor(private routes: ApiRoutesService, private userService: UserService) {}

	canActivate(route: Object, state: Object): boolean {
		if (this.userService.isLoggedIn() === false) {
			this.routes.redirect(Routes.Login);
			return false;
		}
		return true;
	}
}