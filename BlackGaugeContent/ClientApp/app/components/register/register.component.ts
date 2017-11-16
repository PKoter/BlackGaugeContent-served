﻿import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { RegistrationModel, UniqueRegisterValue, GenderModel, AccountFeedback } from '../../models/account';
import { SelectionEntry } from '../../controls/bgcSelect/bgcSelect.control';
import { UserService } from '../../services/user.service';
import { DataFlowService } from '../../services/dataFlow.service';
import { ApiRoutesService, Routes, ApiRoutes } from '../../services/apiRoutes.service';


@Component({
	selector: 'user-registration',
	templateUrl: './userRegistration.html',
	styleUrls: ['./userRegistration.css', '../../controls/bgcButtons.css']
})

export class RegisterComponent implements OnInit {

	private model:            RegistrationModel;
	private submitted:        boolean = false;
	private passwordStrength: number;
	private notMatch:         boolean = true;

	private nameUnique:  boolean = false;
	private emailUnique: boolean = false;
	private nameDone:    boolean;
	private emailDone:   boolean;

	private genders: GenderModel[];
	private genderToString = (item: GenderModel) => item.genderName;

	private openedTerms:    boolean;
	private hideTerms:      boolean = true;
	private agreedToTerms:  boolean = false;
	private termsOfService: string;

	private redirecting: boolean = false;

	constructor(private router: ApiRoutesService,
		titleService: Title,
		private userService: UserService,
		private dataService: DataFlowService
	)
	{
		this.model = new RegistrationModel(0,'', '', '', '', 0);
		titleService.setTitle("BGC registration");
	}

	ngOnInit() {
		this.userService.getGenders()
			.subscribe(data => this.genders = data,
				errors => console.warn(errors)
			);
	}

	public set Password(value: string) {
		this.model.password = value;
		if (value.length >= 16) 
			this.passwordStrength = 3;
		else if (value.length >= 12)
			this.passwordStrength = 2;
		else
			this.passwordStrength = 1;
	}

	public set ConfirmPassword(value: string) {
		this.model.confirmPassword = value;
		this.notMatch = value !== this.model.password;
	}

	onCompleteUniqueCheck(value: string, type: string) {
		this.userService
			.get<UniqueRegisterValue>(`api/User/CheckUniqueness?value=${value}&type=${type}`)
			.subscribe(uniqueness => {
				if (uniqueness.valueType === 'name') {
					this.nameUnique = uniqueness.unique;
					this.nameDone = true;
				} else {
					this.emailUnique = uniqueness.unique;
					this.emailDone = true;
				}
			}, error => console.error(`something is fucked up: ${error}`));

	}

	onGenderSelected(gender: SelectionEntry<GenderModel>) {
		this.model.genderId = gender.item.id;
	}

	openTerms() {
		if (this.termsOfService == null) {
			this.userService.getAny('api/AppMeta/GetTermsOfService')
				.subscribe(result => {
					this.termsOfService = result.terms;
				});
		}
		this.hideTerms = false;
	}

	termsCalled(value: boolean) {
		this.openedTerms = this.openedTerms || value;
	}

	onTermsAgreedSwitched(value: boolean) {
		this.agreedToTerms = value;
	}

	onSubmit() {
		if (this.submitted)
			return;
		this.submitted   = true;
		this.redirecting = true;
		this.userService.post(ApiRoutes.Register, this.model)
			.subscribe(result => {
					let feedback = result.json() as AccountFeedback;
					this.dataService.save('RegistrationRedirect', feedback);
					this.redirecting = false;
					this.router.redirect(Routes.RegisterMessage);
				}, 
				errors => console.warn(errors)
			);
	}
}