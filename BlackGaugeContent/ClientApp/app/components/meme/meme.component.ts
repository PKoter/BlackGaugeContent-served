import { Component, Input } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
	selector: 'meme-wrapping',
	templateUrl: './memeWrapping.html',
	styleUrls: ['./memeWrapping.css']
})

export class MemeComponent {
	@Input() meme: Meme;

	constructor(private _sanitizer: DomSanitizer) {
		
	}

	public getMemeSrc(): any {
		return this._sanitizer.bypassSecurityTrustUrl(this.meme.base64);
	}
}

export interface Meme {
	base64: string;
	title: string;
	rating: number;
	addTime: string;
	commentCount: number;
}