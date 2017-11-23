export class GenderModel {
	constructor(
		public id: number,
		public genderName: string,
		public description: string
	)
	{ }

	toString() : string { return this.genderName; }
}


export class RegistrationModel {

	constructor(
		public id: number,
		public name: string,
		public email: string,
		public password: string,
		public confirmPassword: string,
		public genderId: number
	)
	{ }
}

export class LoginModel {

	constructor(
		public email: string,
		public password: string,
		public rememberMe: boolean
	)
	{ }
}

export class UniqueRegisterValue {

	constructor(
		public valueType: string,
		public unique: boolean
	)
	{ }
}

export class AccountFeedback {
	constructor(
		public result: FeedResult,
		public message: string
	)
	{ }
}

export class AccountDetails {
	constructor(
		public genderId: number,
		public genderName: string,
		public motto: string,
		public coins: number,
		public alive: boolean
	)
	{ }
}

export enum FeedResult {
	error    = 0,
	success  = 1,
	redirect = 2
}

export interface IUserId {
	id: number;
	name: string;
}