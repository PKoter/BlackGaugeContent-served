import { NgModule }     from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule }  from '@angular/forms';
import { HttpModule }   from '@angular/http';
import { RouterModule } from '@angular/router';

import { AppComponent}             from './components/app/app.component';
import { AppHeaderComponent}       from './components/appHeader/appHeader.component';
import { NavMenuComponent}         from './components/navmenu/navmenu.component';
import { HomeComponent}            from './components/home/home.component';
import { MemeListComponent}        from './components/memeList/memeList.component';
import { MemeComponent}            from './components/meme/meme.component';
import { RegisterComponent}        from './components/register/register.component';
import { RegisterMessageComponent} from './components/registerMessage/registerMessage.component';
import { LoginComponent}           from './components/login/login.component';
import { ManageAccountComponent}   from './components/manageAccount/manageAccount.component';
import { FindUsersComponent}       from './components/findUsers/findUsers.component';


import { BgcSelectControl}		     from './controls/bgcSelect/bgcSelect.control';
import { BgcSwitchControl}		     from './controls/bgcSwitch/bgcSwitch.control';
import { BgcSidePanelControl}        from './controls/bgcSidePanel/bgcSidePanel.control';
import { BgcMessageModalControl}     from './controls/bgcMessageModal/bgcMessageModal.control';
import { BgcQuickUserActionsControl} from './controls/bgcQuickUserActions/bgcQuickUserActions.control';

import { BgcLoadingSpinnerHelper}  from './viewApi/bgcLoadingSpinner.helper';

import { BgcDistinctValidator}     from './directives/bgcDistinctChars.validator';
import { BgcEqualValidator}        from './directives/bgcEqual.validator';

import { DataFlowService}          from './services/dataFlow.service';
import { UserService}              from './services/user.service';
import { AuthGuard}                from './auth/auth.guard';
import { Routes, ApiRoutesService} from './services/apiRoutes.service';
import { UserImpulsesService}      from './services/userImpulses.service';

@NgModule({
	declarations: [
		AppComponent,
		AppHeaderComponent,
		NavMenuComponent,
		HomeComponent,
		MemeListComponent,
		MemeComponent,
		RegisterComponent,
		RegisterMessageComponent,
		LoginComponent,
		ManageAccountComponent,
		FindUsersComponent,

		BgcSelectControl,
		BgcSwitchControl,
		BgcSidePanelControl,
		BgcQuickUserActionsControl,
		BgcMessageModalControl,

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
			{ path: Routes.MemeList+"/:page",     component: MemeListComponent},
			{ path: Routes.Register,              component: RegisterComponent },
			{ path: Routes.Login,                 component: LoginComponent },
			{ path: Routes.RegisterMessage,       component: RegisterMessageComponent },
			{ path: Routes.ConfirmEmail,          component: RegisterMessageComponent },
			{ path: Routes.ManageAccount,         component: ManageAccountComponent },
			{ path: Routes.FindUsers,             component: FindUsersComponent },
			{ path: '**', redirectTo: Routes.Home }
		]  /*{enableTracing:true}*/)
	],
	providers: [
		DataFlowService,
		UserService,
		ApiRoutesService,
		AuthGuard,
		UserImpulsesService
	]
})
export class AppModuleShared {
}