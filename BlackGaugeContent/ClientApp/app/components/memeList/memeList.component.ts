import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { BgcMemeService } from '../../services/bgcMeme.service';

import { MemeModel } from '../../models/memes';

@Component({
	selector: 'meme-list',
	templateUrl: './memeList.html',
	styleUrls: ['./memeList.css'],
	providers: [BgcMemeService]
})

export class MemeListComponent implements OnInit {
	public memes: MemeModel[];
	private page: number;

	constructor(private memeService: BgcMemeService, titleService: Title) {
		titleService.setTitle("BGC official memes");
		this.page = 0;
	}

	ngOnInit() {
		this.memeService.getMemePage(this.page)
			.subscribe(data => this.memes = data,
				errors => console.warn(errors)
			);
	}
}
