import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';
import { AuthGuard } from './auth.guard';
import { ApiRoutesService, Routes } from '../services/apiRoutes.service';

@Injectable() 
export class AuthorizeRouteGuard implements CanActivate {

	constructor(private routes: ApiRoutesService, private auth: AuthGuard) {}

	canActivate(route: Object, state: Object): boolean {
		if (this.auth.hasActiveToken() === false) {
			this.routes.redirect(Routes.Login);
			return false;
		}
		return true;
	}
}