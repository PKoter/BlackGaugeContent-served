import { Component, Injectable, Input, Output, EventEmitter } from '@angular/core';
import { trigger, state, style, animate, transition } from '@angular/animations';

@Injectable()
@Component({
	selector: 'bgc-side-panel',
	templateUrl: 'bgcSidePanel.html',
	styleUrls: ['bgcSidePanel.css', '../bgcButtons.css'],
	animations: [
		trigger('drawnState', [
			state('hidden',
				style({
					left: '100%'
				})
			),
			state('drawnNormal',
				style({
					left: '50%'
				})
			),
			state('drawnBig',
				style({
					left: '200px'
				})
			),
			transition('hidden => drawnNormal', animate('200ms ease-out')),
			transition('drawnNormal => hidden', animate('200ms ease-in')),
			transition('hidden => drawnBig',    animate('250ms ease-out')),
			transition('drawnBig => hidden',    animate('250ms ease-in'))
		])
	]
})

export class BgcSidePanelControl {
	@Output() public called = new EventEmitter();

	@Input() public title: string;
	@Input() public closeText: string;
	@Input() public big: boolean;
	/** determines whether called events raised by Draw are welcomed */
	@Input() public notifyOnDraw: boolean = false;

	drawn: boolean = false;
	shelfState: string = 'hidden';

	@Input() set Draw(value: boolean) {
		this.setState(value);
		if(this.notifyOnDraw)
			this.called.emit(value);
	}

	hide() {
		this.setState(false);
		this.called.emit(false);
	}

	private setState(value: boolean) {
		this.drawn = value;
		this.shelfState = value ? (this.big ? 'drawnBig' : 'drawnNormal') : 'hidden';
	}
}