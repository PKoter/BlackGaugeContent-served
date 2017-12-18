import { Component, NgModule, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { IComradeRelations, IComradeEntry, ComradeRequest } from '../../models/users';
import { IComradeRequest } from '../../models/signals';
import { FeedResult } from '../../models/account';
import { ListEntry } from '../../commonTypes.api';
import { UserService } from '../../services/user.service';
import { ApiRoutes, ApiRoutesService } from '../../services/apiRoutes.service';
import { UserImpulsesService } from '../../services/userImpulses.service';


@Component({
	selector: 'comrades',
	templateUrl: './comrades.html',
	styleUrls: ['./comrades.css', '../../controls/bgcButtons.css', '../../controls/bgcViewSections.css', '../../controls/bgcForms.css', '../../controls/bgcGeneral.css']
})

export class ComradesComponent implements OnInit {
	private comrades: IComradeEntry [];
	private received: ComradeRequest[];
	private sent:     ComradeRequest[];
	private loading:  boolean = true;
	private userName: string = '';
	private tempRequest: ComradeRequest|null = null;

	constructor(titleService: Title, private userService: UserService,
		private router: ApiRoutesService, private impulses: UserImpulsesService
	) {
		titleService.setTitle("BGC Comrade relations");
	}

	ngOnInit() {
		// get current relations
		this.userService.getComradeRelations(r => {
			this.comrades = r.comrades;
			this.received = r.received;
			this.sent     = r.sent;
			this.loading  = false;
		});
		this.impulses.comradeRequest.subscribe(this.comradeRequestUpdate.bind(this));
	}

	private comradeRequestUpdate(request: IComradeRequest) {
		
	}

	/**
	 * user agrees to receiver comrade request.
	 * @param index
	 */
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

	/**
	 * user goes meh and hides notification for request.
	 * @param index
	 */
	private onDigest(index: number) {
		let request = this.received[index];
		if (!request.id || request.seen)
			return;
		request.seen = true;
		this.userService.seenComradeRequest(request.id, r => {});

		let counts = this.impulses.getCounts();
		counts.popComradeRequest();
		this.impulses.setCounts(counts);
	}
}