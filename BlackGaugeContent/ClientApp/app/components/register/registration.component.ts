import { Component, NgModule, OnInit, Inject } from '@angular/core';
import { Http } from '@angular/http';
import { Title } from '@angular/platform-browser';
import { RegistrationModel, UniqueRegisterValue, GenderModel } from '../../models/account';
import { SelectionEntry } from '../../controls/bgcSelect/bgcSelect.control';

@Component({
	selector: 'user-registration',
	templateUrl: './userRegistration.html',
	styleUrls: ['./userRegistration.css', '../../controls/bgcButtons.css']
})

export class RegistrationComponent implements OnInit {

	private model: RegistrationModel;
	private submitted: boolean = false;
	private passwordStrength: number;
	private notMatch: boolean = true;
	private passwordErrors: number;
	private nameUnique: boolean;
	private emailUnique: boolean;
	private genders: GenderModel[];
	private genderToString = (item: GenderModel) => item.genderName;
	private openedTerms: boolean;
	private hideTerms: boolean = true;
	private agreedToTerms: boolean;
	private termsOfService: string;

	constructor(private http: Http, @Inject('BASE_URL') private baseUrl: string, titleService: Title) {
		this.model       = new RegistrationModel(0, '', '', '', 0);
		this.nameUnique  = true;
		this.emailUnique = true;
		titleService.setTitle("BGC registration");

		this.http.get(baseUrl + 'api/User/GetGenders').subscribe(result => {
			this.genders = result.json() as GenderModel[];
		});
	}

	ngOnInit() {
		
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
		this.http.get(
			this.baseUrl + `api/Account/CheckUniqueness?value=${value},type=${type}`)
			.subscribe(result => {
				const uniqueness = result.json() as UniqueRegisterValue;
				if (uniqueness.valueType === 'name')
					this.nameUnique = uniqueness.unique;
				else
					this.emailUnique = uniqueness.unique;
			}, error => console.error(`something is fucked up: ${error}`));

	}

	onGenderSelected(gender: SelectionEntry<GenderModel>) {
		this.model.genderId = gender.item.id;
	}

	openTerms() {
		if (this.termsOfService == null) {
			this.http.get(this.baseUrl + 'api/AppMeta/GetTermsOfService')
				.subscribe(result => {
					this.termsOfService = result.json().terms;
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

	onSubmit() { this.submitted = true; }
}