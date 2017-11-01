import { Directive, Input } from '@angular/core';
import { Validator, NG_VALIDATORS, AbstractControl } from '@angular/forms';

@Directive({
	selector: '[bgcEqualModel]',
	providers: [{ provide: NG_VALIDATORS, useExisting: BgcEqualValidator, multi: true }]
})

export class BgcEqualValidator implements Validator {

	@Input() bgcEqualModel: string;

	validate(c: AbstractControl) {
		return c.value === this.bgcEqualModel ? null : { 'bgcNotEqual': {value: c.value } };
	}
}