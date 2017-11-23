import { Component, Injectable, Input, Output, EventEmitter, ContentChild, TemplateRef } from '@angular/core';
import { ListEntry} from '../../commonTypes.api';

@Injectable()
@Component({
	selector: 'bgc-select',
	templateUrl: 'bgcSelect.html',
	styleUrls: ['bgcSelect.css', '../bgcButtons.css']
})


export class BgcSelectControl {
	@Output() public selected = new EventEmitter();
	public item : any;

	/** item to display before any other is selected */
	@Input() public preselectedItem: any;
	/** value to display if no item is present at start */
	@Input() public preselectedValue: string = '';
	@Input() public title: string;
	@Input() public items: any[];
	@Input() public modelToString: (item: any) => any;
	@Input() public require: boolean;
	@ContentChild(TemplateRef) itemTemplate: TemplateRef<any>;

	private selecting: boolean = false;


	private get ShowValue(): string {
		if (this.item == null) {
			if (this.preselectedItem)
				this.item = this.preselectedItem;
			else
				return this.preselectedValue;
		}
		let str = this.modelToString(this.item);
		return str;
	}

	turnSelecting(value: boolean) {
		this.selecting = value;
	}

	select(item: any, index: number) {
		this.selected.emit(new ListEntry(item, index));
		this.item = item;
		this.selecting = false;
	}
}