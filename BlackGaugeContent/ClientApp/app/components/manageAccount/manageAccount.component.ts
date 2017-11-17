import { Component, NgModule, OnInit, Inject } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { LoginModel, AccountFeedback, FeedResult } from '../../models/account';

import { UserService } from '../../services/user.service';
import { DataFlowService } from '../../services/dataFlow.service';
import { ApiRoutes } from '../../services/apiRoutes.service';


@Component({
	selector: 'manage-account',
	templateUrl: './manageAccount.html',
	styleUrls: ['../register/userRegistration.css', '../../controls/bgcButtons.css']
})

export class ManageAccountComponent {
	private readonly MaxMottoLength: number = 255;

	private model: LoginModel;
	private error: string = '';
	private submitted: boolean = false;
	private redirecting: boolean = false;

	constructor(titleService: Title, private userService: UserService,
		private dataService: DataFlowService
	) {
		this.model = new LoginModel('', '', false);
		titleService.setTitle("BGC Account Management");
	}

	onDeathRequest() {
		if (this.submitted)
			return;
		this.error = '';
		this.submitted = true;
		this.redirecting = true;
		this.userService.post<LoginModel>(ApiRoutes.Login, this.model)
			.subscribe(result => {
					let feedback = result.json() as AccountFeedback;
					this.redirecting = false;
					if (feedback.result === FeedResult.success)
						this.userService.logIn(result);
					else {
						this.submitted = false;
						this.error = feedback.message;
					}
				},
				errors => console.warn(errors)
			);
	}
}