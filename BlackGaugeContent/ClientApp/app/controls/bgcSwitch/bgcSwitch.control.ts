import { Component, Input, Output, EventEmitter } from '@angular/core';
import { trigger, state, style, animate, transition } from '@angular/animations';

@Component({
	selector: 'bgc-switch',
	templateUrl: 'bgcSwitch.html',
	styleUrls: ['bgcSwitch.css'],
	animations: [
		trigger('switchSide', [
			state('left',
				style({
					marginLeft:'0'
				})
			),
			state('right',
				style({
					marginLeft: '50%'
				})
			),
			transition('left => right', animate('100ms linear')),
			transition('right => left', animate('100ms linear'))
		])
	]
})

export class BgcSwitchControl {
	@Output() public switched = new EventEmitter();

	@Input() public startValue: boolean;
	@Input() public disabled: boolean;
	@Input() public leftName: string;
	@Input() public rightName: string;

	private value: boolean;
	private buttonState: string;

	constructor() {
		this.value = this.startValue;
		this.setSide(this.startValue);
	}

	setSide(value: boolean) {
		this.buttonState = value ? 'left' : 'right';
	}

	switch() {
		if (this.disabled)
			return;
		this.value = !this.value;
		this.setSide(this.value);
		this.switched.emit(this.value);
	}
}