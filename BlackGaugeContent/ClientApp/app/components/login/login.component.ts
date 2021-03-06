﻿import { Component, NgModule, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { LoginModel, AccountFeedback, FeedResult } from '../../models/account';
import { UserImpulsesService } from '../../services/userImpulses.service';
import { UserService }     from '../../services/user.service';
import { DataFlowService } from '../../services/dataFlow.service';
import { ApiRoutes, Routes, ApiRoutesService } from '../../services/apiRoutes.service';


@Component({
	selector: 'user-login',
	templateUrl: './userLogin.html',
	styleUrls: ['../../controls/bgcButtons.css', '../../controls/bgcViewSections.css', '../../controls/bgcForms.css'],
	encapsulation: ViewEncapsulation.None
})

export class LoginComponent {

	model: LoginModel;
	error: string = '';
	submitted:   boolean = false;
	redirecting: boolean = false;

	constructor(titleService: Title,
		private userService:  UserService,
		private dataService:  DataFlowService,
		private router:       ApiRoutesService,
		private userImpulses: UserImpulsesService
	){
		this.model = new           LoginModel('', '', false);
		titleService.setTitle("BGC login");
	}

	onSubmit() {
		if (this.submitted)
			return;
		this.error = '';
		this.submitted   = true;
		this.redirecting = true;
		this.userService.loginRequest(this.model, feedback =>
		{
			this.redirecting = false;
			if (feedback.result !== FeedResult.success) {
				this.submitted = false;
				this.error = feedback.message;
			}
			else {
				this.userImpulses.digestState(<any>feedback);
			}
		});
	}

	onRegisterRedirect() {
		this.router.redirect(Routes.Register);
	}
}