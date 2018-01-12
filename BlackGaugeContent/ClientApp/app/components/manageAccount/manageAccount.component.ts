import { Component, NgModule, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { SiteTitleService } from '../../services/title.service';
import { LoginModel, GenderModel, AccountFeedback, AccountDetails, FeedResult } from '../../models/account';
import { ListEntry }      from '../../commonTypes.api';
import { UserService}     from '../../services/user.service';
import { ApiRoutes, ApiRoutesService} from '../../services/apiRoutes.service';


@Component({
	selector: 'manage-account',
	templateUrl: './manageAccount.html',
	styleUrls: ['../../controls/bgcButtons.css', '../../controls/bgcForms.css', '../../controls/bgcViewSections.css']
})

export class ManageAccountComponent implements OnInit {
	readonly MaxMottoLength: number = 255;

	genders: GenderModel[] = [];
	model: AccountDetails;
	modelCopy: AccountDetails;
	loadLevel: number = 0;

	genderToString = (model: GenderModel) => model.genderName;
	error: string = '';
	submitted: boolean = false;
	redirecting: boolean = false;

	deathRequest: boolean = false;

	constructor(titleService: SiteTitleService, private userService: UserService, private router: ApiRoutesService
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