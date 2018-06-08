import { Directive, Input} from '@angular/core';
import { Validator, NG_VALIDATORS, AbstractControl } from '@angular/forms';

@Directive({
	selector: '[bgcDistinctChars]',
	providers: [{ provide: NG_VALIDATORS, useExisting: BgcDistinctValidator, multi: true }]
})

export class BgcDistinctValidator implements Validator {

	@Input() bgcDistinctChars: number;

	validate(c: AbstractControl) {
		const value = c.value;
		if (value == null || value == '')
			return null;
		if (value.length < this.bgcDistinctChars)
			return { 'bgcDistinctChars': { value: value.length } };

		const hashmap = new Map<string, any>();
		let count = 0;
		
		for (var i = 0; i < value.length; i++) {
			if (hashmap.has(value[i]) === false) {
				hashmap.set(value[i], 1);
				count++;
			}
		}
		return count >= this.bgcDistinctChars ? null : {'bgcDistinctChars': {value: count}};
	}
}