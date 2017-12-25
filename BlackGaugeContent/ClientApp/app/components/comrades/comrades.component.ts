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
	private tempIndex: number|null = null;

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

	private comradeRequestUpdate(request: ComradeRequest) {
		if (!request.id)
			return;
		
		if (request.agreed !== true) {
			this.received.unshift(request);
		}
		else {
			let index = this.sent.findIndex(r => r.id === request.id);
			if (index < 0) {
				console.log('probaby bug');
				return;
			}
			this.makeComradeFrom(this.sent, index);
		}
	}

	/**
	 * removes request from given array and appends comrade model to list.
	 * @param requests
	 * @param index
	 */
	private makeComradeFrom(requests: ComradeRequest[], index: number) {
		let removedRequest = requests.splice(index, 1);
		this.comrades.push(
			{ name: removedRequest[0].otherName, interactions: 0 } as IComradeEntry);
	}

	/**
	 * user agrees to receiver comrade request.
	 * @param index
	 */
	private onAgree(index: number) {
		let request = this.received[index];
		if (!request.id)
			return;

		request.seen = true;
		this.reduceComradeNotifies();

		this.tempIndex = index;
		this.loading   = true;

		this.userService.confirmComradeRequest(request.id, request.otherName, r => {
			if (r.result !== FeedResult.success || this.tempIndex == null)
				return;
			this.makeComradeFrom(this.received, this.tempIndex);
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

		this.reduceComradeNotifies();
	}

	private reduceComradeNotifies() {
		let counts = this.impulses.getCounts();
		counts.popComradeRequest();
		this.impulses.setCounts(counts);
	}
}