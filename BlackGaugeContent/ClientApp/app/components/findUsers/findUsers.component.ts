import { Component, NgModule, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { SiteTitleService } from '../../services/title.service';
import { FeedResult } from '../../models/account';
import { IUserInfo } from '../../models/users'
import { ListEntry } from '../../commonTypes.api';
import { UserService } from '../../services/user.service';
import { ApiRoutes, ApiRoutesService } from '../../services/apiRoutes.service';


@Component({
	selector: 'find-users',
	templateUrl: './findUsers.html',
	styleUrls: ['../../controls/bgcButtons.css', '../../controls/bgcViewSections.css', '../../controls/bgcForms.css', '../../controls/bgcGeneral.css']
})

export class FindUsersComponent {
	model: IUserInfo;
	userName:           string  = '';
	comradeRequestSent: boolean = false;
	found:              boolean = false;
	searching:          boolean = false;
	success:            number  = 0; // 0 1 2

	constructor(titleService: SiteTitleService, private userService: UserService,
		private router: ApiRoutesService)
	{
		titleService.setTitle("BGC Find User");
	}

	private get comradeRequestTitle(): string {
		if (!this.model)
			return '';
		if (this.model.isComrade)
			return 'Already a comrade!';
		if (this.model.comradeRequestSent || this.comradeRequestSent)
			return "Comrade request has been sent";
		if (this.model.requestReceived)
			return `${this.model.userName} applies as a comrade!`;
		return 'Be comrades!';
	}

	private checkUserNotSearchingHimself(): boolean {
		let name = this.userService.getUserIds().name;
		if (this.userName === name) {
			this.success = 2;
			this.found   = false;
			return true;
		}
		return false;
	}

	findUser() {
		if (this.checkUserNotSearchingHimself()) return;
		// blocks view and sends request.
		this.searching = true;
		this.userService.findUser(this.userName, (r: IUserInfo) => {
			this.searching = false;
			let result     = false;

			if (r.userName && r.userName !== '') {
				this.model = r;
				result     = true;
				this.userName = r.userName;
				this.comradeRequestSent = r.comradeRequestSent || r.isComrade || r.requestReceived;
			}
			this.found   = result;
			this.success = result ? 0 : 1;
			this.checkUserNotSearchingHimself();
		});
	}

	private onComradeRequest() {
		if (this.comradeRequestSent)
			return;
		this.comradeRequestSent = true;
		this.userService.sendComradeRequest(this.model.userName, (r: { result: FeedResult }) => {
			if (r.result === FeedResult.success) {}
		});
	}
}