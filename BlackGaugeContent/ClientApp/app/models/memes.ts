export interface MemeModel {
	core: Meme;
	state: MemeState;
}

export interface Meme {

	id: number;
	base64: string;
	title: string;
	addTime: string;
}

export class MemeReaction {
	constructor(
		userId: number,
		memeId: number,
		vote: number) { }
}

export class MemeState {
	constructor(
		public rating: number,
		public commentCount: number) { }
}