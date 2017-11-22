import { Component, Input } from '@angular/core';
import { ApiRoutesService } from '../../services/apiRoutes.service';
import { IUserId } from '../../models/account';

@Component({
	selector: 'bgc-quick-user-actions',
	templateUrl: './bgcQuickUserActions.html',
	styleUrls: ['bgcQuickUserActions.css']
})

export class BgcQuickUserActionsControl {
	@Input() public items: QuickActionItem[];
	@Input() public userName: string;

	constructor(private router: ApiRoutesService) {
		
	}

	onIconClick() {
		
	}

	/**
	 * Invokes action if present and performs routing, if allowed by action.
	 * @param index index of selected item
	 */
	onSelect(index: number) {
		let item = this.items[index];

		if (item.action) {
			let redirect = item.action();
			if (!redirect)
				return;
		}
		this.router.redirect(item.route);
	}
}

export class QuickActionItem {
	public name: string;
	public route: string;
	public action: (() => boolean)|undefined;

	constructor(name: string, route: string, action?: () => boolean) {
		this.name = name;
		this.route = route;
		this.action = action;
	}
}