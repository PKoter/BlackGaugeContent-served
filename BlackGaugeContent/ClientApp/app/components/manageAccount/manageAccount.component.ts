import { Component, NgModule, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { LoginModel, GenderModel, AccountFeedback, AccountDetails, FeedResult } from '../../models/account';
import { ListEntry }      from '../../commonTypes.api';
import { UserService}     from '../../services/user.service';
import { DataFlowService} from '../../services/dataFlow.service';
import { ApiRoutes}       from '../../services/apiRoutes.service';


@Component({
	selector: 'manage-account',
	templateUrl: './manageAccount.html',
	styleUrls: ['../register/userRegistration.css', '../../controls/bgcButtons.css']
})

export class ManageAccountComponent implements OnInit {
	private readonly MaxMottoLength: number = 255;

	private genders: GenderModel[];
	private model: AccountDetails;
	private loadLevel: number = 0;

	private genderToString = (model: GenderModel) => model.genderName;
	private error: string = '';
	private submitted: boolean = false;
	private redirecting: boolean = false;

	private deathRequest: boolean = false;

	constructor(titleService: Title, private userService: UserService,
		private dataService: DataFlowService
	) {
		this.model = new AccountDetails(0, '', '', 0, true);
		titleService.setTitle("BGC Manage account");
	}

	ngOnInit() {
		this.userService.getGenders()
			.subscribe(g => {
				this.genders = g;
				this.loadLevel += 1;
			},
			err => console.warn(err));
	}

	onGenderSelected(gender: ListEntry<GenderModel>) {
		this.model.genderId = gender.item.id;
	}



	onDeathRequest() {
		//TODO user death request;
		this.deathRequest = true;
	}
}