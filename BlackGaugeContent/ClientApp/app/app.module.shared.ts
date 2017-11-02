import { NgModule }     from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule }  from '@angular/forms';
import { HttpModule }   from '@angular/http';
import { RouterModule } from '@angular/router';

import { AppComponent}          from './components/app/app.component';
import { NavMenuComponent}      from './components/navmenu/navmenu.component';
import { HomeComponent}         from './components/home/home.component';
import { FetchDataComponent}    from './components/fetchdata/fetchdata.component';
import { MemeListComponent}     from './components/memeList/memeList.component';
import { MemeComponent}         from './components/meme/meme.component';
import { RegistrationComponent} from './components/register/registration.component';

import { BgcSelectControl} from './controls/bgcSelect/bgcSelect.control';
import { BgcSwitchControl} from './controls/bgcSwitch/bgcSwitch.control';

import { BgcLoadingSpinnerHelper} from './viewApi/bgcLoadingSpinner.helper';

import { BgcDistinctValidator} from './directives/bgcDistinctChars.validator';
import { BgcEqualValidator } from './directives/bgcEqual.validator';

@NgModule({
	declarations: [
		AppComponent,
		NavMenuComponent,
		FetchDataComponent,
		HomeComponent,
		MemeListComponent,
		MemeComponent,
		RegistrationComponent,

		BgcSelectControl,
		BgcSwitchControl,

		BgcLoadingSpinnerHelper,

		BgcDistinctValidator,
		BgcEqualValidator
	],
	imports: [
		CommonModule,
		HttpModule,
		FormsModule,
		RouterModule.forRoot([
			{ path: '', redirectTo: 'home', pathMatch: 'full' },
			{ path: 'home', component: HomeComponent },
			{ path: 'fetchdata', component: FetchDataComponent },
			{ path: 'memelist', component: MemeListComponent },
			{ path: 'registration', component: RegistrationComponent },
			{ path: '**', redirectTo: 'home' }
		])
	]
})
export class AppModuleShared {
}
