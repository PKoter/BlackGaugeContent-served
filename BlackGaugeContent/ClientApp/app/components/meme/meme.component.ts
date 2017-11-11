import { Component, Input } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
	selector: 'meme-wrapping',
	templateUrl: './memeWrapping.html',
	styleUrls: ['./memeWrapping.css', '../../controls/bgcButtons.css']
})

export class MemeComponent {
	@Input() meme: Meme;
	private voteUnit = 1;
	private hasVoted = false;
	private lastVote: boolean;

	constructor(private _sanitizer: DomSanitizer) {
		
	}

	public getMemeSrc(): any {
		return this._sanitizer.bypassSecurityTrustUrl(this.meme.base64);
	}

	private voted(nicey: boolean) {
		if (this.hasVoted === false) {
			this.meme.vote = nicey ? this.voteUnit : -this.voteUnit;
			this.meme.rating += this.meme.vote;
			this.hasVoted = true;
		}
		else
		{
			this.meme.rating -= this.meme.vote;
			if (this.lastVote === nicey) {
				this.meme.vote = 0;
				this.hasVoted = false;
			}
			else {
				this.meme.vote = nicey ? this.voteUnit : -this.voteUnit;
				this.meme.rating += this.meme.vote;
			}
		}
		this.lastVote = nicey;
	}

}

export interface MemeModel {
	core: Meme;
	state: MemeState;
}

export interface Meme {
	id: number;
	base64: string;
	title: string;
	rating: number;
	addTime: string;
	commentCount: number;
	vote: number;
}

export interface MemeReaction {
	userId: number;
	memeId: number;
	vote: number;
}

export interface MemeState {
	rating: number;
	commentCount: number;
}