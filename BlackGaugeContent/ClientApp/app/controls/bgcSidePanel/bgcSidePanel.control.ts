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
					left: '65%'
				})
			),
			state('drawnBig',
				style({
					left: 'calc(25% - 19px)'
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

	private drawn: boolean = false;
	private shelfState: string = 'hidden';

	@Input() set Draw(value: boolean) {
		this.drawn = value;
		this.shelfState = value ? (this.big? 'drawnBig' : 'drawnNormal') : 'hidden';
		this.called.emit(value);
	}
}