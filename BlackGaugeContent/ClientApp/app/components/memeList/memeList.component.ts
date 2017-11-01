import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

import { Meme } from '../meme/meme.component';

@Component({
	selector: 'meme-list',
	templateUrl: './memeList.html',
	styleUrls:['./memeList.css']
})

export class MemeListComponent {
	public memes: Meme[];

	constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
		http.get(baseUrl + 'api/MemeList/ListAll').subscribe(result => {
			this.memes = result.json() as Meme[];
		}, error => console.error(`something is fucked up: ${error}`));
	}
}
