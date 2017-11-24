import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
	selector: 'bgc-message-modal',
	templateUrl: './bgcMessageModal.html',
	styleUrls: ['./bgcMessageModal.css', '../bgcButtons.css', '../bgcFrontPopups.css']
})

export class BgcMessageModalControl {
	@Input() enabled: boolean;
	@Input() areTwoButtons: boolean = true;
	@Input() button1Class: string;
	@Input() button2Class: string;
	@Input() button1Text: string;
	@Input() button2Text: string;

	@Output() button1 = new EventEmitter();
	@Output() button2 = new EventEmitter();

	onButton1() {
		if (this.button1)
			this.button1.emit();
		this.enabled = false;
	}

	onButton2() {
		if(this.button2)
			this.button2.emit();
		this.enabled = false;
	}
}