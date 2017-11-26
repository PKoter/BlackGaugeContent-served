import { Component, NgModule, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { LoginModel, GenderModel, AccountFeedback, AccountDetails, FeedResult } from '../../models/account';
import { ListEntry }      from '../../commonTypes.api';
import { UserService}     from '../../services/user.service';
import { ApiRoutes, ApiRoutesService} from '../../services/apiRoutes.service';


@Component({
	selector: 'manage-account',
	templateUrl: './manageAccount.html',
	styleUrls: ['../register/userRegistration.css', '../../controls/bgcButtons.css']
})

export class ManageAccountComponent implements OnInit {
	private readonly MaxMottoLength: number = 255;

	private genders: GenderModel[] = [];
	private model: AccountDetails;
	private modelCopy: AccountDetails;
	private loadLevel: number = 0;

	private genderToString = (model: GenderModel) => model.genderName;
	private error: string = '';
	private submitted: boolean = false;
	private redirecting: boolean = false;

	private deathRequest: boolean = false;

	constructor(titleService: Title, private userService: UserService, private router: ApiRoutesService
	) {
		this.model = new AccountDetails(0, '', '', 0, true);
		titleService.setTitle("BGC Manage account");
	}

	ngOnInit() {
		this.userService.getGenders(g => {
				this.genders = g;
				this.getModelGenderName();
				this.loadLevel += 1;
			});
		this.userService.getAccountDetails(d => {
			this.model = d;
			this.getModelGenderName();
			this.modelCopy = new AccountDetails(d.genderId, '', d.motto, d.coins, d.alive);
			this.loadLevel += 1;
		});
	}

	private getModelGenderName() {
		if(this.genders.length > 0 && this.model.genderId > 0)
			this.model.genderName = this.genders[this.model.genderId - 1].genderName;
	}

	onGenderSelected(gender: ListEntry<GenderModel>) {
		this.model.genderId = gender.item.id;
	}

	onDeathRequest() {
		//TODO user death request;
		this.deathRequest = true;
	}

	/**
	 * Detects any changes to user settings and performs save, otherwise does nothing.
	 */
	onSave() {
		let anyChange = this.model.genderId !== this.modelCopy.genderId;
		anyChange = anyChange || this.model.motto !== this.modelCopy.motto;
		if (anyChange === false)
			return;
		this.userService.saveAccountDetails(this.model, r =>
			{
				if (r.result === FeedResult.success)
					this.router.redirectHome();
			});
	}
}