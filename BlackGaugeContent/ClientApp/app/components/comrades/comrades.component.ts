import { Component, NgModule, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { IComradeRelations, IComradeEntry, ComradeRequest } from '../../models/users';
import { FeedResult } from '../../models/account';
import { ListEntry } from '../../commonTypes.api';
import { UserService } from '../../services/user.service';
import { ApiRoutes, ApiRoutesService } from '../../services/apiRoutes.service';


@Component({
	selector: 'comrades',
	templateUrl: './comrades.html',
	styleUrls: ['./comrades.css', '../../controls/bgcButtons.css', '../../controls/bgcViewSections.css', '../../controls/bgcForms.css', '../../controls/bgcGeneral.css']
})

export class ComradesComponent implements OnInit {
	private comrades: IComradeEntry [];
	private received: ComradeRequest[];
	private sent:     ComradeRequest[];
	private loading: boolean = true;
	private userName: string = '';
	private tempRequest: ComradeRequest|null = null;

	constructor(titleService: Title, private userService: UserService,
		private router: ApiRoutesService
	) {
		titleService.setTitle("BGC Comrade relations");
	}

	ngOnInit() {
		this.userService.getComradeRelations(r => {
			this.comrades = r.comrades;
			this.received = r.received;
			this.sent     = r.sent;
			this.loading  = false;
		});
	}

	private onAgree(index: number) {
		let request = this.received[index];
		if (!request.id)
			return;
		this.tempRequest = request;
		this.received.splice(index, 1);
		this.loading = true;
		this.userService.confirmComradeRequest(request.id, r => {
			if (r.result !== FeedResult.success)
				return;
			if (this.tempRequest == null)
				return;
			this.comrades.push(
				{ name: this.tempRequest.otherName, interactions: 0 } as IComradeEntry);
			this.loading = false;
		});
	}
}