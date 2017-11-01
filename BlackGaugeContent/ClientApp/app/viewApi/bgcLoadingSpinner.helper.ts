import { Component, Input } from '@angular/core';

@Component({
	selector: 'bgc-loading-spinner',
	template: `
	<div class="ld-spin-bcgd">
		<img src="Images/bgc_loader_spinner.gif" alt="We are loading some strange things..." class="ld-spinner"/>
	</div>`,
	styles: [`
	div.ld-spin-bcgd {
		height:100%; 
		width: 100%; 
		position:fixed;
		background-color: #fafafa8f;
		left:0;
		top: 0;
	}
	img.ld-spinner {
		border:none; 
		background-color:transparent; 
		left:calc(50% - 35px); 
		top:40%;
		position:absolute;
	}
`]
})

export class BgcLoadingSpinnerHelper {
	@Input() isLoading: boolean;

}