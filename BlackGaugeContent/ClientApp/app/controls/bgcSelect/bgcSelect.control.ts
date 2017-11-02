import { Component, Injectable, Input, Output, EventEmitter, ContentChild, TemplateRef } from '@angular/core';
import { trigger, state, style, animate, transition } from '@angular/animations';

@Injectable()
@
Component({
	selector: 'bgc-select',
	templateUrl: 'bgcSelect.html',
	styleUrls: ['bgcSelect.css'],
	animations: [
		trigger('selectState', [
			state('hidden',
				style({
					left: '100%'
				})
			),
			state('drawn',
				style({
					left: '65%'
				})
			),
			transition('hidden => drawn', animate('200ms ease-out')),
			transition('drawn => hidden', animate('160ms linear'))
		])
	]
})


export class BgcSelectControl {
	@Output() public selected = new EventEmitter();
	public item : any;

	@Input() public title: string;
	@Input() public items: any[];
	@Input() public modelToString: (item: any) => any;
	@Input() public require: boolean;
	@ContentChild(TemplateRef) itemTemplate: TemplateRef<any>;

	private selecting: boolean = false;
	private shelfState : string = 'hidden';


	private get ShowValue(): string {
		if (this.item == null)
			return '';
		let str = this.modelToString(this.item);
		return str;
	}

	turnSelecting(value: boolean) {
		this.selecting = value;
		this.shelfState = value ? 'drawn' : 'hidden';
	}

	select(item: any, index: number) {
		this.selected.emit(new SelectionEntry(item, index));
		this.item = item;
		this.turnSelecting(false);
	}
}

export class SelectionEntry<T> {
	constructor(
		public item: T,
		public index: number) {
	}
}