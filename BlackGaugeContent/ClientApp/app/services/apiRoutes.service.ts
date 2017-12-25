import { Injectable } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/switchMap';


@Injectable()
export class ApiRoutesService {

	constructor(private router: Router, private routes: ActivatedRoute) { }

	public redirect(route: string) {
		this.router.navigate([route]);
	}

	public redirectHome() {
		this.router.navigate(['/']);
	}

	public getParam(paramName: string): any {
		return this.routes.snapshot.paramMap.get(paramName);
	}

	public seekParam(paramName: string): Observable<any> {
		return this.routes.paramMap.map((params: ParamMap) => params.get(paramName));
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
	public static FindUsers       = 'findUsers';
	public static Comrades        = 'comrades';
	public static Messages        = 'messages';
}

/**
 * Server-side router supported by controllers.
 */
export class ApiRoutes {
	public static ConfirmEmail          = 'api/Account/ConfirmEmail';
	public static Login                 = 'api/Account/Login';
	public static Logout                = 'api/Account/Logout';
	public static Register              = 'api/Account/Register';
	public static EnsureAuth            = 'api/Account/EnsureAuthTransfer';
	public static CheckUniqueness       = 'api/User/CheckUniqueness';
	public static AccountDetails        = 'api/User/GetAccountDetails';
	public static SetAccountDetails     = 'api/User/SetAccountDetails';
	public static GetUserInfo           = 'api/User/GetUserInfo';
	public static SendComradeRequest    = 'api/Comrade/MakeComradeRequest';
	public static GetComradeRelations   = 'api/Comrade/GetUserComradeRelations';
	public static GetComrades           = 'api/Comrade/GetUserComrades';
	public static ConfirmComradeRequest = 'api/Comrade/ConfirmComradeRequest';
	public static SeenComradeRequest    = 'api/Comrade/SeenComradeRequest';
	public static GetLastMessages       = 'api/Message/GetSurroundingLastMessages';
	public static SendMessages          = 'api/Message/SendMessage';
	public static MemeReaction          = 'api/MemeList/MemeReaction';
	public static PageMemes             = 'api/MemeList/PageMemes';
	public static CountNewMemes         = 'api/MemeList/CountNewMemes';
	public static TermsOfService        = 'api/AppMeta/GetTermsOfService';
}