export interface IEnumerable<T> {
	FirstOrDefault(): T | null;
	LastOrDefault(): T | null;

	First(): T;
	Last(): T ;
}

export class List<T> extends Array<T> implements IEnumerable<T> {
	private constructor(items: Array<T>) {
		super(... items);
	}

	static create<T>(): List<T> {
		return Object.create(List.prototype);
	}


	public FirstOrDefault(): T | null {
		if (this.length === 0)
			return null;
		return this[0];
	}

	public LastOrDefault(): T | null {
		if (this.length === 0)
			return null;
		return this[this.length - 1];
	}

	public First(): T {
		return this[0];
	}

	public Last(): T {
		return this[this.length - 1];
	}

}