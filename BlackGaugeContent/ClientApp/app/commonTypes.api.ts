export class ListEntry<T> {
	constructor(
		public item: T,
		public index: number) {
	}
}

export class HashSet<T> {
	private elements : {[index: string]: T};

	public count: number;

	constructor() {
		this.elements = {} as { [index: string]: T };
		this.count    = 0;
	}

	public hasKey(key: string): boolean {
		return this.elements.hasOwnProperty(key);
	}

	public add(key: string, value: T) {
		if (!this.elements.hasOwnProperty(key)) 
			this.count ++;

		this.elements[key] = value;
	}

	public item(key: string): T {
		return this.elements[key];
	}
	
	public remove(key: string): T {
		let item = this.elements[key];
		delete this.elements[key];
		this.count --;
		return item;
	}
}