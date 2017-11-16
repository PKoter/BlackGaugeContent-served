import { NgModule}                from '@angular/core';
import { BrowserModule}           from '@angular/platform-browser';
import { BrowserAnimationsModule} from '@angular/platform-browser/animations';
import { HttpModule}              from '@angular/http';
import { RouterModule, Routes}	  from '@angular/router';
import { AppModuleShared}         from './app.module.shared';
import { AppComponent}            from './components/app/app.component';
import { LocalStorage}            from './viewApi/localStorage';


@NgModule({
	bootstrap: [ AppComponent ],
	imports: [
		BrowserModule,
		BrowserAnimationsModule,
		HttpModule,
		AppModuleShared
	],
	providers: [
		{ provide: 'BASE_URL', useFactory: getBaseUrl },
		{ provide: LocalStorage, useValue: window.localStorage }
	]
})
export class AppModule {
}

export function getBaseUrl() {
	return document.getElementsByTagName('base')[0].href;
}
