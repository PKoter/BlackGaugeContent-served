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
	styleUrls: ['../../controls/bgcButtons.css', '../../controls/bgcViewSections.css', '../../controls/bgcForms.css']
})

export class FindUsersComponent {
	private model: IUserInfo;

	private comradeRequestSent: boolean = false;
	private comradeRequirement: string  = "Be comrades!";
	private found:              boolean = false;
	private searching:          boolean = false;
	private success:            boolean = true;

	constructor(titleService: Title, private userService: UserService, private router: ApiRoutesService
	) {
		titleService.setTitle("BGC Find User");
		this.model = {} as IUserInfo;
	}

	private findUser() {
		this.userService.findUser(this.model.userName, (r: IUserInfo) => {
			this.searching = false;
			let result     = false;

			if (r.userName && r.userName !== '') {
				this.model = r;
				result     = true;
			}
			this.found   = result;
			this.success = result;
		});
	}

	private onComradeRequest() {
		if (this.comradeRequestSent)
			return;
		this.comradeRequestSent = true;
		this.userService.sendComradeRequest(this.model.userName, (r: { result: FeedResult }) => {
			if (r.result === FeedResult.success) {
				this.comradeRequirement = "Request has been sent";
			}
		});
	}
}