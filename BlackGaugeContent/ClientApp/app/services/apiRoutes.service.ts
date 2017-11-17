import { Injectable } from '@angular/core';
import { Router } from '@angular/router';


@Injectable()
export class ApiRoutesService {

	constructor(private router: Router) { }

	public redirect(route: string) {
		this.router.navigate([route]);
	}
}

/**
 * Client-side routes.
 */
export class Routes {
	public static Home            = 'home';
	public static Login           = 'login';
	public static Register        = 'register';
	public static MemeList        = 'memeList';
	public static ConfirmEmail    = 'account/confirmEmail';
	public static RegisterMessage = 'register/message';
	public static ManageAccount   = 'manageAccount';
}

/**
 * Server-side router supported by controllers.
 */
export class ApiRoutes {
	public static ConfirmEmail = 'api/Account/ConfirmEmail';
	public static Login        = 'api/Account/Login';
	public static Logout       = 'api/Account/Logout';
	public static Register     = 'api/Account/Register';
	public static MemeReaction = 'api/MemeList/MemeReaction';
	public static PageMemes    = 'api/MemeList/PageMemes';
	public static EnsureAuth   = 'api/Account/EnsureAuthTransfer';
}