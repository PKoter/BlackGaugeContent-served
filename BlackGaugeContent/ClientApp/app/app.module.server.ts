import { NgModule } from '@angular/core';
import { ServerModule } from '@angular/platform-server';
import { AppModuleShared } from './app.module.shared';
import { AppComponent } from './components/app/app.component';
import { LocalStorage } from './viewApi/localStorage';


@NgModule({
	bootstrap: [AppComponent],
	imports: [
		ServerModule,
		AppModuleShared
	],
	providers: [{ provide: LocalStorage, useValue: { getItem() { } } }
	]
})
export class AppModule {
}
