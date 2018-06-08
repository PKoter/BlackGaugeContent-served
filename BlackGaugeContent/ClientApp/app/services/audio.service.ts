
export class AudioService {
	private audioUri: string;
	private signalAudio: any;

	constructor() {
		this.signalAudio = new Audio();
	}

	public loadAudioFromUri(uri: string) {
		this.audioUri = uri;
		this.signalAudio.src = this.audioUri;
		this.signalAudio.load();
	}

	public playAudio() {
		this.signalAudio.play();
	}
}