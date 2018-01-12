import { Title } from '@angular/platform-browser';
import { Injectable } from '@angular/core';

@Injectable()
export class SiteTitleService {
	private baseTitle: string;
	private notifications: number = 0;

	constructor(private titleHandle: Title) {
		
	}

	public setTitle(title: string) {
		this.baseTitle = title;
		this.updateTitle();
	}

	public setNotifications(n: number) {
		this.notifications = n;
		this.updateTitle();
	}

	private updateTitle() {
		if (this.notifications <= 0) {
			this.titleHandle.setTitle(this.baseTitle);
			return;
		}
		this.titleHandle.setTitle("(" + this.notifications + ") " + this.baseTitle);
	}

}