using System.Collections.Generic;
using Bgc.ViewModels.User;

namespace Bgc.ViewModels.Signals
{
	public class UserImpulseState : IComradeModel
	{
		public int NotifyCount {get; set;}

		public IEnumerable<ComradeRequest> RequestsReceived {get; set;}
		public IEnumerable<ComradeRequest> RequestsAgreed {get; set;}
	}
}
