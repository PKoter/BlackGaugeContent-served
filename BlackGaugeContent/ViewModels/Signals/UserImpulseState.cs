using System.Collections.Generic;

namespace Bgc.ViewModels.Signals
{
	public class UserImpulseState
	{
		public int NotifyCount {get; set;}

		public int ComradeRequestCount {get; set;}

		public int MessageCount {get; set;}

		public IList<ChatImpulse> ChatImpulses {get; set;}
	}
}
