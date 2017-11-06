export class GenderModel {
	constructor(
		public id: number,
		public genderName: string,
		public description: string) {
	}

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

export class UniqueRegisterValue {

	constructor(
		public valueType: string,
		public unique: boolean)
	{ }
}

export class RegisterFeedback {
	constructor(
		public type: string,
		public message: string) {
	}
}