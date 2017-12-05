import { Component, NgModule, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { LoginModel, GenderModel, AccountFeedback, AccountDetails, FeedResult } from '../../models/account';
import { IUserInfo } from '../../models/users'
import { ListEntry } from '../../commonTypes.api';
import { UserService } from '../../services/user.service';
import { ApiRoutes, ApiRoutesService } from '../../services/apiRoutes.service';


@Component({
	selector: 'find-users',
	templateUrl: './findUsers.html',
	styleUrls: ['../register/userRegistration.css', '../../controls/bgcButtons.css']
})

export class FindUsersComponent {
	private readonly MaxMottoLength: number = 255;

	private genders: GenderModel[] = [];
	private model: IUserInfo;
	private modelCopy: AccountDetails;
	private loadLevel: number = 0;

	private genderToString = (model: GenderModel) => model.genderName;
	private error: string = '';
	private submitted: boolean = false;
	private redirecting: boolean = false;

	private deathRequest: boolean = false;

	constructor(titleService: Title, private userService: UserService, private router: ApiRoutesService
	) {
		titleService.setTitle("BGC Find User");
	}
}