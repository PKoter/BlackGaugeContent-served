import { Component } from '@angular/core';
import { SiteTitleService } from '../../services/title.service';

@Component({
	selector: 'home',
	templateUrl: './home.html',
	styleUrls: ['../../controls/bgcGeneral.css', '../../controls/bgcViewLayout.css']
})
export class HomeComponent {
	constructor(titleService: SiteTitleService) {
		titleService.setTitle("BGC start page");
	}
}
