import { Component, OnInit } from '@angular/core';
import { ApiRoutesService, Routes } from '../../services/apiRoutes.service';
import { UserService } from '../../services/user.service';
import { UserImpulsesService} from '../../services/userImpulses.service';
import { QuickActionItem } from '../../controls/bgcQuickUserActions/bgcQuickUserActions.control';
import { IImpulsesState }  from '../../models/signals'
import { SiteTitleService } from '../../services/title.service';
import { AudioService } from '../../services/audio.service';

@Component({
	selector: 'app-header',
	templateUrl: './appHeader.html',
	styleUrls: ['./appHeader.css']
})

export class AppHeaderComponent implements OnInit {
	readonly loginRoute    = [`/${Routes.Login}`];
	readonly memeListRoute = [`/${Routes.MemeList}`];
	readonly homeRoute     = [`/${Routes.Home}`];

	userName = '';
	notifications: number = 0;
	comradeRequests: number;

	private userActions: QuickActionItem[] = [
		new QuickActionItem('Find user', Routes.FindUsers),
		new QuickActionItem('Comrades', Routes.Comrades),
		new QuickActionItem('Messages', Routes.Messages),
		new QuickActionItem('Manage account', Routes.ManageAccount),
		new QuickActionItem('Sign out', '/', this.logout.bind(this))
	];

	private audioService: AudioService;

	constructor(private userService: UserService, private impulseService: UserImpulsesService,
	 private titleService: SiteTitleService) {
	}

	ngOnInit() {
		this.userService.logged.subscribe((input: any) => this.userLogged(input));
		this.userName = this.userService.getUserIds().name;
		// wait for notification count update
		this.impulseService.activeCounts.subscribe(this.updateImpulseCount.bind(this));

		this.audioService = new AudioService();
		this.audioService.loadAudioFromUri('audio/msg_sound.mp3');
	}

	private updateImpulseCount(counts: IImpulsesState) {
		this.userActions[1].setBadge(counts.comradeRequestSum);
		this.userActions[2].setBadge(counts.messageSum);

		let count = this.notifications;
		this.notifications = counts.notifySum;
		this.titleService.setNotifications(this.notifications);

		if (count < counts.notifySum) {
			this.audioService.playAudio();
		}
	}

	/**
	 * feeds quick user actions with currect notification count.
	 * @returns {} 
	 */
	private get getImpulsesCount(): number | string {
		let impulses = this.notifications;
		if (impulses === 0)
			return '';
		return impulses;
	}

	private userLogged(really: boolean) {
		this.userName = really ? this.userService.getUserIds().name : '';
	}

	/**
	 * called by quick user actions
	 */
	private logout():boolean {
		this.userService.logOut();
		return true;
	}
}
