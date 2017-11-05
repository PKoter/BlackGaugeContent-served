import { Injectable} from '@angular/core';

@Injectable()
export class DataFlowService {
	private dict : IDictionary = new Map<string, any>();

	/**
	 * gets data under given route. Removes data after reading.
	 * @param route
	 */
	public getOnce(route: string): any {
		let data = this.dict[route];
		this.dict[route] = null;
		return data;
	}

	/**
	 * gets data under given route. Non-destructive read.
	 * @param route
	 */
	public get(route: string) {
		let data = this.dict[route];
		return data;
	}

	/**
	 * saves data under route, which may be used to leave data for components before routing.
	 * @param route
	 * @param data
	 */
	public save(route: string, data: any) {
		this.dict[route] = data;
	}
}

export interface IDictionary {
	[index: string] : any;
}