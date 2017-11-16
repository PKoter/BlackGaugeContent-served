import { NgModule }     from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule }  from '@angular/forms';
import { HttpModule }   from '@angular/http';
import { RouterModule } from '@angular/router';

import { AppComponent}             from './components/app/app.component';
import { NavMenuComponent}         from './components/navmenu/navmenu.component';
import { HomeComponent}            from './components/home/home.component';
import { MemeListComponent}        from './components/memeList/memeList.component';
import { MemeComponent}            from './components/meme/meme.component';
import { RegisterComponent}        from './components/register/register.component';
import { RegisterMessageComponent} from './components/registerMessage/registerMessage.component';
import { LoginComponent}           from './components/login/login.component';


import { BgcSelectControl}		from './controls/bgcSelect/bgcSelect.control';
import { BgcSwitchControl}		from './controls/bgcSwitch/bgcSwitch.control';
import { BgcSidePanelControl}	from './controls/bgcSidePanel/bgcSidePanel.control';

import { BgcLoadingSpinnerHelper} from './viewApi/bgcLoadingSpinner.helper';

import { BgcDistinctValidator} from './directives/bgcDistinctChars.validator';
import { BgcEqualValidator}    from './directives/bgcEqual.validator';

import { DataFlowService}          from './services/dataFlow.service';
import { UserService}              from './services/user.service';
import { AuthGuard}                from './auth/auth.guard';
import { Routes, ApiRoutesService} from './services/apiRoutes.service';

@NgModule({
	declarations: [
		AppComponent,
		NavMenuComponent,
		HomeComponent,
		MemeListComponent,
		MemeComponent,
		RegisterComponent,
		RegisterMessageComponent,
		LoginComponent,

		BgcSelectControl,
		BgcSwitchControl,
		BgcSidePanelControl,

		BgcLoadingSpinnerHelper,

		BgcDistinctValidator,
		BgcEqualValidator
	],
	imports: [
		CommonModule,
		HttpModule,
		FormsModule,
		RouterModule.forRoot([
			{ path: '', redirectTo: Routes.Home,  pathMatch: 'full' },
			{ path: Routes.Home,                  component: HomeComponent },
			{ path: Routes.MemeList,              component: MemeListComponent },
			{ path: Routes.Register,              component: RegisterComponent },
			{ path: Routes.Login,                 component: LoginComponent },
			{ path: Routes.RegisterMessage,       component: RegisterMessageComponent },
			{ path: Routes.ConfirmEmail,          component: RegisterMessageComponent },
			{ path: '**', redirectTo: Routes.Home }
		],  {enableTracing:true})
	],
	providers: [
		DataFlowService,
		UserService,
		ApiRoutesService,
		AuthGuard
	]
})
export class AppModuleShared {
}
