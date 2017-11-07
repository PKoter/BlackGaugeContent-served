import { Component, NgModule, OnInit, Inject } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { LoginModel, AccountFeedback, FeedResult } from '../../models/account';

import { UserService } from '../../services/user.service';
import { DataFlowService } from '../../services/dataFlow.service';
import { Router } from '@angular/router';


@Component({
	selector: 'user-login',
	templateUrl: './userLogin.html',
	styleUrls: ['../register/userRegistration.css', '../../controls/bgcButtons.css'],
	providers: [UserService]
})

export class LoginComponent {

	private model: LoginModel;
	private error: string;
	private submitted:   boolean = false;
	private redirecting: boolean = false;

	constructor(private router: Router, titleService: Title, private userServis: UserService,
		private dataService: DataFlowService
	)
	{
		this.model = new LoginModel('', '', false);
		this.error = '';
		titleService.setTitle("BGC login");
	}

	onSubmit() {
		if (this.submitted)
			return;
		this.error = '';
		this.submitted   = true;
		this.redirecting = true;
		this.userServis.post<LoginModel>('api/Account/Login', this.model)
			.subscribe(result =>
				{
					let feedback = result.json() as AccountFeedback;
					this.redirecting = false;
					if (feedback.result !== FeedResult.error)
						this.router.navigate(['']);
					else {
						this.submitted = false;
						this.error = feedback.message;
					}
				},
				errors => console.warn(errors)
			);
	}
}