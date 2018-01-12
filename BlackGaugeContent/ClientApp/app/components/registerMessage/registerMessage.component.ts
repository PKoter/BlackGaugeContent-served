import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { UserService } from '../../services/user.service';
//import { RequestHandler } from '../requestHandler';
import { DataFlowService } from '../../services/dataFlow.service';
import { AccountFeedback } from '../../models/account';
import 'rxjs/add/operator/switchMap';
import { Http } from '@angular/http';

@Component({
	selector: 'registration-redirect',
	template: `
		<p class="mid-message">{{message}}</p>
		<bgc-loading-spinner *ngIf="posting"></bgc-loading-spinner>
	`,
	styles: [`
		.mid-message {
			text-align: center;
			padding-top: 100px;
			margin-right: 20px;
			font-size: 1.5em;
			display: block;
		}
	`],
	providers: [UserService]
})

export class RegisterMessageComponent implements OnInit {
	message: string = 'Wait...';
	data: AccountFeedback;
	posting: boolean = true;

	constructor(private router: Router, private route: ActivatedRoute,
		private userService: UserService, private dataService: DataFlowService, private http: Http)
	{
		this.data = this.dataService.getOnce('RegistrationRedirect') as AccountFeedback;
	}

	ngOnInit() {
		if (this.data == null) {
			let userId = this.route.snapshot.queryParams['userId'];
			let code = this.route.snapshot.queryParams['code'];
			//@hack: this should be handled by userService, but only this way works properly.
			//this.userService
			this.http
				.get(`api/Account/ConfirmEmail?userId=${userId}&code=${code}`)
				.subscribe(result => {
					this.data = result.json() as AccountFeedback;
					this.message = this.data.message;
					this.posting = false;
				});
		}
		else {
			this.posting = false;
			this.message = this.data.message;
		}
	}
}