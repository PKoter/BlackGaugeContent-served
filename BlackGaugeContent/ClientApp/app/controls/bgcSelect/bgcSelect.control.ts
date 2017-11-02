import { Component, Injectable, Input, Output, EventEmitter, ContentChild, TemplateRef } from '@angular/core';

@Injectable()
@Component({
	selector: 'bgc-select',
	templateUrl: 'bgcSelect.html',
	styleUrls: ['bgcSelect.css', '../bgcButtons.css']
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


	private get ShowValue(): string {
		if (this.item == null)
			return '';
		let str = this.modelToString(this.item);
		return str;
	}

	turnSelecting(value: boolean) {
		this.selecting = value;
	}

	select(item: any, index: number) {
		this.selected.emit(new SelectionEntry(item, index));
		this.item = item;
		this.selecting = false;
	}
}

export class SelectionEntry<T> {
	constructor(
		public item: T,
		public index: number) {
	}
}