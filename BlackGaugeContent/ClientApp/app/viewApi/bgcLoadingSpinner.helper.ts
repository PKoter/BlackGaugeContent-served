import { Component, Input } from '@angular/core';

@Component({
	selector: 'bgc-loading-spinner',
	template: `
	<div class="popup-background">
		<img src="images/bgc_loader_spinner.gif" 
			alt="We are loading some strange things..." 
			class="alpha popup-core"/>
	</div>`,
	styleUrls: ['../controls/bgcFrontPopups.css', '../controls/bgcGeneral.css']
})

export class BgcLoadingSpinnerHelper {
	@Input() isLoading: boolean;

}