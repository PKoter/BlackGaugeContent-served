import { Component, Injectable, Input } from '@angular/core';

@Injectable()
@Component({
	selector: 'string-select',
	template: `
<select [(ngModel)]="selected" required>
	<option selected hidden >Choose one...</option>
	<option *ngFor="let item in items" [value]="item">{{item}}</option>
</select>`
})


export class StringSelectControl {
	public selected: string;
	
	@Input() items: string[];
}