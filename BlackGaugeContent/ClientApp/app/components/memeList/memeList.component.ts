import { Component, OnInit } from '@angular/core';
import { SiteTitleService } from '../../services/title.service';
import { BgcMemeService } from '../../services/bgcMeme.service';
import { ActivatedRoute } from '@angular/router/'

import { MemeModel } from '../../models/memes';

@Component({
	selector: 'meme-list',
	templateUrl: './memeList.html',
	styleUrls: ['./memeList.css', '../../controls/bgcViewSections.css'],
	providers: [BgcMemeService]
})

export class MemeListComponent implements OnInit {
	public memes: MemeModel[];
	page: number;
	memeCount: number;
	newMemeCount: number;

	constructor(private memeService: BgcMemeService, private routes: ActivatedRoute,
		titleService: SiteTitleService)
	{
		titleService.setTitle("BGC official memes");
		this.page = +(this.routes.snapshot.paramMap.get('page') || 0);
	}

	ngOnInit() {
		this.memeService.getMemePage(this.page, data => {
				this.memes = data;
				this.memeCount = data.length;
				this.getNewMemeCount();
			});
	}

	private getNewMemeCount() {
		if (this.page === 0)
			return;
		if (this.memes == undefined || this.memes.length === 0)
			return;
		this.memeService.getNewMemeCount(this.page, this.memes, r => this.newMemeCount = r);
	}
}
