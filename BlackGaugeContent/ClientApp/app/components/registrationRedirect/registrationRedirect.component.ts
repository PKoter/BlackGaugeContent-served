import { Component } from '@angular/core';
import { UserService } from '../../services/user.service';
import { DataFlowService } from '../../services/dataFlow.service';
import { RegistrationFeedback } from '../../models/account';

@Component({
	selector: 'registration-redirect',
	template: `
		<p class="mid-message">{{data.message}}</p>
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

export class RegistrationRedirectComponent {

	private data: RegistrationFeedback;

	constructor(private userService: UserService, private dataService: DataFlowService) {
		this.data = this.dataService.getOnce('RegistrationRedirect') as RegistrationFeedback;
	}
}