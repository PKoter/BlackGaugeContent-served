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
		public userId: number,
		public memeId: number,
		public vote: number) { }
}

export class MemeState {
	constructor(
		public rating: number,
		public commentCount: number,
		public vote: number
	) { }
}