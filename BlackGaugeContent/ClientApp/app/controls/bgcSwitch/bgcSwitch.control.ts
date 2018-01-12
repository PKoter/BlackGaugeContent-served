import { Component, Input, Output, EventEmitter, ViewEncapsulation } from '@angular/core';
import { trigger, state, style, animate, transition } from '@angular/animations';

@Component({
	selector: 'bgc-switch',
	templateUrl: 'bgcSwitch.html',
	styleUrls: ['bgcSwitch.css', '../bgcButtons.css'],
	encapsulation: ViewEncapsulation.None,
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

	@Input() public useCheckbox: boolean = false;
	@Input() public constType:   boolean = false;
	@Input() public startValue:  boolean;
	@Input() public disabled:    boolean;
	@Input() public leftName:    string;
	@Input() public rightName:   string;
	@Input() public switchClass: string;
	@Input() public checkClass:  string;
	//@Input() public switchOrCheckbox: () => boolean;

	value: boolean;
	buttonState: string;

	constructor() {
		this.value = this.startValue;
		/*if (this.switchOrCheckbox == null)
			this.switchOrCheckbox = this.switchPredicate;

		if (this.constType === false) {
			this.useCheckbox = !this.switchOrCheckbox();
		}*/
		this.setSide(this.startValue);
	}
	/*
	switchPredicate(): boolean {
		return window.screen.width >= 700;
	}*/

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